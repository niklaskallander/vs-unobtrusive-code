namespace UnobtrusiveCode.Options
{
#pragma warning disable VSTHRD102, VSTHRD104 // Implement internal logic asynchronously, Offer async methods

    using Microsoft.VisualStudio.Shell;

    internal abstract class BaseOptionPage<T> : DialogPage
        where T : BaseOptionModel<T>, new()
    {
        private readonly BaseOptionModel<T> _model;

        public BaseOptionPage()
            => _model = ThreadHelper.JoinableTaskFactory
                .Run(BaseOptionModel<T>.CreateAsync);

        public override object AutomationObject
            => _model;

        public override void LoadSettingsFromStorage()
            => _model.Load();

        public override void SaveSettingsToStorage()
            => _model.Save();
    }

#pragma warning restore VSTHRD102, VSTHRD104
}
