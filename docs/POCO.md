# **Objetos POCO**

**Los objetos POCO (Plain Old CLR Objects) son clases simples en .NET que no dependen de frameworks ni librerías externas, usadas principalmente para representar datos de manera limpia y desacoplada.** Se emplean mucho en arquitecturas con Entity Framework y otros ORM para mapear tablas de bases de datos a objetos de C# sin añadir lógica extra.

---

## 🔎 Definición de POCO

* **Acrónimo:** *Plain Old CLR Object* (Objeto Común del CLR).
* **Origen:** Inspirado en el término *POJO* (Plain Old Java Object) en Java.
* **Características principales:**
  * Son **clases simples** con propiedades y métodos básicos.
  * No heredan de clases especiales ni implementan interfaces de frameworks.
  * No contienen lógica de persistencia ni dependencias externas.
  * Se usan para  **representar entidades de negocio o datos** .

---

## 📌 Ejemplo en C#

```csharp
public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
}
```

Este objeto `Cliente` es un POCO porque:

* No depende de Entity Framework ni de ninguna librería.
* Solo define propiedades que representan datos.
* Puede ser usado en cualquier capa de la aplicación.

---

## 🛠️ Usos comunes

* **Entity Framework / NHibernate:** Los POCOs se mapean a tablas de bases de datos.
* **DTOs (Data Transfer Objects):** Se usan para transportar datos entre capas.
* **Modelos de dominio:** Representan entidades del negocio sin lógica adicional.

---

## ✅ Ventajas

* **Simplicidad:** Fácil de entender y mantener.
* **Desacoplamiento:** No están atados a frameworks, lo que facilita pruebas unitarias.
* **Flexibilidad:** Se pueden usar en diferentes contextos (persistencia, servicios, APIs).
* **Compatibilidad:** Funcionan bien con serialización (JSON, XML).

---

## ⚠️ Consideraciones

* Aunque son simples, **no deben incluir lógica compleja** de negocio ni dependencias externas.
* En arquitecturas grandes, se recomienda separar POCOs de DTOs y ViewModels para mantener claridad.

---

👉 En resumen: **un objeto POCO es una clase sencilla que representa datos sin depender de frameworks, ideal para mantener el código limpio y desacoplado en aplicaciones .NET.**

---



# Comparación con otros objetos de .NET

Hagamos la comparación clara entre  **POCOs** , **DTOs** y **ViewModels** para que veas cómo se diferencian en una arquitectura típica de .NET:

---

## 📊 Comparación

| Concepto                                   | Definición                                                      | Uso principal                                                | Dependencias                                              | Ejemplo típico                                       |
| ------------------------------------------ | ---------------------------------------------------------------- | ------------------------------------------------------------ | --------------------------------------------------------- | ----------------------------------------------------- |
| **POCO**( *Plain Old CLR Object* ) | Clase simple que representa datos o entidades del dominio.       | Modelar entidades de negocio o persistencia.                 | Ninguna, son clases puras.                                | `Cliente { Id, Nombre, Email }`                     |
| **DTO**( *Data Transfer Object* )  | Objeto diseñado para transportar datos entre capas o servicios. | Reducir acoplamiento y optimizar transferencia de datos.     | Puede depender de contratos de servicio o serialización. | `ClienteDTO { Nombre, Email }`                      |
| **ViewModel**                        | Objeto que representa datos preparados para la vista (UI).       | Adaptar datos para la presentación en interfaces gráficas. | Depende de la capa de presentación (ej. MVVM, MVC).      | `ClienteViewModel { NombreCompleto, EmailVisible }` |

---

## 🔎 Resumen

* **POCO:** Entidad pura, sin lógica extra. Ideal para persistencia y dominio.
* **DTO:** Optimizado para mover datos entre capas o servicios, suele ser más liviano.
* **ViewModel:** Diseñado para la interfaz de usuario, puede transformar o combinar datos para mostrarlos mejor.

---

👉 En pocas palabras: **POCOs son la base de tus modelos de negocio, DTOs son mensajeros entre capas, y ViewModels son traductores para la UI.**

Podríamos decir que  **POCO, DTO y ViewModel son diferentes tipos de objetos en .NET** , pero cada uno cumple un rol distinto dentro de la arquitectura:

