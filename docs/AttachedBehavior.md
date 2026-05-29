# ⭐ Attached Behavior

La técnica del **"Attached Behavior"** (Comportamiento Adjunto) debe su nombre a la forma en que se "inyecta" una funcionalidad nueva en un objeto existente. La técnica se basa en que esa "inyección" se hace sin necesidad de heredar del objeto existente, ni de escribir código en el *Code-behind*.

> [!NOTE]
> Aquí te explico el porqué del nombre desglosándolo:
> 
>**¿Por qué "Attached" (Adjunto)?**
> 
> Se llama así porque utiliza **Attached Properties** (Propiedades Adjuntas). Estas son propiedades que se definen en una clase (la clase Extensions de tu Helper), pero que se "pegan" a otra clase (el Grid donde está contenida la TitleBar).
> 
> * Es como ponerle un "Post-it" a una caja: la caja no sabía que ese papel existiría, pero ahora lleva esa información pegada.
> 
>**¿Por qué "Behavior" (Comportamiento)?**
> 
> Se llama así porque el objetivo de la propiedad no es solo guardar un dato (como un color o un texto), sino **ejecutar una acción** o cambiar cómo se comporta el elemento.
> 
> * En la mayoría de los casos, el "comportamiento" es el siguiente:  *"Cuando este Grid se cargue (Loaded), ejecuta esta lógica específica del Helper"* .
> 
> 


## 1. ¿Cómo funciona visualmente el patrón?

El flujo de un **Attached Behavior** funciona de la siguiente manera:

1. **Definición:** Creas una clase estática con una propiedad especial `DependencyProperty`.
   
2. **Referencia:** En el XAML, aplicas esa propiedad al elemento sobre el que deseas actuar. 
   
3. **Activación:** Cuando el valor de esa propiedad cambia o se asigna por primera vez, se dispara un evento interno en tu Helper llamado de manera genérica `OnPropertyChanged`.
   
4. **Suscripción:** Dentro de ese manejador interno, tu Helper tiene acceso al objeto real (`Grid`) y puede suscribirse a sus eventos (`Grid.Loaded += ...`).


## 2. La diferencia clave con la suscripción manual

La suscripción manual a un evento en C# es la vía más rápida, pero el **Attached Behavior** es el siguiente nivel de elegancia. Aquí la diferencia:

