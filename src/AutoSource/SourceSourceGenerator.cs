using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AutoSource;

[Generator(LanguageNames.CSharp)]
public class SourceSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
        {
            var assembly = Assembly.GetAssembly(typeof(SourceSourceGenerator));
            var filenames = assembly.GetManifestResourceNames();
            foreach (var filename in filenames)
            {
                using var stream = assembly.GetManifestResourceStream(filename);
                using var reader = new StreamReader(stream);
                context.AddSource(filename, SourceText.From(reader.ReadToEnd(), Encoding.UTF8));
            }
        });
    }
}
