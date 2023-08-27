using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoSource.Tests;

public class CompilationTests
{
    [Fact]
    public void CodeBuilderGeneratorCompileTest()
    {
        var ignoredWarnings = new string[] {
        };

        var compilation = Compile();

        var generator = new CodeBuilderGenerator();
        CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        diagnostics.Should().BeEmpty();
        outputCompilation.GetDiagnostics()
            .Where(d => !ignoredWarnings.Contains(d.Id))
            .Should().BeEmpty();
    }

    [Fact]
    public void SourceDisplayFormatsGeneratorCompileTest()
    {
        var ignoredWarnings = new string[] {
        };

        var compilation = Compile();

        var generator = new SourceDisplayFormatsGenerator();
        CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        diagnostics.Should().BeEmpty();
        outputCompilation.GetDiagnostics()
            .Where(d => !ignoredWarnings.Contains(d.Id))
            .Should().BeEmpty();
    }

    [Fact]
    public void SourceToolsGeneratorCompileTest()
    {
        var ignoredWarnings = new string[] {
        };

        var compilation = Compile();

        var generator = new SourceToolsGenerator();
        CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        diagnostics.Should().BeEmpty();
        outputCompilation.GetDiagnostics()
            .Where(d => !ignoredWarnings.Contains(d.Id))
            .Should().BeEmpty();
    }

    private CSharpCompilation Compile()
    {
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .Cast<MetadataReference>();
        return CSharpCompilation.Create(
            "AutoSourceTest",
            references: references,
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary
            ));
    }
}
