using TacticsogreExtracts;

var getDataPath = (string path) => Path.Combine(args[0], "Data", path);
var deserializer = new YamlDotNet.Serialization.Deserializer();
var texts = deserializer.Deserialize<dynamic>(File.ReadAllText(@"..\texts.yaml"));
var langIndex = 2;

{
    var classGrowths = new List<string>()
    { "id| HP |MP |STR|VIT|DEX|AGI|AVD|INT|MND|RES|random" };
    var data = Util.LoadPakd(getDataPath(@"battle\battle_data_release.pack"), id => id == 14)[14];
    var index = -1;
    foreach (var entry_ in Util.GetXlceEntries(data))
    {
        if (index++ == 90) break;
        var entry = entry_.Span;
        var growth = new int[11];
        var random = BitConverter.ToUInt16(entry[0x3e..]);
        if (random == 0) continue;
        if (random % 2 == 1) random--;  // int div 2 before random scaling (0x14032F6CE)
        random--;  // int trunc after random scaling (0x14032F78A)
        growth[10] = random;
        for (var i = 0; i < 10; i++)
        {
            growth[i] = BitConverter.ToUInt16(entry[(0x04 + i * 0x06)..]);
        }
        var text = texts["CLASS_TEXT_DATA"][$"CLASS{index:d4}"];
        string name = text["FEMALE"][langIndex] == text["MALE"][langIndex]
            ? text["MALE"][langIndex]
            : $"{text["MALE"][langIndex]} / {text["FEMALE"][langIndex]}";
        var print = growth.Select(v => $"{v / 10.0:f1}").Prepend($"{index:x2}").Append(name);
        classGrowths.Add(string.Join('|', print));
    }
    File.WriteAllLines(@"..\class-growths.csv", classGrowths);
}

{
    var raceGrowths = new List<string>();
    var data = Util.LoadPakd(getDataPath(@"battle\battle_data_release.pack"), id => id == 23)[23];
    var index = -1;
    foreach (var entry_ in Util.GetXlceEntries(data))
    {
        index++;
        var entry = entry_.Span;
        var growth = new int[10];
        growth[0] = entry[0x1c];
        growth[1] = entry[0x1c];
        for (var i = 2; i < 10; i++)
        {
            growth[i] = entry[0x1f + (i - 2) * 0x02];
        }
        var print = growth.Select(v => (v / 10.0).ToString("f1")).Prepend(index.ToString("x3"));
        raceGrowths.Add(string.Join('|', print));
    }
    // every == "0.2|0.2|0.1|0.1|0.1|0.1|0.1|0.1|0.1|0.1" (or empty)
    // File.WriteAllLines(@"..\race-growths.csv", raceGrowths);
}
