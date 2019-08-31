namespace UnobtrusiveCode.Tests.Spans.Normalizers
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.Linq;

    using UnobtrusiveCode.Spans.Normalizers;
    using UnobtrusiveCode.Spans.Parsers;

    [TestClass]
    public class ObtrusiveCodeSpanNormalizerTests
    {
        private readonly IParser _parser = new AggregateTestParser();
        private readonly IObtrusiveCodeSpanNormalizer _normalizer = new ObtrusiveCodeSpanNormalizer();

        [TestMethod]
        public void ObtrusiveCodeSpanNormalizer_ConcatenatesAdjacentOutliningSpans()
        {
            const string code = @"namespace TestNamespace
{
    public class TestClass
    {
        private readonly ILog _logger = new Log();

        public void TestMethod()
        {
            var apple = ""green"";
            var banana = ""yellow"";

            // banana

            _logger
                .Info($""the banana is {banana}"");

            // apple

            _logger
                .Info($""the apple is {apple}"");
        }
    }
}";
            var options = new ParserTestOptions(loggingIdentifiers: new[] { "logger." });

            var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(code);

            var spans = _parser
                .Parse(syntaxTree, options)
                .ToList();

            Assert.AreEqual(4, spans.Count);

            var normalizedOutliningSpans = _normalizer
                .Normalize(spans, options, code)
                .Where(x => x.AllowsOutlining)
                .ToList();

            Assert.AreEqual(1, normalizedOutliningSpans.Count);

            var firstSpan = spans
                .OrderBy(x => x.Start)
                .First();

            var lastSpan = spans
                .OrderByDescending(x => x.End)
                .First();

            var normalizedSpan = normalizedOutliningSpans.Single();

            Assert.AreEqual(firstSpan.Start, normalizedSpan.Start);
            Assert.AreEqual(lastSpan.End, normalizedSpan.End);
        }

        [TestMethod]
        public void ObtrusiveCodeSpanNormalizer_DoesNotConcatenateNonAdjacentOutliningSpans()
        {
            const string code = @"namespace TestNamespace
{
    public class TestClass
    {
        private readonly ILog _logger = new Log();

        public void TestMethod()
        {
            {
                // banana

                var banana = ""yellow"";

                _logger
                    .Info($""the banana is {banana}"");
            }

            {
                // apple

                var apple = ""green"";

                _logger
                    .Info($""the apple is {apple}"");
            }
        }
    }
}";
            var options = new ParserTestOptions(loggingIdentifiers: new[] { "logger." });

            var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(code);

            var spans = _parser
                .Parse(syntaxTree, options)
                .ToList();

            Assert.AreEqual(4, spans.Count);

            var normalizedOutliningSpans = _normalizer
                .Normalize(spans, options, code)
                .Where(x => x.AllowsOutlining)
                .ToList();

            Assert.AreEqual(4, normalizedOutliningSpans.Count);
        }
    }
}
