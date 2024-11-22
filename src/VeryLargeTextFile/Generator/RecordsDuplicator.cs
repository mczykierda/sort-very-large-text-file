namespace VeryLargeTextFile.Generator;

class RecordsDuplicator 
    : IRecordsDuplicator
{
    static readonly Random _random = new();

    public IList<Record> IntroduceDuplications(IList<Record> records, int textDuplicationFactor)
    {
        var numberOfDuplicationIterations = Math.Max(records.Count / 100, 1) * textDuplicationFactor;

        for (var i = 0; i < numberOfDuplicationIterations; ++i)
        {
            var fromIndex = _random.Next(records.Count);
            var toIndex = _random.Next(records.Count);

            records[toIndex].Text = records[fromIndex].Text;
        }
        return records;
    }
}
