using System.IO.Compression;

var key2 = new uint[]
{
    0x196DBCB0, 0xEAB614E5, 0xB116CBF4, 0xB57FE57D,
    0xC45E7C0B, 0x99314F2D, 0xE8199867, 0xC9DE5828,
    0x83C99BC1, 0x8564C434, 0xDE0EFA36, 0x9968E7FB,
    0x36343271, 0x637D01E5, 0x7C77815C
};
var key3 = new uint[]
{
    0xED9853C6, 0x1BE0AAD0, 0x32163143, 0x57520DCE,
    0xFB0F727B, 0xF1C6A9B7, 0x892C72F1, 0xFA1BB25F,
    0xE2E3DBA8, 0xC9BEB541, 0x21C6AE5D, 0x7488D2E4,
    0x874F2697, 0x7A69F07A, 0x09157EB7, 0xCAAD55BA,
    0x763460B1
};

foreach (var arg in args)
{
    var inputPath = Path.GetFullPath(arg);
    var data = File.ReadAllBytes(inputPath);
    unsafe
    {
        {
            var data0 = data[0];
            for (var i = 0; i < data.Length; i++)
            {
                var j = i + 1;
                var v = j != data.Length ? data[j] : data0;
                data[i] = (byte)(data[i] << 3 | v >> 5);
            }
        }

        fixed (byte* pData = data)
        {
            var pData32 = (uint*)pData;
            var pEnd = pData32 + (data.Length >> 2);
            var j = 8;
            while (pData32 != pEnd)
            {
                j %= 15;
                *pData32 ^= key2[j];
                pData32++;
                j++;
            }
        }

        fixed (byte* pData = data)
        {
            var pData32 = (uint*)pData;
            var pEnd = pData32 + (data.Length >> 2);
            pData32 += 4;
            var j = data[0xf] & 0x7f;
            while (pData32 != pEnd)
            {
                j %= 17;
                *pData32 ^= key3[j];
                pData32++;
                j++;
            }
        }
    }

    var outputPath = "";
    var ext = "raw";
    if (data[0] == 'P' && data[1] == 'K' && data[2] == 3 && data[3] == 4)
    {
        ext = "zip";
        using var zip = new ZipArchive(new MemoryStream(data));
        if (zip.Entries.Count == 1)
        {
            var entry = zip.Entries[0];
            
            outputPath = entry.Name.StartsWith(Path.GetFileNameWithoutExtension(inputPath))
                ? Path.Combine(Path.GetDirectoryName(inputPath)!, entry.Name)
                : Path.ChangeExtension(inputPath, entry.Name);
            entry.ExtractToFile(outputPath, true);
        }
    }
    if (outputPath == "")
    {
        outputPath = Path.ChangeExtension(inputPath, ext);
        File.WriteAllBytes(outputPath, data);
    }
    Console.WriteLine($"Write \"{outputPath}\".");
}
if (args.Length == 0)
{
    Console.WriteLine("Usage: Deobfuscate xxx.dat ...");
}
