using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Globalization;
using Windows.UI;

namespace WinUI3TimerClick.Converters
{
    public class EnabledToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isEnabled = (value is bool b) && b;

            if (isEnabled)
            {
                // Si el parámetro está vacío, usamos Blanco por defecto
                if (parameter == null) return new SolidColorBrush(Colors.White);

                // Intentamos convertir el parámetro (string) a un Color real
                string colorHex = parameter.ToString();
                return new SolidColorBrush(GetColorFromHex(colorHex));
            }

            // Estado Deshabilitado: Gris con opacidad
            //            return new SolidColorBrush(Colors.Gray) { Opacity = 0.5 };
            return new SolidColorBrush(Colors.Red) { Opacity = 0.5 };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();

        private Color GetColorFromHex(string input)
        {
            if (string.IsNullOrEmpty(input)) return Colors.White;

            // Si es un código hexadecimal (ej: #FF5733 o #5733)
            if (input.StartsWith("#"))
            {
                string hex = input.Replace("#", "");

                // Si solo enviaron RGB (ej: 5733), asumimos Alpha completo (FF)
                if (hex.Length == 6) hex = "FF" + hex;

                try
                {
                    // Convertimos el string hexadecimal a un valor entero (UInt32)
                    uint val = uint.Parse(hex, NumberStyles.HexNumber);

                    // Extraemos los componentes
                    byte a = (byte)((val >> 24) & 0xff);
                    byte r = (byte)((val >> 16) & 0xff);
                    byte g = (byte)((val >> 8) & 0xff);
                    byte b = (byte)((val) & 0xff);

                    return Color.FromArgb(a, r, g, b);
                }
                catch
                {
                    return Colors.Gray; // Color de respaldo si el hex es inválido
                }
            }

            // Si es un nombre de color conocido
            return input.ToLower() switch
            {
                "white" => Colors.White,
                "skyblue" => Colors.SkyBlue,
                "orange" => Colors.Orange,
                "lightgreen" => Colors.LightGreen,
                "gray" => Colors.Gray,
                "aliceblue" => Colors.AliceBlue,
                _ => Colors.White
            };
        }
    }

}


