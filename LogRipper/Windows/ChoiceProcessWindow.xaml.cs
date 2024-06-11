using System.Windows;

using LogRipper.ViewModels;

namespace LogRipper.Windows
{
    /// <summary>
    /// Logique d'interaction pour ChoiceProcessWindow.xaml
    /// </summary>
    public partial class ChoiceProcessWindow : Window
    {
        public ChoiceProcessWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public ChoiceProcessViewModel MyDataContext
        {
            get { return (ChoiceProcessViewModel)DataContext; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyDataContext.ParentWindow = this;
        }
    }
}
