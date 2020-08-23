using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;
using Debug = System.Diagnostics.Debug;

namespace GiveMeDateApp.Droid
{
    public class MonthYearPickerDialog : Android.Support.V4.App.DialogFragment
    {
        public event EventHandler<DateTime> OnDateTimeChanged;
        public event EventHandler<DateTime> OnClosed;

        #region Private Fields

        private const int DefaultDay = 1;
        private const int MinNumberOfMonths = 1;
        private const int MaxNumberOfMonths = 12;
        private const int MinNumberOfYears = 1900;
        private const int MaxNumberOfYears = 2100;

        private NumberPicker _monthPicker;
        private NumberPicker _yearPicker;

        #endregion

        #region Public Properties
        
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public DateTime? Date { get; set; }
        public bool InfiniteScroll { get; set; }

        #endregion

        public void Hide() => base.Dialog?.Hide();

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var builder = new AlertDialog.Builder(Activity);
            var inflater = Activity.LayoutInflater;

            var selectedDate = GetSelectedDate();

            var dialog = inflater.Inflate(Resource.Layout.date_picker_dialog, null);
            _monthPicker = (NumberPicker)dialog.FindViewById(Resource.Id.picker_month);
            _yearPicker = (NumberPicker)dialog.FindViewById(Resource.Id.picker_year);

            InitializeMonthPicker(selectedDate.Month);
            InitializeYearPicker(selectedDate.Year);
            SetMaxMinDate(MaxDate, MinDate);

            builder.SetView(dialog)
                .SetPositiveButton("Ok", (sender, e) =>
                {
                    selectedDate = new DateTime(_yearPicker.Value, _monthPicker.Value, DefaultDay);
                    OnDateTimeChanged?.Invoke(dialog, selectedDate);
                })
                .SetNegativeButton("Cancel", (sender, e) =>
                {
                    Dialog.Cancel();
                    OnClosed?.Invoke(dialog, selectedDate);
                });
            return builder.Create();
        }

        protected override void Dispose(bool disposing)
        {
            if (_yearPicker != null)
            {
                _yearPicker.ScrollChange -= YearPicker_ScrollChange;
                _yearPicker.Dispose();
                _yearPicker = null;
            }

            _monthPicker?.Dispose();
            _monthPicker = null;


            base.Dispose(disposing);
        }

        #region Private Methods

        private DateTime GetSelectedDate() => Date ?? DateTime.Now;

        private void InitializeYearPicker(int year)
        {
            _yearPicker.MinValue = MinNumberOfYears;
            _yearPicker.MaxValue = MaxNumberOfYears;
            _yearPicker.Value = year;
            _yearPicker.ScrollChange += YearPicker_ScrollChange;
            if (!InfiniteScroll)
            {
                _yearPicker.WrapSelectorWheel = false;
                _yearPicker.DescendantFocusability = DescendantFocusability.BlockDescendants;
            }
        }

        private void InitializeMonthPicker(int month)
        {
            _monthPicker.MinValue = MinNumberOfMonths;
            _monthPicker.MaxValue = MaxNumberOfMonths;
            _monthPicker.SetDisplayedValues(GetMonthNames());
            _monthPicker.Value = month;
            if (!InfiniteScroll)
            {
                _monthPicker.WrapSelectorWheel = false;
                _monthPicker.DescendantFocusability = DescendantFocusability.BlockDescendants;
            }
        }

        private void YearPicker_ScrollChange(object sender, View.ScrollChangeEventArgs e)
        {
            SetMaxMinDate(MaxDate, MinDate);
        }

        private void SetMaxMinDate(DateTime? maxDate, DateTime? minDate)
        {
            try
            {
                if (maxDate.HasValue)
                {
                    var maxYear = maxDate.Value.Year;
                    var maxMonth = maxDate.Value.Month;

                    if (_yearPicker.Value == maxYear)
                    {
                        _monthPicker.MaxValue = maxMonth;
                    }
                    else if (_monthPicker.MaxValue != MaxNumberOfMonths)
                    {
                        _monthPicker.MaxValue = MaxNumberOfMonths;
                    }

                    _yearPicker.MaxValue = maxYear;
                }

                if (minDate.HasValue)
                {
                    var minYear = minDate.Value.Year;
                    var minMonth = minDate.Value.Month;

                    if (_yearPicker.Value == minYear)
                    {
                        _monthPicker.MinValue = minMonth;
                    }
                    else if (_monthPicker.MinValue != MinNumberOfMonths)
                    {
                        _monthPicker.MinValue = MinNumberOfMonths;
                    }

                    _yearPicker.MinValue = minYear;
                }
                _monthPicker.SetDisplayedValues(GetMonthNames(_monthPicker.MinValue));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
        }

        private string[] GetMonthNames(int start = 1) => 
            System.Globalization.DateTimeFormatInfo.CurrentInfo?.MonthNames.Skip(start - 1).ToArray();

        #endregion

    }
}
