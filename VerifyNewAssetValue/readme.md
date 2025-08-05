# TimerTrigger - C<span>#</span>

The `TimerTrigger` makes it incredibly easy to have your functions executed on a schedule. This sample demonstrates a simple use case of calling your function every 5 minutes.

## How it works

For a `TimerTrigger` to work, you provide a schedule in the form of a [cron expression](https://en.wikipedia.org/wiki/Cron#CRON_expression)(See the link for full details). A cron expression is a string with 6 separate expressions which represent a given schedule via patterns. The pattern we use to represent every 5 minutes is `0 */5 * * * *`. This, in plain text, means: "When seconds is equal to 0, minutes is divisible by 5, for any hour, day of the month, month, day of the week, or year".

## Learn more

## Configuración para ejecutar Azure Functions con .NET 9 localmente

### 1. Actualizar soporte para .NET 9 en Visual Studio

Para que Visual Studio pueda reconocer y ejecutar Azure Functions en .NET 9, realiza los siguientes pasos:

1. Abre Visual Studio.
2. Ve a:  
   **Tools -> Options -> Projects and Solutions -> Azure Functions**.
3. Haz clic en el botón **"Check for updates"**.
4. Espera a que la actualización se complete y reinicia Visual Studio si es necesario.


### 2. Instalar herramientas necesarias de forma global

Ejecuta los siguientes comandos desde una terminal (Git Bash recomendado):

npm install -g azure-functions-core-tools@4 --unsafe-perm true
npm install -g azurite

### 3. Iniciar Azurite antes de ejecutar las funciones
Antes de ejecutar cualquier función, asegúrate de tener Azurite corriendo. En una terminal (Git Bash preferiblemente), ejecuta:

Azurite --silent --location c:\azurite --debug c:\azurite\debug.log

### 4. Ejecutar la Azure Function desde Visual Studio
Con Azurite corriendo:

En Visual Studio, haz clic derecho sobre el proyecto de la Azure Function.

Ve a Debug -> Start new instance.

La función se ejecutará localmente y se conectará automáticamente a Azurite.