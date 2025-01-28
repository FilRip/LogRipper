using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using LogRipper.Helpers;
using LogRipper.ViewModels;
using LogRipper.Windows;

namespace LogRipper.Controls
{
    /// <summary>
    /// Interaction logic for CloseableItem.xaml
    /// </summary>
    public partial class HeaderWithCloseButton : UserControl
    {
        public HeaderWithCloseButton()
        {
            DataContext = this;
            InitializeComponent();
        }

        private TabItemSearch MyTabItem
        {
            get { return (TabItemSearch)Parent; }
        }

        #region Context menu

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            MyTabItem.Dispose();
            LastTabClosed();
        }

        private void CloseAllTab_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel mainWindow = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext;
            for (int i = mainWindow.ListSearchTab.Count - 1; i >= 0; i--)
                if (mainWindow.ListSearchTab[i] is TabItemSearch currentTab)
                    currentTab.Dispose();
            LastTabClosed();
        }

        #endregion

        #region Events

        private void ButtonClose_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ButtonClose.Foreground = Brushes.Red;
        }

        private void ButtonClose_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ButtonClose.Foreground = Brushes.Black;
        }

        #endregion

        private static void LastTabClosed()
        {
            MainWindowViewModel mainWindow = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext;
            if (mainWindow.ListSearchTab.Count == 0)
                mainWindow.ShowSearchResult = false;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            if (MyTabItem is TabItemSearch tab)
                tab.Dispose();
            LastTabClosed();
        }

        private void CloseOtherTab_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel mainWindow = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext;
            for (int i = mainWindow.ListSearchTab.Count - 1; i >= 0; i--)
                if (mainWindow.ListSearchTab[i] is TabItemSearch currentTab && currentTab != MyTabItem)
                    currentTab.Dispose();
            LastTabClosed();
        }
    }
}
