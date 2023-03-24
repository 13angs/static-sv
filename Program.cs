using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Serialization;
using static_sv.Exceptions;
using static_sv.Interfaces;
using static_sv.Models;
using static_sv.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration configuration = builder.Configuration;

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRequestValidator, RequestValidator>();
builder.Services.AddScoped<IStaticfile, StaticfileService>();
builder.Services.AddScoped<IStaticDirectory, StaticDirectoryService>();
builder.Services.AddScoped<IContent, ContentService>();
builder.Services.AddScoped<IFolder, FolderService>();

// configure controller to use Newtonsoft as a default serializer
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
            .Json.ReferenceLoopHandling.Ignore)
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver
                    = new DefaultContractResolver()
);

builder.Services.AddDbContextPool<StaticContext>(option => {
    var env = builder.Environment;
    string conStr = Path.Combine(env.ContentRootPath, configuration["SqliteDb:Path"], configuration["SqliteDb:Name"]);
    Console.WriteLine(conStr);
    option.UseSqlite($"Data Source={conStr}");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseResponseExceptionHandler();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
                    Path.Combine(app.Environment.ContentRootPath, configuration["Static:Name"])
                ),
    RequestPath = configuration["Static:Path"]
});

var scope = app.Services.CreateAsyncScope();
StaticContext context = scope.ServiceProvider.GetRequiredService<StaticContext>();
await context.Database.MigrateAsync();

app.Run();
