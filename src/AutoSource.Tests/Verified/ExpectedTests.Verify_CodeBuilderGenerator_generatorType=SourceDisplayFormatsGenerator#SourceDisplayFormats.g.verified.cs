//HintName: SourceDisplayFormats.g.cs
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
