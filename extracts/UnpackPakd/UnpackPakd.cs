using System.Text;

foreach (var arg in args)
{
    var inputPath = Path.GetFullPath(arg);
    using var inputFile = File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    var data = new byte[inputFile.Length];
    inputFile.Read(data, 0, data.Length);

    var reader = new BinaryReader(new MemoryStream(data));
    if (reader.ReadUInt32() != 0x646B6170/*pakd*/) throw new InvalidDataException();

    var count = reader.ReadInt32();
    var offsets = new int[count + 1];
    for (var i = 0; i < count + 1; i++) offsets[i] = reader.ReadInt32();
    var ids = new int[count];
    var hasId = offsets[0] >= reader.BaseStream.Position + count * 4;
    for (var i = 0; i < count; i++) ids[i] = hasId ? reader.ReadInt32() : i;

    for (var i = 0; i < count; i++)
    {
        var offset = offsets[i];
        var hasReadableSignature = true;
        foreach (var c in data.AsSpan(offset, 4))
        {
            if (!('0' <= c && c <= '9' || 'A' <= c && c <= 'Z' || 'a' <= c && c <= 'z'))
            {
                hasReadableSignature = false;
            }
        }
        var ext = hasReadableSignature
            ? Encoding.ASCII.GetString(data, offset, 4).ToLower()
            : "bin";
        var outputPath = Path.ChangeExtension(inputPath, $".{ids[i]}.{ext}");
        using var outputFile = File.OpenWrite(outputPath);
        outputFile.Write(data, offset, offsets[i + 1] - offset);
        Console.WriteLine($"Write \"{outputPath}\".");
    }
}
if (args.Length == 0)
{
    Console.WriteLine("Usage: UnpackPakd xxx.pack ...");
}
