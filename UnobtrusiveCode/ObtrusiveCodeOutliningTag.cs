namespace UnobtrusiveCode
{
    using Microsoft.VisualStudio.Text.Tagging;

    public class ObtrusiveCodeOutliningTag : OutliningRegionTag
    {
        public ObtrusiveCodeOutliningTag
            (
            bool isDefaultCollapsed,
            string collapsedForm
            )
            : base(isDefaultCollapsed, isDefaultCollapsed, collapsedForm, "")
        {
        }
    }
}
