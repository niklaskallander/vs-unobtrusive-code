namespace UnobtrusiveCode.Spans.Normalizers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnobtrusiveCode.Options;

    public class ObtrusiveCodeSpanNormalizer : IObtrusiveCodeSpanNormalizer
    {
        public IEnumerable<FeaturizedObtrusiveCodeSpan> Normalize
            (
            IEnumerable<ObtrusiveCodeSpan> spans,
            IUnobtrusiveCodeOptions options,
            string text
            )
        {
            return options.IsOutliningEnabled()
                ? NormalizeForOutlining()
                : AsIs();

            IEnumerable<FeaturizedObtrusiveCodeSpan> AsIs()
            {
                foreach (var span in spans)
                {
                    yield return new FeaturizedObtrusiveCodeSpan(span.Start, span.End, options.AllowsOutliningFor(span), options.AllowsDimmingFor(span));
                }
            }

            IEnumerable<FeaturizedObtrusiveCodeSpan> NormalizeForOutlining()
            {
                var outliningSpans = spans
                    .Where(x => options.AllowsOutliningFor(x))
                    .OrderBy(x => x.Start)
                    .ToList();

                var dimmingSpans = spans
                    .Where(x => options.AllowsDimmingFor(x))
                    .ToList();

                foreach (var dimmingSpan in dimmingSpans)
                {
                    yield return new FeaturizedObtrusiveCodeSpan(dimmingSpan.Start, dimmingSpan.End, false, true);
                }

                int maxIndex = outliningSpans.Count - 1;

                for (int i = 0; i < outliningSpans.Count; i++)
                {
                    var current = outliningSpans[i];

                    int start = current.Start;
                    int end = current.End;

                    if (i < maxIndex)
                    {
                        int j = i + 1;

                        for (; j < outliningSpans.Count; j++)
                        {
                            var next = outliningSpans[j];

                            int diff = next.Start - end;

                            if (diff <= 0) // overlap
                            {
                                if (next.End > end) // only change end if it is after current
                                {
                                    end = next.End;
                                }

                                i++;
                            }
                            else if (diff <= 2) // might be a line break/whitespace separating the spans
                            {
                                if (text.AsSpan(end, diff).IsWhiteSpace())
                                {
                                    end = next.End;
                                    i++;
                                }
                            }
                            else
                            {
                                // something not collapsible is separating the spans
                                break;
                            }
                        }
                    }

                    yield return new FeaturizedObtrusiveCodeSpan(start, end, true, false);
                }
            }
        }
    }
}
