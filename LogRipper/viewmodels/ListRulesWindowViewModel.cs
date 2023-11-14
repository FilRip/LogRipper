using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LogRipper.Constants;
using LogRipper.Controls;
using LogRipper.Helpers;
using LogRipper.Models;
using LogRipper.Windows;

namespace LogRipper.ViewModels;

internal partial class ListRulesWindowViewModel : ObservableObject
{
    #region Fields

    [ObservableProperty()]
    private ObservableCollection<OneRule> _listRules;
    [ObservableProperty()]
    private OneRule _selectedRule;

    #endregion

    #region Methods

    [RelayCommand()]
    private void Edit()
    {
        if (SelectedRule == null)
            return;
        SelectedRule.Edit();
        OnPropertyChanged(nameof(ListRules));
        OnPropertyChanged(nameof(SelectedRule));
    }

    [RelayCommand()]
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
