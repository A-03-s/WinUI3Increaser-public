using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUI3TimerClick.Models;

namespace WinUI3TimerClick.ViewModels
{
    public partial class TimerViewModel : ObservableObject
    {
        private readonly TimerModel _timerModel = new(5); // Empezamos en 60 segundos
        private readonly DispatcherQueueTimer _uiTimer;

        private Action<int> _onTickCallback;

        public TimerViewModel(Action<int> onTick) // Recibimos la acción en el constructor
        {
            _onTickCallback = onTick;
            // Inicializamos el timer que corre en el hilo de la UI de WinUI 3
            _uiTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
            _uiTimer.Interval = TimeSpan.FromSeconds(1);
            _uiTimer.Tick += OnTimerTick;
            _uiTimer.Start();
        }

        private void OnTimerTick(DispatcherQueueTimer sender, object args)
        {
            _timerModel.Decrementar();

            // Notificamos a la UI que las propiedades cambiaron
            OnPropertyChanged(nameof(TiempoUI));

            // Ejecutamos el callback pasando el tiempo actual, es decir, llama a MainViewModel.RefreshButtonStatus(SegundosRestantes)
            // y así refresca el estado del <Button>
            _onTickCallback?.Invoke(_timerModel.SegundosRestantes);

            if (_timerModel.HaTerminado)
            {
                _uiTimer.Stop();
                OnPropertyChanged(nameof(EstadoUI));
            }
        }

        public int TiempoUI => _timerModel.SegundosRestantes;
        public string EstadoUI => _timerModel.HaTerminado ? "¡Juego Terminado!" : "Jugando...";

    }
}
