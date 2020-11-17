using Android.Content;
using Android.Support.V7.App;
using Android.Widget;
using GiveMeDateApp;
using GiveMeDateApp.Droid;
using Plugin.CurrentActivity;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MonthYearPickerView), typeof(MonthYearPickerRenderer))]
namespace GiveMeDateApp.Droid
{
    public class MonthYearPickerRenderer : ViewRenderer<MonthYearPickerView, EditText>
    {
        private readonly Context _context;
        private MonthYearPickerDialog _monthYearPickerDialog;

        public MonthYearPickerRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<MonthYearPickerView> e)
        {
            base.OnElementChanged(e);

            CreateAndSetNativeControl();

            Control.KeyListener = null;
            Element.Focused += Element_Focused;
        }

        protected override void Dispose(bool disposing)
        {
            if (Control == null) return;

            Element.Focused -= Element_Focused;

            if (_monthYearPickerDialog != null)
            {
                _monthYearPickerDialog.OnDateTimeChanged -= OnDateTimeChanged;
                _monthYearPickerDialog.OnClosed -= OnClosed;
                _monthYearPickerDialog.Hide();
                _monthYearPickerDialog.Dispose();
                _monthYearPickerDialog = null;
            }

            base.Dispose(disposing);
        }

        #region Private Methods

        private void ShowDatePicker()
        {
            if (_monthYearPickerDialog == null)
            {
                _monthYearPickerDialog = new MonthYearPickerDialog();
                _monthYearPickerDialog.OnDateTimeChanged += OnDateTimeChanged;
                _monthYearPickerDialog.OnClosed += OnClosed;
            }
            _monthYearPickerDialog.Date = Element.Date;
            _monthYearPickerDialog.MinDate = FormatDateToMonthYear(Element.MinDate);
            _monthYearPickerDialog.MaxDate = FormatDateToMonthYear(Element.MaxDate);
            _monthYearPickerDialog.InfiniteScroll = Element.InfiniteScroll;

            var appcompatActivity = CrossCurrentActivity.Current.Activity as AppCompatActivity;
            var mFragManager = appcompatActivity?.SupportFragmentManager;
            if (mFragManager != null)
            {
                _monthYearPickerDialog.Show(mFragManager, nameof(MonthYearPickerDialog));
            }
        }

        private void ClearPickerFocus()
        {
            ((IElementController)Element).SetValueFromRenderer(VisualElement.IsFocusedProperty, false);
            Control.ClearFocus();
        }

        private DateTime? FormatDateToMonthYear(DateTime? dateTime) => 
            dateTime.HasValue ? (DateTime?) new DateTime(dateTime.Value.Year, dateTime.Value.Month, 1) : null;

        private void CreateAndSetNativeControl()
        {
            var tv = new EditText(_context);

            tv.SetTextColor(Element.TextColor.ToAndroid());
            tv.TextSize = (float)Element.FontSize;
            tv.Text = $"{Element.Date.Day:D2} | {Element.Date.Month:D2} | {Element.Date.Year}";
            tv.Gravity = Android.Views.GravityFlags.Center;
            tv.SetBackgroundColor(Element.BackgroundColor.ToAndroid());

            SetNativeControl(tv);
        }

        #endregion

        #region Event Handlers

        private void Element_Focused(object sender, FocusEventArgs e)
        {
            if (e.IsFocused)
            {
                ShowDatePicker();
            }
        }

        private void OnClosed(object sender, DateTime e)
        {
            ClearPickerFocus();
        }

        private void OnDateTimeChanged(object sender, DateTime e)
        {
            Element.Date = e;
            Control.Text = $"{Element.Date.Day:D2} | {Element.Date.Month:D2} | {Element.Date.Year}";
            ClearPickerFocus();
        }

        #endregion
    }
}