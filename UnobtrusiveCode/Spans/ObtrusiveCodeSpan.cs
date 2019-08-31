namespace UnobtrusiveCode.Spans
{
    public struct ObtrusiveCodeSpan
    {
        public ObtrusiveCodeSpan
            (
            int start,
            int end,
            string kind
            )
        {
            Start = start;
            End = end;
            Kind = kind;
        }

        public int End { get; }

        public string Kind { get; }

        public int Length
            => End - Start;

        public int Start { get; }
    }
}
