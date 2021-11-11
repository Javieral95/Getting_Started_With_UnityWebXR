# Getting_Started_With_UnityWebXR

El presente proyecto busca realizar una pequeña prueba de concepto sobre el nuevo estándar [WebXR](https://www.w3.org/TR/webxr/), que permite hacer accesible realidad virtual y aumentada en aplicaciones Web, integrandolo con el motor gráfico **Unity**. La idea es permitir el uso e interacción de la aplicación inmersiva a través de Web tanto si se dispone  de Hardware especifico como si no.

Esta investigación se ha llevado a cabo para desarrollar el Trabajo de Fin de Máster de la titulación [Máster universitario en Ingeniería informática](https://masterinformatica.uniovi.es/) por la [Universidad de Oviedo](https://www.uniovi.es/) y para recabar información que podrá ser utilizado para proyectos propiedad de [Fundación CTIC](https://www.fundacionctic.org/es/home).

## Tabla de contenido
- [Getting_Started_With_UnityWebXR](#getting_started_with_unitywebxr)
  - [Tabla de contenido](#tabla-de-contenido)
- [Pre-requisitos](#pre-requisitos)
- [Comenzando](#comenzando)
  - [Importando los paquetes](#importando-los-paquetes)
    - [WebXR Exporter](#webxr-exporter)
    - [WebXR Exporter Advance](#webxr-exporter-advance)
  - [Probando y exportando el proyecto](#probando-y-exportando-el-proyecto)
    - [Controles de la escena](#controles-de-la-escena)
- [Scripts del proyecto y funcionamiento](#scripts-del-proyecto-y-funcionamiento)
  - [Interaccionables](#interaccionables)
  - [WebXRCameraSet](#webxrcameraset)
  - [HandL y HandR](#handl-y-handr)
  - [Cameras](#cameras)
- [TO-DO:](#to-do)
- [Fin](#fin)
  - [Autores ✒️](#autores-️)
  - [Licencia 📄](#licencia-)


_________________
# Pre-requisitos

Este proyecto se ha llevado a cabo haciendo uso de la versión *LTS 2019.4.27f1* de Unity y se han utilizado unas *Oculus Quest 2* conectadas al PC con cable mediante *Oculus Link*.

Y ha sido desarrollado utilizando los siguientes repositorios como base (más adelante veremos como hacer uso de ellos):
- [Unity-WebXR-Export](https://github.com/De-Panther/unity-webxr-export): Para la exportación del proyecto haciendo uso de WebXR.
- [WebXR Exporter Advanced](https://github.com/Jocyf/WebXR-Exporter-Advanced): Para desarrollar los controladores del usuario.

Estos dos a su vez han sido basados en [Mozilla WebXR Exporter](https://assetstore.unity.com/packages/tools/integration/webxr-exporter-109152), aunque se ha de tener en cuenta que **el paquete de Mozilla en la Asset Store de Unity se encuentra desactualizado** y ocasiona errores que hará que la aplicación no sea accesible por hardware VR una vez exportado a Web. Y, es por ello que, se deberá hacer uso del repositorio *Unity-WebXR-Export* antes mencionado.


# Comenzando

Dado que los paquetes propiedades Mozilla se encuentran desactualizados, hay que acceder a alternativas que han solucionado los errores previamente.

## Importando los paquetes

El proyecto del presente proyecto ya tiene importado todo lo necesario para hacerlo funcionar, no obstante si empiezas tu proyecto de cero tendrás que importar los siguientes paquetes:

### WebXR Exporter
Primero tendremos que importar el paquete que nos permitirá exportar nuestro proyecto en WebXR. Para ello, importa el proyecto **Unity-WebXR-Export**  de *De-Panther* siguiendo las siguientes instrucciones indicadas en su [Readme.md](https://github.com/De-Panther/unity-webxr-export/blob/master/Packages/webxr/README.md):

- Usando **OpenUPM**, para ello añade el registro en ``Project Settings > Package Manager`` con las siguientes opciones:
    ```
    Name: OpenUPM
    URL: https://package.openupm.com
    Scope(s): com.de-panther
    ```    
    - Después, en el Package Manager descarga e importa el paquete **WebXR Export**.

-  Usando **Git**, para ello accede al Package Manager y selecciona la opción ``+ > Add package from girl url..``, indica la siguiente URL:
     ```
     https://github.com/De-Panther/unity-webxr-export.git?path=/Packages/webxr
     ```  
Existen mas paquetes además de **WebXR**, por ejemplo el de **WebXR Interactions** que tiene además una escena de ejemplo para probar el exporter y sus controladores, pero en este ejemplo se han implementado otros controladores.

Una vez importando, accede a ``Window > WebXR > Copy WebGLTemplates`` para copiar las plantillas de exportación en tu proyecto.

### WebXR Exporter Advance

Este paquete de *jocyf* se ha utilizado para usar de base en el desarrollo de los controladores del proyecto, este a su vez parte del paquete WebXR Exporter, puedes descargarlo directamente desde [aquí](https://www.dropbox.com/s/gvnfxfzrg1k1mw7/WebXRAdvancedDePanther.unitypackage?dl=0). Tambin puedes acceder al siguiente [repositorio](https://github.com/Jocyf/WebXR-Exporter-Advanced).

Este paquete otorga funcionalidades no implementadas en el presente repositorio (giro con click del raton, teletransporte en VR, interacción a distancia por Raycaster...). 

En el presente repositorio se han desarrollado los scripts de la siguiente manera:
- **PlayerController**: Para el movimiento, basado en WebXRMovev2 (que hace uso de CharacterController en vez de un collider).
- **ControllerInteraction**: Para interaccionar con objetos, basado en WebXRGrabManager. 
- **WebXRInputManager**: Ha sido modificado para añadir funcionalidad a los botones A, B, X e Y de los controladores VR.

## Probando y exportando el proyecto

En el presente repositorio, con los paquetes ya importado se encuentra una escena de ejemplo: ``Assets/0Assets/Scenes/MainScene``, vamos a utilizarla para probar WebXR.

Una vez importados los paquetes y copiadas las plantillas (si no encuentras el directorio ``Assets/WebGL Templates`` accede a la opción ``Window > WebXR > Copy WebGLTemplates``) tendremos que realizar unas configuraciones. 
- ``Edit > Project Settings > XR Plug-in Management > Initialize XR on Startup``.
  - Habilita tu Hardware en la pestaña *PC, Mac & Linux Standalone Settings*.
  - En la pestaña *WebGL* deberás habilitar la casilla **WebXR Export**.
-  ``File > Build Settings`` añade la escena y cambia a la plataforma **WebGL**.
-  ``Edit > Project Settings > Player > WebGL > Resolution and Presentation > WebGL Template`` selecciona cualquier plantilla (WebXR ha funcionado correctamente). 

¡El proyecto esta listo! Selecciona ``Build and Run``, una vez finalizado el proceso podrás probar la escena en tu navegador:
- Habilita que la aplicación acceda a los dispositivos de realidad virtual.
- Funciona en navegadores Chromium (como Google chrome o Microsoft Edge), pero en Mozilla Firefox la escena no carga en el dispositivo de realidad virtual.

### Controles de la escena

Es posible interactuar con el entorno tanto con hardware de realidad virtual como sin él. Aunque los controles son mostrados en la escena, se detallan a continuación:
- XR activado (VR hardware):
  - Movimiento de cámara moviendo la cabeza (o con stick derecho).
  - Movimiento de translación con stick izquierdo.
  - Agarrar objetos o activar botones manteniendo pulsado *Trigger* y *Grip*.
    - Soltar objetos soltando el botón *Trigger* y *Grip*. 
    - Lanzar objetos con el botón *A* o *X*
  - Saltar con el botón *Y*. 
- Sin XR (sin VR hardware):
  - Movimiento de cámara moviendo el mouse.
  - Movimiento de traslación con *WASD* o las flechas del teclado.
  - Agarrar objetos o activar botones manteniendo pulsado el click izquierdo del mouse.
    - Soltar objetos soltando el click izquierdo del mouse.
    - Lanzar objetos con el click derecho del mouse.
  - Saltar con la barra espaciadora.    

Puedes activar el modo VR en cualquier momento conectado el hardware y clicando en *VR*, la aplicación lo detectará automaticamente habilitando el movimiento e interacción XR y deshabilitando el no XR. Puedes realizar el proceso inverso tan solo quitandote las gafas. Cambiar el modo reiniciará la posición del usuario.

Ahora... ¡lanza cubos y pelotas!

____________________

# Scripts del proyecto y funcionamiento

Para **evitar errores** en el editor se ha tenido que añadir una *Assembly Definition Reference* que apunte a *WebXR* de los paquetes importados.

## Interaccionables

Antes de nada, cabe destacar que los objetos con los cuales puede interaccionar el usuario deben tener la etiqueta **Interactable** (si se le permite agarrarlo y lanzarlo) o **InteractableTrigger** si se le permite interactuar con él de otra forma (un botón, por ejemplo).

## WebXRCameraSet
El objeto principal de la escena es el usuario o jugador, este será el objeto **WebXRCameraSet**, puedes ver el prefab en ``Assets/0Assets/MainScene/Prefabs/Player``.

Este objeto contiene en su interior las manos del usuario y las cámaras, además de los scripts mas importantes de la escena:
- **WebXrManager**: Añadido directamente del paquete WebXR Exporter.
- **PlayerController**: Encargado de detectar el hardware XR y permitir el movimiento del jugador.
  - Ha sido altamente modificado para permitir el movimiento en XR y no XR por igual.
  - En este script la detección y activación del hardware de realidad virtual se hace gracias a los eventos *onXRChange* y *onXRCapabilitiesUpdate*.
  - Cuando se detecte el hardware se deshabilitará la interacción noXR y se activarán las manos del jugador, cuando se detecte que no se dispone de este se habilitarán las manos del jugador. Puedes forzar este cambio modificando el valor de *isXREnabled* en el editor de Unity.
  - Fijarse que contiene los objetos de las cámaras y de las manos del usuario, por lo que en este script también se detecta la información recibida a través de los stickers de los contradores y se rotará la cámara de este.
- **NonXRInteraction**: Permite al usuario interaccionar con los objetos de la escena cuando no dispone de hardware de realidad virtual.
  - Se detecta los objetos para interaccionar haciendo uso de Raycast.

## HandL y HandR

Dentro de **WebXRCameraSet** nos encontramos con las dos manos del jugador, contiene los siguientes scripts:
- **WebXRController**: Añadido directamente del paquete WebXR Exporter, para detectar los movimientos y pulsaciones (inputs) del usuario.
- **CloseHands**: Para añadir una simple animación de abrir y cerrar la mano cuando se pulsan los botones *Grip* o *Trigger*.
- **WebXRInputManager**: Permite detectar las interacciones que realiza el jugador con el controlador, pulsar botones o mover sus sticks.
  - Puedes activar el valor ShowDebug a true para imprimir por pantalla estas interacciones.
  - Se ha modificado para detectar también pulsaciones en los botones A,B,X e Y.
- **ControllerInteraction**: Permite al usuario interaccionar con el entorno usando sus manos o agarrar objetos.
  - Se ha modificado para permitir también lanzar objetos.

## Cameras

Este objeto contiene en su interior un total de 5 camaras y el script **WebXRCamera** (desde paquete WebXR Exporter) para acceder a ellas.

# TO-DO:

Hay varios **errores** o mejoras pendientes:
- El jugador debe moverse en la dirección *forward* de la cámara principal, esto no se da en la aplicación WebXR una vez exportada utilizando hardware de realidad virtual (pero si en el editor).
- La posición del jugador debería ser reiniciada una vez entra en contacto con la Killzone (cuando cae al vacio), se detecta la colisión pero el jugador no se teletransportar.
- La interacción sin XR no funciona de forma correcta (mala posición de mouse y de la dirección al lanzar los objetos).

# Fin
## Autores ✒️

* **Javier Álvarez de la Fuente** - *Investigación inicial* para TFM y Fundación CTIC - [JavierAl95](https://github.com/javieral95)


## Licencia 📄

Este proyecto ha sido realizado para uso propio y para la Fundación CTIC, su uso es libre y no es necesarío ningún crédito en su uso (Revisar las licencia de las librerias y paquetes utilizados).