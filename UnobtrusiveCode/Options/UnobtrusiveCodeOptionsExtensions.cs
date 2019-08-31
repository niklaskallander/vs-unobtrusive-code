namespace UnobtrusiveCode.Options
{
    using UnobtrusiveCode.Spans;

    using static UnobtrusiveCodePackage;

    public static class UnobtrusiveCodeOptionsExtensions
    {
        public static bool IsCommentEnabled(this IUnobtrusiveCodeOptions options)
            => options.IsCommentOutliningEnabled
            || options.IsCommentDimmingEnabled;

        public static bool IsLoggingEnabled(this IUnobtrusiveCodeOptions options)
            => options.IsLoggingOutliningEnabled
            || options.IsLoggingDimmingEnabled;

        public static bool IsDimmingEnabled(this IUnobtrusiveCodeOptions options)
            => options.IsCommentDimmingEnabled
            || options.IsLoggingDimmingEnabled;

        public static bool IsOutliningEnabled(this IUnobtrusiveCodeOptions options)
            => options.IsCommentOutliningEnabled
            || options.IsLoggingOutliningEnabled;

        public static bool AllowsOutliningFor
            (
            this IUnobtrusiveCodeOptions options,
            ObtrusiveCodeSpan span
            )
            => (span.Kind == CommentFeatures && options.IsCommentOutliningEnabled)
            || (span.Kind == LoggingFeatures && options.IsLoggingOutliningEnabled);

        public static bool AllowsDimmingFor
            (
            this IUnobtrusiveCodeOptions options,
            ObtrusiveCodeSpan span
            )
            => (span.Kind == CommentFeatures && options.IsCommentDimmingEnabled)
            || (span.Kind == LoggingFeatures && options.IsLoggingDimmingEnabled);
    }
}
