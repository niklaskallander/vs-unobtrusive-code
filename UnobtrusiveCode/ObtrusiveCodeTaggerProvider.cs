namespace UnobtrusiveCode
{
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using UnobtrusiveCode.Spans.Normalizers;
    using UnobtrusiveCode.Spans.Parsers;
    using UnobtrusiveCode.Spans.Parsers.Detectors;

    [Export(typeof(ITaggerProvider))]
    [ContentType("csharp")]
    [TagType(typeof(IClassificationTag))]
    [TagType(typeof(IOutliningRegionTag))]
    public class ObtrusiveCodeTaggerProvider : ITaggerProvider
    {
        private static readonly IObtrusiveCodeSpanNormalizer Normalizer = new ObtrusiveCodeSpanNormalizer();
        private static readonly IEnumerable<IParser> Parsers = new IParser[]
        {
            new LoggingParser(new LoggingStatementDetector()),
            new CommentParser()
        };

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistryService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer)
            where T : ITag
            => buffer.Properties
                .GetOrCreateSingletonProperty(() => CreateTaggerFor(buffer))
                    as ITagger<T>;

        private ObtrusiveCodeTagger CreateTaggerFor(ITextBuffer buffer)
            => new ObtrusiveCodeTagger(buffer, ClassificationTypeRegistryService, Normalizer, Parsers);
    }
}
