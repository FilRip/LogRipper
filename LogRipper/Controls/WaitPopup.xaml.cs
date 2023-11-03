using System.Windows;

namespace LogRipper.Controls
{
    /// <summary>
    /// Logique d'interaction pour WaitPopup.xaml
    /// </summary>
    public partial class WaitPopup : Window
    {
        public WaitPopup()
        {
            InitializeComponent();
            // TODO : Wait indicator
            // https://learn.microsoft.com/en-us/archive/blogs/dwayneneed/multithreaded-ui-hostvisual
            // https://abrahamheidebrecht.wordpress.com/2009/08/05/creating-a-busy-indicator-in-a-separate-thread-in-wpf/
            // https://dwayneneed.github.io/wpf/2007/04/26/multithreaded-ui-hostvisual.html
        }
    }
}
