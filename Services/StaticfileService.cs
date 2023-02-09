// using static_sv.DTOs;
// using static_sv.Interfaces;

// namespace static_sv.Services
// {
//     public class StaticfileService : IStaticfile
//     {
//         private readonly IConfiguration _configuration;
//         private readonly HttpContextAccessor _contextAccessor;
//         private readonly IRequestValidator _requestValidator;

//         public StaticfileService(IConfiguration configuration, HttpContextAccessor contextAccessor, IRequestValidator requestValidator)
//         {
//             _configuration = configuration;
//             _contextAccessor = contextAccessor;
//             _requestValidator = requestValidator;
//         }
//         public void DeleteImage(string url)
//         {
//             string xStaticSig = _contextAccessor.HttpContext!
//                 .Request.Headers[_configuration["Static:Header"]].ToString();
            

//             var isValidate = _requestValidator.Validate(model, xStaticSig);
//             if (!isValidate.Item1)
//                 return new StaticResModel
//                 {
//                     ErrorCode = "ERROR",
//                 };
//             // Decode the Base64 encoded image data
//             var imageBytes = Convert.FromBase64String(model.Base64EncodedFile!);

//             // Create a MemoryStream from the image bytes
//             string[] allTypes = model.Type!.Split('/');
//             string staticType = allTypes.ElementAt(0);


//             string imgType = allTypes.ElementAt(1);

//             if (staticType == "image")
//             {
//                 using (var memoryStream = new MemoryStream(imageBytes))
//                 {
//                     // Create a new unique file name

//                     DateTime now = DateTime.Now;
//                     string outputDate = now.ToString("yyyy-MM-dd_HH-mm-ss");

//                     string fileName = model.Name!.Replace(" ", "-")
//                                     .Replace("--", "-");

//                     var fullName = $"{fileName}_{outputDate}.{imgType}";

//                     // Save the image to the server's file system
//                     var imagePath = Path.Combine(_configuration["Static:Name"], _configuration["Static:Types:Image"], fullName);
//                     System.IO.File.WriteAllBytes(imagePath, memoryStream.ToArray());

//                     string url = _configuration["ASPNETCORE_DOMAIN_URL"];
//                     string imageUrl = Path.Combine(url, imagePath);

//                     // return Ok(new { imagePath = $"images/{fileName}" });
//                     StaticResModel resModel = new StaticResModel
//                     {
//                         ImageUrl = imageUrl,
//                         Signature = isValidate.Item2,
//                         ErrorCode = "SUCCESS"
//                     };
//                     return resModel;
//                 }
//             }

//             throw new Exception("Only support type=image at the moment");
//         }
//     }
// }