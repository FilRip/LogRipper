using System.Windows;

using LogRipper.ViewModels;

namespace LogRipper.Controls
{
    /// <summary>
    /// Logique d'interaction pour OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
        }

        internal OptionsWindowViewModel MyDataContext
        {
            get { return (OptionsWindowViewModel)DataContext; }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TextBox_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MyDataContext.OpenRulesFilename();
        }
    }
}