* **Suscripción Manual (C#):** Tienes que ir al `MainWindow.xaml.cs` (el *Code-behind*) y escribir el código que une al `Grid` con un Helper "tonto".
  
* **Attached Behavior:** El Helper es "inteligente". Solo con poner una línea en el XAML, el Helper se "ancla" al `Grid` y se encarga de todo lo demás por sí solo. Esto permite reutilizar la lógica sin ensuciar el *Code-behind* .


## 3. ¿Cuándo deberías usarlo?

Es ideal cuando se desea explotar al máximo la naturaleza **declarativa** del XAML obteniendo así todos los beneficios que éso conlleva. Recordemos las diferencias entre un lenguaje "imperativo" y uno "declarativo":

| **Enfoque**            | **Metodología**            | **Ejemplo**                                                                       |
| ---------------------- | -------------------------- | --------------------------------------------------------------------------------- |
| **Imperativo (C#)**    | Instrucciones paso a paso. | "Crea un botón, ponle texto 'Enviar', píntalo de azul, ponlo en la coordenada X." |
| **Declarativo (XAML)** | Descripción del resultado. | "Aquí hay un botón azul que dice 'Enviar'."                                       |

Para redondear el concepto, veámoslo de esta forma:

- **XAML estándar:** Es como un maniquí (estático).
    
- **XAML con Attached Behavior:** Es como darle una instrucción al maniquí ("siempre que alguien te toque el brazo, saluda") y esa instrucción se indica pegándole una etiqueta a su espalda.

Quien finalmente dice "cómo" se saluda es el "behavior" desarrollado en C# y que se encuentra en el módulo helper. 

Así conseguimos que en lugar de tener código en el constructor o en los métodos propios de la ventana, simplemente se etiqueta el Grid con el prefijo "helpers". Esto nos brinda mayor claridad  en el código y eficiencia en la ejecución. 

La línea XAML con el prefijo "`helpers`" incluye referencias a los siguientes ítems:

- La clase (comúnmente conocida como "**Behavior**"). 
- El nombre de su "**Attached property**".
- Asignación de un valor (en general, "**True**").

**XML**

```xml
<Grid x:Name="GridName" 
      helpers:BehaviorClass.AttachedProperty="True" />
```


## 4. Implementar un patrón de "Attached Behavior"

Básicamente, crearemos una propiedad que, al ser asignada en el XAML, "engancha" automáticamente el evento `Loaded` a la lógica de tu Helper, en particular a un método de la clase conocida comúnmente como "**Behavior**".

Dentro de la carpeta Helper, creamos un nuevo archivo de código C# el cual debe incluir por lo menos dos clases:

- Clase del Behavior
- Clase del Helper

Decimos "por lo menos" dos porque en algunos casos se aprovecha el módulo helper para agregar otros métodos que sin bien no forman parte de la técnica "Attached behavior" sí intervienen en la funcionalidad. Esta característica se puede ver claramente en el helper de la "Splash Screen".
### 4.1. Crear la Clase del Behavior

Crea una clase que defina la **DependencyProperty**. Esta propiedad actuará como el puente.

En el siguiente código genérico hemos asignado las siguientes asociaciones de nombres:

- [MyProject]: RegionCaptureSnip.
- [Feature]: TitleBar, SplashScreen, Mica, etc.
- [PropertyName]: Nombre de la propiedad que describa al comportamiento. Por ejemplo: WatchThemeChanges para el TitleBar, InitializeOnLoaded para el SplashScreen, ApplyMica para el Efecto Mica.
- [UIElementName]: Nombre del elemento para el cual se define el comportamiento Por ejemplo: TitleBarImage, BottomPanelGrid, 

**C#**

```csharp
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace [MyProject].Helpers
{
    public static class [Feature]Extensions
    {
        // Definimos la propiedad adjunta
        public static readonly DependencyProperty [PropertyName]Property =
            DependencyProperty.RegisterAttached(
                "[PropertyName]",
                typeof(bool),
                typeof([Feature]Extensions),
                new PropertyMetadata(false, On[PropertyName]Changed));

        public static void Set[PropertyName](DependencyObject element, bool value) => 
            element.SetValue([PropertyName]Property, value);

        public static bool Get[PropertyName](DependencyObject element) => 
            (bool)element.GetValue([PropertyName]Property);

        private static void On[PropertyName]Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Grid rootGrid && (bool)e.NewValue)
            {
                // Aquí nos suscribimos al evento Loaded automáticamente
                rootGrid.Loaded += (s, args) => 
                {
                    // 1) Llamamos a tu lógica en el [Feature]Helper que se encargará de buscar el control para trabajarlo
                    [Feature]Helper.EjecutarLogica(rootGrid);
                    
                    // 2) Buscamos el control dentro del Grid - caso de registración individual
	                if (rootGrid.FindName("[UIElementName]") is [UIElementType] elem)
	                {
	                    // Registramos el icono en tu Helper existente
	                    [Feature]Helper.Register[UIElementName]Behavior(elem);
	                }
	                
	                // 3) Buscamos el control dentro del Grid - caso de Lista de acciones de registración
	                if (rootGrid.FindName("[UIElementName]") is [UIElementType] elem)
                    {
                        [Feature]Helper.RegisterActionListBehavior((param) => {
	                        inlineFunction();
						});
                    }
                };
            }
        }
    }
}
```


### 4.2. Referenciarlo en el XAML

Ahora, en tu `MainWindow.xaml`, solo tienes que importar el namespace y usar la propiedad adjunta en lugar del evento `Loaded`.

**Paso A: Añadir el namespace (normalmente `helpers`):**

**XML**

```xml
<Window
    ...
    xmlns:helpers="using:[MyProject].Helpers">
```

**Paso B: Aplicar la propiedad al Grid:**

**XML**

```xml
<Grid x:Name="GridName" 
      helpers:[Feature]Extensions.[PropertyName]="True">
</Grid>
```

Con esta implementación cuando el `<Grid>` se carga y la propiedad se pone en "True", pasa lo siguiente: 

- Se dispara el evento `"On[PropertyName]Changed"` con lo cual el "Extensions" del Helper detecta que el `Grid` existe, 
- Se intercepta el evento Loaded, es decir, espera a que se cargue el Grid 
- Cuando se dispara el evento Loaded, carga los valores de la animación según el ancho de ventana. 


### 4.3. Cuándo ocurren los eventos interceptados en el Behavior?

Como se puede ver en el código C#, tenemos por un lado el evento asociado al cambio de la "Attached Property" y por otro el evento de carga del  `<Grid>`. 

Entender el orden en que ocurren estos eventos evitará muchos errores de tipo `NullReferenceException` o comportamientos visuales erráticos.

La diferencia clave entre ambos momentos es la siguienter:

**A. `On[PropertyName]Changed` (El disparador)**

Este método se ejecuta **en el instante en que el valor de la propiedad se asigna al objeto**, incluso si el objeto aún no existe visualmente en la pantalla.

- **Cuándo ocurre:** Durante el parseo del XAML (cuando la ventana se está creando) o cuando cambias el valor manualmente desde C#.
    
- **Estado del control:** El `Grid` ya está instanciado en memoria, pero **no tiene dimensiones** (su `ActualWidth` y `ActualHeight` suelen ser 0) y no está conectado a la jerarquía visual activa.
    
- **Uso en tu Helper:** Se usa exclusivamente para **configurar el enganche**. Es el lugar para suscribirse a eventos (como el `Loaded` que usaste) o para realizar cálculos que no dependan de la apariencia visual del control.
    

**B. `Loaded` (La ejecución visual)**

Este evento se dispara cuando el control ya es "real" dentro de la interfaz de usuario de Windows.

- **Cuándo ocurre:** Después de que el sistema ha terminado de medir (`Measure`) y colocar (`Arrange`) el elemento. Es el último paso antes de que el usuario vea el control.
    
- **Estado del control:** El `Grid` ya tiene un padre visual, tiene dimensiones reales y es capaz de renderizar contenido.
    
- **Uso en tu Helper:** Es el momento perfecto para `EjecutarLogica`. Si tu lógica implica animaciones, medir el tamaño de la pantalla o inyectar controles hijos (como un Spinner de carga), **debe ocurrir aquí**.
    

**Comparación del Ciclo de Vida entre estos eventos**

|**Momento**|**¿Qué está pasando?**|**¿Es seguro manipular la UI?**|
|---|---|---|
|**OnPropertyChanged**|El XAML lee `Property="True"` y lo guarda en el objeto.|**No.** El control es "invisible" y puede que sus hijos aún no existan.|
|**Loaded**|El control entra en el árbol visual de la ventana.|**Sí.** Ya puedes hacer animaciones y cálculos de diseño.|

**Resumen:** `OnPropertyChanged` es el aviso de que la funcionalidad ha sido activada, mientras que `Loaded` es la señal de que el escenario está listo para que empiece la función.

**¿Qué acabamos de lograr?**

1. **XAML Limpio:** No hay llamadas a métodos en el Code-behind. El compilador no se quejará de que no encuentra la función.
   
2. **Desacoplamiento total:** `MainWindow.xaml.cs` puede estar vacío. Toda la suscripción ocurre "detrás de escena" gracias a la propiedad adjunta.
   
3. **Reutilización:** Si mañana tienes otro `Grid` en otra ventana que necesita la misma lógica de Splash, solo le pones `InitializeOnLoaded="True"`.

### 4.4. La clase del Helper y el Método Ejecutor

Asegúrate de que tu método en `[Feature]Helper` reciba el objeto que necesita para trabajar:

**C#**

```csharp
public static class [Feature]Helper
{
        public static void EjecutarLogica(Grid grid)
        {
            if (grid != null)
            {
                // 1. Buscamos el control Image por su nombre "x:Name" dentro del Grid
                // FindName devuelve un 'object' y lo asignamos a elem
                if( grid.FindName("UIElementName") is UIElementType elem)
                {
				...
				}
			}
	    }
	}
}
```

Podríamos también hacer los ejemplos con los otros casos que tienen la búsqueda en el Behavior, pero entendemos que no es necesario ya que están muy bien descriptos en los documentos individuales de cada uno de los helpers.

# 5. Solución Alternativa: Referencia "Puente"

En este apartado planteamos a Gemini la solución que habíamos implementado antes de conocer la técnica de "Attached Behavior". Más abajo la respuesta y los motivos para optar por esta última.

---

## Pregunta: 

**Tu dijiste**: Yo hice algo más sencillo para acceder a un control del MainWindow.xaml desde una clase diferente (como el helper) a MainWindow. Se trata de una referencia "puente" en MainWindow.xaml.cs a la cual inicializo con la referencia del elemento que deseo acceder y luego desde el helper lo alcanzo de la siguiente manera:

	elemento = window.elementoPuente

## Respuesta:

**Gemini dijo**: Esa es una solución muy común y práctica, pero tiene un pequeño "pecado" arquitectónico: crea una **dependencia circular** o un  **acoplamiento fuerte** . Estás obligando a tu Helper a conocer la existencia de `MainWindow` y a `MainWindow` a exponer sus "tripas" para que el Helper funcione.

Funciona perfectamente para proyectos pequeños, pero si quieres escalar a algo más profesional (como el patrón MVVM o controles reutilizables) tendrás problemas. Aquí te explico por qué tu método tiene riesgos y cuál es la alternativa "limpia" siguiendo tu lógica:

1. **Fugas de memoria:** Si guardas una referencia de la `Window` o de un `UIElement` en un Helper estático y no la limpias, el recolector de basura no podrá liberar la memoria de la ventana al cerrarla.
   
2. **Rigidez:** Si decides mover la funcionalidad ([Feature]) a otra página o ventana, el Helper dejará de funcionar porque estará buscando específicamente `MainWindow`.

> [!NOTE]
> ¿Por qué utilizar `FindName` como se hace en la técnica del "Attached Behavior" es mejor que una "Referencia Puente"?
> 
> * **Encapsulamiento:** El Helper solo necesita saber que existe un `Grid` con algo llamado "SplashImage" dentro. No necesita saber quién es el dueño del Grid.
> * **Seguridad:** No mantienes referencias globales a ventanas activas.
> * **Limpieza:** Tu `MainWindow.xaml.cs` se mantiene libre de variables "puente".


### Comparativa de enfoques

| Método                    | Acoplamiento                 | Reutilización | Riesgo de Memoria |
| ------------------------- | ---------------------------- | ------------- | ----------------- |
| **Referencia Puente**     | Alto (Depende de MainWindow) | Baja          | Alto              |
| **FindName / VisualTree** | Bajo (Solo depende del Grid) | Alta          | Nulo              |
| **MVVM / Binding**        | Mínimo                       | Máxima        | Nulo              |

> [!IMPORTANT]
> Como dijimos antes, la solución con "Referencia Puente" no está mal si el proyecto es sencillo, pero si notas que tu `MainWindow.xaml.cs` empieza a llenarse de variables solo para que otros las vean, es una señal de que el **Attached Behavior** o el método `FindName` son tus mejores aliados.
> 
> **¿Por qué este cambio (pasarse a "Attached Behavior") es importante?**
>
> 1. **Uniformidad:** Tu `MainWindow.xaml.cs` queda prácticamente vacío. Toda la "magia" ocurre porque el XAML declara que quiere esas funcionalidades.
> 2. **Ciclo de Vida:** Al centralizar el evento `ActualThemeChanged` tanto en el TitleBar como en el MicaHelper, te aseguras de que cuando el usuario cambie el tema, toda la ventana (fondo Mica + Icono + Botones de sistema) cambie en el mismo frame de renderizado.
> 3. **Mantenibilidad:** Si decides que una ventana secundaria no debe tener Mica, simplemente no pones la propiedad en el XAML. No tienes que andar instanciando clases ni guardando variables privadas en cada ventana.



