namespace UnobtrusiveCode.Options
{
    using Community.VisualStudio.Toolkit;

    using System.ComponentModel;
    using System.Threading.Tasks;

    internal class UnobtrusiveCodeOptionsRaw : BaseOptionModel<UnobtrusiveCodeOptionsRaw>
    {
        [Category("Misc")]
        [DisplayName("Parsing delay (ms)")]
        [Description("The delay until the document is re-parsed after a buffer change (default: 1000 ms).")]
        [DefaultValue(1000)]
        public int ParsingDelayMs { get; set; } = 1000;

        // -- dimming

        [Category("Dimming")]
        [DisplayName("Dimming opacity")]
        [Description("Dimming opacity (default: 0.4).")]
        [DefaultValue(0.4)]
        public double DimmingOpacity { get; set; } = 0.4;

        [Category("Dimming")]
        [DisplayName("Dimming opacity toggle enabled")]
        [Description("Disable/Enable toggling of dimming opacity (default: true).")]
        [DefaultValue(true)]
        public bool DimmingOpacityTogglingEnabled { get; set; } = false;

        [Category("Dimming")]
        [DisplayName("Dimming opacity toggle key")]
        [Description("Hold down this key to temporarily display dimmed obtrusive code with full opacity (default: LeftShift).")]
        [DefaultValue(DimmingToggleKeys.LeftShift)]
        public DimmingToggleKeys DimmingToggleKey { get; set; } = DimmingToggleKeys.RightShift;

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

        public override async Task SaveAsync()
        {
            await base.SaveAsync();

            UnobtrusiveCodePackage.CurrentOptions = new UnobtrusiveCodeOptions(this);
        }
    }
}
