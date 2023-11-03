using System;
using System.Windows;

using LogRipper.ViewModels;

namespace LogRipper.Controls
{
    /// <summary>
    /// Code behind for RuleWindow.xaml
    /// </summary>
    public partial class RuleWindow : Window
    {
        public RuleWindow()
        {
            InitializeComponent();

            if (Properties.Settings.Default.RulePosX != 0)
                Left = Properties.Settings.Default.RulePosX;
            if (Properties.Settings.Default.RulePosY != 0)
                Top = Properties.Settings.Default.RulePosY;
            if (Properties.Settings.Default.RuleSizeX != 0)
                Width = Properties.Settings.Default.RuleSizeX;
            if (Properties.Settings.Default.RuleSizeY != 0)
                Height = Properties.Settings.Default.RuleSizeY;
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.RuleWS) && Enum.TryParse(Properties.Settings.Default.RuleWS, out WindowState newState))
                WindowState = newState;
            MyDataContext.SetCloseAction(Close);
        }

        internal RuleWindowViewModel MyDataContext
        {
            get { return (RuleWindowViewModel)DataContext; }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = MyDataContext.DialogResult;
            Properties.Settings.Default.RulePosX = Left;
            Properties.Settings.Default.RulePosY = Top;
            Properties.Settings.Default.RuleSizeX = Width;
            Properties.Settings.Default.RuleSizeY = Height;
            Properties.Settings.Default.RuleWS = WindowState.ToString("G");
            Properties.Settings.Default.Save();
        }
    }
}
