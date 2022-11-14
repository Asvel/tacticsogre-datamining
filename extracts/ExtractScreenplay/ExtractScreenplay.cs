using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TacticsogreExtracts;
using Entry = System.Collections.Generic.Dictionary<string, object>;

var getDataPath = (string path) => Path.Combine(args[0], "Data", path);

var _gameTextPrinter = new GameTextPrinter(2);
string interpolateGameTexts(FormattableString s) => s.ToString(_gameTextPrinter);

var serializer = new YamlDotNet.Serialization.Serializer();
var deserializer = new YamlDotNet.Serialization.Deserializer();

var chapterNames = new string[] { "C1", "C2a", "C2b", "C3a", "C3b", "C3c", "C4", "EP", "DLC" };

var sceneNames = deserializer.Deserialize<Dictionary<ushort, string>>(File.ReadAllText(@"..\_sceneNames.yaml"));
string formatSceneName(ushort sceneId) =>
    $"{sceneId} <{(sceneNames.TryGetValue(sceneId, out var sceneName) ? sceneName : "")}>";

string formatGlobalFlag(ushort flagId)
{
    return $"GF{flagId:x4}" + interpolateGameTexts(flagId switch
    {
        0x0000 => $"",

        >= 0x0001 and <= 0x0031 => $"_strongpoint_{flagId - 0x0000}",
        >= 0x0033 and <= 0x0063 => $"_occupation_{flagId - 0x0032}",
        >= 0x0065 and <= 0x009f => $"_route_{flagId - 0x0064}",
        >= 0x00a1 and <= 0x00bf => $"_minimap_{flagId - 0x00a0}",
        >= 0x00c9 and <= 0x00fb => $"_strongpoint_mini_{flagId - 0x00c8}",
        >= 0x0105 and <= 0x0137 => $"_occupation_mini_{flagId - 0x0104}",

        >= 0x01d7 and <= 0x0208 => $"_playable_chr_{flagId - 0x01d6:ch}_unit_status",  // 1=guest 2=joined
        >= 0x0209 and <= 0x023a => $"_playable_chr_{flagId - 0x0208:ch}_story_status",  // 0=joined 3=dead 4=left
        >= 0x023b and <= 0x02da => $"_WR_chr_{flagId - 0x023a}",  // 1-15, 15=dead
        >= 0x02db and <= 0x03ac => $"_WR_ca_{flagId - 0x02da}",
        >= 0x03ad and <= 0x046a => $"_WR_rumor_{flagId - 0x03ac:wr}",

        0x046b => $"_shop_level",
        0x047d => $"_deneb_shop_unlocked",

        0x047e => $"_chapter_1_progress",
        0x047f => $"_chapter_2a_progress",
        0x0480 => $"_chapter_2b_progress",
        0x0481 => $"_chapter_3a_progress",
        0x0482 => $"_chapter_3b_progress",
        0x0483 => $"_chapter_3c_progress",
        0x0485 => $"_chapter_4_progress",

        0x0486 => $"_chapter_1_cleared",
        0x0487 => $"_chapter_2a_cleared",
        0x0488 => $"_chapter_2b_cleared",
        0x048a => $"_chapter_3a_cleared",
        0x048b => $"_chapter_3b_cleared",
        0x048c => $"_chapter_3c_cleared",
        0x048f => $"_chapter_4_cleared",

        0x0493 => $"_WR_guide_xxx",
        0x0494 => $"_WR_guide_xxx",
        0x0496 => $"_WR_guide_xxx",

        0x049a => $"_chapter_ep_cleared",

        0x0528 => $"_battlestage_142_c1_{61:ch}_sidequest",

        >= 0x049b and <= 0x0549 => $"_battlestage_{flagId - 0x049a:d3}",
        >= 0x054a and <= 0x055d => $"_battlestage_xxx",

        0x056a => $"_storyline_c1_47_{11:ch}_dead",
        0x0582 => $"_storyline_c2a_41_{17:ch}_dead",
        0x0583 => $"_storyline_c2a_42_{17:ch}_dead",
        0x0584 => $"_storyline_c2a_43_{18:ch}_dead",
        0x0585 => $"_storyline_c2a_46_all_alive_and_rescue_{11:ch}",
        0x0586 => $"_storyline_c2a_47_all_alive_and_giveup_{11:ch}",
        0x0587 => $"_storyline_c2a_48_{18:ch}_dead",
        0x0588 => $"_storyline_c2a_50_{17:ch}_dead_and_rescue_{11:ch}",
        0x0589 => $"_storyline_c2a_51_{17:ch}_dead_and_giveup_{11:ch}",
        0x058e => $"_storyline_c2a_58_{17:ch}_dead",
        0x058f => $"_storyline_c2a_59_{18:ch}_dead",

        0x05c1 => $"_storyline_c3a_15_{9:ch}_dead",

        0x0609 => $"_storyline_c4_52_{7:sp}_select_1",
        0x060a => $"_storyline_c4_52_{7:sp}_select_2",
        0x061f => $"_storyline_c4_108_{3:ch}_killed",
        0x0622 => $"_storyline_c4_112_{3:ch}_dead",
        0x0623 => $"_storyline_c4_113_{3:ch}_alive",

        >= 0x0564 and <= 0x0574 => $"_storyline_chapter_1_{flagId - 0x0563}",
        >= 0x0575 and <= 0x05a9 => $"_storyline_chapter_2a_{flagId - 0x0574}",
        >= 0x05aa and <= 0x05bd => $"_storyline_chapter_2b_{flagId - 0x05a9}",
        >= 0x05bf and <= 0x05dc => $"_storyline_chapter_3a_{flagId - 0x05be}",
        >= 0x05dd and <= 0x05ed => $"_storyline_chapter_3b_{flagId - 0x05dc}",
        >= 0x05ee and <= 0x0607 => $"_storyline_chapter_3c_{flagId - 0x05ed}",
        >= 0x0608 and <= 0x0662 => $"_storyline_chapter_4_{flagId - 0x0607}",

        >= 0x0663 and <= 0x0691 => $"_WR_select_{flagId - 0x0662:d2}",

        0x06a3 => $"_chapter_now",

        >= 0x06a6 and <= 0x0709 => $"_appellation_{flagId - 0x6a5}",

        0x0718 => $"_WR_rumor_{151:wr}_viewed",
        0x0719 => $"_WR_rumor_{176:wr}_viewed",
        0x071a => $"_WR_rumor_{174:wr}_viewed",
        0x071b => $"_WR_rumor_{178:wr}_viewed",
        0x071c => $"_WR_rumor_{179:wr}_viewed",
        0x071d => $"_WR_rumor_{180:wr}_viewed",
        0x071e => $"_WR_rumor_{182:wr}_viewed",
        0x071f => $"_WR_rumor_{185:wr}_viewed",
        0x0720 => $"_WR_rumor_{186:wr}_viewed",
        0x0721 => $"_WR_rumor_{ 68:wr}_viewed",
        0x073e => $"_WR_rumor_{175:wr}_viewed",
        0x073f => $"_WR_rumor_{187:wr}_viewed",
        0x0740 => $"_WR_rumor_{188:wr}_viewed",
        0x0741 => $"_WR_rumor_{189:wr}_viewed",
        0x0742 => $"_WR_rumor_{190:wr}_viewed",
        0x076f => $"_WR_rumor_{167:wr}_viewed",
        0x0772 => $"_WR_rumor_{171:wr}_viewed",
        0x0774 => $"_WR_rumor_{177:wr}_viewed",
        0x0783 => $"_WR_rumor_{ 15:wr}_viewed",
        0x07c5 => $"_WR_rumor_{139:wr}_viewed",
        0x07c7 => $"_WR_rumor_{140:wr}_viewed",
        0x082b => $"_WR_rumor_{156:wr}_viewed",

        >= 0x0743 and <= 0x0751 => $"_dlc_{flagId - 0x0743}_installed",  // FUN_088c82f4

        0x075f => $"_storyline_chapter_cc_ep2_finished",

        0x076a => $"_chr_{11:ch}_dead",
        0x076b => $"_chr_{17:ch}_dead",
        0x076c => $"_chr_{18:ch}_dead",

        >= 0x0776 and <= 0x077e => $"_deneb_orb_bought_amount",
        0x077f => $"_deneb_pumpkin_sold_amount",

        0x0782 => $"_enemylevel",
        0x07b4 => $"_unitlist_syscom_disabled",
        0x07c4 => $"_CHARIOT_enabled",
        0x07c9 => $"_battle_ai_enabled",
        //0x07e0 => $"_appellation_100+",  // FUN_088c96ec
        0x0826 => $"_shop_compound_disabled",
        0x082a => $"_enemylevel_fix",
        0x083a => $"_WORLD_enabled",

        0x0839 => $"_all_apocrypha_temple_cleared",
        0x0846 => $"_deneb_wicce_unlocked",
        0x0845 => $"_{110:ch}_defeated",

        0x0882 => $"_union_level",
        0x0884 => $"_CHARIOT_turn_limit",

        _ => $"",
    });
}

