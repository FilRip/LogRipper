using System;
using System.Windows;
using System.Windows.Controls;

using LogRipper.Helpers;
using LogRipper.ViewModels;
using LogRipper.Windows;

namespace LogRipper.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabItemSearch.xaml
    /// </summary>
    public partial class TabItemSearch : TabItem, IDisposable
    {
        private bool disposedValue;

        public TabItemSearch()
        {
            InitializeComponent();
            Header = new HeaderWithCloseButton();
            MyHeader.Label_TabTitle.SizeChanged += TabTitle_SizeChanged;
            MyHeader.Label_TabTitle.Content = MyDataContext.Search;
        }

        internal void SetTitle(string title)
        {
            MyHeader.Label_TabTitle.Content = title.Replace("_", "__");
        }

        private void TabTitle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MyHeader.ButtonClose.Margin = new Thickness(MyHeader.Label_TabTitle.ActualWidth + 5, 3, 0, 0);
        }

        internal HeaderWithCloseButton MyHeader
        {
            get { return (HeaderWithCloseButton)Header; }
        }

        internal TabItemSearchViewModel MyDataContext
        {
            get { return (TabItemSearchViewModel)DataContext; }
        }

        public bool IsDisposed
        {
            get { return disposedValue; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DataContext = null;
                    Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListSearchTab.Remove(this);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void DataGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.GetCurrentWindow<MainWindow>().ScrollToSelected();
        }
    }
}
