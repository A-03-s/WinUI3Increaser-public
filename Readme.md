# ⭐ App WinUI 3 con implementación del patrón MVVM

Para que veas cómo se materializa la arquitectura MVVM en **WinUI 3**, vamos a conectar todos los puntos que la componen en un ejemplo clásico: un contador.

En una primera aproximación, mantendremos fusionado el Model al ViewModel para simplificar la arquitectura. Pero en una segunda implementación del mismo ejemplo, seremos más estrictos y haremos un Model independiente, respetando la estructura teórica del patrón MVVM.

En ambos casos, veremos cómo el **ViewModel** toma el control y deja al **Code-behind** prácticamente vacío.

## 1. La Aplicación "WinUI3Increaser"

A continuación, vamos a dar el paso a paso desde la creación del proyecto hasta la ejecución de la aplicación con **Visual Studio 2022**.

Si deseas descargar la solución completa para VS2022 puedes hacer clic [aquí](./Code/WinUI3Increaser_1.zip).

El decálogo de tareas es el siguiente:

1) Crear el nuevo proyecto "WinUI3Increaser" usando la plantilla "WinUI Blank App (Packaged)"

1.a)

![](./docs/img/Pasted%20image%2020260521210539.png)

1.b)

![](./docs/img/Pasted%20image%2020260521210650.png)

2) En el "Explorador de soluciones" crear la carpeta "ViewModels"
3) Entrar la menú "Project | Manage NuGet Packages ..." e instalar el paquete "CommunityToolkit.Mvvm"
4) Crear la nueva clase "IncreaserViewModel.cs" en la carpeta "ViewModels"
5) Sobreescribir el archivo "IncreaserViewModel.cs" con el contenido mostrado en el punto [1.1. ViewModel - IncreaserViewModel.cs](#11-el-viewmodel---increaserviewmodelcs)
6) Sobreescribir el archivo "MainWindow.xaml" con el contenido mostrado en el punto [1.2. La View (XAML) - MainWindow.xaml](#12-la-view-(xaml)---mainwindowxaml)
7) Sobreescribir el archivo "MainWindow.xaml.cs" con el contenido mostrado en el punto [1.3. El code-behind - MainWindow.xaml.cs](#13-el-code-behind---mainwindowxamlcs)
8) Build | Rebuild Solution
9) Build | Deploy Solution
10) Debug | Start Without Debugging.
    Al ejecutar la aplicación deberíamos ver una ventana similar a la siguiente:

![](./docs/img/Pasted%20image%2020260521210906.png)

Cada vez que presiones el `<Button>` "Sumar 1" deberíamos ver incrementarse el valor mostrado en el "`<TextBlock>`" ubicado arriba del botón.

### 1.1. El ViewModel - IncreaserViewModel.cs

Aquí usamos el **CommunityToolkit.Mvvm**, que es el estándar actual. Nota cómo no hay ninguna referencia a "botones" o "colores", solo a datos y acciones.

```csharp
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
```

### 1.2. La View (XAML) - MainWindow.xaml

En WinUI 3, usamos `{x:Bind}` para conectar la interfaz con el ViewModel. Es rápido y seguro.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Window
    x:Class="WinUI3Increaser.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:WinUI3Increaser"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    Title="WinUI3Increaser">

    <Window.SystemBackdrop>
        <MicaBackdrop />
    </Window.SystemBackdrop>

    <Grid>

        <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center">
            <TextBlock Text="{x:Bind ViewModel.Valor, Mode=OneWay}" 
                       FontSize="48" 
                       HorizontalAlignment="Center" />

            <Button Content="Sumar 1" 
		            Command="{x:Bind ViewModel.IncrementarCommand}" 
	                HorizontalAlignment="Center" />
        </StackPanel>
    
    </Grid>