string formatLocalFlag(uint flagId)
{
    return $"LF{flagId:x4}" + interpolateGameTexts(flagId switch
    {
        >= 0x0001 and <= 0x001e => $"_scenario_{flagId - 0x0000}",
        >= 0x001f and <= 0x0050 => $"_sally_unit_{flagId - 0x001e:ch}",  // FUN_088c8758
        _ => $"",
    });
}

var invkGroups = new string[]
{
    "call_by_next_task",
    "AT_start",
    "Act_end",
    "incoming_strongpoint",
    "outgoing_strongpoint",
    "AT_end",
    "leaving_shop",
    "leaving_warren_report",
    "unknown08",
    "unknown09",
    "unknown0a",
    "unknown0b",
    "unknown0c",
    "unknown0d",
    "unit_died",
    "call_by_warren_report",
};

string formatInstruction(short type, BinaryReader reader)
{
    byte r8() => reader.ReadByte();
    sbyte r8s() => reader.ReadSByte();
    ushort r16() => reader.ReadUInt16();
    static FormattableString _(string s, params object?[] arg) => FormattableStringFactory.Create(s, arg);
    //var positionBeforeParameter = reader.BaseStream.Position;
    var ret = interpolateGameTexts(type switch
    {
        0x00 => $"Noop",
        0x01 => $"{formatGlobalFlag(r16())} = {r8()}",
        0x02 => $"{formatLocalFlag(r16())} = {r8()}",
        0x03 => $"Clan[{r8():cl}]Approval {r8s():+-}",
        0x04 => $"MoveToStrongpoint({r8():sp})",
        0x05 => $"Goth {r16() | (uint)r16() << 0x10:+-}",
        0x06 => $"Date += {r8()}",
        0x07 => $"SallyUnits[L]Loyalty {r8s():+-}",
        0x08 => $"SallyUnits[C]Loyalty {r8s():+-}",
        0x09 => $"SallyUnits[N]Loyalty {r8s():+-}",
        0x0a => $"AllUnits[L]Loyalty {r8s():+-}",
        0x0b => $"AllUnits[C]Loyalty {r8s():+-}",
        0x0c => $"AllUnits[N]Loyalty {r8s():+-}",
        0x0d => $"ProtagonistPersonality = {r8():pe}",
        0x0e => $"ResetGlobalFlagGroup({r8()})",
        0x0f => $"Unit[{r16():ch}]Clan = {r8():cl}",
        0x10 => $"Clan[{r8():cl}]UnitsLoyalty {r8s():+-}",
        0x11 => $"Unit[{r16():ch}]Loyalty = {r8()}",
        0x12 => $"MoveToStrongpointMini({r16():sm})",
        0x13 => $"ProtagonistClan = {r8():cl}",
        0x14 => $"ObtainItem({r16()})",
        0x15 => $"BattleUnit[{r16():ch}]ActionStrategy = {r16()}",
        0x16 => $"SuppressUnitRegularDeathScene({r16():ch})",
        0x17 => $"ChangeClearConditionMessage({r8()})",
        0x18 => $"Noop",
        0x19 => $"ChangeClearConditionUnit({r8():ch})",
        0x1a => $"TransferUnitsAllegianceTo({r8()})",
        0x1b => $"BattleUnit[{r16():ch}] = {r8()}",
        0x1c => $"AddExtraGameOverConditionText({r8()})",  // battle_text[2840+x]
        0x1d => $"DF{r8():x4} = {r8()}",
        0x1e => $"ResetDungeonFlags()",
        0x1f => $"{r8():!0}Unit[{r8():ch}]Class = {r8()}",
        0x20 => $"ConsumeAndUnequipItem({r16()})",
        0x21 => $"ObtainAppellation({r8():ap})",
        0x22 => $"{formatGlobalFlag(0x046b)} ↑= {r8()} (increase only)",
        0x23 => $"DisplayTargetMarkerOnBattleUnit({r16():ch})",
        0x24 => $"ConsumeItemDuringBattle({r16()})",
        0x25 => $"{formatGlobalFlag(0x0882)} ↑= {r8()}",
        0x26 => $"{formatGlobalFlag(0x0884)} ↑= {r8()}",
        0x27 => $"_UnknownInstruction27({r8():x2}-{r8():x2}-{r8():x2})",
        0x28 => $"_UnknownInstruction28({r8():x2}-{r8():x2}-{r8():x2})",
        // >= 0x29 and <= 0x2f => $"Noop",
        0x30 => $"{formatGlobalFlag(r16())} {r8():op} {r8()}",
        0x31 => $"{formatLocalFlag(r16())} {r8():op} {r8()}",
        0x32 => $"CurrentStrongpoint {r8():op} {r8():sp}",
        0x33 => $"CurrentAtUnit {r8():op} {r16():ch}",
        0x34 => $"PrimaryEnemyAmount {r8():op} {r8()}",
        0x35 => _("PrimaryEnemyAmountExcept({2:ch}) <= {0}{1:!0}", r8(), r8(), r16()),
        0x36 => _("PrimaryEnemyAmountExcept({2:ch}) >= {0}{1:!0}", r8(), r8(), r16()),
        0x37 => $"{r16():!0}IsBattleUnitDead({r16():ch})",
        0x38 => _("BattleUnit[{2:ch}]CurrentHp <= {0}%{1:!0}", r8(), r8(), r16()),
        0x39 => _("BattleUnit[{2:ch}]CurrentTp <= {0}{1:!0}", r8(), r8(), r16()),
        0x3a => _("BattleUnit[{2:ch}]CurrentTp <= {0}%{1:!0}", r8(), r8(), r16()),
        0x3b => _("BattleUnit[{2:ch}]CurrentHp >= {0} and <= {1}", r8(), r8(), r8(), r16()),
        0x3c => $"{r16():!1}IsBattleUnitAlive({r16():ch})",
        0x3d => _("BattleUnit[{2:ch}]CurrentHp >= {0}%{1:!0}", r8(), r8(), r16()),
        0x3e => $"CurrentStrongpointMini {r8():op} {r16():sm}",
        0x3f => $"IsOnlyOneSallyUnit {r8():op} {r8() != 0:bo}",
        0x40 => $"IsProtagonistUnequipped {r8():op} {r8() != 0:bo}",
        0x41 => $"IsAnyPlayerUnitAtBattlestagePosition({r8():!0}{r8()})",  // global/2[x]
        0x42 => _("Unit[{2:ch}]HeartCount >= {0}{1:!0}", r8(), r8(), r16()),
        0x43 => _("Unit[{2:ch}]HeartCount <= {0}{1:!0}", r8(), r8(), r16()),
        0x44 => $"IsUnit[{r16():ch}]AtBattlestagePosition({r8():!0}{r8()})",  // global/2[x]
        0x45 => $"DF{r16():x4} {r8():op} {r8()}",
        0x46 => $"CurrentWeather {r8():op} {r8()}",
        0x47 => $"IsEntryUnit[{r8()}]AtBattlestagePosition({r8():!0}{r8()})",  // global/2[x]
        0x48 => _("Clan[{1:cl}]Approval {0:op} {2}", r8(), r8(), r8()),
        0x49 => _("Unit[{2:ch}]Loyalty >= {0}{1:!0}", r8(), r8(), r16()),
        0x4a => _("Unit[{2:ch}]Loyalty <= {0}{1:!0}", r8(), r8(), r16()),
        0x4b => $"{r8():!0}IsBattleUnit[{r16():ch}]ReceivedAttack",
        0x4c => $"{r8():!0}IsBattleUnit[{r16():ch}]ReceivedSupport",
        0x4d => $"_UnknownInstruction4d {r8():op} {r8()}",
        0x4e => $"{r8():!0}HasItem({r16()})",
        0x4f => $"{r8():!0}CurrentShop == {r8()}",
        0x50 => $"OverallDeceasedUnitsAmount {r8():op} {r16()}",
        0x51 => $"OverallIncapacitatedUnitsAmount {r8():op} {r16()}",
        0x52 => $"OverallChariotAndRetreatingNotUsed {r8():op} {r8()}",
        0x53 => _("IsUnit[{1:ch}]ExistsInBarrack {0:op} true", r8(), r16()),
        0x54 => $"_UnknownInstruction54({r8():x2}-{r8():x2})",
        0x55 => $"_UnknownInstruction55({r8():x2}-{r8():x2}-{r8():x2}-{r8():x2})",
        0x56 => $"_UnknownInstruction56({r8():x2}-{r8():x2})",
        // >= 0x57 and <= 0x5f => $"Noop",
        0x60 => r8() switch { 1 => $"$0", 2 => $"$0 and $1", var n => $"$0 and.. ${n}" },
        0x61 => $"{r8():!4}$0 and ($1 or $2) and $3",
        0x62 => $"{r8():!5}$0 and $1 and ($2 or $3) and $4",
        0x63 => $"{r8():!5}$0 and.. $2 and ($3 or $4)",
        0x64 => $"{r8():!3}$0 and ($1 or $2)",
        0x65 => $"{r8():!8}$0 or.. $5 or ($6 and $7)",
        0x66 => $"{r8():!8}($0 or $1) and ($2 or $3) and ($4 or $5) and ($6 or $7)",
        0x67 => $"{r8():!2}$0 or $1",
        0x68 => $"{r8():!3}$0 or.. $2",
        0x69 => $"{r8():!7}$0 and.. $3 and ($4 or.. $6)",
        0x6a => $"{r8():!8}$0 and.. $5 and ($6 or $7)",
        0x6b => $"{r8():!13}$0 and.. $10 and ($11 or $12)",
        0x6c => $"{r8():!11}$0 and.. $8 and ($9 or $10)",
        0x6d => $"{r8():!7}$0 and.. $2 and ($3 or.. $6)",
        0x6e => $"{r8():!7}$0 and.. $4 and ($5 or $6)",
        0x6f => $"{r8():!9}$0 and.. $5 and ($6 or $7) and $8",
        0x70 => $"{r8():!9}$0 and.. $6 and ($7 or $8)",
        0x71 => $"{r8():!8}$0 and.. $3 and ($4 or.. $7)",
        _ => throw new InvalidDataException(),
    });
    //if (reader.BaseStream.Position != positionBeforeParameter + instructionParameterSizes[type]) throw new InvalidDataException();
    return ret;
}

