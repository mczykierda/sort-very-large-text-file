namespace VeryLargeTextFile.Generator;

public interface IRecordsDuplicator
{
    IList<Record> IntroduceDuplications(IList<Record> records, int textDuplicationFactor);
}
