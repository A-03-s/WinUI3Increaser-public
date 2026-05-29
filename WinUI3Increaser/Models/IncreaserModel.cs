namespace WinUI3Increaser.Models
{
    public class IncreaserModel
    {
        // El dato puro
        public int Valor { get; set; } = 0;

        // Lógica de negocio pura
        public void IncrementarValor()
        {
            Valor++;
        }
    }
}
