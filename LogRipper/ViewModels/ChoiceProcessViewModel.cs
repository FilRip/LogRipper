using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LogRipper.Helpers;
using LogRipper.Windows;

namespace LogRipper.ViewModels;

public partial class ChoiceProcessViewModel : ObservableObject
{
    public ChoiceProcessViewModel()
    {
        _listProcess = [];
        foreach (Process process in Process.GetProcesses().OrderBy(p => p.ProcessName))
            _listProcess.Add($"{process.ProcessName} ({process.Id})");
        OnPropertyChanged(nameof(ListProcess));
    }

    [ObservableProperty()]
    private ObservableCollection<string> _listProcess;
    [ObservableProperty()]
    private string _selectedProcess;
    public Window ParentWindow { get; set; }

    [RelayCommand()]
    private void AttachToProcess()
    {
        if (string.IsNullOrWhiteSpace(SelectedProcess))
            return;
        uint pId = uint.Parse(SelectedProcess.Substring(SelectedProcess.LastIndexOf('(') + 1).Replace(")", ""));
        NativeMethods.FreeConsole();
        if (!NativeMethods.AttachConsole(pId))
        {
            WpfMessageBox.ShowModal(Constants.Locale.ERROR_ATTACH_CONSOLE, Constants.Locale.TITLE_ERROR, MessageBoxButton.OK, ParentWindow);
            return;
        }
        ParentWindow.Close();
        Task.Run(() => Application.Current.Dispatcher.Invoke(() => Application.Current.GetCurrentWindow<MainWindow>().MyDataContext.ReadFromConsole(SelectedProcess)));
    }
}
