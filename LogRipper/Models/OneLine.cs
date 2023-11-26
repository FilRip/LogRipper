using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;

using LogRipper.Helpers;
using LogRipper.Windows;

namespace LogRipper.Models;

[XmlRoot()]
public class OneLine : ObservableObject
{
    [XmlElement()]
    public string FilePath { get; private set; }

    public OneLine(string filePath) : this()
    {
        FilePath = filePath;
        FileName = Path.GetFileNameWithoutExtension(filePath);
    }

    public OneLine() : base()
    {
        FilePath = "";
    }

    [XmlElement()]
    public DateTime Date { get; set; }

    [XmlElement()]
    public string Line { get; set; }

    [XmlIgnore()]
    public string FileName { get; private set; }

    [XmlElement()]
    public int NumLine { get; set; }

    [XmlElement()]
    public string GroupLines { get; set; }

    [XmlIgnore()]
    public SolidColorBrush DefaultBackground
    {
        get { return FileManager.GetFile(FilePath)?.DefaultBackground ?? Constants.Colors.BackgroundColorBrush; }
    }

    [XmlIgnore()]
    public SolidColorBrush DefaultForeground
    {
        get { return FileManager.GetFile(FilePath)?.DefaultForeground ?? Constants.Colors.ForegroundColorBrush; }
    }

    [XmlIgnore()]
    public bool SpecialLine
    {
        get
        {
            return (!string.IsNullOrWhiteSpace(GroupLines) && GroupLines.IndexOf(' ') >= 0);
        }
    }

    [XmlIgnore()]
    public bool NotSpecialLine
    {
        get { return !SpecialLine; }
    }

    [XmlIgnore()]
    public string LineOrGroup
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(GroupLines) && GroupLines.IndexOf(' ') >= 0)
                return GroupLines;
            else
                return Line;
        }
    }

    [XmlIgnore()]
    public SolidColorBrush Background
    {
        get
        {
            SolidColorBrush result = ((MainWindow)Application.Current.MainWindow).MyDataContext.ListRules.ExecuteRulesBackground(Line, Date);

            if (result != null)
                return result;
            return FileManager.GetFile(FilePath)?.DefaultBackground ?? Constants.Colors.BackgroundColorBrush;
        }
    }

    [XmlIgnore()]
    public SolidColorBrush Foreground
    {
        get
        {
            SolidColorBrush result = ((MainWindow)Application.Current.MainWindow).MyDataContext.ListRules.ExecuteRulesForeground(Line, Date);

            if (result != null)
                return result;
            return FileManager.GetFile(FilePath)?.DefaultForeground ?? Constants.Colors.ForegroundColorBrush;
        }
    }

    [XmlIgnore()]
    public Visibility HideLine
    {
        get
        {
            bool result = ((MainWindow)Application.Current.MainWindow).MyDataContext.ListRules.ExecuteRulesHideLine(Line, Date);
            return result ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    [XmlIgnore()]
    public FontWeight Weight
    {
        get
        {
            bool bold = ((MainWindow)Application.Current.MainWindow).MyDataContext.ListRules.ExecuteRulesBold(Line, Date);
            return (bold ? FontWeights.Bold : FontWeights.Regular);
        }
    }

    [XmlIgnore()]
    public FontStyle Style
    {
        get
        {
            bool italic = ((MainWindow)Application.Current.MainWindow).MyDataContext.ListRules.ExecuteRulesItalic(Line, Date);
            return (italic ? FontStyles.Italic : FontStyles.Normal);
        }
    }

    internal void RefreshLine()
    {
        OnPropertyChanged(nameof(Line));
        OnPropertyChanged(nameof(Background));
        OnPropertyChanged(nameof(Foreground));
        OnPropertyChanged(nameof(HideLine));
        OnPropertyChanged(nameof(Style));
        OnPropertyChanged(nameof(Weight));
        OnPropertyChanged(nameof(GroupLines));
    }
}