</Window>
```

### 1.3. El Code-behind - MainWindow.xaml.cs

En una arquitectura MVVM pura, el archivo `.xaml.cs` solo se encarga de instanciar el ViewModel y poco más.

```csharp
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUI3Increaser.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3Increaser
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public IncreaserViewModel ViewModel { get; set; }
        public MainWindow()
        {
            ViewModel = new IncreaserViewModel();
            InitializeComponent();
        }
    }
}
```

> [!NOTE]
> **¿Por qué esto es una "Arquitectura"?**
>
> Al separar el código así, has creado una estructura donde:
>
> * **Puedes cambiar la UI:** Podrías cambiar el `TextBlock` por un `ProgressBar` en el XAML y el `ViewModel` seguiría funcionando exactamente igual.
> * **Puedes testear la lógica:** Podrías crear un proyecto de Tests unitarios que instancie `IncreaserViewModel`, ejecute `IncrementarCommand` y verifique si `Valor` es 1, **sin llegar a abrir nunca la ventana de la app**.
> * **Independencia del Modelo:** Si mañana los datos no vienen de una variable local sino de una base de datos SQL, solo cambias el interior del método en el ViewModel (o un Servicio), pero la Vista ni se entera.
>
> Esta separación es la que permite que las apps de WinUI 3 crezcan de forma organizada sin convertirse en un "código espagueti" donde todo está mezclado.

---

## 2. Seamos más estrictos

Como comentamos en los párrafos iniciales de este documento, la primera implementación de "WinUI3Increaser" simplificó el modelo, integrando el "Model" al "ViewModel".

Es común que en los ejemplos sencillos se suela fusionar el "Model" dentro del "ViewModel" para ahorrar líneas, pero en aplicaciones reales esa "simplificación" rompe el principio de responsabilidad única. Separar el modelo nos permitirá escalar la lógica de negocio de forma independiente a la interfaz de usuario.

Aquí mostraremos el código de la misma aplicación reestructurado de manera estricta, manteniendo la magia de los _Source Generators_ de la [Community Toolkit de MVVM](./dotnet-communitytoolkit-mvvm.pdf).

Si deseas descargar la solución completa para VS2022 puedes hacer clic [aquí](./Code/WinUI3Increaser_2.zip).

### 2.1. El Model - Models/IncreaserModel.cs

Esta clase representa puramente tus datos y la lógica de negocio central. No sabe nada de la interfaz de usuario, ni de WinUI, ni de comandos.

Para incluirla debemos crear la carpeta "Models" y en ella el archivo fuente "IncreaserModel.cs".

C#

```csharp
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
```

### 2.2. El ViewModel - ViewModels/IncreaserViewModel.cs

El ViewModel ahora actúa como un verdadero puente. Contiene una instancia del ContadorModel y expone sus propiedades y métodos a la vista, notificando los cambios cuando es necesario.

C#

```csharp
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
```

### 2.3. La View (XAML) - MainWindow.xaml

La vista se mantiene prácticamente igual, solo adaptamos el x:Bind para que apunte a la propiedad expuesta por el ViewModel (ValorUI).

XML

```xml
<?xml version="1.0" encoding="utf-8"?>
<Window
    x:Class="WinUI3Increaser.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:WinUI3Increaser"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    Title="WinUI3Increaser">

    <Window.SystemBackdrop>
        <MicaBackdrop />
    </Window.SystemBackdrop>

    <Grid>
        <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center">
            <!-- Ahora bindeamos a ValorUI -->
            <TextBlock Text="{x:Bind ViewModel.ValorUI, Mode=OneWay}" 
                       FontSize="48" 
                       HorizontalAlignment="Center" />

            <Button Content="Sumar 1" 
                    Command="{x:Bind ViewModel.IncrementarCommand}" 
                    HorizontalAlignment="Center" />
        </StackPanel>
    </Grid>
</Window>
```

### 2.4. El Code-behind - MainWindow.xaml.cs

Se mantiene idéntico a tu ejemplo original, ya que su única responsabilidad en MVVM (en ventanas principales) suele ser instanciar o recibir el ViewModel mediante inyección de dependencias y llamar al inicializador de componentes.

C#

```csharp
using Microsoft.UI.Xaml;
using WinUI3Increaser.ViewModels;

namespace WinUI3Increaser
{
    public sealed partial class MainWindow : Window
    {
        public IncreaserViewModel ViewModel { get; set; }

