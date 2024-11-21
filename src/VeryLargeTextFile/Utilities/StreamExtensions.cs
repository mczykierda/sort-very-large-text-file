namespace VeryLargeTextFile.Utilities;

static class StreamExtensions
{
    public static bool EndOfStream(this Stream stream)
        => !(stream.Position < stream.Length);
}
