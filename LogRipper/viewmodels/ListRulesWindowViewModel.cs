using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using LogRipper.Constants;
using LogRipper.Controls;
using LogRipper.Helpers;
using LogRipper.Models;

namespace LogRipper.ViewModels
{
    internal class ListRulesWindowViewModel : ViewModelBase
    {
        #region Fields

        private readonly ICommand _edit, _delete;
        private ObservableCollection<OneRule> _listRules;
        private OneRule _selectedRule;

        #endregion

        #region Constructor

        public ListRulesWindowViewModel() : base()
        {
            _edit = new RelayCommand((param) => Edit());
            _delete = new RelayCommand((param) => Delete());
        }

        #endregion

        #region Properties

        public ICommand CmdEdit
        {
            get { return _edit; }
        }

        public ICommand CmdDelete
        {
            get { return _delete; }
        }

        public ObservableCollection<OneRule> ListRules
        {
            get { return _listRules; }
            set
            {
                if (_listRules != value)
                {
                    _listRules = value;
                    OnPropertyChanged();
                }
            }
        }

        public OneRule SelectedRule
        {
            get { return _selectedRule; }
            set
            {
                if (_selectedRule != value)
                {
                    _selectedRule = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private void Edit()
        {
            if (SelectedRule == null)
                return;
            SelectedRule.Edit();
            OnPropertyChanged(nameof(ListRules));
            OnPropertyChanged(nameof(SelectedRule));
        }

        private void Delete()
        {
            if (SelectedRule == null)
                return;
            if (WpfMessageBox.ShowModal($"{Locale.ASK_CONFIRM_DEL}{Environment.NewLine}{SelectedRule}", Locale.TITLE_CONFIRM_DEL, MessageBoxButton.YesNo))
            {
                OneRule rule = SelectedRule;
                SelectedRule = null;
                Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListRules.RemoveRule(rule);
                ListRules = null;
                ListRules = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListRules.ListRules;
                Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.UpdateCategory();
                OnPropertyChanged(nameof(ListRules));
            }
        }

        internal void SearchRule(IEnumerable<OneRule> listRules)
        {
            if (SelectedRule == null)
                return;
            Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.SearchRule(listRules);
            Application.Current.GetCurrentWindow<ListRulesWindow>().Close();
        }

        #endregion
    }
}
