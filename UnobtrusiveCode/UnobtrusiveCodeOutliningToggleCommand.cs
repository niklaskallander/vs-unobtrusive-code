namespace UnobtrusiveCode
{
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Outlining;
    using Microsoft.VisualStudio.TextManager.Interop;

    using System;
    using System.ComponentModel.Design;

    public sealed class UnobtrusiveCodeOutliningToggleCommand
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("7c1f10b8-be64-492e-b76f-e6c5ecbd4576");

        private readonly IVsTextManager _textManager;
        private readonly IOutliningManagerService _outliningManagerService;

        public UnobtrusiveCodeOutliningToggleCommand
            (
            IVsTextManager textManager,
            IOutliningManagerService outliningManagerService,
            IMenuCommandService commandService
            )
        {
            _textManager = textManager;
            _outliningManagerService = outliningManagerService;

            commandService
                .AddCommand(new MenuCommand(Execute, new CommandID(CommandSet, CommandId)));
        }

        private IWpfTextViewHost GetCurrentViewHost()
        {
            int mustHaveFocus = 1;

            _textManager
                .GetActiveView(mustHaveFocus, null, out IVsTextView vsTextView);

            if (vsTextView is IVsUserData userData)
            {
                Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;

                userData.GetData(ref guidViewHost, out object holder);

                return holder as IWpfTextViewHost;
            }

            return null;
        }

        private void Execute
            (
            object sender,
            EventArgs eventArgs
            )
        {
            if (_outliningManagerService == null)
            {
                return;
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            var viewHost = GetCurrentViewHost();

            var viewWithFocus = viewHost?.TextView;

            if (viewWithFocus == null)
            {
                return;
            }

            _outliningManagerService
                .GetObtrusiveCodeCollapsibles(viewWithFocus)
                .Toggle();
        }
    }
}
