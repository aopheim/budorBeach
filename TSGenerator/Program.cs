using System;
using System.IO;
using System.Threading.Tasks;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag;
using NSwag.CodeGeneration.TypeScript;

namespace TSGenerator
{
    internal class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("Generating TypeScript client and DTOs...");

            const string swaggerFileName = "swagger.json";
            var workingDir = Environment.CurrentDirectory;
            var projectDirectory = Directory.GetParent(workingDir)?.FullName ?? "";
            
            var swaggerJsonPath = $"{projectDirectory}\\budorWeb.Api\\{swaggerFileName}";
            var openApiDocument = await OpenApiDocument.FromFileAsync(swaggerJsonPath);
            var settings = new TypeScriptClientGeneratorSettings
            {
                TypeScriptGeneratorSettings =
                {
                    TypeStyle = TypeScriptTypeStyle.Class
                }
            };

            var generator = new TypeScriptClientGenerator(openApiDocument, settings);
            var typeScriptServicesPath = $"{projectDirectory}\\budorWeb.Api\\ClientApp\\src\\services.ts";
            var code = generator.GenerateFile();
            await File.WriteAllTextAsync(typeScriptServicesPath, code);

            Console.WriteLine("Generated TypeScript client at budorWeb.Api/ClientApp/src/services.ts");
        }
    }
}