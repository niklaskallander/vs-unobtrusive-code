namespace UnobtrusiveCode.Spans.Parsers.Detectors
{
    using System.Collections.Generic;

    public interface ILoggingStatementDetector
    {
        bool IsLoggingStatement
            (
            string statement,
            IEnumerable<string> identifiers
            );
    }
}
