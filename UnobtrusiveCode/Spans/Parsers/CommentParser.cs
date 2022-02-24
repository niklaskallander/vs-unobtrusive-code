namespace UnobtrusiveCode.Spans.Parsers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                var allTriv = token.GetAllTrivia();

                var leading = token.LeadingTrivia;

                if (leading.Any(x => x.IsComment()))
                {
                    if (leading.All(x => x.IsCommentOrWhiteSpace()))
                    {
                        var start = leading.First();

                        if (start.IsKind(SyntaxKind.EndOfLineTrivia) && leading.Count() > 1)
                        {
                            start = leading.ElementAt(1);
                        }

                        int startPosition = start.Span.Start;
                        int endPosition;

                        var end = leading.Last(x => x.IsComment());

                        if (end.HasStructure &&
                            end.GetStructure() is DocumentationCommentTriviaSyntax structure)
                        {
                            // fix opening '/// ' in doc comment not being part of the Span-prop
                            startPosition = start.FullSpan.Start;
                            endPosition = structure.Span.End;

                            if (structure.Content.LastOrDefault() is XmlTextSyntax text)
                            {
                                foreach (var textToken in text.TextTokens.Reverse())
                                {
                                    if (!textToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                                    {
                                        endPosition = textToken.Span.End;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var endIndex = leading.IndexOf(end);

                            endPosition = end.Span.End;

                            endPosition = leading
                                .Skip(endIndex + 1)
                                .GetEndPositionOfNextToLastLine()
                                    ?? endPosition;
                        }

                        yield return new ObtrusiveCodeSpan(startPosition, endPosition, UnobtrusiveCodePackage.CommentFeatures);
                    }
                }

                var trailing = token.TrailingTrivia;

                if (trailing.Any(x => x.IsComment()))
                {
                    if (trailing.All(x => x.IsCommentOrWhiteSpace()))
                    {
                        var start = trailing.First(x => x.IsComment());
                        var end = trailing.Last(x => !x.IsKind(SyntaxKind.EndOfLineTrivia));

                        yield return new ObtrusiveCodeSpan(start.Span.Start, end.Span.End, UnobtrusiveCodePackage.CommentFeatures);
                    }
                }
            }
        }
    }
}
