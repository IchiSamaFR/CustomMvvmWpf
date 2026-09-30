using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;

namespace CustomMvvmWpf.UI.Behaviors;

/// <summary>
/// Comportements attachés aux grilles de résultats.
/// </summary>
public static class DataGridBehavior
{
    /// <summary>
    /// Largeur maximale d'une colonne : sans plafond, une colonne de texte long rend
    /// la mesure coûteuse et le défilement horizontal inutilisable.
    /// </summary>
    private const double MaxColumnWidth = 400;

    private static readonly BooleanTextConverter BooleanConverter = new();

    /// <summary>
    /// Remplace les colonnes générées automatiquement par des colonnes dont le contenu
    /// est sélectionnable à la souris, et affiche les booléens en texte plutôt qu'en
    /// case à cocher.
    /// </summary>
    public static readonly DependencyProperty SelectableTextProperty =
        DependencyProperty.RegisterAttached(
            "SelectableText",
            typeof(bool),
            typeof(DataGridBehavior),
            new PropertyMetadata(false, OnSelectableTextChanged));

    public static bool GetSelectableText(DependencyObject element) =>
        (bool)element.GetValue(SelectableTextProperty);

    public static void SetSelectableText(DependencyObject element, bool value) =>
        element.SetValue(SelectableTextProperty, value);

    private static void OnSelectableTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGrid grid)
        {
            return;
        }

        grid.AutoGeneratingColumn -= OnAutoGeneratingColumn;
        grid.CurrentCellChanged -= OnCurrentCellChanged;

        if (e.NewValue is true)
        {
            grid.AutoGeneratingColumn += OnAutoGeneratingColumn;
            grid.CurrentCellChanged += OnCurrentCellChanged;
        }
    }

    /// <summary>
    /// Entre dans la cellule dès qu'elle devient courante : un simple clic — ou une
    /// flèche du clavier — suffit alors à sélectionner le texte, sans passer par le
    /// double-clic d'édition de la grille.
    /// </summary>
    private static void OnCurrentCellChanged(object? sender, EventArgs e)
    {
        if (sender is not DataGrid grid)
        {
            return;
        }

        // Différé : au moment de l'événement, la grille valide encore la cellule
        // précédente et refuserait d'en ouvrir une nouvelle.
        _ = grid.Dispatcher.BeginInvoke(DispatcherPriority.Input, () =>
        {
            if (grid.CurrentCell.IsValid)
            {
                _ = grid.BeginEdit();
            }
        });
    }

    private static void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        // Seules les colonnes liées à une valeur sont concernées : on récupère leur
        // liaison pour la réutiliser telle quelle (chemin, format, échappements).
        if (e.Column is not DataGridBoundColumn { Binding: Binding binding })
        {
            return;
        }

        Binding cellBinding = new(binding.Path.Path)
        {
            Mode = BindingMode.OneWay,
            StringFormat = binding.StringFormat,
        };

        if (IsBoolean(e.PropertyType))
        {
            // « true » / « false » au lieu d'une case à cocher en lecture seule.
            cellBinding.Converter = BooleanConverter;
            cellBinding.StringFormat = null;
        }

        e.Column = new DataGridTemplateColumn
        {
            Header = e.Column.Header,
            SortMemberPath = e.PropertyName,
            MaxWidth = MaxColumnWidth,

            // Affichage : un simple TextBlock, le moins coûteux à réaliser quand la
            // virtualisation recycle les lignes pendant le défilement.
            CellTemplate = BuildTemplate(typeof(TextBlock), TextBlock.TextProperty, cellBinding),

            // Sélection du texte : la zone de saisie n'est créée que pour la cellule
            // consultée (double-clic ou F2), jamais pour toute la grille.
            CellEditingTemplate = BuildTemplate(typeof(TextBox), TextBox.TextProperty, cellBinding),
        };
    }

    /// <summary>
    /// Modèle de cellule minimal : un seul élément lié à la valeur.
    /// </summary>
    private static DataTemplate BuildTemplate(Type elementType, DependencyProperty textProperty, Binding binding)
    {
        FrameworkElementFactory factory = new(elementType);

        factory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
        factory.SetValue(FrameworkElement.MarginProperty, new Thickness(4, 0, 4, 0));
        factory.SetBinding(textProperty, binding);

        if (elementType == typeof(TextBlock))
        {
            factory.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        }
        else
        {
            // Lecture seule : le contenu se sélectionne et se copie, jamais ne se modifie.
            factory.SetValue(TextBoxBase.IsReadOnlyProperty, true);
            factory.SetValue(TextBoxBase.IsReadOnlyCaretVisibleProperty, true);
            factory.SetValue(Control.BorderThicknessProperty, new Thickness(0));
            factory.SetValue(Control.BackgroundProperty, System.Windows.Media.Brushes.Transparent);
            factory.SetValue(Control.PaddingProperty, new Thickness(0));
            factory.SetValue(TextBox.TextWrappingProperty, TextWrapping.NoWrap);
            factory.SetValue(TextBoxBase.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden);
        }

        DataTemplate template = new() { VisualTree = factory };

        template.Seal();

        return template;
    }

    private static bool IsBoolean(Type type) =>
        (Nullable.GetUnderlyingType(type) ?? type) == typeof(bool);

    /// <summary>
    /// Rend un booléen en « true » / « false », et une valeur absente en chaîne vide.
    /// </summary>
    private sealed class BooleanTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
        {
            bool boolean => boolean ? "true" : "false",
            _ => string.Empty,
        };

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException($"{nameof(BooleanTextConverter)} ne supporte pas la conversion inverse.");
    }
}
