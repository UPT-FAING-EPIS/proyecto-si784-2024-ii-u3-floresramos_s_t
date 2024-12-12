Diagrama de clases

```mermaid
classDiagram

class DetalleSuscripcionRepository
DetalleSuscripcionRepository : +ObtenerPorUsuarioId() DetalleSuscripcion
DetalleSuscripcionRepository : +ActualizarSuscripcion() Void

class OperacionesPDFRepository
OperacionesPDFRepository : +RegistrarOperacionPDF() Boolean
OperacionesPDFRepository : +ObtenerOperacionesPorUsuario() IEnumerable~OperacionPDF~
OperacionesPDFRepository : +ContarOperacionesRealizadas() Int
OperacionesPDFRepository : +ValidarOperacion() Boolean

class UsuarioRepository
UsuarioRepository : +Login() Usuario
UsuarioRepository : +RegistrarUsuario() Void
UsuarioRepository : +ObtenerUsuarioPorId() Usuario
UsuarioRepository : +ObtenerUsuarios() IEnumerable~Usuario~

class DetalleSuscripcion
DetalleSuscripcion : +Int Id
DetalleSuscripcion : +String tipo_suscripcion
DetalleSuscripcion : +Nullable~DateTime~ fecha_inicio
DetalleSuscripcion : +Nullable~DateTime~ fecha_final
DetalleSuscripcion : +Nullable~Decimal~ precio
DetalleSuscripcion : +Int operaciones_realizadas
DetalleSuscripcion : +Int UsuarioId
DetalleSuscripcion : +Usuario Usuario

class OperacionPDF
OperacionPDF : +Int Id
OperacionPDF : +Int UsuarioId
OperacionPDF : +String TipoOperacion
OperacionPDF : +DateTime FechaOperacion

class Usuario
Usuario : +Int Id
Usuario : +String Nombre
Usuario : +String Correo
Usuario : +String Password


Usuario <-- DetalleSuscripcion

```
Diagrama de Casos de Uso

```mermaid
graph TD
    Usuario[Usuario] --> IniciarSesion[Iniciar sesión]
    Usuario --> CerrarSesion[Cerrar sesión]
    Usuario --> RegistrarNuevoUsuario[Registrar nuevo usuario]
    Usuario --> SubirArchivoPDF[Subir un archivo PDF]
    Usuario --> FusionarPDFs[Fusionar PDFs]
    Usuario --> CortarPDFs[Cortar PDFs]
    Usuario --> VisualizarPlanesSuscripcion[Visualizar planes de suscripción]
    Usuario --> ActualizarSuscripcion[Actualizar suscripción]
```

Diagrama de Componentes

```mermaid
graph LR
    subgraph PROYECTOPDF
        PCORE[PROYECTOPDF.dll]
        MSASPNET[Microsoft.AspNetCore.Mvc.dll]
        PDFSHARP[PDFSharp.dll]
        MSRELATIONAL[Microsoft.EntityFrameworkCore.Relational.dll]
        PANELSENTITY[Panels.EntityFrameworkCore.MyPag.dll]
        MSTOOLS[Microsoft.EntityFrameworkCore.Tools.dll]
    end

    subgraph NegocioPDF.Tests
        MOQLIB[Moq.dll]
        NEGOPDFTEST[NegocioPDF.Tests.dll]
        NEGOPDF[NegocioPDF.dll]
        GENERICCOL[Generic.collector.dll]
        DAPPER[Dapper.dll]
        MSCORE[Microsoft.EntityFrameworkCore.dll]
        MSMEMORY[Microsoft.EntityFrameworkCore.InMemory.dll]
        MSTEST[Microsoft.NET.Test.Sdk.dll]
        MSPAYMENT[Microsoft.Playwright.dll]
        MSPLAYTEST[Microsoft.Playwright.MSTest.dll]
        SFLOW[SpecFlow.dll]
        SFLOWMSTEST[SpecFlow.MsTest.dll]
        SFLOWTOOLS[SpecFlow.Tools.MsBuild.Generation.dll]
        SFLOWPLUS[SpecFlow.Plus.LivingDocPlugin.dll]
         MSADAPTER[MSTest.TestAdapter.dll]
        MSFRAMEWORK[MSTest.TestFramework.dll]
        MSDATA[MSData.dll]
        ANALYZERLIB[NUnit.Analyzers.dll]
        NUNITADAPTER[NUnit3TestAdapter.dll]
    end

    subgraph NegocioPDF
        NEGOPDFCORE[NegocioPDF.dll]
        DAPPERCONTROLS[Dapper.Controls.dll]
        MSSQLSERVER[Microsoft.EntityFrameworkCore.SqlServer.dll]
    end
```

