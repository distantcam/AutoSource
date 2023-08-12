using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AutoSource;

[Generator(LanguageNames.CSharp)]
public class SourceDisplayFormatsGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("SourceDisplayFormats.g.cs", SourceText.From(@"
using global::Microsoft.CodeAnalysis;

#nullable enable

namespace AutoSource
{
    internal static class SourceDisplayFormats
    {
        public static readonly SymbolDisplayFormat FullyQualifiedParameterFormat = SymbolDisplayFormat.FullyQualifiedFormat
            .WithParameterOptions(
                SymbolDisplayParameterOptions.IncludeName |
                SymbolDisplayParameterOptions.IncludeType |
                SymbolDisplayParameterOptions.IncludeParamsRefOut
            );
    }
}
", Encoding.UTF8));
    }
}
