// extract intermediate data for other projects

using System.Text;
using TacticsogreExtracts;

var getDataPath = (string path) => Path.Combine(args[0], "Data", path);


var sceneNames = new SortedDictionary<int, string>();
foreach (var file in Directory.GetFiles(getDataPath(@"event\scene"), "*.pack"))
{
    var pakd = Util.LoadPakd(file, id => id >> 15 == 0);
    foreach (var (id, script) in pakd)
    {
        var scriptNameOffset = script[0x0c];
        var scriptNameLength = 1;
        while (script[scriptNameOffset + scriptNameLength] != '.') scriptNameLength++;
        var sceneName = Encoding.ASCII.GetString(script.AsSpan(scriptNameOffset, scriptNameLength));
        sceneNames[id] = sceneName;

    }
}
File.WriteAllText(@"..\_sceneNames.yaml", Util.YamlSerializer.Serialize(sceneNames));


var unitNameIds = new SortedDictionary<ushort, ushort>();
foreach (var file in Directory.GetFiles(getDataPath(@"battle\entry"), "*.pack"))
{
    if (file[^8] == 'k') continue;
    var data = Util.LoadPakd(file, id => id == 0)[0];
    foreach (var entry_ in Util.GetXlceEntries(data, 0xc4))
    {
        var entry = entry_.Span;
        var nameId = BitConverter.ToUInt16(entry[0..]);
        var unitId = BitConverter.ToUInt16(entry[2..]);
        if (nameId == 0 || nameId == unitId) continue;
        if (unitId == 1) continue;
        if (!unitNameIds.TryGetValue(unitId, out var prev))
        {
            unitNameIds.Add(unitId, nameId);
        }
        else
        {
            // only Punkin conflicts
            if (nameId != prev) Console.WriteLine($"{nameId} {prev}");
        }
    }
}
File.WriteAllText(@"..\_unitNameIds.yaml", Util.YamlSerializer.Serialize(unitNameIds));


var strongpointMiniNameIds = new SortedDictionary<ushort, ushort>();
foreach (var data in Util.LoadPakd(getDataPath(@"worldmap\worldmap_data.pack"), id => id % 2 == 1).Values)
{
    foreach (var entry_ in Util.GetXlceEntries(data, 0x10))
    {
        var entry = entry_.Span;
        var minimapId = BitConverter.ToUInt16(entry[2..]);
        var nameId = BitConverter.ToUInt16(entry[4..]);
        if (nameId == 0) continue;
        if (!strongpointMiniNameIds.TryGetValue(minimapId, out var prev))
        {
            strongpointMiniNameIds.Add(minimapId, nameId);
        }
        else
        {
            if (nameId != prev) throw new InvalidDataException();
        }
    }
}
File.WriteAllText(@"..\_strongpointMiniNameIds.yaml", Util.YamlSerializer.Serialize(strongpointMiniNameIds));
