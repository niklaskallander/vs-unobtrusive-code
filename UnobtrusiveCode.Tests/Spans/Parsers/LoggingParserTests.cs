namespace UnobtrusiveCode.Tests.Spans.Parsers
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.Linq;

    using UnobtrusiveCode.Spans.Parsers;
    using UnobtrusiveCode.Spans.Parsers.Detectors;

    [TestClass]
    public class LoggingParserTests
    {
        private readonly IParser _parser = new LoggingParser(new LoggingStatementDetector());

        [TestMethod]
        public void LoggingParser_DoesNotGroupMultipleNonAdjacentLoggingStatementsInOneSpan()
        {
            const string code = @"namespace TestNamespace
{
    public class TestClass
    {
        private readonly ILog _logger = new Log();

        public void TestMethod()
        {
            var banana = ""yellow"";

            _logger
                .Info($""the banana is {banana}"");

            var apple = ""green"";

            _logger
                .Info($""the apple is {apple}"");
        }
    }
}";
            var options = new ParserTestOptions(loggingIdentifiers: new[] { "logger." });

            var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(code);

            var spans = _parser
                .Parse(syntaxTree, options);

            Assert.AreEqual(2, spans.Count());

            var span1 = spans.ElementAt(0);
            var span2 = spans.ElementAt(1);

            var expected1 = @"
            _logger
                .Info($""the banana is {banana}"");
";

            var expected2 = @"
            _logger
                .Info($""the apple is {apple}"");
";

            var actual1 = code.Substring(span1.Start, span1.Length);
            var actual2 = code.Substring(span2.Start, span2.Length);

            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(expected2, actual2);
        }

        [TestMethod]
        public void LoggingParser_DoesNotParseWhenNotSupposedTo()
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

            _logger
                .Info($""the banana is {banana}"");

            _logger
                .Info($""the apple is {apple}"");
        }
    }
}";
            var options = new ParserTestOptions(isLoggingDimmingEnabled: false, isLoggingOutliningEnabled: false, loggingIdentifiers: new[] { "logger." });

            var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(code);

            var spans = _parser
                .Parse(syntaxTree, options);

            Assert.AreEqual(0, spans.Count());
        }

        [TestMethod]
        public void LoggingParser_EatsUpLeadingAndTrailingWhiteSpaceExceptFirstLeadingNewLine()
        {
            const string code = @"namespace TestNamespace
{
    public class TestClass
    {
        private readonly ILog _logger = new Log();

        public void TestMethod()
        {
            var banana = ""yellow"";

              



            _logger
                .Info($""the banana is {banana}"");

              
              


        }
    }
}";
            var options = new ParserTestOptions(loggingIdentifiers: new[] { "logger." });

            var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(code);

            var spans = _parser
                .Parse(syntaxTree, options);

            Assert.AreEqual(1, spans.Count());

            var span = spans.Single();

            var expected = @"
              



            _logger
                .Info($""the banana is {banana}"");

              
              

";

            var actual = code.Substring(span.Start, span.Length);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LoggingParser_GroupsMultipleAdjacentLoggingStatementsInOneSpan()
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

            _logger
                .Info($""the banana is {banana}"");

            _logger
                .Info($""the apple is {apple}"");
        }
    }
}";
            var options = new ParserTestOptions(loggingIdentifiers: new[] { "logger." });

            var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(code);

            var spans = _parser
                .Parse(syntaxTree, options);

            Assert.AreEqual(1, spans.Count());

            var span = spans.Single();

            var expected = @"
            _logger
                .Info($""the banana is {banana}"");

            _logger
                .Info($""the apple is {apple}"");
";

            var actual = code.Substring(span.Start, span.Length);

            Assert.AreEqual(expected, actual);
        }
    }
}
