namespace Tests_Generator.Core;

public class TargetContainer
{
    public string Path { get; set; }
    public string Code { get; set; }

    public TargetContainer(string path, string code)
    {
        Path = path;
        Code = code;
    }
}