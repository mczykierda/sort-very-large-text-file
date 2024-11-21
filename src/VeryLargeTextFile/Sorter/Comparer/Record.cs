namespace VeryLargeTextFile.Sorter.Comparer;

readonly ref struct Record(ReadOnlySpan<char> numberAsText, ReadOnlySpan<char> text)
{
    public ReadOnlySpan<char> NumberAsText { get; } = numberAsText;
    public readonly int Number => int.Parse(NumberAsText);

    public ReadOnlySpan<char> Text { get; } = text;

    public static Record Parse(string value)
    {
        const char dot = '.';

        var span = value.AsSpan();
        for (var i = 0; i < span.Length; ++i)
        {
            if (span[i].Equals(dot))
            {
                return new Record(span[..i], span[(i + 2)..]);
            }
        }
        throw new InvalidRecordFormatException(value);
    }
}
