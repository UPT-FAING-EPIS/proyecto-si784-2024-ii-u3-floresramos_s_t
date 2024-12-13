using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using MSTest = Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
namespace NegocioPDF.Tests.UITests;

[MSTest.TestClass]
public class SubscriptionTests : PageTest
{
    private string baseUrl = "http://localhost:5260";
    
    // Al inicio de la clase, añade el email y password como variables estáticas
private static string testEmail = $"test{Guid.NewGuid()}@example.com";
private static string testPassword = "Password123!";
    
     private readonly string _connectionString = "Server=161.132.49.127;Database=pdfsolutions;Uid=ballmer;Pwd=Upt2024;";
    [TestInitialize]
    public async Task TestSetup()
    {
        // Verificar que la aplicación esté corriendo
        using (var client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync(baseUrl);
                if (!response.IsSuccessStatusCode)
                {
                     MSTest.Assert.Fail("La aplicación web no está respondiendo. Asegúrese de que esté corriendo en " + baseUrl);
                }
            }
            catch (Exception)
            {
                 MSTest.Assert.Fail("No se puede conectar a la aplicación web. Asegúrese de que esté corriendo en " + baseUrl);
            }
        }
    }
   [TestCleanup]
public async Task CleanupAfterTest()
{
    using var connection = new MySqlConnection(_connectionString);
    await connection.OpenAsync();

    // Limpiar datos de prueba con los nombres correctos de las tablas
    var commands = new[]
    {
        "DELETE FROM detalles_suscripciones WHERE 1=1",
        "DELETE FROM operaciones_pdf WHERE 1=1",
        "DELETE FROM Usuarios WHERE Correo LIKE '%test%' OR Correo = 'wrong@email.com'"
    };

    foreach (var command in commands)
    {
        using var cmd = new MySqlCommand(command, connection);
        try
        {
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing cleanup: {ex.Message}");
            // Continuar con el siguiente comando incluso si este falla
        }
    }
}
    [MSTest.TestMethod]
    public async Task LoginFailure_ShouldShowError()
    {
        var browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = true,
            SlowMo = 1000
        });

        var context = await browser.NewContextAsync(new()
        {
            RecordVideoDir = "videos",
            RecordVideoSize = new() { Width = 1024, Height = 768 }
        });

        var page = await context.NewPageAsync();

        try
        {
            Console.WriteLine("Iniciando login fallido...");
            await page.GotoAsync($"{baseUrl}/Auth/Login");

            // Llenar formulario con credenciales inválidas
            await page.FillAsync("input[name='correo']", "wrong@email.com");
            await page.FillAsync("input[name='password']", "WrongPassword123!");
            
            // Tomar screenshot antes de enviar
            await page.ScreenshotAsync(new() { Path = "before-login-failure.png" });

            // Click en login
            await page.GetByRole(AriaRole.Button, new() { Name = "Iniciar Sesión" }).ClickAsync();
            await Task.Delay(2000);

            // Verificar mensaje de error
            MSTest.Assert.IsTrue(await page.Locator(".alert-danger").IsVisibleAsync());

            await page.ScreenshotAsync(new() { Path = "after-login-failure.png" });
        }
        finally
        {
            await context.CloseAsync();
            await browser.CloseAsync();
        }
    }

    [MSTest.TestMethod]
    public async Task Register_NewUser()
    {
        var browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = true,
            SlowMo = 1000
        });

        var context = await browser.NewContextAsync(new()
        {
            RecordVideoDir = "videos",
            RecordVideoSize = new() { Width = 1024, Height = 768 }
        });

        var page = await context.NewPageAsync();

        try
        {
            Console.WriteLine("Iniciando registro de usuario...");
            await page.GotoAsync($"{baseUrl}/Registration/Registrarse");

            // Llenar formulario de registro
            await page.FillAsync("input[name='Nombre']", "Usuario Test");
            await Task.Delay(1000);
            
            await page.FillAsync("input[name='Correo']", "test@example.com");
            await Task.Delay(1000);
            
            await page.FillAsync("input[name='Password']", "Password123!");
            await Task.Delay(1000);

            // Screenshot antes de enviar
            await page.ScreenshotAsync(new() { Path = "before-registration.png" });

            // Click en registrarse
            await page.GetByRole(AriaRole.Button, new() { Name = "Registrarse" }).ClickAsync();
            await Task.Delay(2000);

            // Verificar mensaje de éxito
            MSTest.Assert.IsTrue(await page.Locator(".alert-success").IsVisibleAsync());

            await page.ScreenshotAsync(new() { Path = "after-registration.png" });
        }
        finally
        {
            await context.CloseAsync();
            await browser.CloseAsync();
        }
    }

    [MSTest.TestMethod]
            public async Task TestPremiumSubscriptionFlow()
            {
                var browser = await Playwright.Chromium.LaunchAsync(new()
                {
                    Headless = true,
                    SlowMo = 1000
                });

                var context = await browser.NewContextAsync(new()
                {
                    RecordVideoDir = "videos",
                    RecordVideoSize = new() { Width = 1024, Height = 768 }
                });

                var page = await context.NewPageAsync();

                try
                {
                    // 1. Login
                     Console.WriteLine("Iniciando registro de usuario...");
            await page.GotoAsync($"{baseUrl}/Registration/Registrarse");

            // Llenar formulario de registro
            await page.FillAsync("input[name='Nombre']", "Usuario Test");
            await Task.Delay(1000);
            
            await page.FillAsync("input[name='Correo']", "test@example.com");
            await Task.Delay(1000);
            
            await page.FillAsync("input[name='Password']", "Password123!");
            await Task.Delay(1000);
            await page.GetByRole(AriaRole.Button, new() { Name = "Registrarse" }).ClickAsync();
            await Task.Delay(2000);

                    Console.WriteLine("Iniciando login...");
                    await page.GotoAsync($"{baseUrl}/Auth/Login");
                    await page.FillAsync("input[name='correo']", "test@example.com");
                    await page.FillAsync("input[name='password']", "Password123!");
                    await page.GetByRole(AriaRole.Button, new() { Name = "Iniciar Sesión" }).ClickAsync();
                    
                    // Esperar a que la página se cargue completamente
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await Task.Delay(5000); // Esperar 5 segundos adicionales

                    // Tomar screenshot después del login
                    await page.ScreenshotAsync(new() { Path = "after-login-premium.png" });

                    // 2. Buscar y verificar el enlace de Premium
                    Console.WriteLine("Buscando enlace Premium...");
                    var premiumLink = page.GetByRole(AriaRole.Link, new() { Name = "Comprar Premium" });
                    if (!await premiumLink.IsVisibleAsync())
                    {
                        // Tomar screenshot si no se encuentra el enlace
                        await page.ScreenshotAsync(new() { Path = "premium-link-not-found.png" });
                        MSTest.Assert.Fail("No se encontró el enlace 'Comprar Premium'");
                    }

                    await premiumLink.ClickAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await Task.Delay(2000);

                    // Tomar screenshot de la página de suscripción
                    await page.ScreenshotAsync(new() { Path = "premium-page.png" });

                    // 3. Verificar elementos en la página
                    var elements = new[] 
                    {
                        "text=Detalles de la Suscripción",
                        "text=Fecha de Inicio",
                        "text=Fecha Final",
                        "text=Precio"
                    };

                    foreach (var element in elements)
                    {
                        if (!await page.Locator(element).IsVisibleAsync())
                        {
                            await page.ScreenshotAsync(new() { Path = $"missing-element-{element}.png" });
                            MSTest.Assert.Fail($"No se encontró el elemento: {element}");
                        }
                    }

                    // 4. Confirmar compra
                    var buyButton = page.GetByRole(AriaRole.Button, new() { Name = "Comprar Suscripción Premium" });
                    if (!await buyButton.IsVisibleAsync())
                    {
                        await page.ScreenshotAsync(new() { Path = "buy-button-not-found.png" });
                        MSTest.Assert.Fail("No se encontró el botón de compra");
                    }

                    await buyButton.ClickAsync();
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    await Task.Delay(2000);

                    // Verificación final
                    var successElement = page.Locator("text=Operaciones Realizadas: Ilimitadas");
                    if (!await successElement.IsVisibleAsync())
                    {
                        await page.ScreenshotAsync(new() { Path = "premium-verification-failed.png" });
                        MSTest.Assert.Fail("No se pudo verificar la compra premium");
                    }

                    await page.ScreenshotAsync(new() { Path = "premium-success.png" });
                }
                finally
                {
                    await context.CloseAsync();
                    await browser.CloseAsync();
                }
            }
 [MSTest.TestMethod]
