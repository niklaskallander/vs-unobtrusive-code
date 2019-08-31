namespace UnobtrusiveCode
{
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Outlining;
    using Microsoft.VisualStudio.Utilities;
    using System.ComponentModel.Composition;

    using UnobtrusiveCode.Options;

    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("csharp")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public sealed class UnobtrusiveCodeViewCreationListener : IWpfTextViewCreationListener
    {
        [Import]
        internal IOutliningManagerService OutliningManagerService { get; set; }

        public void TextViewCreated(IWpfTextView view)
        {
            var options = new UnobtrusiveCodeOptions();

            OutliningManagerService
                .GetObtrusiveCodeCollapsibles(view)?
                .Toggle(options.OutliningIsDefaultCollapsed);
        }
    }
}