        public MainWindow()
        {
            ViewModel = new IncreaserViewModel();
            this.InitializeComponent();
        }
    }
}
```

> [!NOTE]
> **¿Qué ganamos al respetar el modelo estricto?**
>
> 1. **Desacoplamiento total:** Si mañana decides cambiar tu ContadorModel para que en lugar de memoria guarde el valor en una base de datos SQLite o un archivo de texto, la Vista (MainWindow) ni se entera.
> 2. **Testabilidad:** Ahora puedes hacerle pruebas unitarias (Unit Tests) a la clase ContadorModel de forma aislada, sin requerir librerías de MVVM ni entornos de UI simulados.

### 2.5. Porqué la clase "IncreaserModel" es un objeto POCO

Como dijimos en el documento que describe las características básicas del patrón MVVM, las clases que componen el modelo suelen ser objetos [POCO](./POCO.md) (Plain Old CLR Object).

 La clase "IncreaserModel" entra perfectamente en la categoría de un **objeto POCO**.

De hecho, el ejemplo es excelente porque ilustra un detalle que a menudo se malinterpreta: un POCO puede tener métodos y lógica, siempre y cuando sea lógica de negocio pura.

Aquí tienes el desglose de por qué la clase  "IncreaserModel" sí califica como POCO:

#### ¿Por qué es un POCO?

- **No depende de frameworks externos:** No hereda de ninguna clase base (DbContext, Controller, etc.) ni implementa interfaces forzadas por herramientas de terceros (como ORMs o frameworks de persistencia).
- **Es portable:** Podrías mover esta clase a un proyecto de consola, una API, una aplicación móvil (MAUI) o una biblioteca de clases, y funcionaría exactamente igual sin romper nada.
- **Lógica de negocio pura (Dominio):** El método IncrementarValor() solo manipula el estado interno de la clase (Valor). No intenta guardar el dato en una base de datos, no hace un Console.WriteLine(), ni llama a un servicio web.

#### La diferencia clave: Anémico vs. Rico

En el mundo del diseño de software, los POCOs se dividen en dos estilos:

| **Tipo de POCO**               | **Características**                                                                                                  | **¿Tu clase califica?**                                                                              |
| ------------------------------------ | --------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------- |
| **Modelo Anémico**            | Solo tiene propiedades (get; set;) y actúa como un simple contenedor de datos. La lógica está en otra parte (servicios). | No, tu clase tiene comportamiento.                                                                          |
| **Modelo Rico (Domain Model)** | Contiene tanto los datos como las reglas de negocio que manipulan esos datos (encapsulamiento).                             | **Sí.** Tu clase es un POCO que sigue las buenas prácticas del Diseño Guiado por el Dominio (DDD). |

> **Veredicto:** La clase  "IncreaserModel" es un POCO de libro. Es simple, independiente, testeable y mantiene el control de su propio estado. ¡Buen trabajo de diseño!

---

## 3. Escalando el modelo

Ahora es momento de hacer crecer nuestro modelo. Para ello, desarrollaremos una aplicacón "real" de **WinUI 3** que nos hará saber cómo escalar **MVVM**.

Podemos imaginar un ejemplo que muestre cómo se vería una implementación con más de un ViewModel, en la que a su vez cada ViewModel sea el intermediario para un Model distinto, que tenga diferentes clases y comandos.

Basada en el ejemplo "WinUI3Increaser", la nueva app a la que llamaremos "**WinUI3TimerClick**" es un sencillo "juego" que además de tener un contador y el botón "Sumar 1", tendrá otro `<TextBlock>` que indique cómo se decrementa segundo a segundo un timer hasta llegar a 0 y  mostrará un mensaje final con la cantidad de clics detectados en el período.

Al separar el temporizador (Timer) del contador de clics (Increaser) en diferentes ViewModels y Models, logramos dos cosas fundamentales en arquitectura de software: **Single Responsibility Principle** (Principio de Responsabilidad Única) y **Decoupling** (Desacoplamiento). Cada componente se encarga de lo suyo.

Para estructurar esto de forma limpia en WinUI 3, usaremos un **ViewModel principal** (o global) que orqueste y contenga a los sub-ViewModels. De este modo, la vista principal (`MainWindow`) solo tiene que conocer a ese ViewModel padre.

Aquí tienes el código completo y estructurado para expandir tu aplicación. Si deseas descargar la solución completa para VS2022 puedes hacer clic [aquí](./Code/WinUI3TimerClick_3.zip).

### 3.1. Los Models

Mantendremos tu modelo original y añadiremos el del temporizador.

#### `Models/IncreaserModel.cs`

Se mantiene igual que en el ejemplo anterior.

```csharp
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
```

#### `Models/TimerModel.cs`

Este modelo maneja la lógica de negocio del tiempo restante. No sabe nada de hilos (threads) de la UI ni de formatos de texto.

```csharp
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
```

### 3.2. Los ViewModels

Aquí viene el cambio clave. Crearemos el `TimerViewModel` y luego un `MainViewModel` que unificará a ambos.

#### `ViewModels/IncreaserViewModel.cs`

_(Igual al tuyo, hereda de `ObservableObject`)_

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
```

#### `ViewModels/TimerViewModel.cs`

Este ViewModel utiliza un `DispatcherQueueTimer` (el temporizador nativo y seguro para hilos de WinUI 3) para actualizar el modelo segundo a segundo.

