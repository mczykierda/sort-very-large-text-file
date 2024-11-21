namespace VeryLargeTextFile.Sorter.Comparer;

public class InvalidRecordFormatException(string record) : Exception($"Invalid record format detected.\n{record}")
{
    public string Record { get; } = record;
}