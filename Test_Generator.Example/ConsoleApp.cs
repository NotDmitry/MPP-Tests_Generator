using Tests_Generator.Core.Dataflow;

namespace Test_Generator.Example;

public static class ConsoleApp
{
    private const int DefaultMaxDegreeOfParallelism = 10;

    public static async Task Main(string[] args)
    {

        if (args.Length <= 0 || args.Length > 5)
        {
            Console.WriteLine("Invalid number of parameters. Print \"man\" for help\n");
            return;
        }

        if (args[0] == "man")
        {
            ShowManual();
            return;
        }

        if (!Directory.Exists(args[0]))
        {
            Console.WriteLine("No such directory exists\n");
            return;
        }

        string sourcePath = args[0];
        string destinationPath = sourcePath;

        if (args.Length == 5)
        {
            destinationPath = args[4];
        }

        int maxReadCount = DefaultMaxDegreeOfParallelism;
        int maxProcessCount = DefaultMaxDegreeOfParallelism;
        int maxWriteCount = DefaultMaxDegreeOfParallelism;

        if (args.Length >= 2)
        {
            if (!int.TryParse(args[1], out var value))
            {
                Console.WriteLine("Invalid second parameter value\n");
                return;
            }
            maxReadCount = value;
        }

        if (args.Length >= 3)
        {
            if (!int.TryParse(args[2], out var value))
            {
                Console.WriteLine("Invalid third parameter value\n");
                return;
            }
            maxProcessCount = value;
        }

        if (args.Length >= 4)
        {
            if (!int.TryParse(args[3], out var value))
            {
                Console.WriteLine("Invalid fourth parameter value\n");
                return;
            }
            maxWriteCount = value;
        }

        var generator = new GeneratorPipeline();
        await generator.Generate(sourcePath, maxReadCount, maxProcessCount, maxWriteCount, destinationPath);
    }

    private static void ShowManual()
    {
        Console.WriteLine(
            """ 
            Print "man" for help;
            ###############################################################################

                Arguments:
                1) [string] <SourcePath>: Path to the .cs code file for producing tests 
                2) [int] <MaxReadCount>: degree of parallelism for reading
                3) [int] <MaxProcessCount>: degree of parallelism for processing
                4) [int] <MaxWriteCount>: degree of parallelism for writing
                5) [string] <DestinationPath>: Path to the generated tests

            ###############################################################################
            """
        );
    }

}
