using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WinUI3Increaser.ViewModels
{
    public partial class IncreaserViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _valor = 0; // El "Model" simplificado

        [RelayCommand]
        private void Incrementar()
        {
            Valor++; // Lógica de negocio/presentación
        }
    }
}
