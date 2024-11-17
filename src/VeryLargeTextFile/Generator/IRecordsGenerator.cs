
namespace VeryLargeTextFile.Generator;

public interface IRecordsGenerator
{
    IEnumerable<Record> CreateRecords(int textSize, int textDuplicationFactor, int numberOfRecordsToGenerate);
}