string formatNexting(byte type, ushort flagId, byte operator_, ushort value)
{
    return interpolateGameTexts(type switch
    {
        0x00 => $".",
        0x01 => flagId switch
        {
            0 => $"PlayerSelection {operator_:op} {value}",
            _ => $"{formatGlobalFlag(flagId)}(=PlayerSelection) {operator_:op} {value}",
        },
        0x02 => $"{formatGlobalFlag(flagId)} {operator_:op} {value}",
        0x03 => $"{formatLocalFlag(flagId)} {operator_:op} {value}",
        0x04 => $"CheckInvk({formatSceneName(flagId)})",
        0x05 => $"CurrentWarrenReportReplayBranch {operator_:op} {value}",
        0x06 => (operator_, value) switch
        {
            (2, 2) => $"IsUnit[{flagId - 0x01d6:ch}]ExistsInBarrack == false",
            (0, 2) => $"IsUnit[{flagId - 0x01d6:ch}]ExistsInBarrack == true",
            _ => $"<error>"

        },
        0x07 => $"_UnknownNextType07 {flagId} {operator_:op} {value}",
        _ => throw new InvalidDataException(),
    });
}

string formatMenuTask(byte type, ushort param1, ushort param2)
{
    return interpolateGameTexts(type switch
    {
        0x00 => $".",
        0x01 => $"AddGuest(entry_units_{param1:d3}, {param2:ch})",
        0x02 => $"AddUnit(entry_units_{param1:d3}, {param2:ch})",
        0x03 => $"AddGeneralUnit(entry_units_{param1:d3}[{param2} - 1])",
        0x04 => $"RemoveGuest({(param1 == 1 ? "true" : "false")}, {param2:ch})",
        0x05 => $"SystemMessage({param2})",
        0x06 => $"PromptDataSavingWithMessage({param2})",
        0x08 => $"NameInput({param1})",
        0x09 => $"SystemMessageDuringBattle({param2})",
        0x0a => $"SaveFlagsIntoWorldAnchor({param2})",
        0x0b => $"ChangeChapter({param1})",
        0x0d => $"AddGuestWithMessage(entry_units_{param1:d3}, {param2:ch})",
        0x0e => $"RemoveDeadUnit({param2:ch})",
        _ => $"MenuTask{type:x2}({param1}, {param2})",
    });
}

