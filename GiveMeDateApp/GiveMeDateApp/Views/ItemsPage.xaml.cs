using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

using GiveMeDateApp.ViewModels;

namespace GiveMeDateApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = new ItemsViewModel(DependencyService.Get<INumberPickerDialog>());
        }
    }
}