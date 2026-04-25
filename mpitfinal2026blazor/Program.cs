using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using mpitfinal2026blazor;
using mpitfinal2026blazor.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddScoped<ZhgutLlmService>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var storageService = sp.GetRequiredService<StorageService>();
    return new ZhgutLlmService(httpClient, storageService);
});
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<StorageService>();

await builder.Build().RunAsync();