Diagrama de Despliegue

```mermaid
graph TB
    subgraph Cliente
        Browser[Navegador Web]
    end

    subgraph ServidorWeb[Servidor Web]
        subgraph Controladores
            AuthController
            RegistrationController
            SuscripcionController
            OperacionesPDFController
        end

        subgraph Modelos
            Usuario
            DetalleSuscripcion
        end

        subgraph Vistas
            LoginView
            MenuPrincipalView
            OperacionesView
            RegistroView
        end

        ASPNET[ASP.NET Core MVC]
    end

    subgraph ServidorBD[Servidor de Base de Datos]
        subgraph Tablas
            Operaciones
            MySQL[MySQL/MariaDB]
        end
    end

    Browser -->|HTTP/HTTPS| ASPNET
    ASPNET -->|Consultas SQL| MySQL
```
DIAGRAMA DE SECUENCIA

1.Iniciar Sesion
```mermaid
sequenceDiagram
    actor Usuario
    participant SA as Sistema de Autenticación
    participant BD as Base de Datos
    
    Usuario->>SA: Ingresar credenciales
    SA->>BD: Validar credenciales
    BD-->>SA: Resultado validación
    SA-->>Usuario: Acceso concedido/denegado
```
2. Comprar Suscripción:
```mermaid
   sequenceDiagram
    actor Usuario
    participant SS as Sistema de Suscripciones
    participant PP as Pasarela de Pago
    participant BD as Base de Datos
    
    Usuario->>SS: Seleccionar plan
    SS->>PP: Iniciar transacción
    PP->>PP: Procesar pago
    PP-->>SS: Confirmación de pago
    SS->>BD: Registrar suscripción
    BD-->>SS: Confirmación registro
    SS-->>Usuario: Suscripción activada
```
4. Fusionar PDF:
```mermaid
   sequenceDiagram
    actor Usuario
    participant SP as Servicio de PDF
    participant GA as Gestor de Archivos
    
    Usuario->>SP: Subir archivos PDF
    SP->>GA: Validar archivos
    GA-->>SP: Archivos válidos
    GA->>GA: Fusionar PDFs
    GA->>GA: Guardar PDF fusionado
    GA-->>SP: Confirmación guardado
    SP-->>Usuario: Enlace descarga PDF
```
6. Cortar PDF:
```mermaid
   sequenceDiagram
    actor Usuario
    participant SP as Servicio de PDF
    participant GA as Gestor de Archivos
    
    Usuario->>SP: Subir PDF
    Usuario->>SP: Seleccionar páginas a cortar
    SP->>GA: Validar archivo
    GA-->>SP: Archivo válido
    GA->>GA: Cortar páginas
    GA->>GA: Guardar PDF cortado
    GA-->>SP: Confirmación guardado
    SP-->>Usuario: Enlace descarga PDF
```
8. Ver Operaciones Realizadas:
```mermaid
   sequenceDiagram
    actor Usuario
    participant SR as Sistema de Registro
    participant BD as Base de Datos
    
    Usuario->>SR: Solicitar historial
    SR->>BD: Consultar operaciones usuario
    BD-->>SR: Listado de operaciones
    SR-->>Usuario: Mostrar historial operaciones
```

DIAGRAMA DE ARQUITECTURA DE SOFTWARE

```mermaid
graph TD
    subgraph Presentation Layer
        Views
        Views -->|Invoca| AuthController
        Views -->|Invoca| OperacionesPDFController
        Views -->|Invoca| RegistrationController
        Views -->|Invoca| SuscripcionController
    end

    subgraph Business Logic Layer
        ErrorViewModel
        Models
        AuthController -->|Usa| Models
        OperacionesPDFController -->|Usa| Models
        RegistrationController -->|Usa| Models
        SuscripcionController -->|Usa| Models
    end

    subgraph Data Access Layer
        Database
        Models -->|Accede a datos| Database
    end
```
 
