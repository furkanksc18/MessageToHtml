using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();      // Controller desteği ekliyoruz
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();        // Swagger (OpenAPI) için

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1073741824; // 1 GB = 1024 * 1024 * 1024 = 1073741824 bytes
});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 1073741824; // 1 GB
});


// CORS EKLENDİ
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(); // CORS middleware'i eklendi

app.UseAuthorization();

app.UseDefaultFiles();

app.UseStaticFiles();

app.MapControllers();   // Controller’larımızı eşle


var serverAddresses = app.Urls.Any() ? app.Urls : builder.Configuration.GetValue<string>("urls")?.Split(';') ?? Array.Empty<string>();

app.Lifetime.ApplicationStarted.Register(() =>
{
    foreach (var address in serverAddresses)
    {
        try
        {
            var url = address.Replace("*", "localhost");
            var frontendUrl = url.TrimEnd('/');
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = frontendUrl,
                UseShellExecute = true
            });
            break;
        }
        catch
        {
            // tarayıcı açılamazsa sessiz geç
        }
    }
});

app.Run();
