using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

using LogRipper.ViewModels;

namespace LogRipper.Helpers
{
    /// <summary>
    /// Logique d'interaction pour InputBoxWindow.xaml
    /// </summary>
    public partial class InputBoxWindow : Window
    {
        public InputBoxWindow()
        {
            InitializeComponent();

            if (Properties.Settings.Default.InputBoxPosX != 0)
                Left = Properties.Settings.Default.InputBoxPosX;
            if (Properties.Settings.Default.InputBoxPosY != 0)
                Top = Properties.Settings.Default.InputBoxPosY;
        }

        private void CommonInit(string title, string question)
        {
            Title = title;
            TxtQuestion.Text = question.Replace(@"\r\n", Environment.NewLine).Replace(@"\r", Environment.NewLine).Replace(@"\n", Environment.NewLine);
        }

        internal void ShowModal(string title, string question, string defaultValue = "")
        {
            CommonInit(title, question);
            TxtUserEdit.ItemsSource = new List<string>() { defaultValue };
            TxtUserEdit.Text = defaultValue;
            TxtUserEdit.SelectedIndex = 0;
            ShowDialog();
        }

        internal void ShowModal(string title, string question, string[] defaultValue)
        {
            CommonInit(title, question);
            TxtUserEdit.ItemsSource = defaultValue;
            TxtUserEdit.SelectedIndex = 0;
            ShowDialog();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.InputBoxPosX = Left;
            Properties.Settings.Default.InputBoxPosY = Top;
            Properties.Settings.Default.Save();
        }

        private void TxtUserEdit_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
                BtnOk_Click(this, null);
            else if (e.Key == Key.Escape)
                Close();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                // HACK : Send tab to enter in TxtUserEdit (Focus, Keyboard Focus & FocusManager doesn't work)
                InputManager.Current.ProcessInput(new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Tab) { RoutedEvent = Keyboard.KeyDownEvent });
                InputManager.Current.ProcessInput(new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Tab) { RoutedEvent = Keyboard.KeyUpEvent });
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        internal InputBoxViewModel MyDataContext
        {
            get { return (InputBoxViewModel)DataContext; }
        }
    }
}
