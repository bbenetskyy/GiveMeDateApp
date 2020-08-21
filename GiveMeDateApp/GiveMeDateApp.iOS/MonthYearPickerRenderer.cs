using System;
using GiveMeDateApp;
using GiveMeDateApp.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MonthYearPickerView), typeof(MonthYearPickerRenderer))]
namespace GiveMeDateApp.iOS
{
    public class MonthYearPickerRenderer : ViewRenderer<MonthYearPickerView, UITextField>
    {
        private DateTime _selectedDate;
        private UITextField _dateLabel;
        private PickerDateModel _pickerModel;

        protected override void OnElementChanged(ElementChangedEventArgs<MonthYearPickerView> e)
        {
            base.OnElementChanged(e);
            _dateLabel = new UITextField();
            
            var dateToday = DateTime.Today;
            SetupPicker(new DateTime(dateToday.Year, dateToday.Month, 1));
            
            SetNativeControl(_dateLabel);

            Control.EditingChanged += ControlOnEditingChanged;
            Element.PropertyChanged += Element_PropertyChanged;
        }

        private void ControlOnEditingChanged(object sender, EventArgs e)
        {
            var currentDate = $"{Element.Date.Month:D2} | {Element.Date.Year}";
            if (_dateLabel.Text != currentDate)
            {
                _dateLabel.Text = currentDate;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Element.PropertyChanged -= Element_PropertyChanged;
            base.Dispose(disposing);
        }

        private void SetupPicker(DateTime date)
        {
            var datePicker = new UIPickerView();
            _pickerModel = new PickerDateModel(datePicker, date);
            datePicker.ShowSelectionIndicator = true;
            _selectedDate = date;
            _pickerModel.PickerChanged += (sender, e) =>
            {
                _selectedDate = e;
            };
            datePicker.Model = _pickerModel;
            _pickerModel.MaxDate = Element.MaxDate ?? DateTime.MaxValue;
            _pickerModel.MinDate = Element.MinDate ?? DateTime.MinValue;

            var toolbar = new UIToolbar
            {
                BarStyle = UIBarStyle.Default, 
                Translucent = true
            };
            toolbar.SizeToFit();

            var doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done,
                (s, e) =>
                {
                    Element.Date = _selectedDate;
                    _dateLabel.Text = $"{Element.Date.Month:D2} | {Element.Date.Year}";
                    _dateLabel.ResignFirstResponder();
                });

            toolbar.SetItems(new[] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), doneButton }, true);

            _dateLabel.InputView = datePicker;
            _dateLabel.Text = $"{Element.Date.Month:D2} | {Element.Date.Year}";
            _dateLabel.InputAccessoryView = toolbar;
            _dateLabel.TextColor = Element.TextColor.ToUIColor();
        }

        private void Element_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MonthYearPickerView.MaxDateProperty.PropertyName)
            {
                _pickerModel.MaxDate = Element.MaxDate ?? DateTime.MinValue;
            }
            else if (e.PropertyName == MonthYearPickerView.MinDateProperty.PropertyName)
            {
                _pickerModel.MinDate = Element.MinDate ?? DateTime.MaxValue;
            }
        }
    }
}