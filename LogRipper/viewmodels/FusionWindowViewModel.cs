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

    public FusionWindowViewModel() : base()
    {
        BackColor = Constants.Colors.DefaultBackgroundColor;
        ForeColor = Constants.Colors.DefaultForegroundColor;
    }

    public Brush BackColorBrush
    {
        get { return new SolidColorBrush(BackColor); }
    }

    public Brush ForeColorBrush
    {
        get { return new SolidColorBrush(ForeColor); }
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