public async Task LoginSuccess_ShouldShowOperationsButton()
{
    var browser = await Playwright.Chromium.LaunchAsync(new()
    {
        Headless = true,
        SlowMo = 1000
    });

    var context = await browser.NewContextAsync(new()
    {
        RecordVideoDir = "videos",
        RecordVideoSize = new() { Width = 1024, Height = 768 }
    });

    var page = await context.NewPageAsync();

    try
    {
               Console.WriteLine("Iniciando registro de usuario...");
            await page.GotoAsync($"{baseUrl}/Registration/Registrarse");

            // Llenar formulario de registro
            await page.FillAsync("input[name='Nombre']", "Usuario Test");
            await Task.Delay(1000);
            
            await page.FillAsync("input[name='Correo']", "test@example.com");
            await Task.Delay(1000);
            
            await page.FillAsync("input[name='Password']", "Password123!");
            await Task.Delay(1000);
            await page.GetByRole(AriaRole.Button, new() { Name = "Registrarse" }).ClickAsync();
            await Task.Delay(2000);
        Console.WriteLine("Iniciando login exitoso...");
        await page.GotoAsync($"{baseUrl}/Auth/Login");

        await page.FillAsync("input[name='correo']", "test@example.com");
        await page.FillAsync("input[name='password']", "Password123!");
        
        await page.ScreenshotAsync(new() { Path = "before-login.png" });
        await page.GetByRole(AriaRole.Button, new() { Name = "Iniciar Sesión" }).ClickAsync();
        
        // Esperar a que la página se cargue completamente
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Task.Delay(5000);

        await page.ScreenshotAsync(new() { Path = "after-login-verification.png" });

        // Verificar elementos con mejor manejo de errores
        var operationsButton = page.GetByRole(AriaRole.Link, new() { Name = "Realizar Operaciones" });
        var logoutButton = page.GetByRole(AriaRole.Link, new() { Name = "Cerrar Sesión" });

        if (!await operationsButton.IsVisibleAsync())
        {
            await page.ScreenshotAsync(new() { Path = "operations-button-missing.png" });
            MSTest.Assert.Fail("No se encontró el botón 'Realizar Operaciones'");
        }

        if (!await logoutButton.IsVisibleAsync())
        {
            await page.ScreenshotAsync(new() { Path = "logout-button-missing.png" });
            MSTest.Assert.Fail("No se encontró el botón 'Cerrar Sesión'");
        }

        await page.ScreenshotAsync(new() { Path = "after-login-success.png" });
    }
    finally
    {
        await context.CloseAsync();
        await browser.CloseAsync();
    }
}
    
}