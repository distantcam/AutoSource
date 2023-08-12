using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AutoSource;

[Generator(LanguageNames.CSharp)]
public class SourceToolsGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("SourceTools.g.cs", SourceText.From(@"
using global::Microsoft.CodeAnalysis;
using global::Microsoft.CodeAnalysis.CSharp.Syntax;
using global::System.Threading;

#nullable enable

namespace AutoSource
{
    internal static class SourceTools
    {
        public static bool IsCorrectAttribute(string attributeName, SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            if (syntaxNode is not AttributeSyntax attribute) return false;
            var name = attribute.Name switch
            {
                SimpleNameSyntax ins => ins.Identifier.Text,
                QualifiedNameSyntax qns => qns.Right.Identifier.Text,
                _ => null
            };
            return name == attributeName || name == attributeName + ""Attribute"";
        }

        public static IMethodSymbol? GetMethodFromAttribute(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            var attributeSyntax = (AttributeSyntax)context.Node;
            if (attributeSyntax.Parent?.Parent is not MethodDeclarationSyntax methodNode) return null;
            if (context.SemanticModel.GetDeclaredSymbol(methodNode) is not IMethodSymbol method) return null;
            return method;
        }

        public static ITypeSymbol? GetTypeFromAttribute(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            var attributeSyntax = (AttributeSyntax)context.Node;

            // ""attribute.Parent"" is ""AttributeListSyntax""
            // ""attribute.Parent.Parent"" is a C# fragment the attributes are applied to
            TypeDeclarationSyntax? typeNode = attributeSyntax.Parent?.Parent switch
            {
                ClassDeclarationSyntax classDeclarationSyntax => classDeclarationSyntax,
                RecordDeclarationSyntax recordDeclarationSyntax => recordDeclarationSyntax,
                StructDeclarationSyntax structDeclarationSyntax => structDeclarationSyntax,
                _ => null
            };

            if (typeNode == null) return null;
            if (context.SemanticModel.GetDeclaredSymbol(typeNode) is not ITypeSymbol type) return null;
            return type;
        }
    }
}
", Encoding.UTF8));
    }
}
