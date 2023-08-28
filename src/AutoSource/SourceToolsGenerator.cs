using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AutoSource;

[Generator]
public class SourceToolsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SourceTools.cs");
            using var reader = new StreamReader(stream);
            var source = reader.ReadToEnd();
            ctx.AddSource("SourceTools.g.cs", SourceText.From(source, Encoding.UTF8));
        });
    }
}
