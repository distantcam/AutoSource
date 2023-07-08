using global::Microsoft.CodeAnalysis;

internal static class SourceDisplayFormats
{
    public static readonly SymbolDisplayFormat FullyQualifiedParameterFormat = SymbolDisplayFormat.FullyQualifiedFormat
        .WithParameterOptions(
            SymbolDisplayParameterOptions.IncludeName |
            SymbolDisplayParameterOptions.IncludeType |
            SymbolDisplayParameterOptions.IncludeParamsRefOut
        );
}
