using System.Text;
using TacticsogreExtracts;
using Entry = System.Collections.Generic.Dictionary<string, object>;

var getDataPath = (string path) => Path.Combine(args[0], "Data", path);
var deserializer = new YamlDotNet.Serialization.Deserializer();
var texts = deserializer.Deserialize<dynamic>(File.ReadAllText(@"..\texts.yaml"));
var langIndex = 2;
var equipmentTypeNameIds = new Dictionary<byte, string>
{
    [1] = "WPNCAT01",
    [2] = "WPNCAT02",
    [3] = "WPNCAT03",
    [4] = "WPNCAT04",
    [5] = "WPNCAT05",
    [7] = "WPNCAT07",
    [8] = "WPNCAT08",
    [10] = "WPNCAT10",
    [11] = "WPNCAT11",
    [12] = "WPNCAT12",
    [14] = "WPNCAT14",
    [15] = "WPNCAT20",
    [16] = "WPNCAT21",
    [17] = "WPNCAT15",
    [18] = "WPNCAT16",
    [19] = "WPNCAT17",
    [20] = "WPNCAT18",
    [22] = "ARMCAT01",
};

{
    var battleData = Util.LoadPakd(getDataPath(@"battle\battle_data_release.pack"));
    var xlces = new Dictionary<int, string>();
    foreach (var (id, data) in battleData)
    {
        if (BitConverter.ToInt32(data, 0) != 0x65636c78/*xlce*/) continue;
        var count = BitConverter.ToInt32(data, 4);
        var entrySize = BitConverter.ToInt32(data, 12);
        xlces.Add(id, $"{count},{entrySize}");
    }
    File.WriteAllText(@"..\_battleXlces.yaml", Util.YamlSerializer.Serialize(xlces));
}

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
                for (var deadOrEscaped = 0; deadOrEscaped <= 1; deadOrEscaped++)
                {
                    var rawDrops = new List<(int, int, int)>();
                    if (deadOrEscaped == 0)
                    {
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
                    }
                    else
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            var dropData = entry.Slice(178 + i * 4, 4);
                            rawDrops.Add((BitConverter.ToUInt16(dropData), dropData[2], dropData[3]));
                        }
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
                            itemName += $"（{texts["COMMAND_LC_COMMAND"]["NAME"][equipmentTypeNameIds[equipmentSkillIds[itemId]]]["NAME"][langIndex]}）";
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
                        if (deadOrEscaped == 1)
                        {
                            battlestageUnits.Last().Add("shiftstone", true);
                        }
                    }
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

{
    var skillChances = new HashSet<string>();
    var skillLevels = new char[] { 'Ⅰ', 'Ⅱ', 'Ⅲ', 'Ⅳ' };
    var data = Util.LoadPakd(getDataPath(@"battle\battle_data_release.pack"), id => id == 25)[25];
    var index = -1;
    foreach (var entry_ in Util.GetXlceEntries(data))
    {
        index++;
        var entry = entry_.Span;
        var type = entry[2];
        if (type != 5) continue;
        var chance = entry[15];
        var nameId = BitConverter.ToUInt16(entry[90..]);
        string name = texts["SKILLTEXT_LC_SKILL"][nameId.ToString("d3")]["NAME"][langIndex];
        name = name.TrimEnd(skillLevels);
        skillChances.Add($"{name}|{chance}%");
    }
    File.WriteAllLines(@"..\skill-chances.csv", skillChances);
}

