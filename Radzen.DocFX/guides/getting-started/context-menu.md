# Context menu setup
This article shows the additional setup steps required to use the RadzenContextMenu component.

1. [Register](#service-registration) the service.
1. [Add](#add-to-layout) the component to your layout.

## Service registration
The RadzenContextMenu is used via the [ContextMenuService](xref:Radzen.ContextMenuService) class which must be registered as a service.

# [Server-side Blazor](#tab/server-side)
1. Open `Startup.cs`
1. Import the Radzen namespace
   ```
   using Radzen;
   ```
1. Register the `ContextMenuService` in the `ConfigureServices` method.
   ```
   public void ConfigureServices(IServiceCollection services)
   {
       // Other registrations
       services.AddScoped<ContextMenuService>();
       // Other registrations
   }
   ```
# [Client-side Blazor](#tab/client-side)
1. Open `Program.cs`
1. Import the Radzen namespace
   ```
   using Radzen;
   ```
1. Register the `ContextMenuService` in the `Main` method.
   ```
   public static async Task Main(string[] args)
   {
       // Other registrations
       builder.Services.AddScoped<ContextMenuService>();
       // Other registrations
   }
   ```
***
## Add to layout
You also need to add the RadzeContextMenu component to the layout used by your pages (most commonly `MainLayout.razor`). 
```
<RadzenContextMenu />
```