string formatNextOther(byte id)
{
    return id switch
    {
        0x00 => $".",
        0x01 => $"1 world_map",
        0x02 => $"2 mini_map",
        0x03 => $"3 game_over",
        0x05 => $"5 update_world_map",
        _ => id.ToString(),
    };
}


Dictionary<int, Entry> parseInvk(BinaryReader reader)
{
    var stream = reader.BaseStream;
    var pInvk = stream.Position;
    var invk = new Dictionary<int, Entry>();

    var signature = new string(reader.ReadChars(4));
    if (signature != "INVK") throw new InvalidDataException();
    var groupCount = reader.ReadUInt16();
    reader.ReadUInt16();
    for (int group = 0; group < groupCount; group++)
    {
        var offset = reader.ReadUInt32();
        var positionBackup = stream.Position;
        stream.Position = pInvk + offset;
        var count = reader.ReadUInt16();
        for (int i = 0; i < count; i++)
        {
            var invkId = reader.ReadUInt16();
            var entry = new Entry();
            entry["when"] = invkGroups[group];
            var instruction0 = reader.ReadInt16();
            if (instruction0 < 0x60 || instruction0 > 0x71) throw new InvalidDataException();
            var conditionExpression = $"if {formatInstruction(instruction0, reader)}";
            var condition = new List<string>();
            entry[conditionExpression] = condition;
            while (true)
            {
                var instructionType = reader.ReadInt16();
                if (instructionType == -1) break;
                condition.Add(formatInstruction(instructionType, reader));
            }
            if (invk.ContainsKey(invkId))
            {
                var firstEntry = invk[invkId];
                if (!condition.SequenceEqual((List<string>)firstEntry[conditionExpression]))
                    if (invkId != 12169)  // maybe a bug?
                        throw new InvalidDataException();
                firstEntry["when"] += $", {entry["when"]}";
            }
            else
            {
                invk.Add(invkId, entry);
            }
        }
        stream.Position = positionBackup;
    }
    return invk;
}

