using System.Windows;

namespace LogRipper.Controls
{
    /// <summary>
    /// Logique d'interaction pour AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
