using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Microsoft.Extensions.Logging;
using VeryLargeTextFile.Generator;
using VeryLargeTextFile.Sorter;
using VeryLargeTextFile.Sorter.FileSplitting;
using VeryLargeTextFile.Sorter.Merging;

namespace VeryLargeTextFile;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Generates or sorts a very large text file");
        rootCommand.AddCommand(CreateGenerateCommand());
        rootCommand.AddCommand(CreateSortCommand());

        return await rootCommand.InvokeAsync(args);
    }

    static Command CreateGenerateCommand()
    {
        var fileOption = new Option<FileInfo>(
            ["--file", "-f"],
            "The very large text file to generate.")
        {
            IsRequired = true,
        };

        var fileSizeOption = new Option<long>(
            ["--file-size", "-fs"],
            () => 10737418240, //10 gb
            $"Approximate size of file in bytes.");

        var textSizeOption = new Option<int>(
            ["--text-size", "-ts"],
            () => 1024,
            description: "Number of bytes for text part of each row.");

        var duplicationFactorOption = new Option<int>(
            ["--duplication-factor", "-df"],
            () => 5,
            description: "Text duplication level: 0 - no duplications, then the larger number the duplicated rows (the text part)");

        var verboseOption = new Option<bool>(
            ["--verbose", "-v"],
            () => false,
            description: "Verbose output to the console");

        duplicationFactorOption.AddValidator(x =>
        {
            if(x.GetValueForOption(duplicationFactorOption) < 0)
            {
                x.ErrorMessage = "Duplication factor cannot be negative";
            }

            if (x.GetValueForOption(duplicationFactorOption) >100)
            {
                x.ErrorMessage = "Duplication factor cannot be larger than 100";
            }
        });

        var command = new Command("generate", "Generates the very large text file.")
        {
            fileOption,
            fileSizeOption,
            textSizeOption,
            duplicationFactorOption,
            verboseOption
        };

        command.SetHandler(
            async (fileInfo, fileSize, textSize, duplicationFactor, verbose) =>
            {
                var (serviceProvider, logger) = GetServiceProvider(verbose);

                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var generator = scope.ServiceProvider.GetRequiredService<IFileGenerator>();

                    await generator.GenerateFile(fileInfo, fileSize, textSize, duplicationFactor);

                    logger.LogDebug("Program finished");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Generation failed");
                }
            },
            fileOption,
            fileSizeOption,
            textSizeOption,
            duplicationFactorOption,
            verboseOption
            );

        return command;
    }

    static Command CreateSortCommand()
    {
        var inputFileOption = new Option<FileInfo>(
            ["--file", "-f"],
            "The very large text file to sort.")
        {
            IsRequired = true,
        };

        var outputFileOption = new Option<FileInfo>(
            ["--output", "-o"],
            "The sorted very large text file to save. Default: the input file with extension *.sorted");

        var splittedFileSizeOption = new Option<int>(
            ["--splitted-file-size", "-sfs"],
            () => 100 * 1024 * 1024,
            "The size of each splitted temporary file");

        var mergeRunFileCountOption = new Option<int>(
            ["--merge-run-file-count", "-mrfc"],
            () => 16,
            "The number of files merged together during single run of merging");
        
        var overwriteOutputFileOption = new Option<bool>(
            ["--overwrite-output-file", "-oof"],
            () => false,
            "Overwrites the output file if it already exists");

        var verboseOption = new Option<bool>(
            ["--verbose", "-v"],
            () => false,
            description: "Verbose output to the console");

        var command = new Command("sort", "Sorts the very large text file.")
        {
            inputFileOption,
            outputFileOption,
            splittedFileSizeOption,
            mergeRunFileCountOption,
            overwriteOutputFileOption,
            verboseOption
        };

        command.SetHandler(
            async (inputFileInfo, outputFileInfo, splittedFileSize, mergeRunFileCount, overwriteOutputFile, verbose) =>
            {
                var (serviceProvider, logger) = GetServiceProvider(verbose);

                try
                {
                    var splittedFilesLocation = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    var config = new FileSortingConfig(
                        new InputFileSplitterConfig(splittedFileSize, splittedFilesLocation),
                        new MergeConfig(mergeRunFileCount),
                        overwriteOutputFile
                        );
                    
                    outputFileInfo ??= new FileInfo($"{inputFileInfo.FullName}.sorted");

                    using var scope = serviceProvider.CreateScope();
                    var sorter = scope.ServiceProvider.GetRequiredService<IFileSorter>();

                    await sorter.SortFile(inputFileInfo, outputFileInfo, config, CancellationToken.None);

                    logger.LogDebug("Program finished");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Sorting failed");
                }
            },
            inputFileOption,
            outputFileOption,
            splittedFileSizeOption,
            mergeRunFileCountOption,
            overwriteOutputFileOption,
            verboseOption
            );

        return command;
    }

    static (ServiceProvider,ILogger) GetServiceProvider(bool verboseLogging)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder
                                        .AddFilter("Microsoft", LogLevel.Warning)
                                        .AddFilter("System", LogLevel.Warning)
                                        .SetMinimumLevel(verboseLogging ? LogLevel.Trace : LogLevel.Information)
                                        .AddSimpleConsole(x => {
                                            x.SingleLine = true;
                                            x.IncludeScopes = false;
                                            x.TimestampFormat = "HH:mm:ss.fff ";
                                            x.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                                        }));

        services.Scan(scan => scan.FromCallingAssembly()
                                      .AddClasses(classes => classes.Where(x => !typeof(Exception).IsAssignableFrom(x)))
                                      .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                                      .AsImplementedInterfaces()
                                      .WithScopedLifetime());

        var serviceProvider = services.BuildServiceProvider();

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Program");

        return (serviceProvider, logger);
    }
}