using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUI3TimerClick.Models
{
    public class TimerModel
    {
        public int SegundosRestantes { get; private set; }

        public TimerModel(int segundosIniciales)
        {
            SegundosRestantes = segundosIniciales;
        }

        public void Decrementar()
        {
            if (SegundosRestantes > 0)
            {
                SegundosRestantes--;
            }
        }

        public bool HaTerminado => SegundosRestantes <= 0;
    }
}
