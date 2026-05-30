using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUI3TimerClick.Models;

namespace WinUI3TimerClick.ViewModels
{
    public partial class IncreaserViewModel : ObservableObject
    {
        private readonly IncreaserModel _counter = new();

        public int ValorUI => _counter.Valor;

        [RelayCommand]
        private void Incrementar()
        {
            _counter.IncrementarValor();
            OnPropertyChanged(nameof(ValorUI));
        }

    }
}
