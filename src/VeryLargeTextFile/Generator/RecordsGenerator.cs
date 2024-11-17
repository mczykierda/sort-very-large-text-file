
namespace VeryLargeTextFile.Generator;

class RecordsGenerator(IRandomTextGenerator randomTextGenerator) : IRecordsGenerator
{
    static readonly Random _random = new();
    
    public IEnumerable<Record> CreateRecords(int textSize, int textDuplicationFactor, int numberOfRecordsToGenerate)
        => IntroduceDuplications(CreateRecordsWithoutDuplications(textSize, numberOfRecordsToGenerate),
                                textDuplicationFactor);

    List<Record> CreateRecordsWithoutDuplications(int textSize, int numberOfRecordsToGenerate)
        => Enumerable.Repeat(0, numberOfRecordsToGenerate)
                    .Select(x => new Record(_random.Next(),
                                            randomTextGenerator.GenerateRandomText(textSize)))
                    .ToList();

    static List<Record> IntroduceDuplications(List<Record> list, int textDuplicationFactor)
    {
        var numberOfDuplicationIterations = (list.Count / 100) * textDuplicationFactor;

        for (var i = 0; i < numberOfDuplicationIterations; ++i)
        {
            var fromIndex = _random.Next(list.Count);
            var toIndex = _random.Next(list.Count);

            list[toIndex].Text = list[fromIndex].Text;
        }
        return list;
    }
}