{
    var battleData = Util.LoadPakd(getDataPath(@"battle\battle_data_release.pack"));
    var attackTypes = Util.GetXlceEntries(battleData[19]).ToArray();

    var statNames = new string[] { "STR", "VIT", "DEX", "INT", "MND", "RES" };
    string printStatFactors(Span<byte> attackEntry)
    {
        var statFactors = new List<string>();
        for (int i = 0; i < statNames.Length; i++)
        {
            var statFactor = BitConverter.ToInt16(attackEntry[(12 + i * 2)..]);
            if (statFactor != 0)
            {
                statFactors.Add($"{statFactor / 100.0:f1}{statNames[i].ToLowerInvariant()}");
            }
        }
        return string.Join("+", statFactors);
    }
    string printSelfToActionElementFactors(Span<byte> attackEntry)
    {
        var same = BitConverter.ToInt16(attackEntry[4..]);
        var other = BitConverter.ToInt16(attackEntry[6..]);
        var conflict = BitConverter.ToInt16(attackEntry[8..]);
        if (other != 100) throw new InvalidDataException();
        return $"{same / 100.0:f1},{conflict / 100.0:f1}";
    }
    string printSelfToTargetElementFactors(Span<byte> attackEntrySlice)
    {
        var effective = BitConverter.ToInt16(attackEntrySlice[236..]);
        var normal = BitConverter.ToInt16(attackEntrySlice[238..]);
        var uneffective = BitConverter.ToInt16(attackEntrySlice[240..]);
        if (normal != 0) throw new InvalidDataException();
        return $"{effective / 100}%,{uneffective / 100}%";
    }

    var weaponTypeAttackType = new Dictionary<(byte, bool), byte>();
    foreach (var equipement_ in Util.GetXlceEntries(battleData[11]))
    {
        var equipmentEntry = equipement_.Span;
        var equipmentType = equipmentEntry[2];
        var isTwoHand = equipmentEntry[13] == 1;
        var attackTypeId = equipmentEntry[18];
        if (attackTypeId == 0 || equipmentType == 30 || equipmentType == 31) continue;
        weaponTypeAttackType.TryAdd((equipmentType, isTwoHand), attackTypeId);
    }
    var attackTypeWeapons = new Dictionary<byte, HashSet<string>>();
    foreach (var ((weaponType, isTwoHand), attackTypeId) in weaponTypeAttackType)
    {
        string weaponTypeName = texts["COMMAND_LC_COMMAND"]["NAME"][equipmentTypeNameIds[weaponType]]["NAME"][langIndex];
        if (weaponTypeAttackType.TryGetValue((weaponType, !isTwoHand), out var anotherAttackTypeId)
            && attackTypeId != anotherAttackTypeId)
        {
            weaponTypeName += $"（{(isTwoHand ? "双手" : "单手")}）";
        }
        attackTypeWeapons.TryAdd(attackTypeId, new());
        attackTypeWeapons[attackTypeId].Add(weaponTypeName);
    }
    foreach (var equipement_ in Util.GetXlceEntries(battleData[11]))
    {
        var equipmentEntry = equipement_.Span;
        var equipmentType = equipmentEntry[2];
        var isTwoHand = equipmentEntry[13] == 1;
        var attackTypeId = equipmentEntry[18];
        if (equipmentType == 30 || equipmentType == 31) continue;
        if (!attackTypeWeapons.ContainsKey(attackTypeId)) continue;
        if (weaponTypeAttackType[(equipmentType, isTwoHand)] == attackTypeId) continue;

        var nameId = BitConverter.ToUInt16(equipmentEntry[134..]);
        string name;
        try { name = texts["ARMSTEXT_LC_ARMS"][nameId.ToString("d3")]["NAME"][langIndex]; } catch { continue; }
        if ((610 <= nameId && nameId <= 626))  // cursed weapon
        {
            name += $"（{texts["COMMAND_LC_COMMAND"]["NAME"][equipmentTypeNameIds[equipmentType]]["NAME"][langIndex]}）";
        }
        attackTypeWeapons[attackTypeId].Add(name);
    }
    var weaponFactors = new List<Entry>();
    foreach (var (attackTypeId, weapons) in attackTypeWeapons)
    {
        var offenseEntry = attackTypes[attackTypeId].Span;
        var defenseEntry = attackTypes[attackTypeId + 1].Span;
        weaponFactors.Add(new()
        {
            ["weapons"] = weapons,
            ["offenseEquipmentAtkFactor"] = $"{BitConverter.ToInt16(offenseEntry[0..]) / 100.0:f1}",
            ["defenseEquipmentDefFactor"] = $"{BitConverter.ToInt16(defenseEntry[2..]) / 100.0:f1}",
            ["offenseStatFactor"] = printStatFactors(offenseEntry),
            ["defenseStatFactor"] = printStatFactors(defenseEntry),
            ["elementSelfToActionFactor"] = printSelfToActionElementFactors(offenseEntry),
            ["elementSelfToTatgetFactor"] = printSelfToTargetElementFactors(offenseEntry),
        });
    }
    weaponFactors.Insert(0, weaponFactors[2]);
    weaponFactors.RemoveAt(3);
    File.WriteAllText(@"..\damage-factors.weapon.yaml", Util.YamlSerializer.Serialize(weaponFactors));

    var magicGroupMagics = new Dictionary<(byte, ushort, byte), List<string>>();
    foreach (var action_ in Util.GetXlceEntries(battleData[16]))
    {
        var actionEntry = action_.Span;
        var actionCategory = actionEntry[2];
        if (actionCategory > 12) break;  // magics only
        var mpCost = actionEntry[8];
        for (var i = 0; i < 3; i++)
        {
            var actionSub = actionEntry[(42 + i * 26)..];
            if (!(actionSub[0] == 1 && actionSub[8] == 3)) continue;

            var attackTypeId = actionSub[9];
            if (attackTypeId == 0) continue;
            var actionAtk = BitConverter.ToUInt16(actionSub[10..]);
            //var actionElementBonus = BitConverter.ToUInt16(actionSub[24..]);  // always 0
            var nameId = BitConverter.ToUInt16(actionEntry[148..]);
            string name = texts["EFFECTTEXT_LC_EFFECT"][nameId.ToString("d3")]["NAME"][langIndex];
            var key = (attackTypeId, actionAtk, mpCost);
            magicGroupMagics.TryAdd(key, new());
            magicGroupMagics[key].Add(name);

            break;
        }
    }
    var magicGrouoNames = new Dictionary<(byte, ushort, byte), string>()
    {
        [(21, 25, 15)] = "投射Ⅰ", [(21, 60, 25)] = "投射Ⅱ", [(21, 90, 35)] = "投射Ⅲ", [(21, 120, 45)] = "投射Ⅳ",
        [(23, 40, 25)] = "放射Ⅰ", [(23, 70, 45)] = "放射Ⅱ", [(23, 55, 50)] = "放射Ⅲ", [(23, 95, 70)] = "放射Ⅳ",
        [(69, 100, 45)] = "召唤Ⅰ", [(69, 100, 70)] = "召唤Ⅱ",
        [(23, 95, 60)] = "龙语Ⅰ", [(23, 120, 80)] = "龙语Ⅱ",
        [(69, 70, 35)] = "忍术Ⅰ", [(69, 70, 50)] = "忍术Ⅱ",
    };
    var magicFactors = new List<Entry>();
    foreach (var (key, magics) in magicGroupMagics)
    {
        var (attackTypeId, actionAtk, _) = key;
        var offenseEntry = attackTypes[attackTypeId].Span;
        var defenseEntry = attackTypes[attackTypeId + 1].Span;
        magicGrouoNames.TryGetValue(key, out var group);
        magicFactors.Add(new()
        {
            ["magics"] = magics,
            ["group"] = group ?? "",
            ["actionAtk"] = actionAtk * BitConverter.ToInt16(offenseEntry[10..]) / 100,
            ["offenseEquipmentAtkFactor"] = $"{BitConverter.ToInt16(offenseEntry[0..]) / 100.0:f1}",  // always 0
            ["defenseEquipmentDefFactor"] = $"{BitConverter.ToInt16(defenseEntry[2..]) / 100.0:f1}",  // always 0.5
            ["offenseStatFactor"] = printStatFactors(offenseEntry),
            ["defenseStatFactor"] = printStatFactors(defenseEntry), // always 0.7mnd+1.0res
            ["defenseStepFactor"] = BitConverter.ToInt16(defenseEntry[70..]) / 100.0,
            ["defenseInverseStepFactor"] = BitConverter.ToInt16(defenseEntry[72..]) / 100.0,
            ["elementSelfToActionFactor"] = printSelfToActionElementFactors(offenseEntry),
            ["elementSelfToTatgetFactor"] = printSelfToTargetElementFactors(offenseEntry),
        });

    }
    File.WriteAllText(@"..\damage-factors.magic.yaml", Util.YamlSerializer.Serialize(magicFactors));

    var finishingFactors = new List<Entry>();
    var finishingDedup = new HashSet<object>();
    foreach (var action_ in Util.GetXlceEntries(battleData[16]))
    {
        var actionEntry = action_.Span;
        var actionCategory = actionEntry[2];
        if (actionCategory < 13) continue;  // exclude magics

        for (var i = 0; i < 3; i++)
        {
            var actionSub = actionEntry[(42 + i * 26)..];
            if (!(actionSub[0] == 1 && actionSub[8] == 3)) continue;

            var attackTypeId = actionSub[9];
            if (attackTypeId == 0) continue;
            var actionAtk = BitConverter.ToUInt16(actionSub[10..]);
            var actionElementId = actionSub[19];
            var actionElementBonus = BitConverter.ToUInt16(actionSub[24..]);
            var nameId = BitConverter.ToUInt16(actionEntry[148..]);
            string name = texts["EFFECTTEXT_LC_EFFECT"][nameId.ToString("d3")]["NAME"][langIndex];

            var dedupKey = (nameId, attackTypeId, actionAtk, actionElementBonus);
            if (finishingDedup.Contains(dedupKey)) continue;
            finishingDedup.Add(dedupKey);

            var offenseEntry = attackTypes[attackTypeId].Span;
            var defenseEntry = attackTypes[attackTypeId + 1].Span;
            finishingFactors.Add(new()
            {
                ["name"] = name,
                ["actionCategory"] = actionCategory,
                ["actionAtk"] = actionAtk * BitConverter.ToInt16(offenseEntry[10..]) / 100,
                ["actionElement"] = actionElementBonus * BitConverter.ToInt16(offenseEntry[(202 + actionElementId * 2)..]) / 100,
                ["offenseEquipmentAtkFactor"] = $"{BitConverter.ToInt16(offenseEntry[0..]) / 100.0:f1}",
                ["defenseEquipmentDefFactor"] = $"{BitConverter.ToInt16(defenseEntry[2..]) / 100.0:f1}",
                ["offenseStatFactor"] = printStatFactors(offenseEntry),
                ["defenseStatFactor"] = printStatFactors(defenseEntry),
                ["elementSelfToActionFactor"] = printSelfToActionElementFactors(offenseEntry),
                ["elementSelfToTatgetFactor"] = printSelfToTargetElementFactors(offenseEntry),
            });
            break;
        }
    }
    var finishingFactors_ = new List<Entry>();
    var otherFactors = new List<Entry>();
    foreach (var entry in finishingFactors)
    {
        var category = (byte)entry["actionCategory"];
        (22 <= category && category <= 43 ? finishingFactors_ : otherFactors).Add(entry);
    }
    File.WriteAllText(@"..\damage-factors.finishing.yaml", Util.YamlSerializer.Serialize(finishingFactors_));
    File.WriteAllText(@"..\damage-factors.other.yaml", Util.YamlSerializer.Serialize(otherFactors));
}
