namespace UnobtrusiveCode.Spans.Normalizers
{
    using System.Collections.Generic;

    using UnobtrusiveCode.Options;

    public interface IObtrusiveCodeSpanNormalizer
    {
        IEnumerable<FeaturizedObtrusiveCodeSpan> Normalize
            (
            IEnumerable<ObtrusiveCodeSpan> spans,
            IUnobtrusiveCodeOptions options,
            string text
            );
    }
}
