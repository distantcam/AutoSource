using global::Microsoft.CodeAnalysis;
using global::Microsoft.CodeAnalysis.Text;
using global::System.Text;

#nullable enable

namespace AutoSource
{
    internal class CodeBuilder
    {
        private readonly StringBuilder _stringBuilder = new();
        private int _indent = 0;

        public CodeBuilder AppendLine()
        {
            _stringBuilder.AppendLine();
            return this;
        }
        public CodeBuilder AppendLine(string line)
        {
            _stringBuilder.AppendLine(Indent + line);
            return this;
        }

        public void IncreaseIndent() { _indent++; }
        public void DecreaseIndent()
        {
            if (_indent > 0)
            {
                _indent--;
            }
        }

        public CodeBuilder StartBlock()
        {
            AppendLine("{");
            IncreaseIndent();
            return this;
        }
        public CodeBuilder EndBlock()
        {
            DecreaseIndent();
            AppendLine("}");
            return this;
        }

        public char IndentChar { get; set; } = '\t';
        public string Indent => new string(IndentChar, _indent);

        public IDisposable StartPartialType(ITypeSymbol type)
        {
            var ns = type.ContainingNamespace.IsGlobalNamespace
                    ? null
                    : type.ContainingNamespace.ToString();
            var typeKeyword = type.IsRecord
                ? "record"
                : type.IsValueType
                    ? "struct"
                    : "class";

            if (!string.IsNullOrEmpty(ns))
            {
                AppendLine($"namespace {ns}");
                StartBlock();
            }

            var typeStack = new Stack<string>();
            var containingType = type.ContainingType;
            while (containingType is not null)
            {
                var contTypeKeyword = containingType.IsRecord
                    ? "record"
                    : containingType.IsValueType
                        ? "struct"
                        : "class";
                var typeName = containingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                typeStack.Push(contTypeKeyword + " " + typeName);
                containingType = containingType.ContainingType;
            }

            var nestedCount = typeStack.Count;
            while (typeStack.Count > 0)
            {
                AppendLine($"partial {typeStack.Pop()}");
                StartBlock();
            }

            AppendLine($"partial {typeKeyword} {type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}");
            StartBlock();

            return new CloseBlock(this, 1 + nestedCount + (ns != null ? 1 : 0));
        }

        public static implicit operator SourceText(CodeBuilder codeBuilder)
            => SourceText.From(codeBuilder._stringBuilder.ToString(), Encoding.UTF8);

        private readonly struct CloseBlock : IDisposable
        {
            private readonly CodeBuilder _codeBuilder;
            private readonly int _count;
            public CloseBlock(CodeBuilder codeBuilder, int count) { _codeBuilder = codeBuilder; _count = count; }
            public void Dispose() { for (var i = 0; i < _count; i++) _codeBuilder.EndBlock(); }
        }
    }
}
