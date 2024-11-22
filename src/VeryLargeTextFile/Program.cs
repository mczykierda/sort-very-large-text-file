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
        var serviceProvider = GetServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Program");

        var rootCommand = new RootCommand("Generates or sorts a very large text file");
        rootCommand.AddCommand(CreateGenerateCommand(serviceProvider, logger));
        rootCommand.AddCommand(CreateSortCommand(serviceProvider, logger));

        return await rootCommand.InvokeAsync(args);
    }

    static Command CreateGenerateCommand(ServiceProvider serviceProvider, ILogger logger)
    {
        var fileOption = new Option<FileInfo>(
            ["--file", "-f"],
            "The very large text file to generate.")
        {
            IsRequired = true,
        };

        var fileSizeOption = new Option<long>(
            ["--file-size", "-fs"],
            () => 1024 * 1024 * 1024,
            $"Approximate size of file in bytes.");

        var textSizeOption = new Option<int>(
            ["--text-size", "-ts"],
            () => 1024,
            description: "Number of bytes for text part of each row.");

        var duplicationFactorOption = new Option<int>(
            ["--duplication-factor", "-df"],
            () => 5,
            description: "Text duplication level: 0 - no duplications, then the larger number the duplicated rows (the text part)");

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
            duplicationFactorOption
        };

        command.SetHandler(
            async (fileInfo, fileSize, textSize, duplicationFactor) =>
            {
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
            duplicationFactorOption
            );

        return command;
    }

    static Command CreateSortCommand(ServiceProvider serviceProvider, ILogger logger)
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
            () => 200 * 1024 * 1024,
            "The size of each splitted temporary file");

        var mergeRunFileCountOption = new Option<int>(
            ["--merge-run-file-count", "-mrfc"],
            () => 10,
            "The number of files merged together during single run of merging");
        
        var overwriteOutputFileOption = new Option<bool>(
            ["--overwrite-output-file", "-oof"],
            () => false,
            "Overwrites the output file if it already exists");

        var command = new Command("sort", "Sorts the very large text file.")
        {
            inputFileOption,
            outputFileOption,
            splittedFileSizeOption,
            mergeRunFileCountOption,
            overwriteOutputFileOption
        };

        command.SetHandler(
            async (inputFileInfo, outputFileInfo, splittedFileSize, mergeRunFileCount, overwriteOutputFile) =>
            {

                try
                {
                    var splittedFilesLocation = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    var config = new SortingConfig(
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
            overwriteOutputFileOption
            );

        return command;
    }

    static ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder
                                        .AddFilter("Microsoft", LogLevel.Warning)
                                        .AddFilter("System", LogLevel.Warning)
                                        .SetMinimumLevel(LogLevel.Trace)
                                        .AddConsole());

        services.Scan(scan => scan.FromCallingAssembly()
                                      .AddClasses(classes => classes.Where(x => !typeof(Exception).IsAssignableFrom(x)))
                                      .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                                      .AsImplementedInterfaces()
                                      .WithScopedLifetime());

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider;
    }
}