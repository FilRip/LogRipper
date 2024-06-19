using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LogRipper.Constants;

using LogRipper.Helpers;
using LogRipper.Windows;

namespace LogRipper.ViewModels
{
    internal partial class InputBoxViewModel : ObservableObject
    {
        #region Fields

        private DateTime? _minDate, _maxDate;
        [ObservableProperty()]
        private bool _filterByDate;
        [ObservableProperty()]
        private DateTime? _startDateTime, _endDateTime;

        #endregion

        #region Constructors

        public InputBoxViewModel()
        {
            ResetMinMaxDate();
            _startDateTime = DateTime.MinValue;
            _endDateTime = DateTime.MaxValue;
        }

        #endregion

        #region Properties

        public DateTime? MinDate
        {
            get { return _minDate; }
        }

        public DateTime? MaxDate
        {
            get { return _maxDate; }
        }

        public Func<InputBoxWindow, Task> ExecuteIfOk { get; set; }

        #endregion

        #region Methods

        private void ResetMinMaxDate()
        {
            _minDate = DateTime.MinValue;
            _maxDate = DateTime.MaxValue;
            OnPropertyChanged(nameof(MinDate));
            OnPropertyChanged(nameof(MaxDate));
        }

        internal void SetMinMaxDate(DateTime minDate, DateTime maxDate)
        {
            _minDate = minDate;
            _maxDate = maxDate;
            OnPropertyChanged(nameof(MinDate));
            OnPropertyChanged(nameof(MaxDate));
        }

        [RelayCommand()]
        private void ChangeFilterByDate()
        {
            MainWindowViewModel dataContext = Application.Current.GetCurrentWindow<MainWindow>().MyDataContext;
            if (dataContext.ListFiles?.Count == 0 || (dataContext.ListFiles.ToList().TrueForAll(f => string.IsNullOrWhiteSpace(f.DateFormat))))
            {
                WpfMessageBox.ShowModal(Locale.NO_DATEFORMAT_IN_FILE, Locale.TITLE_ERROR);
                return;
            }
            if (StartDateTime == DateTime.MinValue)
            {
                try
                {
                    StartDateTime = dataContext.ListLines.Where(line => line.Date != DateTime.MinValue).Min(line => line.Date);
                    EndDateTime = dataContext.ListLines.Where(line => line.Date != DateTime.MinValue).Max(line => line.Date);
                }
                catch (Exception)
                {
                    StartDateTime = DateTime.MinValue;
                    EndDateTime = DateTime.MaxValue;
                }
                _minDate = StartDateTime;
                _maxDate = EndDateTime;
                OnPropertyChanged(nameof(MinDate));
                OnPropertyChanged(nameof(MaxDate));
            }
            FilterByDate = !FilterByDate;
        }

        #endregion
    }
}