* **POCO (Plain Old CLR Object):**
  * Son clases simples, sin dependencias externas.
  * Representan entidades del dominio o de la base de datos.
  * Ejemplo: `Cliente { Id, Nombre, Email }`.
* **DTO (Data Transfer Object):**
  * Diseñados para transportar datos entre capas o servicios.
  * Reducen acoplamiento y evitan exponer toda la entidad.
  * Ejemplo: `ClienteDTO { Nombre, Email }`.
* **ViewModel:**
  * Preparados para la capa de presentación (UI).
  * Transforman o adaptan datos para mostrarlos en la vista.
  * Ejemplo: `ClienteViewModel { NombreCompleto, EmailVisible }`.

---

📌 **En resumen:**

Sí, son distintos tipos de objetos en .NET, pero no porque el framework los defina como categorías oficiales, sino porque la comunidad y las buenas prácticas los usan con diferentes propósitos: POCO para el dominio, DTO para transporte, y ViewModel para presentación.

---



# Ejemplo práctico en C#

Aquí tienes un ejemplo práctico en C# mostrando cómo la misma entidad `Cliente` puede representarse como  **POCO** , **DTO** y **ViewModel** en un proyecto MVC:

---

## 🟦 POCO (Entidad de dominio)

```csharp
public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
}
```

* Clase simple, sin dependencias.
* Se usa en la capa de dominio o persistencia (ej. Entity Framework).

---

## 🟨 DTO (Data Transfer Object)

```csharp
public class ClienteDTO
{
    public string Nombre { get; set; }
    public string Email { get; set; }
}
```

* Solo incluye los datos necesarios para transportar entre capas o servicios.
* Puede omitir propiedades internas como `Id` si no son relevantes para la transferencia.

---

## 🟩 ViewModel (Para la vista en MVC)

```csharp
public class ClienteViewModel
{
    public string NombreCompleto { get; set; }
    public string EmailVisible { get; set; }
    public bool MostrarEmail { get; set; }
}
```

* Preparado para la interfaz de usuario.
* Puede transformar datos (ej. concatenar nombre y apellido en `NombreCompleto`).
* Incluye lógica de presentación como `MostrarEmail`.

---

## 📐 Cómo fluyen los datos en una arquitectura MVC

A continuación presentamos un diagrama visual sencillo que muestra cómo fluyen los datos entre POCO → DTO → ViewModel en una arquitectura MVC.

```
Base de datos (tablas)
        │
        ▼
   POCO (Entidad)
        │
        ▼
   DTO (Transferencia)
        │
        ▼
Controlador (lógica)
        │
        ▼
ViewModel (presentación)
        │
        ▼
   Vista (UI)
```

---

### 🔎 Explicación del flujo

* **POCO:** Se obtiene desde la base de datos (ej. `Cliente`).
* **DTO:** Se usa para transportar datos entre capas o servicios, evitando exponer toda la entidad.
* **Controlador:** Recibe el DTO y lo transforma en un  **ViewModel** .
* **ViewModel:** Adapta los datos para la vista (ej. concatenar nombre y apellido, decidir si mostrar email).
* **Vista (UI):** Renderiza la información lista para el usuario.

---

👉 Así, cada capa cumple un rol distinto:  **POCO = datos crudos** ,  **DTO = datos en tránsito** ,  **ViewModel = datos listos para mostrar** .

¿Quieres que te arme un ejemplo más completo con **POCO + DTO + ViewModel + Vista Razor** para ver cómo se integran en un proyecto MVC real?


---



## 🔎 Ejemplo de uso en un controlador MVC

```csharp
public ActionResult Detalle(int id)
{
    // Supongamos que obtenemos el POCO desde la base de datos
    Cliente cliente = _db.Clientes.Find(id);

    // Convertimos a DTO para enviar a otra capa (ej. API)
    ClienteDTO dto = new ClienteDTO
    {
        Nombre = cliente.Nombre,
        Email = cliente.Email
    };

    // Creamos un ViewModel para la vista
    ClienteViewModel vm = new ClienteViewModel
    {
        NombreCompleto = cliente.Nombre,
        EmailVisible = cliente.Email,
        MostrarEmail = true
    };

    return View(vm);
}
```

---

👉 Para fijar los conceptos, digamos que el **POCO** es la base, el **DTO** es el mensajero entre capas, y el **ViewModel** es el traductor para la UI.