Dictionary<int, List<string>> parseTask(BinaryReader reader)
{
    var stream = reader.BaseStream;
    var pTask = stream.Position;
    var task = new Dictionary<int, List<string>>();

    var signature = new string(reader.ReadChars(4));
    if (signature != "TASK") throw new InvalidDataException();
    var count = reader.ReadUInt16();
    reader.ReadUInt16();
    var taskIds = new ushort[count];
    for (int i = 0; i < count; i++)
    {
        taskIds[i] = reader.ReadUInt16();
    }
    stream.Position += (count % 2) * 2;
    for (int i = 0; i < count; i++)
    {
        var offset = reader.ReadUInt32();
        var positionBackup = stream.Position;
        stream.Position = pTask + offset;
        var instructions = new List<string>();
        task.Add(taskIds[i], instructions);
        while (true)
        {
            var instructionType = reader.ReadInt16();
            if (instructionType == -1) break;
            instructions.Add(formatInstruction(instructionType, reader));
        }
        stream.Position = positionBackup;
    }

    return task;
}

var pgrs = new Dictionary<string, Entry>();
for (var chapterIndex = 0u; chapterIndex < chapterNames.Length; chapterIndex++)
{
    var chapterName = chapterNames[chapterIndex];
    var pakd = Util.LoadPakd(getDataPath($@"event\chapter\chapter_{chapterName.TrimStart('C')}.pack"));

    // pgrs preprocess, generate minor scene names
    var reader = new BinaryReader(new MemoryStream(pakd[1]));
    {
        var stageFirstSceneName = new Dictionary<ushort, string>();
        var sceneStage = new Dictionary<ushort, ushort>();

        if (new string(reader.ReadChars(4)) != "PGRS") throw new InvalidDataException();
        reader.ReadUInt16();
        var count = reader.ReadUInt16();
        for (int i = 0; i < count; i++)
        {
            if (chapterName == "DLC" && i == 0x0133) break;  // repetitions of C4
            var stageId = reader.ReadUInt16();
            reader.BaseStream.Position += 2;
            var sceneId = reader.ReadUInt16();
            reader.BaseStream.Position += 38;
            sceneStage.TryAdd(sceneId, stageId);
            if (sceneNames.TryGetValue(sceneId, out var sceneName))
            {
                stageFirstSceneName.TryAdd(stageId, sceneName);
            }
        }

        var sceneStagePairs = sceneStage.ToList();
        sceneStagePairs.Sort((a, b) => a.Key - b.Key);
        ushort lastStgaeId = 0;
        string lastNamed = "";
        byte suffix = 0;
        foreach (var (sceneId, stageId) in sceneStagePairs)
        {
            if (sceneNames.TryGetValue(sceneId, out var sceneName))
            {
                lastStgaeId = stageId;
                lastNamed = sceneName;
                suffix = 0;
            }
            else
            {
                var namable = false;
                if (stageId == lastStgaeId)
                {
                    namable = true;
                }
                else if (stageFirstSceneName.TryGetValue(stageId, out var name))
                {
                    namable = true;
                    lastStgaeId = stageId;
                    lastNamed = name;
                    suffix = 0;
                }
                else if (stageId - lastStgaeId < 10)
                {
                    namable = true;
                }
                if (namable && lastNamed != "")
                {
                    if (suffix < 26)
                    {
                        sceneNames[sceneId] = $"{lastNamed}_{(char)((byte)'a' + suffix)}";
                    }
                    suffix += 1;
                }
            }
        }
    }

    // invk and task
    var invk = parseInvk(new BinaryReader(new MemoryStream(pakd[2])));
    var task = parseTask(new BinaryReader(new MemoryStream(pakd[3])));

    // pgrs
    {
        var read8 = () => reader.ReadByte();
        var read16 = () => reader.ReadUInt16();
        var read32 = () => reader.ReadUInt32();

        reader.BaseStream.Position = 0;
        if (new string(reader.ReadChars(4)) != "PGRS") throw new InvalidDataException();
        reader.ReadUInt16();
        var count = reader.ReadUInt16();
        for (int i = 0; i < count; i++)
        {
            if (chapterName == "DLC" && i == 0x0133) break;  // repetitions of C4

            var entry = pgrs[$"${chapterIndex + 1:x2}{reader.BaseStream.Position:x4}"] = new();
            void AddIfNZ(string field, int value)
            {
                if (value != 0) entry[field] = value;
            };

            var stageId = read16();
            var mapId = read16();
            var sceneId = read16();
            var invkId = read16();
            var taskId = read16();
            var nextSceneId = read16();
            var nextTaskId = read16();
            var menuTaskDataId = read16();
            var battlestageId = read16();
            var battleTeamFormation = read8();
            var encounterRate = read8();
            var eventEntryId = read16();
            var nextingCompareValue = read16();
            var nextingCompareOperator = read8();
            var nextingType = read8();
            var nextingFlagId = read16();
            var dayNightVariation = read8();
            var weatherDefault = read8();
            var flags = read8();
            if ((flags & 0b01111110) != 0) throw new InvalidDataException();
            var menuTaskType = read8();
            var menuTaskParam1 = read16();
            var menuTaskParam2 = read16();
            var nextOtherId = read8();
            var seOfCircumstance = read8();
            var bgmForEvent = read8();
            var bgmForBattle = read8();
            var bgmControlOpening = read8();
            var bgmControlEnding = read8();
            var _unknown1 = read8();  // 0 or 1
            if (read8() != 0xff) throw new InvalidDataException();

            entry["stage"] = $"{chapterName}_ST_{stageId:d3}";
            entry["scene"] = formatSceneName(sceneId);

            //entry["mapId"] = mapId;
            AddIfNZ("battlestageId", battlestageId);
            if (battleTeamFormation != 0)
            {
                entry["battleTeamFormation"] = true;
            }
            AddIfNZ("encounterRate", encounterRate);

            // isScenario == sceneNames.ContainsKey(sceneId)
            // entry["isScenario"] = (flags & 0b10000000) != 0;

            if (invkId != 0)
            {
                entry["invk"] = invk.TryGetValue(invkId, out var e) ? e : $"DanglingInvk{invkId:x4}";
            }

            if (taskId != 0)
            {
                entry["task"] = task.TryGetValue(taskId, out var e) ? e : $"DanglingTask{taskId:x4}";
            }

            if (menuTaskType != 0)
            {
                entry["menuTask"] = formatMenuTask(menuTaskType, menuTaskParam1, menuTaskParam2);
            }
            AddIfNZ("menuTaskDataId", menuTaskDataId);

            //AddIfNZ("weatherDefault", weatherDefault);
            //AddIfNZ("dayNightVariation", dayNightVariation);

            //AddIfNZ("seOfCircumstance", seOfCircumstance);
            //AddIfNZ("bgmForEvent", bgmForEvent);
            //AddIfNZ("bgmForBattle", bgmForBattle);
            //AddIfNZ("bgmControlOpening", bgmControlOpening);
            //AddIfNZ("bgmControlEnding", bgmControlEnding);

            //entry["eventEntryId"] = eventEntryId;

            if (nextingType != 0)
            {
                entry["nextIf"] = formatNexting(nextingType, nextingFlagId, nextingCompareOperator, nextingCompareValue);
            }
            if (nextTaskId != 0)
            {
                entry["nextTask"] = task.TryGetValue(nextTaskId, out var e) ? e : $"DanglingTask{nextTaskId:x4}";
            }
            if (nextSceneId != 0)
            {
                entry["nextScene"] = formatSceneName(nextSceneId);
            }
            if (nextOtherId != 0)
            {
                entry["nextOther"] = formatNextOther(nextOtherId);
            }
            //if ((flags & 0b00000001) != 0)
            //{
            //    entry["wrTerminate"] = true;
            //}
        }
    }
}

