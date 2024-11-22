using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace VeryLargeTextFile.Utilities;

class ExecutionTimer
    : IDisposable
{
    readonly ILogger _logger;
    readonly string _text;
    readonly Stopwatch _timer = new();

    public ExecutionTimer(ILogger logger, string text)
    {
        _logger = logger;
        _text = text;

        _logger.LogInformation("{text}", _text);
        _timer.Start();
    }
    public void Dispose()
    {
        _timer.Stop();
        _logger.LogInformation("{text} - completed in {elapsed}",_text, _timer.Elapsed);
    }
}
