# 🏛️ ECOSISTEMA INMERSIVO DE REALIDAD EXTENDIDA (XR)
### *IUMAFIS - Conmemoración 40 Años*

---

## 📝 Descripción del Proyecto
Este ecosistema digital interactivo y de Realidad Extendida (XR) fue diseñado y desarrollado con motivo de la conmemoración de los 40 años de la **Institución Universitaria Marco Fidel Suárez (IUMAFIS)**. 

El sistema integra tecnologías inmersivas para recopilar, procesar y proyectar la identidad y trayectoria histórica de la institución a través de **cinco (5) módulos interactivos** independientes desplegados en hardware de gran formato.

---

## 🛠️ Stack Tecnológico

* **Motor de Desarrollo:** `Unity 6000.0.58f2` *(Versión LTS)*
* **Lenguaje de Programación:** `C#` (.NET Framework / .NET Standard)
* **Tecnologías XR & Tracking:** * Realidad Aumentada (AR)
    * MediaPipe
    * CardboardPlugin
    * Pipeline de renderizado optimizado en tiempo real
* **Control de Versiones:** `Git` / `GitHub`

---

## 📦 Estructura del Ecosistema (Módulos)
El software se encuentra dividido modularmente para garantizar su escalabilidad y un rendimiento óptimo de renderizado en estaciones independientes:

* **Módulo 1: Muro de Bienvenida Institucional** *Interfaz de recepción y despliegue cronológico de la historia de la U.*
* **Módulo 2: Galería del Tiempo** *Interacción inmersiva con hitos históricos de la institución.*
* **Módulo 3: [Nombre de tu Exp. 3]** *Experiencia interactiva central optimizada para alta concurrencia.*
* **Módulo 4: [Nombre de tu Exp. 4]** *Visualización de datos y proyección institucional.*
* **Módulo 5: Espejo de Identidad (AR)** *Módulo de Realidad Aumentada interactiva enfocado en tracking facial para la superposición en tiempo real de la toga y birrete institucional.*

---

## 🚀 Requisitos del Sistema y Despliegue

### 💻 Requisitos de Hardware Mínimos

| Componente | Especificación Mínima |
| :--- | :--- |
| **Procesador** | Intel Core i5 / AMD Ryzen 5 (o superior) |
| **Memoria RAM** | 8 GB |
| **Tarjeta Gráfica** | Dedicada NVIDIA GTX 1650 / AMD equivalente *(Garantiza 60 FPS estables)* |
| **S.O.** | Windows 10 / Windows 11 (64 bits) |

### 📦 Instrucciones de Despliegue Rápido (Producción)

1.  **Descarga:** Obtener las compilaciones ejecutables ubicadas en la ruta de `Releases` del repositorio.
2.  **Ejecución:** Lanzar el archivo `.exe` correspondiente al módulo de la estación en modo *Fullscreen* (ajustado a la resolución nativa de la pantalla de la U).
3.  **Periféricos:** Verificar que las dependencias de hardware (cámaras de tracking y periféricos de visualización) estén conectadas y cuenten con los drivers activos en Windows antes de iniciar la aplicación.

---

## ⚙️ Buenas Prácticas de Mantenimiento y Código

* **Arquitectura de Software:** El proyecto implementa patrones de diseño modulares en C# para separar la lógica de captura de hardware del pipeline de renderizado gráfico de Unity.
* **Optimización de Memoria:** Se aplicaron técnicas avanzadas de optimización de *Draw Calls*, oclusión ambiental y compresión de texturas (*Texture Atlasing*) para asegurar un rendimiento fluido bajo uso continuo e intensivo durante los eventos institucionales.
* **Documentación Interna:** Cada script principal cuenta con comentarios estructurados XML (`/// <summary>`) detallando la funcionalidad de las clases y métodos públicos para facilitar futuras fases de soporte o actualización.

---

## ✒️ Autoría y Créditos de Desarrollo

* **Desarrollador Principal:** Juan Esteban Quinchia Duque
* **Rol:** Desarrollador Full-Stack & Especialista en Tecnologías Inmersivas (XR)
* **Contacto Técnico:** [juanes.10qd@gmail.com](mailto:juanes.10qd@gmail.com) | 📱 3502382111
* **Institución:** Institución Universitaria Marco Fidel Suárez (IUMAFIS)

---