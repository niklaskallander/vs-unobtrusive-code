namespace UnobtrusiveCode
{
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Outlining;

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading;

    using Task = System.Threading.Tasks.Task;

    public static class Extensions
    {
        private static IComponentModel _compositionService;

        public static void SatisfyImportsOnce(this object o)
            => ThreadHelper.JoinableTaskFactory
                .Run(() => SatisfyImportsOnceAsync(o));

        public static async Task SatisfyImportsOnceAsync
            (
            this object o,
            CancellationToken cancellationToken = default
            )
        {
            await ThreadHelper.JoinableTaskFactory
                .SwitchToMainThreadAsync(cancellationToken);

            if (_compositionService == null)
            {
                _compositionService = ServiceProvider.GlobalProvider
                    .GetService(typeof(SComponentModel)) as IComponentModel;
            }

            if (_compositionService != null)
            {
                _compositionService.DefaultCompositionService
                    .SatisfyImportsOnce(o);
            }
        }

        internal static OutliningManagerBoundCollapsibleCollection GetObtrusiveCodeCollapsibles
            (
            this IOutliningManagerService outliningManagerService, ITextView view
            )
        {
            var outliningManager = outliningManagerService?
                .GetOutliningManager(view);

            if (outliningManager == null)
            {
                return null;
            }

            var snapshot = view.TextSnapshot;
            var fullSpan = new SnapshotSpan(snapshot, 0, snapshot.Length);

            var collapsibles = outliningManager
                .GetAllRegions(fullSpan)
                .Where(x => x.Tag is ObtrusiveCodeOutliningTag)
                .ToList();

            return new OutliningManagerBoundCollapsibleCollection(outliningManager, collapsibles);
        }

        public static void Toggle
            (
            this IOutliningManager outliningManager,
            IEnumerable<ICollapsible> collapsibles,
            bool collapse = true
            )
        {
            foreach (var collapsible in collapsibles)
            {
                if (collapse && collapsible.IsCollapsible && !collapsible.IsCollapsed)
                {
                    outliningManager
                        .TryCollapse(collapsible);
                }
                else if (!collapse && collapsible is ICollapsed collapsed)
                {
                    outliningManager
                        .Expand(collapsed);
                }
            }
        }

        internal sealed class OutliningManagerBoundCollapsibleCollection
        {
            public IOutliningManager Manager { get; }
            public IEnumerable<ICollapsible> Collapsibles { get; }

            public OutliningManagerBoundCollapsibleCollection
                (
                IOutliningManager manager,
                IEnumerable<ICollapsible> collapsibles
                )
            {
                Manager = manager
                    ?? throw new ArgumentNullException(nameof(manager));

                Collapsibles = collapsibles
                    ?? throw new ArgumentNullException(nameof(collapsibles));
            }

            public void Toggle(bool? collapse = null)
            {
                bool shouldCollapse = collapse
                    ?? Collapsibles
                        .Any(x => !x.IsCollapsed);

                foreach (var collapsible in Collapsibles)
                {
                    if (shouldCollapse && collapsible.IsCollapsible && !collapsible.IsCollapsed)
                    {
                        var collapsed = Manager.TryCollapse(collapsible);
                    }
                    else if (!shouldCollapse && collapsible is ICollapsed collapsed)
                    {
                        Manager.Expand(collapsed);
                    }
                }
            }
        }
    }
}
