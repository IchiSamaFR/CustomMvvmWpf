using Microsoft.Win32;
using System.IO;

namespace CustomMvvmWpf.Services;

/// <summary>
/// Boîtes de dialogue « Ouvrir » et « Enregistrer sous » du système, isolées ici
/// pour que les vue-modèles n'aient pas à connaître Win32.
/// </summary>
public class FileDialogService
{
    /// <summary>
    /// Demande un fichier à ouvrir ; retourne <c>null</c> si l'utilisateur annule.
    /// </summary>
    public string? PickOpenFile(string filter, string? initialPath = null)
    {
        OpenFileDialog dialog = new()
        {
            Filter = filter,
            CheckFileExists = true,
            Multiselect = false
        };

        ApplyInitialDirectory(dialog, initialPath);

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    /// <summary>
    /// Demande l'emplacement d'enregistrement ; retourne <c>null</c> si l'utilisateur annule.
    /// </summary>
    public string? PickSaveFile(string filter, string suggestedFileName, string? initialPath = null)
    {
        SaveFileDialog dialog = new()
        {
            Filter = filter,
            FileName = suggestedFileName,
            AddExtension = true,
            OverwritePrompt = true
        };

        ApplyInitialDirectory(dialog, initialPath);

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    private static void ApplyInitialDirectory(FileDialog dialog, string? initialPath)
    {
        if (string.IsNullOrWhiteSpace(initialPath))
        {
            return;
        }

        string? folder = Directory.Exists(initialPath)
            ? initialPath
            : Path.GetDirectoryName(initialPath);

        if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
        {
            dialog.InitialDirectory = folder;
        }
    }
}
