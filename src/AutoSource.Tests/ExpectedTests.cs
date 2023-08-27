using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoSource.Tests;

[UsesVerify]
public class ExpectedTests
{
    [Theory]
    [InlineData(typeof(CodeBuilderGenerator))]
    [InlineData(typeof(SourceDisplayFormatsGenerator))]
    [InlineData(typeof(SourceToolsGenerator))]
    public Task Verify_CodeBuilderGenerator(Type generatorType)
    {
        var compilation = Compile();

        var options = new CompilerAnalyzerConfigOptionsProvider(ImmutableDictionary<object, AnalyzerConfigOptions>.Empty, new DictionaryAnalyzerConfigOptions(ImmutableDictionary<string, string>.Empty
            .Add("build_property.PackageProjectUrl", "PackageProjectUrl")
            .Add("build_property.AssemblyName", "AssemblyName")
            .Add("build_property.Version", "Version")
            .Add("build_property.GitSha", "GitSha")
        ));

        var generator = (IIncrementalGenerator)Activator.CreateInstance(generatorType)!;
        var driver = CSharpGeneratorDriver.Create(new[] { GeneratorExtensions.AsSourceGenerator(generator) }, optionsProvider: options).RunGenerators(compilation);

        return Verify(driver)
            .UseDirectory("Verified")
            .UseParameters(generatorType.Name);
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

    // https://github.com/dotnet/roslyn/blob/main/src/Compilers/Core/Portable/DiagnosticAnalyzer/DictionaryAnalyzerConfigOptions.cs
    internal sealed class DictionaryAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        internal static readonly ImmutableDictionary<string, string> EmptyDictionary = ImmutableDictionary.Create<string, string>(KeyComparer);

        public static DictionaryAnalyzerConfigOptions Empty { get; } = new DictionaryAnalyzerConfigOptions(EmptyDictionary);

        // Note: Do not rename. Older versions of analyzers access this field via reflection.
        // https://github.com/dotnet/roslyn/blob/8e3d62a30b833631baaa4e84c5892298f16a8c9e/src/Workspaces/SharedUtilitiesAndExtensions/Compiler/Core/Options/EditorConfig/EditorConfigStorageLocationExtensions.cs#L21
        internal readonly ImmutableDictionary<string, string> Options;

        public DictionaryAnalyzerConfigOptions(ImmutableDictionary<string, string> options)
            => Options = options;

        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
            => Options.TryGetValue(key, out value);

        public override IEnumerable<string> Keys
            => Options.Keys;
    }

    // https://github.com/dotnet/roslyn/blob/main/src/Compilers/Core/Portable/DiagnosticAnalyzer/CompilerAnalyzerConfigOptionsProvider.cs
    internal sealed class CompilerAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly ImmutableDictionary<object, AnalyzerConfigOptions> _treeDict;

        public static CompilerAnalyzerConfigOptionsProvider Empty { get; }
            = new CompilerAnalyzerConfigOptionsProvider(
                ImmutableDictionary<object, AnalyzerConfigOptions>.Empty,
                DictionaryAnalyzerConfigOptions.Empty);

        internal CompilerAnalyzerConfigOptionsProvider(
            ImmutableDictionary<object, AnalyzerConfigOptions> treeDict,
            AnalyzerConfigOptions globalOptions)
        {
            _treeDict = treeDict;
            GlobalOptions = globalOptions;
        }

        public override AnalyzerConfigOptions GlobalOptions { get; }

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
            => _treeDict.TryGetValue(tree, out var options) ? options : DictionaryAnalyzerConfigOptions.Empty;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
            => _treeDict.TryGetValue(textFile, out var options) ? options : DictionaryAnalyzerConfigOptions.Empty;

        internal CompilerAnalyzerConfigOptionsProvider WithAdditionalTreeOptions(ImmutableDictionary<object, AnalyzerConfigOptions> treeDict)
            => new CompilerAnalyzerConfigOptionsProvider(_treeDict.AddRange(treeDict), GlobalOptions);

        internal CompilerAnalyzerConfigOptionsProvider WithGlobalOptions(AnalyzerConfigOptions globalOptions)
            => new CompilerAnalyzerConfigOptionsProvider(_treeDict, globalOptions);
    }
}
