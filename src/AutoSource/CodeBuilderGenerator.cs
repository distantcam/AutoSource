using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AutoSource;

[Generator]
public class CodeBuilderGenerator : IIncrementalGenerator
{
    private record SourceProperty(string? PackageProjectUrl, string? AssemblyName, string? Version, string? GitSha);

    private static string GetCode()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CodeBuilder.cs");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var properties = context.AnalyzerConfigOptionsProvider
            .Select(static (o, _) => new SourceProperty(
                o.GlobalOptions.TryGetValue("build_property.PackageProjectUrl", out var packageProjectUrl) ? packageProjectUrl : null,
                o.GlobalOptions.TryGetValue("build_property.AssemblyName", out var assemblyName) ? assemblyName : null,
                o.GlobalOptions.TryGetValue("build_property.Version", out var version) ? version : null,
                o.GlobalOptions.TryGetValue("build_property.GitSha", out var gitSha) ? gitSha : null
            ));

        context.RegisterSourceOutput(
            properties,
            static (ctx, props) =>
            {
                var source = GetCode()
                    .Replace("[[PACKAGEPROJECTURL]]", props.PackageProjectUrl)
                    .Replace("[[ASSEMBLYNAME]]", props.AssemblyName)
                    .Replace("[[VERSION]]", props.Version)
                    .Replace("[[GITSHA]]", props.GitSha);

                ctx.AddSource("CodeBuilder.g.cs", SourceText.From(source, Encoding.UTF8));
            });
    }
}
