namespace UnobtrusiveCode
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;

    using UnobtrusiveCode.Options;
    using UnobtrusiveCode.Spans;
    using UnobtrusiveCode.Spans.Normalizers;
    using UnobtrusiveCode.Spans.Parsers;

    using static UnobtrusiveCodePackage;

    public class ObtrusiveCodeTagger : ITagger<IClassificationTag>, ITagger<IOutliningRegionTag>
    {
        private readonly ITextBuffer _buffer;
        private readonly IClassificationTypeRegistryService _classificationService;
        private readonly IEnumerable<IParser> _parsers;
        private readonly IObtrusiveCodeSpanNormalizer _normalizer;
        private ITextSnapshot _snapshot;
        private NormalizedSnapshotSpanCollection _spans;

        private IEnumerable<ITagSpan<ITag>> _tags;

        public ObtrusiveCodeTagger
            (
            ITextBuffer buffer,
            IClassificationTypeRegistryService classificationService,
            IObtrusiveCodeSpanNormalizer normalizer,
            IEnumerable<IParser> parsers
            )
        {
            _buffer = buffer;
            _classificationService = classificationService;
            _parsers = parsers;
            _normalizer = normalizer;
            _snapshot = buffer.CurrentSnapshot;
            _spans = new NormalizedSnapshotSpanCollection();
            _tags = Enumerable.Empty<ITagSpan<ITag>>();

            Parse(CurrentOptions);

            Observable
                .FromEventPattern<TextContentChangedEventArgs>
                (
                    x => _buffer.Changed += x,
                    x => _buffer.Changed -= x
                )
                .Throttle(TimeSpan.FromMilliseconds(CurrentOptions.ParsingDelayMs))
                .Subscribe(x => OnBufferChanged(x.EventArgs));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private static (int Start, int End) GetChangedBounds
            (
            NormalizedSnapshotSpanCollection oldSpanCollection,
            NormalizedSnapshotSpanCollection newSpanCollection
            )
        {
            //the changed regions are regions that appear in one set or the other, but not both.
            var removed = NormalizedSpanCollection
                .Difference(oldSpanCollection, newSpanCollection);

            int start = int.MaxValue;
            int end = -1;

            if (removed.Count > 0)
            {
                start = removed[0].Start;
                end = removed[removed.Count - 1].End;
            }

            if (newSpanCollection.Count > 0)
            {
                start = Math.Min(start, newSpanCollection[0].Start);
                end = Math.Max(end, newSpanCollection[newSpanCollection.Count - 1].End);
            }

            return (start, end);
        }

        private IEnumerable<(SnapshotSpan Span, bool AllowsOutlining, bool AllowsDimming)> GetSpansFrom
            (
            ITextSnapshot snapshot,
            UnobtrusiveCodeOptions options
            )
        {
            var text = snapshot.GetText();

            var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(text);

            var obtrusiveCodeSpans = _parsers
                .SelectMany(x => x.Parse(syntaxTree, options))
                .OrderBy(x => x.Start)
                .ToList();

            return Normalize(obtrusiveCodeSpans, snapshot, text, options).ToList();
        }

        private IEnumerable<ITagSpan<TTag>> GetTags<TTag>(NormalizedSnapshotSpanCollection spans)
            where TTag : ITag
        {
            if (_tags == null)
            {
                return null;
            }

            int start = spans[0].Start.Position;
            int end = spans[spans.Count - 1].End.Position;

            return _tags
                .Where(x => x.Span.End >= start && x.Span.Start <= end)
                .OfType<ITagSpan<TTag>>()
                .ToList();
        }

        private IEnumerable<(SnapshotSpan Span, bool AllowsOutlining, bool AllowsDimming)> Normalize
            (
            IReadOnlyList<ObtrusiveCodeSpan> spans,
            ITextSnapshot snapshot,
            string text,
            UnobtrusiveCodeOptions options
            )
            => _normalizer
                .Normalize(spans, options, text)
                .Select(x => (new SnapshotSpan(new SnapshotPoint(snapshot, x.Start), new SnapshotPoint(snapshot, x.End)), x.AllowsOutlining, x.AllowsDimming));

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void OnBufferChanged(TextContentChangedEventArgs info)
        {
            try
            {
                if (info.After == _buffer.CurrentSnapshot)
                {
                    await ThreadHelper.JoinableTaskFactory
                        .SwitchToMainThreadAsync();

                    Parse(CurrentOptions);
                }
            }
            catch (Exception exception)
            {
                try
                {
                    Debug.Write(exception);
                }
                catch
                {
                    // ignore
                }
            }
        }
#pragma warning restore VSTHRD100

        private void Parse(UnobtrusiveCodeOptions options)
        {
            ITextSnapshot snapshot = _buffer.CurrentSnapshot;

            var spans = GetSpansFrom(snapshot, options);

            if (!spans.Any())
            {
                return;
            }

            //determine the changed span, and send a changed event with the new spans
            var oldSpans = _spans
                .Select(x => new SnapshotSpan(x.Start, x.End).TranslateTo(snapshot, SpanTrackingMode.EdgeExclusive))
                .ToList();

            var oldSpanCollection = new NormalizedSnapshotSpanCollection(oldSpans);
            var newSpanCollection = new NormalizedSnapshotSpanCollection(spans.Select(x => x.Span));

            var (changeStart, changeEnd) = GetChangedBounds(oldSpanCollection, newSpanCollection);

            _snapshot = snapshot;
            _spans = newSpanCollection;

            if (changeStart <= changeEnd)
            {
                var tags = new List<ITagSpan<ITag>>();

                if (options.IsOutliningEnabled())
                {
                    var outliningTagDefinition = new ObtrusiveCodeOutliningTag(options.OutliningIsDefaultCollapsed, options.OutliningCollapsedForm);

                    var outliningTags = spans
                        .Where(x => x.AllowsOutlining)
                        .Select(x => new TagSpan<IOutliningRegionTag>(x.Span, outliningTagDefinition));

                    tags.AddRange(outliningTags);
                }

                if (options.IsDimmingEnabled())
                {
                    var dimmingTagDefinition = new ClassificationTag(_classificationService.GetClassificationType(ObtrusiveCodeClassification));

                    var dimmingTags = spans
                        .Where(x => x.AllowsDimming)
                        .Select(x => new TagSpan<IClassificationTag>(x.Span, dimmingTagDefinition));

                    tags.AddRange(dimmingTags);
                }

                _tags = tags;

                TagsChanged?
                    .Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(_snapshot, Span.FromBounds(changeStart, changeEnd))));
            }
        }

        IEnumerable<ITagSpan<IClassificationTag>> ITagger<IClassificationTag>.GetTags(NormalizedSnapshotSpanCollection spans)
            => GetTags<IClassificationTag>(spans);

        IEnumerable<ITagSpan<IOutliningRegionTag>> ITagger<IOutliningRegionTag>.GetTags(NormalizedSnapshotSpanCollection spans)
            => GetTags<IOutliningRegionTag>(spans);
    }
}