```csharp
using System;
using Microsoft.UI.Dispatching;
using CommunityToolkit.Mvvm.ComponentModel;
using WinUI3TimerClick.Models;

namespace WinUI3TimerClick.ViewModels
{
    public partial class TimerViewModel : ObservableObject
    {
        private readonly TimerModel _timerModel = new(60); // Empezamos en 60 segundos
        private readonly DispatcherQueueTimer _uiTimer;

        public int TiempoUI => _timerModel.SegundosRestantes;
        public string EstadoUI => _timerModel.HaTerminado ? "¡Juego Terminado!" : "Jugando...";

        public TimerViewModel()
        {
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

            if (_timerModel.HaTerminado)
            {
                _uiTimer.Stop();
                OnPropertyChanged(nameof(EstadoUI));
            }
        }
    }
}
```

#### `ViewModels/MainViewModel.cs`

**La pieza clave.** Este ViewModel actúa como el contenedor raíz de toda la pantalla. Expone los dos sub-ViewModels para que la vista pueda acceder a ellos de forma ordenada.

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace WinUI3TimerClick.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // Exponemos los sub-viewmodels como propiedades
        public IncreaserViewModel ContadorVM { get; } = new();
        public TimerViewModel TimerVM { get; } = new();
    }
}
```

### 3.3. La View (XAML)

Ahora adaptamos el XAML para navegar a través del `MainViewModel` usando rutas de puntos (`ContadorVM.Propiedad`).

#### `MainWindow.xaml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<Window
    x:Class="WinUI3TimerClick.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:WinUI3TimerClick"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    Title="WinUI 3 MVVM - Multi VM Juego">

    <Window.SystemBackdrop>
        <MicaBackdrop />
    </Window.SystemBackdrop>

    <Grid Padding="40">
        <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center" Spacing="30">
        
            <!-- SECCIÓN DEL TEMPORIZADOR (TimerVM) -->
            <StackPanel Spacing="5">
                <TextBlock Text="Tiempo Restante" HorizontalAlignment="Center" Foreground="Gray"/>
                <TextBlock Text="{x:Bind MainVM.TimerVM.TiempoUI, Mode=OneWay}" 
                           FontSize="64" 
                           FontWeight="Bold"
                           HorizontalAlignment="Center" />
                <TextBlock Text="{x:Bind MainVM.TimerVM.EstadoUI, Mode=OneWay}" 
                           HorizontalAlignment="Center" 
                           Foreground="LightGreen"/>
            </StackPanel>

            <MenuFlyoutSeparator />

            <!-- SECCIÓN DEL CONTADOR (ContadorVM) -->
            <StackPanel Spacing="15">
                <TextBlock Text="{x:Bind MainVM.ContadorVM.ValorUI, Mode=OneWay}" 
                           FontSize="48" 
                           HorizontalAlignment="Center" />

                <!-- El comando ahora se busca dentro de ContadorVM -->
                <Button Content="Sumar 1" 
                        Command="{x:Bind MainVM.ContadorVM.IncrementarCommand}" 
                        HorizontalAlignment="Center"
                        Width="120"
                        Style="{ThemeResource AccentButtonStyle}"/>
            </StackPanel>
        
        </StackPanel>
    </Grid>
</Window>
```

### 3.4. El Code-behind

Modificamos la propiedad para que apunte al nuevo `MainViewModel`.

#### `MainWindow.xaml.cs`

```csharp
using Microsoft.UI.Xaml;
using WinUI3TimerClick.ViewModels;

namespace WinUI3TimerClick
{
    public sealed partial class MainWindow : Window
    {
        // Apuntamos al ViewModel contenedor principal
        public MainViewModel MainVM { get; set; }

