namespace VeryLargeTextFile.Generator;

class RecordsGenerator(
    IRandomTextGenerator randomTextGenerator, 
    IRecordsDuplicator duplicator
    ) 
    : IRecordsGenerator
{
    static readonly Random _random = new();

    public IEnumerable<Record> CreateRecords(int textSize, int textDuplicationFactor, int numberOfRecordsToGenerate)
    {
        var records = CreateRecordsWithoutDuplications(textSize, numberOfRecordsToGenerate);
        var recordsWithDuplications = duplicator.IntroduceDuplications(records, textDuplicationFactor);
        return recordsWithDuplications;
    }

    List<Record> CreateRecordsWithoutDuplications(int textSize, int numberOfRecordsToGenerate)
        => Enumerable.Repeat(0, numberOfRecordsToGenerate)
                    .Select(x => new Record(_random.Next(),
                                            randomTextGenerator.GenerateRandomText(textSize)))
                    .ToList();
}
