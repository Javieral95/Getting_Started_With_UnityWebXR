<!-- omit in toc -->
# Getting_Started_With_UnityWebXR

El presente proyecto busca realizar una pequeña prueba de concepto sobre el nuevo estándar [WebXR](https://www.w3.org/TR/webxr/), que permite hacer accesible realidad virtual y aumentada en aplicaciones Web, integrandolo con el motor gráfico **Unity**. La idea es permitir el uso e interacción de la aplicación inmersiva a través de Web tanto si se dispone de Hardware especifico como si no.

Esta investigación se ha llevado a cabo para desarrollar el Trabajo de Fin de Máster de la titulación [Máster universitario en Ingeniería informática](https://masterinformatica.uniovi.es/) por la [Universidad de Oviedo](https://www.uniovi.es/) y para recabar información que podrá ser utilizado para proyectos propiedad de [Fundación CTIC](https://www.fundacionctic.org/es/home).

<!-- omit in toc -->
## Tabla de contenido
- [Qué es WebXR](#qué-es-webxr)
- [Pre-requisitos](#pre-requisitos)
- [Comenzando](#comenzando)
  - [Importando los paquetes](#importando-los-paquetes)
    - [WebXR Exporter](#webxr-exporter)
    - [WebXR Exporter Advance](#webxr-exporter-advance)
    - [InputManager](#inputmanager)
  - [Probando y exportando el proyecto](#probando-y-exportando-el-proyecto)
    - [Una vez exportado](#una-vez-exportado)
    - [Controles de la escena](#controles-de-la-escena)
- [Scripts del proyecto y funcionamiento](#scripts-del-proyecto-y-funcionamiento)
  - [Interaccionables:](#interaccionables)
  - [WebXRCameraSet:](#webxrcameraset)
  - [HandL y HandR:](#handl-y-handr)
  - [Cameras](#cameras)
  - [Interaccionables especiales](#interaccionables-especiales)
  - [Cambio de escena](#cambio-de-escena)
- [Errores:](#errores)
- [Fin](#fin)
  - [Autores](#autores)
  - [Licencia](#licencia)


_________________
# Qué es WebXR

[WebXR](https://www.w3.org/TR/webxr/) es un grupo de estándares que se juntan para representar escenas 3D en hardware inmersivo (realidad virtual) como para añadir imágenes al mundo real (realidad aumentada).

En este proyecto no se abarca la realidad aumentada.

# Pre-requisitos

Este proyecto se ha llevado a cabo haciendo uso de la versión *LTS 2019.4.27f1* (con *WebGL Build Support* instalado) de Unity y se han utilizado unas *Oculus Quest 2* conectadas al PC con cable mediante *Oculus Link*.

Y ha sido desarrollado utilizando los siguientes repositorios como base (más adelante veremos como hacer uso de ellos):
- [Unity-WebXR-Export](https://github.com/De-Panther/unity-webxr-export): Para la exportación del proyecto haciendo uso de WebXR.
- [WebXR Exporter Advanced](https://github.com/Jocyf/WebXR-Exporter-Advanced): Para desarrollar los controladores del usuario.

Estos dos a su vez han sido basados en [Mozilla WebXR Exporter](https://assetstore.unity.com/packages/tools/integration/webxr-exporter-109152), aunque se ha de tener en cuenta que **el paquete de Mozilla en la Asset Store de Unity se encuentra desactualizado** y ocasiona errores que hará que la aplicación no sea accesible por hardware VR una vez exportado a Web. Y, es por ello que, se deberá hacer uso del repositorio *Unity-WebXR-Export* antes mencionado.

También será necesario un servidor https para que todo funcione correctamente, debido a que con un servidor http no se podrá disfrutar una experiencia inmersiva accediendo desde el navegador del propio hardware de realidad virtual (probado desde el navegador de unas Oculus Quest 2)


# Comenzando

Dado que los paquetes propiedad de Mozilla se encuentran desactualizados, se debe acceder a alternativas que han solucionado los errores previamente.

## Importando los paquetes

El proyecto del presente repositorio ya tiene importado todo lo necesario para hacerlo funcionar, no obstante si empiezas tu proyecto de cero tendrás que importar los siguientes paquetes:

### WebXR Exporter
Primero tendremos que importar el paquete que nos permitirá exportar nuestro proyecto en WebXR. Para ello, importa el proyecto **Unity-WebXR-Export**  de *De-Panther* siguiendo las siguientes instrucciones indicadas en su [Readme.md](https://github.com/De-Panther/unity-webxr-export/blob/master/Packages/webxr/README.md):

- Usando **OpenUPM**, para ello añade el registro en ``Project Settings > Package Manager`` con las siguientes opciones:
    ```
    Name: OpenUPM
    URL: https://package.openupm.com
    Scope(s): com.de-panther
    ```    
    - Tras ello, accede a *Package Manager* y selecciona *My Registries* para importar el paquete **WebXR Export**.

-  Usando **Git**, para ello accede al Package Manager y selecciona la opción ``+ > Add package from girl url..``, indica la siguiente URL:
     ```
     https://github.com/De-Panther/unity-webxr-export.git?path=/Packages/webxr
     ```  
Existen mas paquetes además de **WebXR**, por ejemplo el de **WebXR Interactions** que tiene además una escena de ejemplo para probar el exporter y sus controladores, pero en este ejemplo se han implementado otros especificos.

**Una vez importando**, accede a ``Window > WebXR > Copy WebGLTemplates`` para copiar las plantillas de exportación en tu proyecto.

### WebXR Exporter Advance

Este paquete de *jocyf* se ha utilizado para usar de base en el desarrollo de los controladores del proyecto, este a su vez parte del paquete WebXR Exporter, puedes descargarlo directamente desde [aquí](https://www.dropbox.com/s/gvnfxfzrg1k1mw7/WebXRAdvancedDePanther.unitypackage?dl=0). También puedes acceder al siguiente [repositorio](https://github.com/Jocyf/WebXR-Exporter-Advanced).

Tamibén otorga funcionalidades no implementadas en el presente repositorio (giro con click del raton, interacción a distancia por Raycaster...). 

En el presente repositorio se han desarrollado los scripts de la siguiente manera:
- **PlayerController**: Para el movimiento, basado en WebXRMovev2 (que hace uso de CharacterController en vez de un collider).
- **ControllerInteraction**: Para interaccionar con objetos, basado en WebXRGrabManager. 
- **WebXRInputManager**: Ha sido modificado para añadir funcionalidad a los botones A, B, X e Y de los controladores VR.

### InputManager

Este paso no será necesario para exportar el proyecto correctamente, pero es recomendado por Mozilla. Modifica el contenido de ``ProjectsSettings/InputManager.asset`` (deberás hacerlo desde fuera del editor de Unity) y añade el conteniedo del siguiente [fichero](https://github.com/MozillaReality/unity-webxr-export/blob/master/ProjectSettings/InputManager.asset).

## Probando y exportando el proyecto

En el presente repositorio, con los paquetes ya importado se encuentra una escena de ejemplo: ``Assets/0Assets/Scenes/MainScene``, vamos a utilizarla para probar WebXR.

Una vez importados los paquetes y copiadas las plantillas (si no encuentras el directorio ``Assets/WebGL Templates`` accede a la opción ``Window > WebXR > Copy WebGLTemplates``) tendremos que realizar unas configuraciones. 
- ``Edit > Project Settings > XR Plug-in Management > Initialize XR on Startup``.
  - Habilita tu Hardware en la pestaña *PC, Mac & Linux Standalone Settings*.
  - En la pestaña *WebGL* deberás habilitar la casilla **WebXR Export**.
-  ``File > Build Settings`` añade la escena y cambia a la plataforma **WebGL**.
-  ``Edit > Project Settings > Player > WebGL > Resolution and Presentation > WebGL Template`` selecciona cualquier plantilla (WebXR ha funcionado correctamente, las versiones 2020 parecen no funcionar). 

¡El proyecto esta listo! Selecciona ``Build and Run``, una vez finalizado el proceso podrás probar la escena en tu navegador:
- Habilita que la aplicación acceda a los dispositivos de realidad virtual.
- Funciona en navegadores Chromium (como Google chrome o Microsoft Edge), pero en Mozilla Firefox la escena no carga en el dispositivo de realidad virtual.

### Una vez exportado
Una vez que el proyecto ha sido exportado, utilizando la opción Build (o Build and run para comprobar su correcto funcionamiento) tendremos que alojar nuestro proyecto resultante en un servidor Http.

No se va a entrar en mucho detalle sobre como realizar este paso, pues hay multitud de alternativas. Pero, para hacer pequeñas pruebas, podemos lanzar los siguientes comandos dentro del directorio del proyecto exportado:
  -	Python:
    -	*Python -m SimpleHttpServer \<PUERTO>*
    -	*Python3 -m http.server*
  -	NodeJS:
    - *Node http-server*, una vez instalado el paquete http-server a través de npm.

Recordar que se precisa de *https* para que la aplicación funcione accediendo desde el hardware de realidad virtual (*Oculus Browser* por ejemplo). Los ejemplos antes mencionados solo usan *http*.

### Controles de la escena

Es posible interactuar con el entorno tanto con hardware de realidad virtual como sin él. Aunque los controles son mostrados en la escena, se detallan a continuación:
- XR activado (VR hardware):
  - Movimiento de cámara moviendo la cabeza (o con stick derecho, mediante tics o plano).
  - Movimiento de translación con stick izquierdo.
  - Teletransporte manteniendo el *Trigger* derecho, apuntando con el controlador (girando camara con el joystick derecho) y soltando el botón.
  - Agarrar objetos o activar botones manteniendo pulsado *Grip*.
    - Soltar objetos soltando el botón *Grip*. 
    - Lanzar objetos con el botón *A* o *X*
  - Saltar con el botón *Y*. 
- Sin XR (sin VR hardware):
  - Movimiento de cámara moviendo el mouse.
  - Movimiento de traslación con *WASD* o las flechas del teclado.
  - Agarrar objetos o activar botones manteniendo pulsado el click izquierdo del mouse.
    - Soltar objetos soltando el click izquierdo del mouse.
    - Lanzar objetos con el click derecho del mouse.
  - Saltar con la barra espaciadora.    

Puedes activar el modo VR en cualquier momento conectado el hardware y clicando en *VR*, la aplicación lo detectará automaticamente habilitando el movimiento e interacción XR y deshabilitando el no XR. Puedes realizar el proceso inverso tan solo quitandote las gafas. Cambiar el modo reiniciará la posición y altura del usuario.

Ahora... ¡lanza cubos y pelotas!

____________________

# Scripts del proyecto y funcionamiento

Para evitar errores en el editor se ha tenido que añadir dentro de la carpeta _Scripts una Assembly Definition (Project references) para importar las funciones de WebXR y el resto de paquetes a utilizar (por el momento solo WebXR y TMPro).

## Interaccionables:

En primer lugar cabe destacar que los objetos con los que el usuario puede interaccionar deben tener la etiqueta:

* *Interactable*: Para permitir que el objeto sea agarrado con las manos y lanzado con total libertad.
* *InteractableNotMovable*: Para permitir que el objeto sea agarrado, pero con ciertas limitaciones (no permite lanzarlo y se moverá con físicas). Pensado para puertas, botones, palancas, etc.

## WebXRCameraSet:

El objeto principal de la escena es el usuario o jugador, en este caso es el objeto WebXRCameraSet, cuyo prefab se encuentra en ``Assets/0Assets/MainScene/Prefabs/Player``

Este *game object* permite el movimiento gracias a un *character controller* (que le otorga físicas y collider). En su interior se encuentran las manos del usuario, la cámara y los scripts más importantes:

* **WebXRManager**: Añadido directamente del paquete WebXR Exporter, gestiona los cambios a XR.
PlayerController: Encargado de detectar el hardware XR y permitir el movimiento del jugador.
Ha sido altamente modificado para permitir el **movimiento en XR y no XR** por igual.
En este script la detección y activación del hardware de realidad virtual se hace gracias a los **eventos *onXRChange* y *onXRCapabilitiesUpdate***.
  * Cuando se detecta un cambio a XR:
    * Se desactiva el cursor del usuario (Cambiando la propiedad Cursor.Lockstate de Locked a None), también se oculta la imagen que hace la función de cursor.
    * Se desactiva el script de interacción no XR (detallado más adelante).
    * Se habilitan las manos del usuario.
    * Se actualiza la altura del usuario (para adecuar el collider que le otorga el Character Controller) y su posición/rotación.
  * Cuando el estado de XR cambia (bien manualmente o automáticamente quitándonos las gafas) se revertirá todo lo anterior.
  * Se puede forzar el cambio de XR a NoXR modificando el valor booleano de la propiedad isXREnabled en el script PlayerController colocado en WebXRCameraSet (cuando se pruebe la aplicación desde el editor será necesario hacer este cambio manualmente).
  * Fijarse que también contiene:
    * CameraMainTransform y LeftCameraTransform, además de la cámara myCamera (inicializada utilizando CameraMainTransform).
      * Para detectar a donde esta mirando el usuario para que la dirección forward sea correcta. **En VR y desde el editor se utiliza la camera Main, una vez exportado se pasa a utilizar la cámara Left**, esto evita errores.
        * La rotación de la cámara en XR se hace tanto moviendo la cabeza como con el joystick derecho (solo en el eje X y seleccionando si se desea una rotación plana o mediante toques de Xº grados).
      * Permite además el movimiento de la cámara con interacción no XR.
* **NonXRInteraction**: Permite al usuario interaccionar con los objetos de la escena cuando no dispone de hardware de realidad virtual.
  * Se detecta los objetos para interaccionar haciendo uso de **Raycast, se evita el uso del ratón** para no sufrir restricciones propias de WebGL.
    * Mantiene el RayCast en el centro de la pantalla aunque esta se redimensione.
  * Permite agarrar objetos, moverlos (manteniéndolos en la posición del cursor a una distancia fija) y lanzarlos.
* **XR Teleporter Controller**: Permite al usuario teleportarse usando el controlador derecho (el objeto Teleporter que se encuentra en HandR) cuando se encuentre utilizando la aplicación en XR.
 * **DontDestroy**: Evita que el objeto se destruya al cambiar del escena, permitiendo al jugador moverse entre estas sin resetear sus propiedades (mantenerlo en XR si se da el caso).

## HandL y HandR:

Dentro de WebXRCameraSet nos encontramos con las dos manos del jugador, contienen los siguientes scripts:

* **WebXRController**: Añadido directamente del paquete WebXR Exporter, para detectar los movimientos y pulsaciones (inputs) del usuario.
* **CloseHands**: Para añadir una simple animación de abrir y cerrar la mano cuando se pulsan los botones *Grip* o *Trigger*.
* **WebXRInputManager**: Permite detectar las interacciones que realiza el jugador con el controlador, pulsar botones o mover sus sticks.
  * Puedes activar el valor *ShowDebug* a true para imprimir por pantalla estas interacciones.
  * Se ha modificado para detectar también pulsaciones en los botones *A, B, X* e *Y*.
* **ControllerInteraction**: Permite al usuario interaccionar con el entorno usando sus manos y permite agarrar objetos.
  * Se ha modificado para permitir también lanzar objetos (con un impulso de fuerza).

Además, en el caso especifico de ***HandR***, nos encontramos con el objeto ***Teleporter*** que se encuentra referenciado en el script XR Teleporter Controller (definido anteriormente) y que permite al usuario trazar la ruta (marcada gracias al objeto hijo *Quad*) al lugar donde quiere teleportarse. En él se encuentran todos los parámetros de configuración de este aspecto (entre los que se encuentra la **lista de capas que quedan excluidas del teletransporte** y a las que el usuario no podrá dirigirse).

## Cameras

Este objeto contiene en su interior un total de 5 cámaras y el script *WebXRCamera* (que viene del paquete WebXR Exporter) para acceder a ellas.

## Interaccionables especiales

Como se comentaba anteriormente los objetos con el que el usuario puede interaccionar son los que estan marcados con las layers **Interactable** e **InteractableNotMovable**.

Para permitir un acceso homogéneo a todo tipo de objetos interaccionables que tengan un comportamiento diferente al resto (controladores, botones, sliders, etc) se ha creado la clase abstracta *SpecialInteractable* (que a su vez implementa la interfaz *ISpecialInteractable*). Todas las clases que hereden de *SpecialInteractable*:

* Disponen de tres métodos (a implementar): 
  * **Grab()**: Para cuando se selecciona el objeto o se agarra. 
  * **Drop()**: Para cuando se suelta el objeto, una vez que estaba agarrado.
  * **Throw()**: Cuando se lanza o arroja el objeto (clicando en el botón del controlador).
  * Estos tres métodos aceptan como parámetro el valor booleano **isXR**, cuyo valor por defecto es false, por si se precisa de un comportamiento especial en el caso de una interacción inmersiva.
* Implementan la propiedad ***Break Interaction***, que puede habilitarse activando el parámetro *HaveBreakInteraction*. Esta propiedad permite que la interacción con el objeto se "rompa" cuando el usuario se aleja lo suficiente de él, la distancia a la que la interacción se rompe puede ser calculada usando un objeto de referencia (*Reference*) o, si no se especifica esta referencia, usando la posición inicial del objeto.
  * Si se activa la propiedad *AuthomaticUpdate* la posición del objeto será reseteada cuando se cumpla la restricción.
* Si el objeto puede agarrarse y moverse con libertad, se debe activar la propiedad *allowTranslation*.

El scripts *PlayerController* y *NonXRInteraction* detectan con que objeto se esta interaccionando, se comprueba si hereda de la clase *SpecialInteractable* y en todas las interacciones se llamará a los metodos *Grab, Drop y Throw* (de esta forma se permite llamar a multiples comportamientos diferentes sin crear muchos scripts diferentes). Puede consultarse los siguientes scripts que heredan de esta clase: ``_Scripts/Interactable/Controllers/VRPotentiometer``,  ``_Scripts/Interactable/Controllers/VRSlider`` o ``_Scripts/Interactable/Controllers/GrabbableDoor``.

**NOTA:** Debido a la característica especial que implementa *Break Interaction*, cuando un nuevo script hereda de *SpecialInteraction* se debe modificar sus metodos *Start* y *Update* de la siguiente forma:
```
    new void Start()
    {
        base.Start();
         . . .
    }

    new void Update()
    {
        base.Update();
        . . .

    }
```

## Cambio de escena

En el repositorio también se implementa un sencillo cambio de escena que permite al usuario moverse entre estas sin perder información acerca del estado de uso de XR. En los cambios de escena entran en juego tres aspectos:

* **TeleporterBehaviour.cs** este script se añade a un *gameobject* con un *trigger collider* para detectar cuando el usuario se acerca a la zona de cambio de escena.
* **GameManager.cs**, entre otras muchas funciones este script contiene las funciones que cargan escenas o teleportan al usuario. Cuando se carga una nueva escena se almacena en *SceneArguments* el índice del punto de comienzo en el que debe comenzar el usuario, luego se cambia su posición (y rotación) para coincidir con la de este punto.
  * **NOTA:** Debido a que el *PlayerController* hace uso de un *CharacterController* para el movimiento, se sufre de un pequeño error a la hora de modificar el transform del usuario. Por ello se debe **deshabilitar el CharacterController**, **modificar** el valor del **transform** para después **volver a habilitarlo**. Además, **también se debe modificar la rotación original** (inicial) del usuario, este parámetro (*originalRotation*) se encuentra en el script *PlayerController.cs* y se modifica llamando a la función publica **SetOriginalRotation(transform)**.

# Errores:

Hay varios **errores** o mejoras pendientes:
- Parece ser que el proyecto no funciona desde el navegador *Mozilla Firefox*, se deben utilizar navegadores Chronium (*Google Chrome* o *Microsoft Edge*)
- Una vez exportado el proyecto, si se accede a este desde el navegador Oculus Browser no se detecta el hardware VR y no es posible la navegación inmersiva.
  - Se precisa de usar *https* para arreglar este error.
- La cámara principal, una vez exportado el proyecto, no actualizasu rotación.
  - Una vez realizado el build se debe comprobar el *transform* de **LeftCamera** en vez de **MainCamera**.

# Fin
## Autores

* **Javier Álvarez de la Fuente** - *Investigación inicial* para Trabajo Fin de Máster y Fundación CTIC - [JavierAl95](https://github.com/javieral95)
* **Roberto Abad Jiménez** - *VR Teleport* - [Cronorobs](https://github.com/cronorobs)


## Licencia

Este proyecto ha sido realizado para uso propio y para la Fundación CTIC, su uso es libre y no es necesarío ningún crédito en su uso (Revisar las licencia de las librerias y paquetes utilizados).