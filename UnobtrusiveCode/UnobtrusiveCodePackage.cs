namespace UnobtrusiveCode
{
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text.Outlining;
    using Microsoft.VisualStudio.TextManager.Interop;

    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Design;
    using System.Runtime.InteropServices;
    using System.Threading;

    using UnobtrusiveCode.Options;

    using Task = System.Threading.Tasks.Task;

    [Guid(PackageGuidString)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.DocumentWindowActive_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(DialogPageProvider.General), "Unobtrusive Code", "General", 0, 0, true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class UnobtrusiveCodePackage : AsyncPackage
    {
        public const string ObtrusiveCodeClassification = "UnobtrusiveCode/ObtrusiveCode";
        public const string LoggingFeatures = "UnobtrusiveCode/Logging";
        public const string CommentFeatures = "UnobtrusiveCode/Comment";
        public const string PackageGuidString = "377f72ad-751e-4b9a-863a-2cd3b7f0a034";

        [Import]
        internal IOutliningManagerService OutliningManagerService { get; set; }

        protected override async Task InitializeAsync
            (
            CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress
            )
        {
            await JoinableTaskFactory
                .SwitchToMainThreadAsync(cancellationToken);

            await this
                .SatisfyImportsOnceAsync(cancellationToken);

            var commandService = await GetServiceAsync(typeof(IMenuCommandService)) as IMenuCommandService;
            var textManager = await GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager;

            new UnobtrusiveCodeOutliningToggleCommand(textManager, OutliningManagerService, commandService);
        }
    }
}
