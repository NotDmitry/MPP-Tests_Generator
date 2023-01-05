namespace Tests_Generator.Core;

public class TargetContainer
{
    // File path or name
    public string Path { get; set; }

    // File content
    public string Code { get; set; }

    public TargetContainer(string path, string code)
    {
        Path = path;
        Code = code;
    }
}