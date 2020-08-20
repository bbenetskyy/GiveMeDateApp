using System.Threading.Tasks;

namespace GiveMeDateApp
{
    public interface INumberPickerDialog
    {
        Task<(bool, int)> ShowPicker(string title, string okButtonText, string cancelButtonText, NumberPickerOptions options);
    }
}