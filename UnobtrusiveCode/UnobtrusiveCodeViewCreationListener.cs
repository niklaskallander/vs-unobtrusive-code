namespace UnobtrusiveCode
{
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Outlining;
    using Microsoft.VisualStudio.Utilities;

    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using UnobtrusiveCode.Options;

    using static UnobtrusiveCodePackage;

    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("csharp")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public sealed class UnobtrusiveCodeViewCreationListener : IWpfTextViewCreationListener
    {
        [Import]
        internal IClassificationTypeRegistryService Registry { get; set; }

        [Import]
        internal IClassificationFormatMapService Map { get; set; }

        [Import]
        internal IOutliningManagerService OutliningManagerService { get; set; }

#pragma warning disable VSTHRD100 // Avoid async void methods
        public async void TextViewCreated(IWpfTextView view)
        {
            try
            {
                if (CurrentOptions.IsDimmingOpacityTogglingEnabled)
                {
                    new ToggleDimmingFeature(view, Registry, Map.GetClassificationFormatMap(view));
                }

                // yield to allow taggers to first collect regions
                await Task.Yield();

                OutliningManagerService
                    .GetObtrusiveCodeCollapsibles(view)?
                    .Toggle(CurrentOptions.OutliningIsDefaultCollapsed);
            }
            catch
            {
                // ignore
            }
        }
#pragma warning restore VSTHRD100

        private class ToggleDimmingFeature
        {
            private readonly IWpfTextView _textView;
            private readonly IClassificationTypeRegistryService _registry;
            private readonly IClassificationFormatMap _map;

            public ToggleDimmingFeature
                (
                IWpfTextView textView,
                IClassificationTypeRegistryService registry,
                IClassificationFormatMap map
                )
            {
                _textView = textView;
                _registry = registry;
                _map = map;

                CurrentOptionsChanged += OnCurrentOptionsChanged;
                _textView.Closed += OnTextViewClosed;
                _textView.VisualElement.Loaded += VisualElementOnLoaded;
                _textView.VisualElement.Unloaded += VisualElementOnUnloaded;
            }

            private void OnCurrentOptionsChanged(object sender, EventArgs e)
                => SetDimmingOpacity(CurrentOptions.DimmingOpacity);

            private void VisualElementOnLoaded(object sender, RoutedEventArgs e)
            {
                _textView.VisualElement.PreviewKeyDown += VisualElementOnPreviewKeyDown;
                _textView.VisualElement.PreviewKeyUp += VisualElementOnPreviewKeyUp;
            }

            private void VisualElementOnUnloaded(object sender, RoutedEventArgs e)
            {
                _textView.VisualElement.PreviewKeyDown -= VisualElementOnPreviewKeyDown;
                _textView.VisualElement.PreviewKeyUp -= VisualElementOnPreviewKeyUp;
            }

            private void OnTextViewClosed(object sender, EventArgs e)
            {
                CurrentOptionsChanged -= OnCurrentOptionsChanged;
                _textView.Closed -= OnTextViewClosed;
                _textView.VisualElement.Loaded -= VisualElementOnLoaded;
                _textView.VisualElement.Unloaded -= VisualElementOnUnloaded;
                _textView.VisualElement.PreviewKeyDown -= VisualElementOnPreviewKeyDown;
                _textView.VisualElement.PreviewKeyUp -= VisualElementOnPreviewKeyUp;
            }

            private void VisualElementOnPreviewKeyUp(object sender, KeyEventArgs e)
                => PotentiallyToggleOn(e);

            private void VisualElementOnPreviewKeyDown(object sender, KeyEventArgs e)
                => PotentiallyToggleOn(e);

            private void PotentiallyToggleOn(KeyEventArgs e)
            {
                if (!CurrentOptions.IsDimmingOpacityTogglingEnabled)
                {
                    return;
                }

                if (e.Key == TranslateToEventKey(CurrentOptions.DimmingToggleKey))
                {
                    double opacity = e.IsUp
                        ? CurrentOptions.DimmingOpacity
                        : 1;

                    SetDimmingOpacity(opacity);
                }
            }

            private void SetDimmingOpacity(double opacity)
            {
                var type = _registry.GetClassificationType(ObtrusiveCodeClassification);

                var props = _map
                    .GetExplicitTextProperties(type);

                if (props.ForegroundOpacity == opacity &&
                    props.BackgroundOpacity == opacity)
                {
                    return;
                }

                props = props
                    .SetForegroundOpacity(opacity)
                    .SetBackgroundOpacity(opacity);

                _map.SetExplicitTextProperties(type, props);
            }

            private static Key TranslateToEventKey(DimmingToggleKeys key)
            {
                switch (key)
                {
                    case DimmingToggleKeys.LeftCtrl:
                        return Key.LeftCtrl;
                    case DimmingToggleKeys.RightCtrl:
                        return Key.RightCtrl;
                    case DimmingToggleKeys.RightShift:
                        return Key.RightShift;
                    case DimmingToggleKeys.LeftShift:
                    default:
                        return Key.LeftShift;
                }
            }
        }
    }
}
