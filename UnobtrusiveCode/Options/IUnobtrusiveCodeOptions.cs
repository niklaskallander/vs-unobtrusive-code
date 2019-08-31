namespace UnobtrusiveCode.Options
{
    using System.Collections.Generic;

    public interface IUnobtrusiveCodeOptions
    {
        bool IsCommentDimmingEnabled { get; }

        bool IsLoggingDimmingEnabled { get; }

        bool IsCommentOutliningEnabled { get; }

        bool IsLoggingOutliningEnabled { get; }

        IEnumerable<string> LoggingIdentifiers { get; }
    }
}
