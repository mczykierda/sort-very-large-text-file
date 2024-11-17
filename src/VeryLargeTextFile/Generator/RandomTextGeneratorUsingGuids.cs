using System.Text;

namespace VeryLargeTextFile.Generator;

class RandomTextGeneratorUsingGuids : IRandomTextGenerator
{
    public string GenerateRandomText(int length)
    {
        var sb = new StringBuilder();
        while (sb.Length < length)
        {
            sb.Append(Guid.NewGuid().ToString("N"));
        }

        return sb.ToString(0, length);
    }
}

