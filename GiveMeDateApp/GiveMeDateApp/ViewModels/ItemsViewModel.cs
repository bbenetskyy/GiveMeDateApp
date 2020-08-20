using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GiveMeDateApp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private readonly INumberPickerDialog _pickerDialog;

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        private string _fillPercentage;
        public string FillPercentage
        {
            get => _fillPercentage;
            set => SetProperty(ref _fillPercentage, value);
        }

        public ICommand FillPercentageCommand { get; }

        public ItemsViewModel(INumberPickerDialog pickerDialog)
        {
            Date = DateTime.Today;
            FillPercentageCommand = new Command(async () => await FillPercentageAction());
            _pickerDialog = pickerDialog;
        }

        private async Task FillPercentageAction()
        {
            var options = new NumberPickerOptions
            {
                DisplaySuffix = "%",
                Step = 5,
                Minimum = 0,
                Maximum = 20,
                Initial = 0
            };
            var (succeed, percentage) = await _pickerDialog.ShowPicker("Select the filled percentage", "Complete", "Cancel", options);

            if (succeed)
            {
                FillPercentage = percentage.ToString();
            }
        }
    }
}
