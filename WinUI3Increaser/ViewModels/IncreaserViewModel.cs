using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WinUI3Increaser.Models;

namespace WinUI3Increaser.ViewModels
{
    public partial class IncreaserViewModel : ObservableObject
    {
        // Instanciamos el modelo estricto
        private readonly IncreaserModel _counter = new();

        // Exponemos la propiedad para la Vista.
        // Al cambiar, notificamos a la UI que "ValorUI" se actualizó.
        public int ValorUI => _counter.Valor;

        [RelayCommand]
        private void Incrementar()
        {
            // 1. Ejecutamos la lógica en el modelo
            _counter.IncrementarValor();

            // 2. Notificamos a la vista que la propiedad 'ValorUI' cambió
            // (Esta es una directiva del CommunityToolkit para avisar al x:Bind)
            OnPropertyChanged(nameof(ValorUI));
        }
    }
}
