using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AutoSource;

[Generator(LanguageNames.CSharp)]
public class CodeBuilderGenerator : IIncrementalGenerator
{
    private static string GetCode()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CodeBuilder.cs");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(
            context.AnalyzerConfigOptionsProvider,
            static (ctx, opts) =>
            {
                opts.GlobalOptions
                    .TryGetValue("build_property.PackageProjectUrl", out var packageProjectUrl);
                opts.GlobalOptions
                    .TryGetValue("build_property.AssemblyName", out var assemblyName);
                opts.GlobalOptions
                    .TryGetValue("build_property.Version", out var version);
                opts.GlobalOptions
                    .TryGetValue("build_property.GitSha", out var gitSha);

                var source = GetCode()
                    .Replace("[[PACKAGEPROJECTURL]]", packageProjectUrl)
                    .Replace("[[ASSEMBLYNAME]]", assemblyName)
                    .Replace("[[VERSION]]", version)
                    .Replace("[[GITSHA]]", gitSha);

                ctx.AddSource("CodeBuilder.g.cs", SourceText.From(source, Encoding.UTF8));
            });
    }
}
