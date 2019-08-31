namespace UnobtrusiveCode.Options
{
    using System.ComponentModel;

    internal class UnobtrusiveCodeOptionsRaw : BaseOptionModel<UnobtrusiveCodeOptionsRaw>
    {
        // -- misc

        [Category("Misc")]
        [DisplayName("Parsing delay (ms)")]
        [Description("The delay until the document is re-parsed after a buffer change (default: 1000 ms).")]
        [DefaultValue(1000)]
        public int ParsingDelayMs { get; set; } = 1000;

        // -- dimming

        [Category("Dimming")]
        [DisplayName("Dimming opacity")]
        [Description("Dimming opacity (default: 0.25)")]
        [DefaultValue(0.25)]
        public double DimmingOpacity { get; set; } = 0.25;

        // -- outlining

        [Category("Outlining")]
        [DisplayName("Collapsed by default")]
        [Description("Collapse obtrusive code outlining regions as they are found (default: true)")]
        [DefaultValue(true)]
        public bool OutliningDefaultCollapsed { get; set; } = true;

        [Category("Outlining")]
        [DisplayName("Collapsed form")]
        [Description("Collapsed form of obtrusive code outlining regions (default: \"\")")]
        [DefaultValue("")]
        public string OutliningCollapsedForm { get; set; } = "";

        // -- comments

        [Category("Comments")]
        [DisplayName("Dimming enabled")]
        [Description("Disable/Enable comment dimming (default: true)")]
        [DefaultValue(true)]
        public bool CommentDimmingEnabled { get; set; } = true;

        [Category("Comments")]
        [DisplayName("Outlining enabled")]
        [Description("Disable/Enable comment outlining (default: true)")]
        [DefaultValue(true)]
        public bool CommentOutliningEnabled { get; set; } = true;

        // -- logging

        [Category("Logging")]
        [DisplayName("Dimming enabled")]
        [Description("Disable/Enable logging dimming (default: true)")]
        [DefaultValue(true)]
        public bool LoggingDimmingEnabled { get; set; } = true;

        [Category("Logging")]
        [DisplayName("Outlining enabled")]
        [Description("Disable/Enable logging outlining (default: true)")]
        [DefaultValue(true)]
        public bool LoggingOutliningEnabled { get; set; } = true;

        [Category("Logging")]
        [DisplayName("Logging identifiers")]
        [Description("Set logging identifiers (default: \"log. logger. logging.\", case-insensitive)")]
        [DefaultValue("log. logger. logging.")]
        public string LoggingIdentifiers { get; set; } = "log. logger. logging.";
    }
}
