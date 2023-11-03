using System.Collections.Generic;
using System.Windows.Media;

using LogRipper.Constants;
using LogRipper.Helpers;

namespace LogRipper.ViewModels
{
    public class FileWindowViewModel : ViewModelBase
    {
        private string _formatDate;
        private Color _backColor, _foreColor;
        private readonly List<string> _listEncoders;
        private string _encoder;

        public FileWindowViewModel() : base()
        {
            _listEncoders = new List<string>()
            {
                Locale.MENU_ENC_DEFAULT.Replace("_", ""),
                Locale.MENU_ENC_ASCII.Replace("_", ""),
                Locale.MENU_ENC_UTF7.Replace("_", ""),
                Locale.MENU_ENC_UTF8.Replace("_", ""),
                Locale.MENU_ENC_UTF32.Replace("_", ""),
                Locale.MENU_ENC_UNICODE.Replace("_", ""),
            };
        }

        public List<string> ListEncoders
        {
            get { return _listEncoders; }
        }

        public string CurrentEncoder
        {
            get { return _encoder; }
            set
            {
                if (_encoder != value)
                {
                    _encoder = value;
                    OnPropertyChanged();
                }
            }
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
    }
}
