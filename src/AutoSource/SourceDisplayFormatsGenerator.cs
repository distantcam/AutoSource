using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AutoSource;

[Generator(LanguageNames.CSharp)]
public class SourceDisplayFormatsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SourceDisplayFormats.cs");
            using var reader = new StreamReader(stream);
            var source = reader.ReadToEnd();
            ctx.AddSource("SourceDisplayFormats.g.cs", SourceText.From(source, Encoding.UTF8));
        });
    }
}
