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

    public FusionWindowViewModel() : base()
    {
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
    }

    [RelayCommand()]
    private void BrowseFilename()
    {
        OpenFileDialog dialog = new()
        {
            Filter = "Log file|*.log|All files|*.*",
        };
        if (dialog.ShowDialog() == true)
        {
            FileName = dialog.FileName;
        }
    }
}
