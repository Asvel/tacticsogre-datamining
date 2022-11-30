using System.Text;
using TacticsogreExtracts;

foreach (var arg in args)
{
    var inputPath = Path.GetFullPath(arg);
    foreach (var (id, data) in Util.LoadPakd(inputPath))
    {
        var hasReadableSignature = true;
        foreach (var c in data.AsSpan(0, 4))
        {
            if (!('0' <= c && c <= '9' || 'A' <= c && c <= 'Z' || 'a' <= c && c <= 'z'))
            {
                hasReadableSignature = false;
            }
        }
        var ext = hasReadableSignature
            ? Encoding.ASCII.GetString(data, 0, 4).ToLower()
            : "bin";
        var outputPath = Path.ChangeExtension(inputPath, $".{id}.{ext}");
        using var outputFile = File.OpenWrite(outputPath);
        outputFile.Write(data);
        Console.WriteLine($"Write \"{outputPath}\".");
    }
}
if (args.Length == 0)
{
    Console.WriteLine("Usage: UnpackPakd xxx.pack ...");
}
