namespace UnobtrusiveCode.Tests.Spans.Normalizers
{
    using Microsoft.CodeAnalysis.CSharp;

    using System.Collections.Generic;
    using System.Linq;

    using UnobtrusiveCode.Options;
    using UnobtrusiveCode.Spans;
    using UnobtrusiveCode.Spans.Parsers;
    using UnobtrusiveCode.Spans.Parsers.Detectors;

    public class AggregateTestParser : IParser
    {
        private readonly IEnumerable<IParser> _parsers = new IParser[]
        {
            new LoggingParser(new LoggingStatementDetector()),
            new CommentParser()
        };

        public IEnumerable<ObtrusiveCodeSpan> Parse
            (
            CSharpSyntaxTree syntaxTree,
            IUnobtrusiveCodeOptions options
            )
            => _parsers
                .SelectMany(x => x.Parse(syntaxTree, options));
    }
}