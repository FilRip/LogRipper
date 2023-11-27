using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using LogRipper.Constants;
using LogRipper.Models;

namespace LogRipper.ViewModels;

public partial class FileWindowViewModel : ObservableObject
{
    [ObservableProperty()]
    private string _formatDate;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(BackColorBrush))]
    private Color _backColor;
    [ObservableProperty(), NotifyPropertyChangedFor(nameof(ForeColorBrush))]
    private Color _foreColor;
    private readonly List<string> _listEncoders;
    [ObservableProperty()]
    private string _currentEncoder;
    [ObservableProperty()]
    private string _firstLine;

    public FileWindowViewModel() : base()
    {
        _listEncoders =
        [
            Locale.MENU_ENC_DEFAULT.Replace("_", ""),
            Locale.MENU_ENC_ASCII.Replace("_", ""),
            Locale.MENU_ENC_UTF7.Replace("_", ""),
            Locale.MENU_ENC_UTF8.Replace("_", ""),
            Locale.MENU_ENC_UTF32.Replace("_", ""),
            Locale.MENU_ENC_UNICODE.Replace("_", ""),
        ];
    }

    public List<string> ListEncoders
    {
        get { return _listEncoders; }
    }

    public Brush BackColorBrush
    {
        get { return new SolidColorBrush(BackColor); }
    }

    public Brush ForeColorBrush
    {
        get { return new SolidColorBrush(ForeColor); }
    }
}
