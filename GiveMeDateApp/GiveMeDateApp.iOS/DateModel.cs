using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UIKit;

namespace GiveMeDateApp.iOS
{
    public class DateModel : UIPickerViewModel
    {
        public event EventHandler PickerChanged;

        #region Fields
        
        private readonly int _numberOfComponents;

        private readonly List<string> _mainNamesOfMonthSource;
        private readonly List<int> _mainYearsSource;
        private readonly UIPickerView _picker;

        private List<string> _namesOfMonth;
        private List<int> _years;

        private DateTime _selectedDate;
        private DateTime? _maxDate;
        private DateTime? _minDate;
        
        #endregion Fields

        #region Constructors
        
        public DateModel(UIPickerView datePicker, DateTime selectedDate)
        {
            _mainNamesOfMonthSource = DateTimeFormatInfo.CurrentInfo?.MonthNames
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
            _mainYearsSource = Enumerable.Range(1900, 2100).ToList();
            _picker = datePicker;
            SelectedDate = selectedDate;
            _numberOfComponents = 2;
            _namesOfMonth = _mainNamesOfMonthSource;
            _years = _mainYearsSource;
        }
        
        #endregion Constructors

        #region Properties
        
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                SetCarrousel(value);
                PickerChanged?.Invoke(this, new PickerChangedEventArgs(value));
            }
        }
        
        public DateTime? MaxDate
        {
            get => _maxDate;
            set
            {
                _maxDate = value;
                ReloadSections(SelectedDate, MinDate, _maxDate);
            }
        }
        
        public DateTime? MinDate
        {
            get => _minDate;
            set
            {
                _minDate = value;
                ReloadSections(SelectedDate, MinDate, _maxDate);
            }
        }
        
        #endregion Properties

        #region Private Methods
        
        private void ReloadSections(DateTime selected, DateTime? maxDate, DateTime? minDate)
        {
            var minYear = 0;
            var minMonth = 0;
            var maxYear = 0;
            var maxMonth = 0;

            selected = selected == DateTime.MinValue ? DateTime.Today : selected;

            if (MinDate.HasValue && MinDate != DateTime.MinValue)
            {
                minYear = MinDate.Value.Year;
                minMonth = MinDate.Value.Month;
            }
            if (MaxDate.HasValue && MaxDate != DateTime.MinValue)
            {
                maxYear = MaxDate.Value.Year;
                maxMonth = MaxDate.Value.Month;
            }

            var monthsSource = _mainNamesOfMonthSource;
            var yearsSource = _mainYearsSource;

            if (minYear > 0)
            {
                yearsSource = yearsSource.Where(x => x >= minYear).ToList();
            }
            if (maxYear > 0)
            {
                yearsSource = yearsSource.Where(x => x <= maxYear).ToList();
            }

            if (selected.Year == maxYear)
            {
                var maxMonthIndex = _mainNamesOfMonthSource.IndexOf(DateTimeFormatInfo.CurrentInfo?.GetMonthName(maxMonth));
                monthsSource = _mainNamesOfMonthSource.Take(maxMonthIndex + 2).ToList();
            }
            if (selected.Year == minYear)
            {
                var minMonthIndex = _mainNamesOfMonthSource.IndexOf(DateTimeFormatInfo.CurrentInfo?.GetMonthName(minMonth));
                monthsSource = _mainNamesOfMonthSource.Skip(minMonthIndex).ToList();
            }
            _namesOfMonth = monthsSource;
            _years = yearsSource;

            _picker.ReloadComponent(0);
            _picker.ReloadComponent(1);

            SetCarrousel(selected);
        }
        
        private int GetNumberOfMonth(string nameOfMonth)
        {
            switch (nameOfMonth)
            {
                case "January":
                    return 1;
                case "February":
                    return 2;
                case "March":
                    return 3;
                case "April":
                    return 4;
                case "May":
                    return 5;
                case "June":
                    return 6;
                case "July":
                    return 7;
                case "August":
                    return 8;
                case "September":
                    return 9;
                case "October":
                    return 10;
                case "November":
                    return 11;
                case "December":
                    return 12;
                default:
                    return -1;
            }
        }
        
        #endregion Private Methods

        #region Public Methods
        public void SetCarrousel(DateTime dateTime)
        {
            var numberOfComponents = _picker.NumberOfComponents;
            if (numberOfComponents == _numberOfComponents)
            {
                _picker.Select(_namesOfMonth.IndexOf(DateTimeFormatInfo.CurrentInfo?.GetMonthName(dateTime.Month)), 0, false);
                _picker.Select(_years.IndexOf(dateTime.Year), 1, false);

                _picker.ReloadComponent(0);
                _picker.ReloadComponent(1);
            }
        }
        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return _numberOfComponents;
        }
        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            switch (component)
            {
                case 0: return _namesOfMonth.Count - 1;
                case 1: return _years.Count;
            }

            return 0;
        }
        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            switch (component)
            {
                case 0: return _namesOfMonth[(int)row];
                case 1: return _years[(int)row].ToString();
            }

            return row.ToString();
        }
        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            var monthRow = pickerView.SelectedRowInComponent(0);
            var yearRow = pickerView.SelectedRowInComponent(1);

            var numberOfMonth = GetNumberOfMonth(_namesOfMonth[(int)monthRow]);

            var selectedDate = new DateTime(_years[(int)yearRow], numberOfMonth, 1);
            ReloadSections(selectedDate, MinDate, _maxDate);
            SelectedDate = selectedDate;
            pickerView.ReloadAllComponents();
        }
        #endregion Public Methods

        public class PickerChangedEventArgs : EventArgs
        {
            public DateTime SelectedValue { get; set; }

            public PickerChangedEventArgs(DateTime selectedValue)
            {
                SelectedValue = selectedValue;
            }
        }
    }
}