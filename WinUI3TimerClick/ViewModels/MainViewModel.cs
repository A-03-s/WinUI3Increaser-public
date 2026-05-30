using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace WinUI3TimerClick.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isButtonEnabled = true;
        
        // Exponemos los sub-viewmodels como propiedades
        public IncreaserViewModel ContadorVM { get; } = new();
        public TimerViewModel TimerVM { get; }

        public MainViewModel()
        {
            // Al crear el TimerVM, le decimos que ejecute RefreshButtonStatus en cada tick
            TimerVM = new TimerViewModel((segundos) => {
                RefreshButtonStatus(segundos.ToString());
            });
        }

        public void RefreshButtonStatus(string sTimer)
        {
            IsButtonEnabled = (sTimer != "0") ? true : false;
        }
    }
}
