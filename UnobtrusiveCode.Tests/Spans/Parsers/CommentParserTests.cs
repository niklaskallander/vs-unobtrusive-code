namespace UnobtrusiveCode.Tests.Spans.Parsers
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.Linq;

    using UnobtrusiveCode.Spans.Parsers;

    [TestClass]
    public class CommentParserTests
    {
        private readonly IParser _parser = new CommentParser();

        [TestMethod]
        public void CommentParser_DoesNotGroupMultipleNonAdjacentCommentsInOneSpan()
        {
            const string code = @"namespace TestNamespace
{
    public class TestClass
    {
        private readonly ILog _logger = new Log();

        public void TestMethod()
        {
            // banana

            var banana = ""yellow"";

            _logger
                .Info($""the banana is {banana}"");

            // apple

            var apple = ""green"";

            _logger
                .Info($""the apple is {apple}"");
        }
    }
}";
            var options = new ParserTestOptions();

            var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(code);

            var spans = _parser
                .Parse(syntaxTree, options);

            Assert.AreEqual(2, spans.Count());

            var span1 = spans.ElementAt(0);
            var span2 = spans.ElementAt(1);

            var expected1 = @"            // banana
";

            var expected2 = @"            // apple
";

            var actual1 = code.Substring(span1.Start, span1.Length);
            var actual2 = code.Substring(span2.Start, span2.Length);

            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(expected2, actual2);
        }

        [TestMethod]
        public void CommentParser_DoesNotParseWhenNotSupposedTo()
        {
            const string code = @"namespace TestNamespace
{
    public class TestClass
    {
        private readonly ILog _logger = new Log();

        public void TestMethod()
        {
            // apple
            // banana
        }
    }
}";
            var options = new ParserTestOptions(isCommentDimmingEnabled: false, isCommentOutliningEnabled: false);

            var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(code);

            var spans = _parser
                .Parse(syntaxTree, options);

            Assert.AreEqual(0, spans.Count());
        }

        [TestMethod]
        public void CommentParser_EatsUpLeadingAndTrailingWhiteSpaceExceptFirstLeadingNewLine_MultiLineComment()
        {
            const string code = @"namespace TestNamespace
{
    public class TestClass
    {
        private readonly ILog _logger = new Log();

        public void TestMethod()
        {

              



            /*
             * apple
             */

              
              


        }
    }
}";
            var options = new ParserTestOptions();

            var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(code);

            var spans = _parser
                .Parse(syntaxTree, options);

            Assert.AreEqual(1, spans.Count());

            var span = spans.Single();

            var expected = @"              



            /*
             * apple
             */

              
              

";

            var actual = code.Substring(span.Start, span.Length);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CommentParser_EatsUpLeadingAndTrailingWhiteSpaceExceptFirstLeadingNewLine_RegularComment()
        {
            const string code = @"namespace TestNamespace
{
    public class TestClass
    {
        private readonly ILog _logger = new Log();

        public void TestMethod()
        {

              



            // apple

              
              


        }
    }
}";
            var options = new ParserTestOptions();

            var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(code);

            var spans = _parser
                .Parse(syntaxTree, options);

            Assert.AreEqual(1, spans.Count());

            var span = spans.Single();

            var expected = @"              



            // apple

              
              

";

            var actual = code.Substring(span.Start, span.Length);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CommentParser_EatsUpLeadingAndTrailingWhiteSpaceExceptFirstLeadingNewLine_DocComment()
        {
            const string code = @"namespace TestNamespace
{
    public class TestClass
    {
        private readonly ILog _logger = new Log();

        /// <summary>
        ///     apple
        ///     banana
        /// </summary>
        public void TestMethod()
        {
        }
    }
}";
            var options = new ParserTestOptions();

            var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(code);

            var spans = _parser
                .Parse(syntaxTree, options);

            Assert.AreEqual(1, spans.Count());

            var span = spans.Single();

            var expected = @"        /// <summary>
        ///     apple
        ///     banana
        /// </summary>
";

            var actual = code.Substring(span.Start, span.Length);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CommentParser_GroupsMultipleAdjacentCommentsInOneSpan()
        {
            const string code = @"namespace TestNamespace
{
    public class TestClass
    {
        private readonly ILog _logger = new Log();

        public void TestMethod()
        {
            // apple
            // banana
        }
    }
}";
            var options = new ParserTestOptions(loggingIdentifiers: new[] { "logger." });

            var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(code);

            var spans = _parser
                .Parse(syntaxTree, options);

            Assert.AreEqual(1, spans.Count());

            var span = spans.Single();

            var expected = @"            // apple
            // banana";

            var actual = code.Substring(span.Start, span.Length);

            Assert.AreEqual(expected, actual);
        }
    }
}
