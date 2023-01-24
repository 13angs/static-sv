using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using static_sv.DTOs;

var builder = WebApplication.CreateBuilder(args);
IConfiguration Configuration = builder.Configuration;
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/api/v1/statics", ([FromBody] StaticModel model) =>
{
    // Decode the Base64 encoded image data
    var imageBytes = Convert.FromBase64String(model.Base64EncodedFile!);

    // Create a MemoryStream from the image bytes
    string[] allTypes = model.Type!.Split('/');
    string staticType = allTypes.ElementAt(0);


    string imgType = allTypes.ElementAt(1);

    if(staticType == "image")
    {
        using (var memoryStream = new MemoryStream(imageBytes))
        {
            // Create a new unique file name
            var fileName = $"{Guid.NewGuid().ToString("N")}.{imgType}";

            // Save the image to the server's file system
            var imagePath = Path.Combine(Configuration["Static:Name"], Configuration["Static:Types:Image"], fileName);
            System.IO.File.WriteAllBytes(imagePath, memoryStream.ToArray());

            string url = Configuration["ASPNETCORE_DOMAIN_URL"];
            string imageUrl = Path.Combine(url, imagePath);

            // return Ok(new { imagePath = $"images/{fileName}" });
            StaticResModel resModel = new StaticResModel{
                ImageUrl=imageUrl
            };
            return resModel;
        }
    }

    throw new Exception("Only support type=image at the moment");
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
                    Path.Combine(app.Environment.ContentRootPath, Configuration["Static:Name"])
                ),
    RequestPath = Configuration["Static:Path"]
});

app.Run();
