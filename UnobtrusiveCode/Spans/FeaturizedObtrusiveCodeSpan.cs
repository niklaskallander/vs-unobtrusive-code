namespace UnobtrusiveCode.Spans
{
    public struct FeaturizedObtrusiveCodeSpan
    {
        public FeaturizedObtrusiveCodeSpan
            (
            int start,
            int end,
            bool allowsOutlining,
            bool allowsDimming
            )
        {
            Start = start;
            End = end;
            AllowsOutlining = allowsOutlining;
            AllowsDimming = allowsDimming;
        }

        public bool AllowsDimming { get; }

        public bool AllowsOutlining { get; }

        public int End { get; }

        public int Length
            => End - Start;

        public int Start { get; }
    }
}
