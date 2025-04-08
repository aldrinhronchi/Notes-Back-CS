using Notes_Back_CS.Extensions;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
NativeInjector.RegisterBuild(builder);

IServiceCollection services = builder.Services;
IConfiguration configuration = builder.Configuration;
NativeInjector.RegisterServices(configuration, services);

WebApplication? app = builder.Build();

NativeInjector.ConfigureApp(app, app.Environment);
app.MapControllers();

Console.WriteLine($"App Started running in {DateTime.Now:dd/MM/yyyy HH:mm:ss}");

app.Run();