        public MainWindow()
        {
            MainVM = new MainViewModel();
            this.InitializeComponent();
        }
    }
}
```

> [!NOTE]
> **¿Por qué esta estructura es la correcta?**
>
> 1. **Escalabilidad:** Si mañana quieres añadir un sistema de "Puntuaciones Máximas" (HighScores), solo creas un `HighScoreModel`, un `HighScoreViewModel` y lo agregas como tercera propiedad en el `MainViewModel`. El resto del código no se entera ni se rompe.
> 2. **Encapsulamiento:** El `IncreaserViewModel` no tiene por qué saber cómo funciona un reloj, y el `TimerViewModel` no tiene por qué saber qué pasa cuando el usuario hace clic.
> 3. **Limpieza en el XAML:** Gracias a las rutas de propiedades complejas de `{x:Bind}`, el mapeo en la vista queda jerárquico y muy fácil de leer.

> [!TIP]
>
> ### 🏆 CHALLENGE: ¡Ponlo a prueba!
>
> Intenta modificar el código para que cuando el timer llegue a 0, el mensaje incluya la cantidad de clics contados y a su vez se inhabilite el botón "Sumar 1".
>
> Conviene que reduzcas el valor inicial del temporizador de 60 a 5, como para apreciar más fácilmente qué es lo que sucede.

---

## 4. Agreguemos un Helper

En el ejemplo que venimos desarrollando, seguramente has podido experimentar que la ventana principal de la aplicación es exageradamente grande para el contenido que tiene.

Entonces, para ejemplificar la implementación de un módulo "helper" se nos ocurrió utilizarlo para inicializar el tamaño de la ventana principal, de tal manera que su ancho y alto sean proporcionales al tamaño de los controles que tiene incluidos.

La idea es desarrollar un Helper aplicando el modelo "Attached behavior" para diseñarlo, creando  una class "Extensions" y otra class "Helper".  Si deseas conocer o repasar en qué consiste la técnica "**Attached Behavior**" puedes consultar el siguiente documento: [Attached Behavior](./AttachedBehavior.md).

Sólo mostraremos los nuevos archivos o los fuentes que cambian respecto al ejemplo del punto anterior. Si deseas descargar la solución completa para VS2022 puedes hacer clic [aquí](./Code/WinUI3TimerClick_4.zip).

### 4.1 El Helper

Antes que nada, creamos una nueva carpeta que denominaremos "Helpers" que servirá para albergar todos los módulos del tipo helpers desarrollados en nuestra app.

En esa carpeta creamos el archivo "WindowHelper.cs" que incluye las siguientes clases:

- WindowExtensions
- WindowHelper

En la clase *WindowExtensions* se declara la property *AutoSizeToWindow* y se incluye el handler que atiende su cambio de valor *OnAutoSizeToWindowChanged* . Cuando desde el XAML se ponga a "True", el hilo de ejecución pasará por allí y se registrará en la clase *WindowHelper*.

En este caso, la registración es el comportamiento propiamente dicho, es decir el cambio del tamaño inicial de la ventana principal.

#### Helpers/WindowHelper.cs

```csharp

