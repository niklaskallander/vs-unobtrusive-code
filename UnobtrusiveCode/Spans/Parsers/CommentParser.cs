namespace UnobtrusiveCode.Spans.Parsers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    using System.Collections.Generic;
    using System.Linq;

    using UnobtrusiveCode.Options;

    public class CommentParser : IParser
    {
        public IEnumerable<ObtrusiveCodeSpan> Parse
            (
            CSharpSyntaxTree syntaxTree,
            IUnobtrusiveCodeOptions options
            )
        {
            if (!options.IsCommentEnabled() ||
                syntaxTree == null ||
                !syntaxTree.TryGetRoot(out var root))
            {
                yield break;
            }

            foreach (var token in root.DescendantTokens())
            {
                var leading = token.LeadingTrivia;

                if (leading.Any(x => x.IsComment()))
                {
                    if (leading.All(x => x.IsCommentOrWhiteSpace()))
                    {
                        var end = leading.Last(x => x.IsComment());

                        var endIndex = leading.IndexOf(end);

                        var endPosition = end.Span.End;

                        endPosition = leading
                            .Skip(endIndex + 1)
                            .GetEndPositionOfNextToLastLine()
                                ?? endPosition;

                        var start = leading.First();

                        if (start.Kind() == SyntaxKind.EndOfLineTrivia && leading.Count() > 1)
                        {
                            start = leading.ElementAt(1);
                        }

                        yield return new ObtrusiveCodeSpan(start.Span.Start, endPosition, UnobtrusiveCodePackage.CommentFeatures);
                    }
                }

                var trailing = token.TrailingTrivia;

                if (trailing.Any(x => x.IsComment()))
                {
                    if (trailing.All(x => x.IsCommentOrWhiteSpace()))
                    {
                        yield return new ObtrusiveCodeSpan(trailing.First(x => x.IsComment()).Span.Start, trailing.Last().Span.End, UnobtrusiveCodePackage.CommentFeatures);
                    }
                }
            }
        }
    }
}
