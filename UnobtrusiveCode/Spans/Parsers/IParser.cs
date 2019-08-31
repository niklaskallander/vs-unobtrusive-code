namespace UnobtrusiveCode.Spans.Parsers
{
    using Microsoft.CodeAnalysis.CSharp;

    using System.Collections.Generic;

    using UnobtrusiveCode.Options;

    public interface IParser
    {
        IEnumerable<ObtrusiveCodeSpan> Parse
            (
            CSharpSyntaxTree syntaxTree,
            IUnobtrusiveCodeOptions options
            );
    }
}
