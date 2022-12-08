using System.Text;
using TacticsogreExtracts;
using Entry = System.Collections.Generic.Dictionary<string, object>;

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
        var nameId = BitConverter.ToUInt16(entry[0xb2..]);
        if (nameId != index) throw new InvalidDataException();
        var text = texts["CLASS_TEXT_DATA"][$"CLASS{nameId:d4}"];
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

{
    var ignoredItems = new HashSet<int>();
    for (int i = 537; i <= 583; i++) ignoredItems.Add(i);  // craftable rings
    for (int i = 1145; i <= 1149; i++) ignoredItems.Add(i);  // oberyth coin
    for (int i = 1012; i <= 1027; i++) ignoredItems.Add(i);  // magic leaf etc.
    for (int i = 1621; i <= 1625; i++) ignoredItems.Add(i);  // experience charm
    var shopItemData = Util.LoadPakd(getDataPath(@"menu\menu_data.pack"), id => id == 26)[26];
    foreach (var entry_ in Util.GetXlceEntries(shopItemData))
    {
        var entry = entry_.Span;
        var itemId = BitConverter.ToUInt16(entry[0..]);
        var requiredLevel = entry[19];
        if (requiredLevel != 0) ignoredItems.Add(itemId);
    }

    var battlestages = deserializer.Deserialize<List<Dictionary<string, string>>>(File.ReadAllText(@"..\_battlestages.yaml"));

    var battleData = Util.LoadPakd(getDataPath(@"battle\battle_data_release.pack"));
    var battlestageEntries = Util.GetXlceEntries(battleData[17]).ToArray();
    var equipmentNameIds = Util.GetXlceEntries(battleData[11]).Select(e => BitConverter.ToUInt16(e.Span[0x86..])).ToArray();
    var equipmentSkillIds = Util.GetXlceEntries(battleData[11]).Select(e => e.Span[2]).ToArray();
    var classjobNameIds = Util.GetXlceEntries(battleData[14]).Select(e => BitConverter.ToUInt16(e.Span[0xb2..])).ToArray();
    var races = Util.GetXlceEntries(battleData[23]).ToArray();

    var entryunitFilepaths = new List<string>();
    var filetable = File.ReadAllBytes(getDataPath("FileTable.bin"));
    for (var i = 0; i < 536; i++)  // too lazy to parse structurally
    {
        var fileInfoOffset = BitConverter.ToUInt32(filetable, 0x00057A08 + i * 4);
        var offset = 0x00058384 + fileInfoOffset * 8;
        var length = 0;
        while (filetable[offset + length] != 0) length++;
        var filepath = Encoding.ASCII.GetString(filetable, (int)offset, length - 3) + "pack";
        if (!filepath.StartsWith("battle/")) throw new InvalidDataException();
        entryunitFilepaths.Add(filepath);
    }

    var battlestageDrops = new List<Entry>();
    var previousBattlestageDrops = "";
    foreach (var battlestage in battlestages)
    {
        var strongpoint = battlestage["strongpoint"];
        var battlestageId = int.Parse(battlestage["battlestageId"]);
        var battlestageEntry = battlestageEntries[battlestageId].Span;
        var entryunitIdNormal = BitConverter.ToUInt16(battlestageEntry[40..]);
        var entryunitIdSpecial = BitConverter.ToUInt16(battlestageEntry[218..]);

        void parseEntryunit(ushort entryunitId)
        {
            if (entryunitId == 0) return;
            var entryunitFilepath = entryunitFilepaths[entryunitId];
            var entryunitData = Util.LoadPakd(getDataPath(entryunitFilepath), id => id == 0)[0];
            var battlestageUnits = new List<Entry>();
            foreach (var entry_ in Util.GetXlceEntries(entryunitData))
            {
                var entry = entry_.Span;
                var rawDrops = new List<(int, int, int)>();
                for (var i = 0; i < 7; i++)
                {
                    var dropData = entry.Slice(74 + i * 4, 4);
                    if (dropData[2] == 0) continue;
                    rawDrops.Add((BitConverter.ToUInt16(dropData), 1, dropData[2]));
                }
                for (var i = 0; i < 4; i++)
                {
                    var dropData = entry.Slice(118 + i * 4, 4);
                    rawDrops.Add((BitConverter.ToUInt16(dropData), dropData[2], dropData[3]));
                }

                var unitDrops = new List<Entry>();
                foreach (var (itemId, amount, rate) in rawDrops)
                {
                    if (itemId == 0) continue;
                    if (ignoredItems.Contains(itemId)) continue;
                    string itemName = itemId < 1000
                        ? texts["ARMSTEXT_LC_ARMS"][equipmentNameIds[itemId].ToString("d3")]["NAME"][langIndex]
                        : texts["COMMODITYTEXT_LC_COMMODITY"][(itemId - 1000).ToString("d3")]["NAME"][langIndex];
                    if ((610 <= itemId && itemId <= 626) || itemName == "迦楼罗")  // cursed weapon and a translation glitch
                    {
                        itemName += $"（{texts["SKILLTEXT_LC_SKILL"][equipmentSkillIds[itemId].ToString("d3")]["NAME"][langIndex]}）"
                            .Replace("射击", "枪械");
                    }
                    var dropRate = (int)Math.Round(rate / 255.0 * 100);
                    unitDrops.Add(new()
                    {
                        //["itemId"] = itemId,
                        ["item"] = itemName,
                        ["amount"] = amount,
                        ["rate"] = $"{dropRate}%",
                    });
                }

                if (unitDrops.Count > 0)
                {
                    var nameId = BitConverter.ToUInt16(entry[0..]);
                    if (nameId == 248) continue;  // CODA1 Sirene, not actually killed, won't drop
                    var race = races[BitConverter.ToUInt16(entry[2..])].Span;
                    var gender = race[3] == 0 ? "MALE" : "FEMALE";
                    var raceName = "";
                    var raceType = race[57];
                    if (raceType != 0)
                    {
                        if (raceType == 1 && race[3] == 0) raceType = 0;
                        raceName = texts["COMMAND_LC_COMMAND"]["NAME"][$"RACE{raceType:d2}"]["NAME"][langIndex];
                    }
                    string name = nameId != 0 ? texts["UNITNAME0_LC_UNITNAME"][nameId.ToString("d4")]["TEXT"][langIndex] : "";
                    string classjobName = texts["CLASS_TEXT_DATA"][$"CLASS{classjobNameIds[entry[6]]:d4}"][gender][langIndex];
                    var position = $"{entry[10]},{entry[11]}";
                    battlestageUnits.Add(new()
                    {
                        ["name"] = name,
                        ["race"] = raceName,
                        ["class"] = classjobName,
                        ["position"] = position,
                        ["drops"] = unitDrops,
                    });
                }
            }
            if (battlestageUnits.Count > 0)
            {
                var battlestageUnitsString = Util.YamlSerializer.Serialize(battlestageUnits);  // FIXME: more efficient way?
                if (battlestageUnitsString != previousBattlestageDrops)
                {
                    battlestageDrops.Add(new()
                    {
                        ["stage"] = battlestage["stage"],
                        ["strongpoint"] = strongpoint,
                        //["entryunit"] = entryunitFilepath,
                        ["enemies"] = battlestageUnits,
                    });
                }
                previousBattlestageDrops = battlestageUnitsString;
            }    
        }
        parseEntryunit(entryunitIdNormal);
        strongpoint += langIndex == 2 ? "（特殊）" : " (Special)";
        parseEntryunit(entryunitIdSpecial);
    }
    File.WriteAllText(@"..\drops.yaml", Util.YamlSerializer.Serialize(battlestageDrops));
}
