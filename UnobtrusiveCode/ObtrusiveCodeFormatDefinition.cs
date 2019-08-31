namespace UnobtrusiveCode
{
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    using System.ComponentModel.Composition;

    using UnobtrusiveCode.Options;

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = UnobtrusiveCodePackage.ObtrusiveCodeClassification)]
    [Name(UnobtrusiveCodePackage.ObtrusiveCodeClassification)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public class ObtrusiveCodeFormatDefinition : ClassificationFormatDefinition
    {
        public ObtrusiveCodeFormatDefinition()
        {
            var options = new UnobtrusiveCodeOptions();

            ForegroundOpacity = options.DimmingOpacity;
            BackgroundOpacity = options.DimmingOpacity;

            DisplayName = UnobtrusiveCodePackage.ObtrusiveCodeClassification;
        }

        [Export]
        [Name(UnobtrusiveCodePackage.ObtrusiveCodeClassification)]
        internal static ClassificationTypeDefinition ObtrusiveCodeTypeDefinition = null;
    }
}
