using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUI3TimerClick.Models
{
    public class IncreaserModel
    {
        public int Valor { get; set; } = 0;

        public void IncrementarValor()
        {
            Valor++;
        }
    }
}
