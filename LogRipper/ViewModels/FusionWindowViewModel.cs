using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

namespace LogRipper.ViewModels;

public partial class FusionWindowViewModel : ObservableObject
{
    [ObservableProperty()]
    private string _formatDate;
    [ObservableProperty()]
    private string _fileName;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(BackColorBrush))]
    private Color _backColor;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(ForeColorBrush))]
    private Color _foreColor;
    [ObservableProperty()]
    private bool _isFileReadOnly;
    [ObservableProperty()]
    private string _firstLine;
    internal string[] ListFiles;

    public FusionWindowViewModel() : base()
    {
        FormatDate = Properties.Settings.Default.DefaultDateFormat;
        BackColor = Constants.Colors.DefaultBackgroundColor;
        ForeColor = Constants.Colors.DefaultForegroundColor;
    }

    public SolidColorBrush BackColorBrush
    {
        get { return new SolidColorBrush(BackColor); }
    }

    public SolidColorBrush ForeColorBrush
    {
        get { return new SolidColorBrush(ForeColor); }
    }

    internal void SetFileReadOnly(string file)
    {
        FileName = file;
        IsFileReadOnly = true;
        FillFirstLine(file);
    }

    internal void FillFirstLine(string filename)
    {
        StringBuilder firstLine = new();
        try
        {
            using FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            while (true)
            {
                int read = fs.ReadByte();
                if (read < 0)
                    break;
                if ((read == 13 || read == 10) && !string.IsNullOrWhiteSpace(firstLine.ToString().Trim()))
                    break;
                char c = (char)read;
                firstLine.Append(c);
            }
        }
        catch (Exception) { /* Ignore errors */ }
        FirstLine = firstLine.ToString().Trim();
    }

    [RelayCommand()]
    private void BrowseFilename()
    {
        OpenFileDialog dialog = new()
        {
            Filter = "Log file|*.log|All files|*.*",
            Multiselect = true,
        };
        if (dialog.ShowDialog() == true)
        {
            FileName = dialog.FileNames[0];
            FillFirstLine(FileName);
            if (dialog.FileNames.Length > 1)
            {
                List<string> otherFiles = dialog.FileNames.ToList();
                otherFiles.RemoveAt(0);
                ListFiles = otherFiles.ToArray();
            }
        }
    }
}
