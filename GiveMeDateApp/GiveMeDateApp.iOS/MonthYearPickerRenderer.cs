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
        private DateModel _pickerModel;

        protected override void OnElementChanged(ElementChangedEventArgs<MonthYearPickerView> e)
        {
            base.OnElementChanged(e);
            _dateLabel = new UITextField();
            
            var dateToday = DateTime.Today;
            SetupPicker(new DateTime(dateToday.Year, dateToday.Month, 1));
            
            SetNativeControl(_dateLabel);

            Control.EditingChanged += ControlOnEditingChanged;  
            
            if (Element != null)
            {
                Element.PropertyChanged += Element_PropertyChanged;
            }
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
            if (Element != null)
            {
                Element.PropertyChanged -= Element_PropertyChanged;
            }

            base.Dispose(disposing);
        }

        private void SetupPicker(DateTime date)
        {
            var datePicker = new UIPickerView();
            _pickerModel = new DateModel(datePicker, date);
            datePicker.ShowSelectionIndicator = true;
            _selectedDate = date;
            _pickerModel.PickerChanged += (sender, e) =>
            {
                _selectedDate = ((DateModel.PickerChangedEventArgs)e).SelectedValue;
            };
            datePicker.Model = _pickerModel;
            _pickerModel.MaxDate = FormatDateToMonthYear(Element?.MaxDate);
            _pickerModel.MinDate = FormatDateToMonthYear(Element?.MinDate);

            var toolbar = new UIToolbar
            {
                BarStyle = UIBarStyle.Default, 
                Translucent = true
            };
            toolbar.SizeToFit();

            var doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done,
                (s, e) =>
                {
                    if (Element == null) return;
                    
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
                _pickerModel.MaxDate = FormatDateToMonthYear(Element.MaxDate);
            }
            else if (e.PropertyName == MonthYearPickerView.MinDateProperty.PropertyName)
            {
                _pickerModel.MinDate = FormatDateToMonthYear(Element.MinDate);
            }
        }
        
        private DateTime? FormatDateToMonthYear(DateTime? dateTime) => 
            dateTime.HasValue ? (DateTime?) new DateTime(dateTime.Value.Year, dateTime.Value.Month, 1) : null;
    }
}