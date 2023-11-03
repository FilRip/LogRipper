using System.Linq;
using System.Windows;
using System.Xml.Serialization;

using LogRipper.Controls;
using LogRipper.Helpers;
using LogRipper.Windows;

namespace LogRipper.Models
{
    [XmlRoot()]
    public class OneCategory
    {
        private bool _active;

        public OneCategory()
        {
            Active = true;
        }

        public OneCategory(string category) : this()
        {
            Category = category;
        }

        [XmlElement()]
        public string Category { get; set; }

        [XmlElement()]
        public bool Active
        {
            get { return _active; }
            set
            {
                if (Active != value)
                {
                    _active = value;
                    foreach (OneRule rule in Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListRules.ListRules.Where(r => r.Category == Category))
                    {
                        rule.Active = value;
                        rule.Refresh();
                    }
                    Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.RefreshVisibleLines();
                    Application.Current.GetCurrentWindow<MainWindow>().RefreshMargin();
                }
            }
        }
    }
}
