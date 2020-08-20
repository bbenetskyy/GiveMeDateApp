using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using GiveMeDateApp.Droid;
using Xamarin.Forms;
using AlertDialog = Android.App.AlertDialog;
using Application = Android.App.Application;

[assembly: Dependency(typeof(NumberPickerDialog))]
namespace GiveMeDateApp.Droid
{
    public class NumberPickerDialog: DialogFragment, INumberPickerDialog
    {
        private AlertDialog _dialog;
        
        public Task<(bool, int)> ShowPicker(string title, string okButtonText, string cancelButtonText, NumberPickerOptions options)
        {
            var tcs = new TaskCompletionSource<(bool, int)>();

            Task.Run(() =>
            {
                Application.SynchronizationContext.Post(ignored =>
                {
                    var view = Activity.LayoutInflater.Inflate(Resource.Layout.number_picker_dialog, null);
                    var numberPicker = view.FindViewById<NumberPicker>(Resource.Id.number_picker);

                    numberPicker.MaxValue = options.Maximum - 1;
                    numberPicker.MinValue = options.Minimum;
                    numberPicker.Value = options.Initial <= options.Minimum ? options.Minimum : options.Initial;

                    var values = new List<string>();
                    for (var i = options.Minimum; i < options.Maximum; i += 1)
                    {
                        values.Add((i * options.Step) + options.DisplaySuffix);
                    }
                    numberPicker.SetDisplayedValues(values.ToArray());

                    var builder = new AlertDialog.Builder(Activity)//todo here we have bug!!!
                        .SetTitle(title)
                        .SetView(view)
                        .SetPositiveButton(okButtonText, (x, y) => tcs.TrySetResult((true, numberPicker.Value)))
                        .SetNegativeButton(cancelButtonText, (x, y) => tcs.TrySetResult((false, numberPicker.MinValue)));

                    _dialog = null;
                    _dialog = builder.Create();
                    _dialog.Show();
                }, null);
            });

            return tcs.Task;
        }
    }
}