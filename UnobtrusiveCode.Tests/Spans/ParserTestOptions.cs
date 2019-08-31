namespace UnobtrusiveCode.Tests.Spans
{
    using System.Collections.Generic;

    using UnobtrusiveCode.Options;

    public class ParserTestOptions : IUnobtrusiveCodeOptions
    {
        public ParserTestOptions
            (
            bool isCommentDimmingEnabled = true,
            bool isCommentOutliningEnabled = true,
            bool isLoggingDimmingEnabled = true,
            bool isLoggingOutliningEnabled = true,
            IEnumerable<string> loggingIdentifiers = null
            )
        {
            IsCommentDimmingEnabled = isCommentDimmingEnabled;
            IsCommentOutliningEnabled = isCommentOutliningEnabled;
            IsLoggingDimmingEnabled = isLoggingDimmingEnabled;
            IsLoggingOutliningEnabled = isLoggingOutliningEnabled;
            LoggingIdentifiers = loggingIdentifiers;
        }

        public bool IsCommentDimmingEnabled { get; }

        public bool IsLoggingDimmingEnabled { get; }

        public bool IsCommentOutliningEnabled { get; }

        public bool IsLoggingOutliningEnabled { get; }

        public IEnumerable<string> LoggingIdentifiers { get; }
    }
}
