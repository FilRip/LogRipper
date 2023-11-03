using System.Windows.Input;
using System.Windows.Media;

using LogRipper.Helpers;

using Microsoft.Win32;

namespace LogRipper.ViewModels
{
    public class FusionWindowViewModel : ViewModelBase
    {
        private string _formatDate;
        private string _filename;
        private readonly ICommand _browseFilename;
        private Color _backColor, _foreColor;

        public FusionWindowViewModel() : base()
        {
            _browseFilename = new RelayCommand((param) => BrowseFilename());
            BackColor = Constants.Colors.DefaultBackgroundColor;
            ForeColor = Constants.Colors.DefaultForegroundColor;
        }

        public string FormatDate
        {
            get { return _formatDate; }
            set
            {
                if (_formatDate != value)
                {
                    _formatDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FileName
        {
            get { return _filename; }
            set
            {
                if (_filename != value)
                {
                    _filename = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color BackColor
        {
            get { return _backColor; }
            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(BackColorBrush));
                }
            }
        }

        public Color ForeColor
        {
            get { return _foreColor; }
            set
            {
                if (_foreColor != value)
                {
                    _foreColor = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ForeColorBrush));
                }
            }
        }

        public Brush BackColorBrush
        {
            get { return new SolidColorBrush(_backColor); }
        }

        public Brush ForeColorBrush
        {
            get { return new SolidColorBrush(_foreColor); }
        }

        public ICommand CmdBrowseFilename
        {
            get { return _browseFilename; }
        }

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
}
