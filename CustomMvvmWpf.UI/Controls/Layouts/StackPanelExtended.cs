using System.Windows;
using System.Windows.Controls;

namespace CustomMvvmWpf.UI.Controls.Layouts;

public class StackPanelExtended : StackPanel
{
    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.Register(
            nameof(Spacing),
            typeof(double),
            typeof(StackPanelExtended),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, OnSpacingChanged));

    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    private static readonly DependencyProperty OriginalMarginProperty =
        DependencyProperty.RegisterAttached(
            "OriginalMargin",
            typeof(Thickness?),
            typeof(StackPanelExtended),
            new PropertyMetadata(null));

    private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((StackPanelExtended)d).ApplySpacing();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        ApplySpacing();
        return base.MeasureOverride(constraint);
    }

    private void ApplySpacing()
    {
        var isFirstVisible = true;

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

            var skip = isFirstVisible || element.Visibility == Visibility.Collapsed;

            if (element.Visibility != Visibility.Collapsed)
                isFirstVisible = false;

            var left = !skip && Orientation == Orientation.Horizontal ? Spacing : 0d;
            var top = !skip && Orientation == Orientation.Vertical ? Spacing : 0d;

            var margin = new Thickness(
                original.Value.Left + left,
                original.Value.Top + top,
                original.Value.Right,
                original.Value.Bottom);

            if (element.Margin != margin)
                element.Margin = margin;
        }
    }
}
