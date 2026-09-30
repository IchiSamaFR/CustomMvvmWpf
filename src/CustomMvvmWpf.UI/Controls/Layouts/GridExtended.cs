using System.Windows;
using System.Windows.Controls;

namespace CustomMvvmWpf.UI.Controls.Layouts;

public class GridExtended : Grid
{
    public static readonly DependencyProperty RowsProperty =
        DependencyProperty.Register(
            nameof(Rows),
            typeof(string),
            typeof(GridExtended),
            new PropertyMetadata(null, OnRowsChanged));

    public string Rows
    {
        get => (string)GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.Register(
            nameof(Columns),
            typeof(string),
            typeof(GridExtended),
            new PropertyMetadata(null, OnColumnsChanged));

    public string Columns
    {
        get => (string)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public static readonly DependencyProperty ColumnSpacingProperty =
        DependencyProperty.Register(
            nameof(ColumnSpacing),
            typeof(double),
            typeof(GridExtended),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, OnSpacingChanged));

    public double ColumnSpacing
    {
        get => (double)GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }

    public static readonly DependencyProperty RowSpacingProperty =
        DependencyProperty.Register(
            nameof(RowSpacing),
            typeof(double),
            typeof(GridExtended),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, OnSpacingChanged));

    public double RowSpacing
    {
        get => (double)GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    private static readonly DependencyProperty OriginalMarginProperty =
        DependencyProperty.RegisterAttached(
            "OriginalMargin",
            typeof(Thickness?),
            typeof(GridExtended),
            new PropertyMetadata(null));

    private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GridExtended)d).ApplySpacing();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        ApplySpacing();
        return base.MeasureOverride(constraint);
    }

    private void ApplySpacing()
    {
        foreach (UIElement child in InternalChildren)
        {
            if (child is not FrameworkElement element)
                continue;

            var original = (Thickness?)element.GetValue(OriginalMarginProperty);

            if (original is null)
            {
                original = element.Margin;
                element.SetValue(OriginalMarginProperty, original);
            }

            var left = GetColumn(element) > 0 ? ColumnSpacing : 0d;
            var top = GetRow(element) > 0 ? RowSpacing : 0d;

            var margin = new Thickness(
                original.Value.Left + left,
                original.Value.Top + top,
                original.Value.Right,
                original.Value.Bottom);

            if (element.Margin != margin)
                element.Margin = margin;
        }
    }

    private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var grid = (GridExtended)d;
        ParseDefinitions(e.NewValue as string, grid.RowDefinitions);
    }

    private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var grid = (GridExtended)d;
        ParseDefinitions(e.NewValue as string, grid.ColumnDefinitions);
    }

    private static void ParseDefinitions<T>(string? value, ICollection<T> collection)
        where T : DefinitionBase, new()
    {
        collection.Clear();

        if (string.IsNullOrWhiteSpace(value))
            return;

        var converter = new GridLengthConverter();

        foreach (var part in value.Split(','))
        {
            var trimmed = part.Trim();

            if (string.IsNullOrEmpty(trimmed))
                continue;

            var length = (GridLength)converter.ConvertFromString(trimmed)!;

            if (collection is IList<RowDefinition> rows)
            {
                rows.Add(new RowDefinition { Height = length });
            }
            else if (collection is IList<ColumnDefinition> columns)
            {
                columns.Add(new ColumnDefinition { Width = length });
            }
        }
    }
}