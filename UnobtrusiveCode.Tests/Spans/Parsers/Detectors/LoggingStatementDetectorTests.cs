namespace UnobtrusiveCode.Tests.Spans.Parsers.Detectors
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using UnobtrusiveCode.Spans.Parsers.Detectors;

    [TestClass]
    public class LoggingStatementDetectorTests
    {
        private readonly ILoggingStatementDetector _detector = new LoggingStatementDetector();

        [TestMethod]
        public void LoggingStatementDetector_EatsUpAnyAllAllWhiteSpace()
        {
            const string statement = " \t\n\r log \t\n\r . \t\n\r info \t\n\r (\"hello world\");";

            bool isLoggingCode = _detector
                .IsLoggingStatement(statement, new[] { "log." });

            Assert.IsTrue(isLoggingCode);
        }

        [TestMethod]
        public void LoggingStatementDetector_EatsUpBaseKeyword()
        {
            const string statement = "base.log.info(\"hello world\");";

            bool isLoggingCode = _detector
                .IsLoggingStatement(statement, new[] { "log." });

            Assert.IsTrue(isLoggingCode);
        }

        [TestMethod]
        public void LoggingStatementDetector_EatsUpThisKeyword()
        {
            const string statement = "this.log.info(\"hello world\");";

            bool isLoggingCode = _detector
                .IsLoggingStatement(statement, new[] { "log." });

            Assert.IsTrue(isLoggingCode);
        }

        [TestMethod]
        public void LoggingStatementDetector_EatsUpUnderscoreNamingPrefix()
        {
            const string statement = "_log.info(\"hello world\");";

            bool isLoggingCode = _detector
                .IsLoggingStatement(statement, new[] { "log." });

            Assert.IsTrue(isLoggingCode);
        }

        [TestMethod]
        public void LoggingStatementDetector_EatsUpMUnderscoreNamingPrefix()
        {
            const string statement = "m_log.info(\"hello world\");";

            bool isLoggingCode = _detector
                .IsLoggingStatement(statement, new[] { "log." });

            Assert.IsTrue(isLoggingCode);
        }

        [TestMethod]
        public void LoggingStatementDetector_IsCaseInsensitiveForIdentifiers()
        {
            const string statement = "log.info(\"hello world\");";

            bool isLoggingCode = _detector
                .IsLoggingStatement(statement, new[] { "LOG." });

            Assert.IsTrue(isLoggingCode);
        }

        [TestMethod]
        public void LoggingStatementDetector_UsesAllIdentifiersUntilAMatchIsFound()
        {
            const string statement = "log.info(\"hello world\");";

            bool isLoggingCode = _detector
                .IsLoggingStatement(statement, new[] { "apple.", "banana.", "log." });

            Assert.IsTrue(isLoggingCode);
        }

        [TestMethod]
        public void LoggingStatementDetector_OnlyReturnsTrueForCorrectlyIdentifiedMatches()
        {
            const string statement = "myLog.info(\"hello world\");";

            bool isLoggingCode = _detector
                .IsLoggingStatement(statement, new[] { "log." });

            Assert.IsFalse(isLoggingCode);
        }
    }
}
