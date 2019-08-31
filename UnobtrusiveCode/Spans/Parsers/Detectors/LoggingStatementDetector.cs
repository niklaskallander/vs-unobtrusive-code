namespace UnobtrusiveCode.Spans.Parsers.Detectors
{
    using System;
    using System.Collections.Generic;

    public class LoggingStatementDetector : ILoggingStatementDetector
    {
        public bool IsLoggingStatement
            (
            string statement,
            IEnumerable<string> identifiers
            )
        {
            var text = statement
                .AsSpan()
                .TrimStart();

            text = StripKeywordsAndCommonNamingPatternsFrom(text);

            foreach (var identifier in identifiers)
            {
                var needle = identifier
                    .AsSpan()
                    .Trim();

                bool checkContinuesWithDot = needle[needle.Length - 1] == '.';

                if (checkContinuesWithDot)
                {
                    needle = needle
                        .Slice(0, needle.Length - 1)
                        .TrimEnd();
                }

                if (text.StartsWith(needle, StringComparison.OrdinalIgnoreCase))
                {
                    if (!checkContinuesWithDot)
                    {
                        return true;
                    }

                    if (text.Length > needle.Length)
                    {
                        var temp = text
                            .Slice(needle.Length)
                            .TrimStart();

                        if (temp.Length > 0 && temp[0] == '.')
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static ReadOnlySpan<char> StripKeywordsAndCommonNamingPatternsFrom(ReadOnlySpan<char> text)
        {
            if (text.StartsWith("this".AsSpan()) ||
                text.StartsWith("base".AsSpan()))
            {
                var temp = text
                    .Slice(4)
                    .TrimStart();

                if (temp.Length > 1 && temp[0] == '.')
                {
                    text = temp
                        .Slice(1)
                        .TrimStart();
                }
            }

            return text
                .StartsWith("m_".AsSpan(), StringComparison.OrdinalIgnoreCase)
                    ? text.Slice(2)
                    : text.TrimStart('_');
        }
    }
}
