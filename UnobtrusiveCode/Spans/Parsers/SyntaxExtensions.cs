namespace UnobtrusiveCode.Spans.Parsers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Text;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class SyntaxExtensions
    {
        public static bool OnlyHasWhiteSpaceTrivia(this SyntaxNode node)
            => node.OnlyHasLeadingWhiteSpaceTrivia()
            && node.OnlyHasTrailingWhiteSpaceTrivia();

        public static bool OnlyHasLeadingWhiteSpaceTrivia(this SyntaxNode node)
            => node
                .GetLeadingTrivia()
                .OnlyHasWhiteSpace();

        public static bool OnlyHasTrailingWhiteSpaceTrivia(this SyntaxNode node)
            => node
                .GetTrailingTrivia()
                .OnlyHasWhiteSpace();

        public static bool HasLeadingNonWhiteSpaceTrivia(this SyntaxNode node)
            => !node.OnlyHasLeadingWhiteSpaceTrivia();

        public static bool HasTrailingNonWhiteSpaceTrivia(this SyntaxNode node)
            => !node.OnlyHasTrailingWhiteSpaceTrivia();

        public static bool OnlyHasWhiteSpace(this SyntaxTriviaList trivia)
            => trivia
                .All(IsWhiteSpace);

        private static TextSpan? GetSpanWhiteSpaceBoundary
            (
            this SyntaxNode node,
            IEnumerable<SyntaxTrivia> triviaList
            )
        {
            SyntaxTrivia? last = null;

            foreach (var trivia in triviaList)
            {
                if (!trivia.IsWhiteSpace())
                {
                    break;
                }

                last = trivia;
            }

            return last.HasValue
                ? last.Value.Span
                : node.DescendantNodesAndTokensAndSelf().First().Span;
        }

        public static int? GetEndPositionOfNextToLastLine(this IEnumerable<SyntaxTrivia> triviaList)
        {
            var newLines = triviaList
                .Where(x => x.IsKind(SyntaxKind.EndOfLineTrivia))
                .Reverse()
                .ToList();

            return newLines.Count > 1
                ? newLines[1].Span.End
                : (int?)null;
        }

        public static int GetTagStart(this SyntaxNode node)
            => node.GetSpanWhiteSpaceBoundary(node.GetLeadingTrivia().Reverse())?.Start
            ?? node.DescendantNodesAndTokensAndSelf().First().Span.Start;

        public static int GetTagEnd(this SyntaxNode node)
            => node.GetSpanWhiteSpaceBoundary(node.GetTrailingTrivia())?.End
            ?? node.DescendantNodesAndTokensAndSelf().Last().Span.End;

        public static bool IsCommentOrWhiteSpace(this SyntaxTrivia trivia)
            => trivia.IsComment()
            || trivia.IsWhiteSpace();

        public static bool IsComment(this SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
                case SyntaxKind.DocumentationCommentExteriorTrivia:
                case SyntaxKind.EndOfDocumentationCommentToken:
                case SyntaxKind.MultiLineCommentTrivia:
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.XmlComment:
                case SyntaxKind.XmlCommentEndToken:
                case SyntaxKind.XmlCommentStartToken:
                    return true;
            }

            return false;
        }

        public static bool IsWhiteSpace(this SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
                case SyntaxKind.WhitespaceTrivia:
                case SyntaxKind.EndOfLineTrivia:
                    return true;
            }

            return false;
        }
    }
}
