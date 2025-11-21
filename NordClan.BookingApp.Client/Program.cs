using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NordClan.BookingApp.Client;
using NordClan.BookingApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<IRoomService, RoomService>(x =>
{
    x.BaseAddress = new Uri("http://localhost:5000/");
})
.AddHttpMessageHandler<AuthHttpHandler>()
.AddHttpMessageHandler<ResponseTimeHandler>();

builder.Services.AddHttpClient<IBookingService, BookingService>(x =>
{
    x.BaseAddress = new Uri("http://localhost:5000/");
})
.AddHttpMessageHandler<AuthHttpHandler>()
.AddHttpMessageHandler<ResponseTimeHandler>();

builder.Services.AddTransient<AuthHttpHandler>();
builder.Services.AddTransient<ResponseTimeHandler>();

builder.Services.AddHttpClient<IAuthService, AuthService>(x =>
{
    x.BaseAddress = new Uri("http://localhost:5000/");
})
.AddHttpMessageHandler<ResponseTimeHandler>();

await builder.Build().RunAsync();