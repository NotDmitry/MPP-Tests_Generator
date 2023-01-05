using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using 

namespace Tests_Generator.ConsoleApp;

public class GeneratorPipeline
{
    public async Task Generate(string path, int maxReadCount, int maxProcessCount, int maxWriteCount)
    {
        var readBlock = new TransformBlock<string, string>(ReadFile,
            new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxReadCount });

        var generateBlock = new TransformBlock<string, List<TestInfo>>(GenerateCode,
            new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxProcessCount });

        var writeBlock = new ActionBlock<List<TestInfo>>(code => WriteFile(Path.Combine(path, "Tests"), code),
            new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxWriteCount });

        readBlock.LinkTo(generateBlock, new DataflowLinkOptions { PropagateCompletion = true });
        generateBlock.LinkTo(writeBlock, new DataflowLinkOptions { PropagateCompletion = true });

        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        var files = directoryInfo.GetFiles().Select(f => f.FullName);
        foreach (var file in files)
        {
            readBlock.Post(file);
        }

        readBlock.Complete();
        await writeBlock.Completion;
    }

    private async Task<string> ReadFile(string fileName)
    {
        return await File.ReadAllTextAsync(fileName);
    }

    private List<Targ> GenerateCode(string code)
    {
        var writer = new Writer(code);
        return writer.Generate().ToList();
    }

    private void WriteFile(string outputFolder, List<TestInfo> testInfos)
    {
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        foreach (var info in testInfos)
        {
            File.WriteAllTextAsync(Path.Combine(outputFolder, $"{info.Name}.cs"), info.Code);
        }
    }
}