{
    var strongpoints = new HashSet<string>();
    var pgrsString = serializer.Serialize(pgrs).Replace("CurrentStrongpoint == 99:", "");
    var matches = new Regex(@"trongpoint.*?(\d+):").Matches(pgrsString);
    foreach (var match in matches.Cast<Match>())
    {
        strongpoints.Add(match.Groups[1].Value);
    }
    File.WriteAllText(@"..\_strongpointsScreenplayOrdered.yaml", serializer.Serialize(strongpoints));
}

Dictionary<string, object> pgrsGrouped;
{
    var sceneToPgrsIndexes = new Dictionary<string, List<string>>();
    var sceneReferenceCount = new Dictionary<string, int>();
    var sceneParent = new Dictionary<string, Entry>();
    foreach (var (index, entry) in pgrs)
    {
        var scene = (string)entry["scene"];
        sceneToPgrsIndexes.TryAdd(scene, new());
        sceneToPgrsIndexes[scene].Add(index);

        if (entry.TryGetValue("invk", out var invk) && invk is not string &&
            !((Entry)invk)["when"].Equals("call_by_next_task"))
        {
            sceneReferenceCount.TryGetValue(scene, out var referenceCount);
            sceneReferenceCount[scene] = referenceCount + 1;
        }
        if (entry.TryGetValue("nextScene", out var nextScene_))
        {
            var nextScene = (string)nextScene_;
            sceneReferenceCount.TryGetValue(nextScene, out var referenceCount);
            sceneReferenceCount[nextScene] = referenceCount + 1;
            sceneParent.TryAdd(nextScene, entry);
        }
    }
    pgrsGrouped = sceneToPgrsIndexes.Values.ToDictionary(v => v[0], v => (object)
        (v.Count == 1 ? pgrs[v[0]] : v.Select(index => pgrs[index]).ToList()));
    foreach (var entry in pgrs.Values)
    {
        if (entry.TryGetValue("nextScene", out var nextScene_))
        {
            var nextScene = (string)nextScene_;
            if (sceneReferenceCount[nextScene] > 1) continue;
            var indexes = sceneToPgrsIndexes[nextScene];
            entry["nextScene"] = pgrsGrouped[indexes[0]];
            pgrsGrouped.Remove(indexes[0]);
        }
    }
    foreach (var entry in pgrs.Values.Reverse())
    {
        var scene = (string)entry["scene"];
        if (sceneParent.TryGetValue(scene, out var parentScene))
        {
            if (sceneReferenceCount[scene] > 1) continue;
            if (entry["stage"].Equals(parentScene["stage"]))
            {
                entry.Remove("stage");
            }
            if (entry.TryGetValue("battlestageId", out var v1) &&
                parentScene.TryGetValue("battlestageId", out var v2) &&
                v1.Equals(v2))
            {
                entry.Remove("battlestageId");
            }
        }
    }
}
File.WriteAllText(@"..\screenplays.yaml", serializer.Serialize(pgrsGrouped));


var wrInvkTasks = new Dictionary<int, object>();
{
    var pakd = Util.LoadPakd(getDataPath(@"WarrenReport\WarrenReport.pack"), x => x < 30);
    for (var i = 0; i < 9; i++)
    {
        var invk = parseInvk(new BinaryReader(new MemoryStream(pakd[11 + i])));
        var task = parseTask(new BinaryReader(new MemoryStream(pakd[20 + i])));

        var combined = new Dictionary<int, Entry>();
        foreach (var (id, invkEntry) in invk)
        {
            var entry = combined[id] = new Entry();
            entry["invk"] = invkEntry;
            if (task.TryGetValue(id, out var taskEntry))
            {
                entry["task"] = taskEntry;
                task.Remove(id);
            }
        }
        if (task.Count > 0) throw new InvalidDataException();
        wrInvkTasks.Add(i + 1, combined);
    }
}
File.WriteAllText(@"..\wrInvkTasks.yaml", serializer.Serialize(wrInvkTasks));
