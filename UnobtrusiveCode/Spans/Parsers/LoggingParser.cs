namespace UnobtrusiveCode.Spans.Parsers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using System.Collections.Generic;
    using System.Linq;

    using UnobtrusiveCode.Options;
    using UnobtrusiveCode.Spans.Parsers.Detectors;

    public class LoggingParser : IParser
    {
        private readonly ILoggingStatementDetector _loggingStatementDetector;

        public LoggingParser(ILoggingStatementDetector loggingStatementDetector)
            => _loggingStatementDetector = loggingStatementDetector;

        public IEnumerable<ObtrusiveCodeSpan> Parse
            (
            CSharpSyntaxTree syntaxTree,
            IUnobtrusiveCodeOptions options
            )
        {
            if (!options.IsLoggingEnabled() ||
                syntaxTree == null ||
                !syntaxTree.TryGetRoot(out var root))
            {
                yield break;
            }

            foreach (var statementGroup in root.DescendantNodes().GroupBy(x => x.Parent))
            {
                var statements = statementGroup.ToList();

                for (int i = 0; i < statements.Count; i++)
                {
                    var statement = statements[i];

                    if (!IsLoggingStatement(statement, options))
                    {
                        continue;
                    }

                    var start = statement;
                    var end = statement;

                    int j = i;

                    while (++j < statements.Count && IsLoggingStatement(statements[j], options) && !statements[j].HasLeadingNonWhiteSpaceTrivia() && !end.HasTrailingNonWhiteSpaceTrivia())
                    {
                        i++;
                        end = statements[j];
                    }

                    int endPosition = end.GetTagEnd();

                    if (j < statements.Count)
                    {
                        if (statements[j].OnlyHasLeadingWhiteSpaceTrivia())
                        {
                            endPosition = statements[j]
                                .GetLeadingTrivia()
                                .GetEndPositionOfNextToLastLine()
                                    ?? endPosition;
                        }
                    }
                    else if (ReferenceEquals(statement, statements.Last()))
                    {
                        if (statement.Parent is BlockSyntax block)
                        {
                            endPosition = block.CloseBraceToken.LeadingTrivia
                                .GetEndPositionOfNextToLastLine()
                                    ?? endPosition;
                        }
                    }

                    yield return new ObtrusiveCodeSpan(start.GetTagStart(), endPosition, UnobtrusiveCodePackage.LoggingFeatures);
                }
            }
        }

        private bool IsLoggingStatement
            (
            SyntaxNode node,
            IUnobtrusiveCodeOptions options
            )
        {
            if (!(node is ExpressionStatementSyntax statement))
            {
                return false;
            }

            var text = statement
                .GetText()
                .GetSubText(statement.Expression.SpanStart - statement.FullSpan.Start)
                .ToString();

            return _loggingStatementDetector
                .IsLoggingStatement(text, options.LoggingIdentifiers);
        }
    }
}
