namespace UnobtrusiveCode.Options
{
    using System;
    using System.Collections.Generic;

    internal class UnobtrusiveCodeOptions : IUnobtrusiveCodeOptions
    {
        private readonly UnobtrusiveCodeOptionsRaw _options;

        public UnobtrusiveCodeOptions()
            : this(UnobtrusiveCodeOptionsRaw.Instance)
        {
        }

        public UnobtrusiveCodeOptions(UnobtrusiveCodeOptionsRaw options)
        {
            _options = options;
            LoggingIdentifiers = _options.LoggingIdentifiers
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public double DimmingOpacity
            => _options.DimmingOpacity;

        public bool IsDimmingOpacityTogglingEnabled
            => _options.DimmingOpacityTogglingEnabled;

        public DimmingToggleKeys DimmingToggleKey
            => _options.DimmingToggleKey;

        public bool IsCommentDimmingEnabled
            => _options.CommentDimmingEnabled;

        public bool IsCommentOutliningEnabled
            => _options.CommentOutliningEnabled;

        public bool IsLoggingDimmingEnabled
            => _options.LoggingDimmingEnabled;

        public bool IsLoggingOutliningEnabled
            => _options.LoggingOutliningEnabled;

        public IEnumerable<string> LoggingIdentifiers { get; }

        public string OutliningCollapsedForm
            => _options.OutliningCollapsedForm;

        public bool OutliningIsDefaultCollapsed
            => _options.OutliningDefaultCollapsed;

        public int ParsingDelayMs
            => _options.ParsingDelayMs;
    }
}
