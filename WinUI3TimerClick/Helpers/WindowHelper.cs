using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;
using Windows.Graphics;


namespace WinUI3TimerClick.Helpers
{
    /// <summary>
    /// Expone la propiedad adjunta para el XAML.
    /// </summary>
    public static class WindowExtensions
    {
        public static readonly DependencyProperty AutoSizeToWindowProperty =
            DependencyProperty.RegisterAttached(
                "AutoSizeToWindow",
                typeof(Window),
                typeof(WindowExtensions),
                new PropertyMetadata(null, OnAutoSizeToWindowChanged));

        public static void SetAutoSizeToWindow(FrameworkElement element, Window value) =>
            element.SetValue(AutoSizeToWindowProperty, value);

        public static Window GetAutoSizeToWindow(FrameworkElement element) =>
            (Window)element.GetValue(AutoSizeToWindowProperty);

        private static void OnAutoSizeToWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement container && e.NewValue is Window window)
            {
                // Delegamos la ejecución al Helper
                WindowHelper.RegisterBehavior(container, window);
            }
        }
    }

    /// <summary>
    /// Implementa la lógica de redimensión de WinUI 3.
    /// </summary>
    public static class WindowHelper
    {
        public static void RegisterBehavior(FrameworkElement container, Window window)
        {
            // Forzamos alineaciones para obtener medidas reales
            container.HorizontalAlignment = HorizontalAlignment.Left;
            container.VerticalAlignment = VerticalAlignment.Top;

            container.SizeChanged += (sender, e) =>
            {
                // Acceso a la API de bajo nivel para redimensionar la ventana de escritorio
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
                var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

                if (appWindow != null)
                {
                    // OBTENEMOS EL FACTOR DE ESCALADO (DPI)
                    // Para que los pixeles de WinUI se traduzcan bien a pixeles de pantalla
                    double rasterizationScale = container.XamlRoot?.RasterizationScale ?? 1.0;

                    // CALCULAMOS EL TAMAÑO DESEADO DEL ÁREA DE CONTENIDO
                    // Multiplicamos por la escala para que se vea igual en cualquier monitor
                    SizeInt32 clientSize = new SizeInt32
                    {
                        Width = (int)(e.NewSize.Width * rasterizationScale),
                        Height = (int)(e.NewSize.Height * rasterizationScale)
                    };

                    // EL TRUCO: Redimensionamos el CLIENT AREA, no la ventana completa
                    appWindow.ResizeClient(clientSize);
                }
            };
        }
    }
}
