using System;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

using LogRipper.Controls;
using LogRipper.Helpers;
using LogRipper.Windows;

namespace LogRipper.Models
{
    [XmlRoot()]
    public class OneLine : ViewModelBase
    {
        public OneLine() { }

        [XmlElement()]
        public DateTime Date { get; set; }

        [XmlElement()]
        public string Line { get; set; }

        [XmlElement()]
        public string FileName { get; set; }

        [XmlElement()]
        public int NumLine { get; set; }

        [XmlIgnore()]
        public SolidColorBrush DefaultBackground
        {
            get { return FileManager.GetFile(FileName).DefaultBackground; }
        }

        [XmlIgnore()]
        public SolidColorBrush DefaultForeground
        {
            get { return FileManager.GetFile(FileName).DefaultForeground; }
        }

        [XmlIgnore()]
        public SolidColorBrush Background
        {
            get
            {
                SolidColorBrush result = ((MainWindow)Application.Current.MainWindow).MyDataContext.ListRules.ExecuteRulesBackground(Line, Date);

                if (result != null)
                    return result;
                return FileManager.GetFile(FileName)?.DefaultBackground;
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
                return FileManager.GetFile(FileName)?.DefaultForeground;
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
        }
    }
}