using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace WinUI3TimerClick.Helpers
{
    /// Expone la propiedad adjunta para el XAML.
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

    /// Implementa la lógica de redimensión de WinUI 3.
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
```

### 4.2. La View (XAML) - MainWindow.xaml

```xml

<?xml version="1.0" encoding="utf-8"?>
<Window
    x:Class="WinUI3TimerClick.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:WinUI3TimerClick"
    xmlns:helpers="using:WinUI3TimerClick.Helpers"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    x:Name="RootWindow"
    Title="WinUI 3 MVVM - Multi VM Juego">

    <Window.SystemBackdrop>
        <MicaBackdrop />
    </Window.SystemBackdrop>

    <Grid helpers:WindowExtensions.AutoSizeToWindow="{x:Bind RootWindow}"
          Padding="40">
        <StackPanel VerticalAlignment="Center" 
                    HorizontalAlignment="Center" 
                    Spacing="30">
    
        ...
    
        </StackPanel>
    </Grid>
</Window>
```

Los detalles del código implementado son los siguientes:

1) **Limpieza y elegancia**:
   Usar un "Attached Behavior" es la cúspide de la elegancia en XAML. Es la forma arquitectónicamente correcta de resolver esto en WinUI 3 porque elimina por completo el código del _code-behind_. (El archivo "MainWindow.xaml.cs" no se toca)

   Así podemos activar este comportamiento directamente desde el XAML usando una propiedad personalizada.
2) **Encapsulamiento:**

   Al estar en el mismo archivo bajo el mismo _namespace_, puedes ver la relación directa entre el Attached Property y su Behavior.
3) **Referencia en XAML:**

   Solo necesitas importar el namespace una vez:
   `xmlns:helpers="using:WinUI3TimerClick.Helpers"`

   asignarle un nombre a la ventana:
   `x:Name="RootWindow"`

   y usar `x:Bind` sobre el `Window`:
   `<Grid helpers:WindowExtensions.AutoSizeToWindow="{x:Bind RootWindow}"`
4) **Manejo de DPI:**

   Un punto a tener en cuenta es que `appWindow.Resize` utiliza píxeles físicos. Si notas que en pantallas con escala (ej. 150%) el tamaño no es exacto, podrías necesitar multiplicar `e.NewSize` por el factor de escala de la pantalla, aunque para la mayoría de herramientas internas, los valores fijos de _padding_ (+40, +60) suelen ser suficientes para evitar recortes.

> [!TIP]
>
> ### 🏆 CHALLENGE: ¡Ponlo a prueba!
>
> Intenta modificar el código para que si el usuario modifica el tamaño de la ventana, el helper cambie el fontsize de los TextBlock.

## 5. El último paso: un Converter

Para completar la lista de módulos de una app WinUI 3 que mencionamos en los primeros apartados de este documento, nos está faltando desarrollar un "Converter".

Lo haremos sobre la base del código de la etapa anterior en la cual implementamos un "Helper", y el "Converter" hará que el botón "Sumar 1" de la app deberá ponerse en color rojo cuando el timer llegue a 0.

El input del módulo será el valor booleano proveniente del estado del botón y el output el color de fondo con el que queremos pintar el botón. Para ser más claros, el botón "Sumar 1" tendrá el color clásico de un botón de Windows cuando esté habilitado, es decir, mientras el timer tenga un valor distinto de cero y el color rojo cuando pase a deshabilitado, o sea, cuando el timer haya terminado.

Como venimos haciendo a medida que avanzamos en los ejemplos, sólo mostraremos los nuevos archivos o los fuentes que cambian respecto al ejemplo del punto anterior. Si deseas descargar la solución completa para VS2022 puedes hacer clic [aquí](./Code/WinUI3TimerClick_5.zip).

### 5.1. El Converter

Antes que nada, creamos una nueva carpeta que denominaremos "Converters" que servirá para albergar todos los módulos del tipo converters desarrollados en nuestra app.

En esa carpeta creamos el archivo "EnabledToColorConverter.cs" que tendrá el siguiente contenido:

#### Converters/EnabledToColorConverter.cs

```csharp
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

            // Estado Deshabilitado: Rojo con opacidad
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
```

En el contexto de tu aplicación **WinUI 3** y siguiendo el patrón **MVVM**, el **Converter** actúa como un "traductor" de datos entre la Vista y el ViewModel.

Su función principal es **transformar un tipo de dato lógico en un tipo de dato visual**.

### 5.2. La View (XAML) - MainWindow.xaml

Luego de crear el módulo "Converter" tenemos que armar el enlace de datos en XAML.

Inicialmente, lo más intuitivo sería establecer un enlace entre el "Background" del `<Button>` y su propio estado disponible en el atributo "IsEnabled". Pero esto no funciona, o por lo menos no lo hace de manera directa. Más abajo describimos el [porqué](#porque-no-funciona-un-enlace-entre-el-background-y-el-isenabled).

Una alternativa es poner el Background del botón a "Transparent" y declarar un `<Border>` como "contenedor" del `<Button>`. De esta manera, sí funciona un binding entre el "Background" del `<Border>` y el "IsEnabled" del `<Button>`, pero tiene algunas desventajas que también explicamos más abajo.

```xml

<?xml version="1.0" encoding="utf-8"?>
<Window
    x:Class="WinUI3TimerClick.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:WinUI3TimerClick"
    xmlns:helpers="using:WinUI3TimerClick.Helpers"
    xmlns:converters="using:WinUI3TimerClick.Converters"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    x:Name="RootWindow"
    Title="WinUI 3 MVVM - Multi VM Juego">

    <Window.SystemBackdrop>
        <MicaBackdrop />
    </Window.SystemBackdrop>

    <Grid helpers:WindowExtensions.AutoSizeToWindow="{x:Bind RootWindow}"
          Padding="40">

        <Grid.Resources>
            <converters:EnabledToColorConverter x:Key="ColorConverter" />
        </Grid.Resources>
    
        <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center" Spacing="30">

            <!-- SECCIÓN DEL TEMPORIZADOR (TimerVM) -->
            <StackPanel Spacing="5">
                <TextBlock Text="Tiempo Restante" HorizontalAlignment="Center" Foreground="Gray"/>
                <TextBlock x:Name="StatusLabel" 
                           Text="{x:Bind MainVM.TimerVM.TiempoUI, Mode=OneWay}" 
                           FontSize="64" 
                           FontWeight="Bold"
                           HorizontalAlignment="Center" />
                <TextBlock Text="{x:Bind MainVM.TimerVM.EstadoUI, Mode=OneWay}" 
                           HorizontalAlignment="Center" 
                           Foreground="LightGreen"/>
            </StackPanel>

            <MenuFlyoutSeparator />

            <!-- SECCIÓN DEL CONTADOR (ContadorVM) -->
            <StackPanel Spacing="15">
                <TextBlock Text="{x:Bind MainVM.ContadorVM.ValorUI, Mode=OneWay}" 
                           FontSize="48" 
                           HorizontalAlignment="Center" />

                <!-- El comando ahora se busca dentro de ContadorVM -->
                <Border Background="{Binding IsEnabled, ElementName=IncrementButton, Mode=OneWay, Converter={StaticResource ColorConverter}, ConverterParameter='#1E90FF'}" 
                        BorderBrush="AliceBlue"
                        BorderThickness="0" 
                        CornerRadius="8" 
                        Padding="0" 
                        HorizontalAlignment="Center">
                    <Button x:Name="IncrementButton" 
                        IsEnabled="{x:Bind MainVM.IsButtonEnabled, Mode=OneWay}"
                        Content="Sumar 1" 
                        Command="{x:Bind MainVM.ContadorVM.IncrementarCommand}" 
                        HorizontalAlignment="Center"
                        Width="120"
                        Style="{ThemeResource AccentButtonStyle}"
                        Background="Transparent"
                        />
                </Border>
            </StackPanel>

        </StackPanel>
    </Grid>
</Window>
```

### ¿Qué hace específicamente en tu código?

1. **Desacoplamiento:** Permite que tu ViewModel (`MainViewModel`) no sepa nada sobre colores o pinceles (`SolidColorBrush`). El ViewModel solo maneja una propiedad booleana `IsButtonEnabled`.
2. **Transformación Dinámica:** Toma el valor booleano de `IsEnabled` del botón y, en tiempo real, lo convierte en un color para el fondo del `Border`:

   - **Si es `true`:** Convierte el parámetro hexadecimal (como `#1E90FF`) en un color azul.
   - **Si es `false`:** Devuelve automáticamente un color rojo con opacidad para indicar visualmente que el juego ha terminado.
3. **Formateo de UI:** Resuelve la incompatibilidad de tipos; la propiedad `Background` espera un `Brush`, pero tu estado lógico es un `bool`. El converter llena ese vacío.

### Análisis del Flujo de Datos

- **Modelo:** `TimerModel` llega a 0.
- **ViewModel:** `MainViewModel` cambia `IsButtonEnabled` a `false`.
- **XAML (Binding):** Detecta el cambio y le pasa ese `false` al **Converter**.
- **Converter:** Recibe el `false`, ejecuta su lógica interna y devuelve un `SolidColorBrush(Colors.Red)`.
- **Vista:** El borde del botón se vuelve rojo instantáneamente.

> [!TIP]
>
> ### 🏆 CHALLENGE: ¡Ponlo a prueba!
>
> Intenta modificar el código para que cuando el timer llegue a 0 no solo cambie el color del botón sino que también el `<TextBlock>` que ahora pone únicamente "Juego terminado", agregue la cantidad de clics que hizo el usuario. Por ejemplo, "Juego terminado con 15 clics".
>
> También podrías intentar agregarle algún botón o menú para reiniciar el juego sin tener que salir y volver a lanzar la aplicación para hacerlo.

### Porqué no funciona un enlace entre el "Background" y el "IsEnabled"

En WinUI 3, intentar cambiar el fondo de un botón basándote solo en `IsEnabled` mediante un simple _Binding_ o un _Setter_ básico suele fallar porque los controles de WinUI utilizan un sistema llamado **Visual States** (Estados Visuales).

Aquí te explico qué está pasando y cómo solucionarlo.

##### ¿Por qué no funciona?

El `Button` en WinUI 3 tiene una propiedad interna llamada **Template**. Dentro de esa plantilla, existen definiciones específicas para cada estado: `Normal`, `PointerOver` (hover), `Pressed` y, el que te interesa, **`Disabled`**.

Cuando `IsEnabled` cambia a `false`, el **VisualStateManager** toma el control y aplica los valores definidos para el estado `Disabled`. Estos valores tienen una **prioridad mayor** que cualquier propiedad que tú asignes directamente en el tag del botón o mediante un estilo simple.

> **Dato clave:** Por defecto, el estado `Disabled` de un botón suele usar un recurso del sistema llamado `ButtonDisabledBackground`. Aunque tú cambies el `Background` a "Red", el estado visual lo sobreescribe con el gris estándar de Windows.

##### La Solución: Sobreescribir los Recursos de Sistema

La forma más limpia y eficiente de hacer esto sin rediseñar toda la plantilla del botón es sobreescribir los **ThemeResources** específicos que el botón busca cuando está deshabilitado.

Puedes hacerlo directamente en el botón o en el `ResourceDictionary` de tu página:

XML

```xml
<Button Content="Mi Botón" HorizontalAlignment="Center">
    <Button.Resources>
        <SolidColorBrush x:Key="ButtonBackgroundDisabled" Color="DarkRed" />
        <SolidColorBrush x:Key="ButtonForegroundDisabled" Color="WhiteSmoke" />
    </Button.Resources>
</Button>
```

##### Los recursos más comunes que querrás cambiar son:

| **Recurso**             | **Descripción**                         |
| ----------------------------- | ---------------------------------------------- |
| `ButtonBackgroundDisabled`  | El color de fondo cuando está deshabilitado.  |
| `ButtonForegroundDisabled`  | El color del texto cuando está deshabilitado. |
| `ButtonBorderBrushDisabled` | El color del borde cuando está deshabilitado. |

##### Alternativa: Crear un Estilo (Reutilizable)

Si quieres que varios botones tengan este comportamiento, lo mejor es definir un estilo en tu `App.xaml` o `Page.Resources`:

XML

```xml
<Style x:Key="CustomDisabledButtonStyle" TargetType="Button">
    <Setter Property="Background" Value="Blue" />
    <Style.Resources>
        <SolidColorBrush x:Key="ButtonBackgroundDisabled" Color="LightBlue" />
    </Style.Resources>
</Style>

<Button Style="{StaticResource CustomDisabledButtonStyle}" Content="Click me" IsEnabled="False" />
```

##### ¿Por qué no usar un Converter?

Podrías usar un `IValueConverter` vinculado a `IsEnabled`, pero seguiría perdiendo la "pelea" contra el **VisualStateManager**. La única forma de que un Converter funcione es que también modifiques la `ControlTemplate` para eliminar los estados visuales, lo cual es mucho más trabajo y rompe las animaciones nativas (como el sutil cambio de color al pasar el mouse).

**Resumen:** No es que tu código esté "mal", es que el sistema de temas de WinUI es muy persistente. Cambiando los `StaticResource` con las llaves mágicas que te puse arriba, el botón obedecerá sin rechistar.

##### Alternativa usando un "Border" contenedor

Es una solución ingeniosa y muy común cuando se quiere "saltar" las restricciones del `VisualStateManager`, pero como todo en el desarrollo de interfaces, tiene sus "letras pequeñas".

No está **mal** per se, y de hecho te da un control total sobre la estética, pero aquí te detallo las ventajas y desventajas para que decidas si te conviene:

###### Ventajas de usar un `Border` como contenedor

- **Simplicidad de Binding:** Como bien dices, el `Border` no tiene estados visuales complejos que sobreescriban tus valores. Tu `Binding` con un `Converter` funcionará a la primera.
- **Control total:** Puedes aplicar gradientes, efectos o bordes redondeados al contenedor sin pelearte con la plantilla interna del botón.

###### Desventajas y Riesgos

1. El "Hit Test" y el área de clic

Si el botón tiene `Background="Transparent"`, algunas versiones del framework XAML pueden tener problemas para detectar el clic en las zonas donde no hay texto (el área transparente).

> **Tip:** Asegúrate de usar `Background="Transparent"` y no dejar el fondo nulo, para que el botón siga capturando los eventos del mouse en toda su superficie.

2. Perplejidad Visual (Feedback)

Al poner el fondo del botón transparente y delegar el color al `Border`, pierdes las **animaciones nativas**:

- El botón ya no cambiará ligeramente de color al pasar el mouse (**PointerOver**).
- No verás la animación de "hundido" o cambio de tono al presionar (**Pressed**).
- El usuario podría sentir que la interfaz está "muerta" o que es menos responsiva.

3. Mantenimiento y Accesibilidad

- **Código sucio:** Si tienes 20 botones, tendrás 20 Borders envolviéndolos. Esto infla el árbol visual (Visual Tree) y hace el XAML más difícil de leer.
- **High Contrast:** WinUI tiene modos de "Alto Contraste" para personas con visión reducida. Al forzar colores mediante un `Border` y un `Binding`, podrías romper la adaptabilidad automática que WinUI ofrece para estos usuarios.

##### Tabla Comparativa

| **Característica** | **Usar Border + Binding**       | **Sobreescribir ThemeResources** |
| ------------------------- | ------------------------------------- | -------------------------------------- |
| **Facilidad**       | Alta (si sabes usar Converters)       | Media (hay que conocer las Keys)       |
| **Interactividad**  | Pobre (pierdes efectos Hover/Press)   | Excelente (mantiene animaciones)       |
| **Rendimiento**     | Ligeramente inferior (más elementos) | Óptimo                                |
| **Consistencia**    | Manual (tienes que definir todo)      | Automática con el sistema             |

###### La recomendación de la IA

Si buscas una aplicación con acabado profesional que se sienta "nativa" de Windows, te sugiero la opción de los **ThemeResources** que mencioné antes. Es la forma en que WinUI fue diseñado para ser extendido.

Sin embargo, si necesitas un diseño **extremadamente personalizado** que no se parece en nada al botón estándar de Windows, la técnica del `Border` es totalmente válida, siempre y cuando manejes tú mismo los estados de _Hover_ y _Pressed_ (quizás con un `VisualStateManager` en el propio Border).
