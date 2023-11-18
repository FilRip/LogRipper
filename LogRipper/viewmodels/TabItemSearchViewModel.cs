using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LogRipper.Constants;
using LogRipper.Helpers;
using LogRipper.Models;
using LogRipper.Windows;

using Microsoft.Win32;

namespace LogRipper.ViewModels;

public partial class TabItemSearchViewModel : ObservableObject
{
    [ObservableProperty()]
    private List<OneLine> _listResult;
    [ObservableProperty()]
    private OneLine _selectedLine;
    [ObservableProperty()]
    private string _search;
    private ECurrentSearchMode _currentSearchMode;
    private List<OneRule> _listSearchRules;
    [ObservableProperty()]
    private bool _currentShowNumLine, _currentShowFileName;

    internal void SetNewSearch(List<OneLine> result, ECurrentSearchMode currentSearchMode, string search = null, List<OneRule> rules = null)
    {
        ListResult = result;
        _currentSearchMode = currentSearchMode;
        Search = search;
        _listSearchRules = rules;
        OnPropertyChanged(nameof(WhatToSearch));
    }

    public string WhatToSearch
    {
        get
        {
            string ret = Search;
            if (_currentSearchMode == ECurrentSearchMode.BY_RULES)
                ret = _listSearchRules[0].ToString();
            if (!string.IsNullOrWhiteSpace(Locale.LBL_RESULT))
                return $"{ret} " + string.Format(Locale.LBL_RESULT, ListResult?.Count ?? 0);
            else
                return "";
        }
    }

    [RelayCommand()]
    private async Task SaveSearchResult()
    {
        Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ActiveProgressRing = true;
        await Task.Delay(10);
        SaveFileDialog dialog = new()
        {
            Filter = "Log files|*.log",
        };
        if (dialog.ShowDialog() == true)
        {
            try
            {
                if (File.Exists(dialog.FileName))
                    File.Delete(dialog.FileName);
                File.WriteAllLines(dialog.FileName, ListResult.Select(l => l.Line), Encoding.Default);
            }
            catch (Exception ex)
            {
                WpfMessageBox.ShowModal(Locale.ERROR_SAVE_FILE + Environment.NewLine + ex.Message, Locale.TITLE_ERROR);
            }
        }
        Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ActiveProgressRing = false;
    }
}
