using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AutoSource;

[Generator(LanguageNames.CSharp)]
public class SourceSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var assembly = Assembly.GetAssembly(typeof(SourceSourceGenerator));
        var filenames = assembly.GetManifestResourceNames();
        foreach (var filename in filenames)
        {
            context.AddSource(filename,
                SourceText.From(assembly.GetManifestResourceStream(filename), Encoding.UTF8, canBeEmbedded: true));
        }
    }
}
