using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

    #region Properties

    internal List<OneRule> SelectedItems { get; set; } = [];

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
        List<OneRule> listToDelete = [.. SelectedItems];
        foreach (OneRule rule in listToDelete)
        {
            MessageBoxResult result;
            result = WpfMessageBox.ShowModalReturnButton($"{Locale.ASK_CONFIRM_DEL}{Environment.NewLine}{rule}", Locale.TITLE_CONFIRM_DEL, MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel)
                break;
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListRules.RemoveRule(rule);
                SelectedRule = null;
                SelectedItems.Clear();
                ListRules = null;
                ListRules = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ListRules.ListRules;
                Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.UpdateCategory();
                OnPropertyChanged(nameof(ListRules));
            }
        }
    }

    [RelayCommand()]
    private async Task SearchRule(IEnumerable<OneRule> listRules)
    {
        if (SelectedRule == null)
            WpfMessageBox.ShowModal(Locale.ERROR_SELECT_RULE, Locale.TITLE_ERROR);
        IEnumerable<OneRule> listRulesToSearch = Application.Current.GetCurrentWindow<ListRulesWindow>().ListRulesToManage.SelectedItems.OfType<OneRule>();
        Application.Current.GetCurrentWindow<ListRulesWindow>().Close();
        await Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.SearchRule(listRulesToSearch);
    }

    #endregion
}
