// this is a data generator for https://github.com/Asvel/tacticsogre
// it won't be very useful for you in general

using System.Text;
using System.Text.RegularExpressions;
using TacticsogreExtracts;

var dataPsp = @"..\..\..\..\tacticsogre-psp-datamining\extracts\data\";

var deserializer = new YamlDotNet.Serialization.Deserializer();
var texts = deserializer.Deserialize<dynamic>(File.ReadAllText(@"..\texts.yaml"));
static List<string> castTexts(dynamic textsLeaf) => ((List<object>)textsLeaf).Cast<string>().ToList();


// translations.yaml
var translationsPsp = deserializer.Deserialize<Dictionary<string, List<List<string>>>>(
    File.ReadAllText(dataPsp + "translations.yaml"));
var translations = new Dictionary<string, List<List<string>>>();
{
    var dictPsp = translationsPsp["people"].ToDictionary(entry => entry[1]);
    dictPsp["ベルダJr."] = dictPsp["ベルダＪｒ．"];
    dictPsp["オブダJr."] = dictPsp["オブダＪｒ．"];

    var node = texts["CHARA_TEXT_LC_WR_CHARA"];
    var dict = new Dictionary<string, List<string>>();
    foreach (var child in node.Values)
    {
        var entry = castTexts(child["CHR"]["NAME"]);
        if (entry[0] == "{93}")
        {
            entry = castTexts(texts["CHARACTER_NAME_LC_CHRNAME"]["001"]["NAME"]);
        }
        var key = entry[1];
        if (key == "剣士ハボリム") continue;
        if (dict.ContainsKey(key)) continue;
        if (dictPsp.TryGetValue(key, out var entryPsp))
        {
            entry.AddRange(entryPsp.Skip(3));
        }
        else
        {
            entry.Add("");
            Console.WriteLine(string.Join('|', entry));
        }
        dict.Add(key, entry);
    }
    translations["people"] = dict.Values.ToList();
}
{
    var dictPsp = translationsPsp["class"].ToDictionary(entry => entry[1]);

    var node = texts["CLASS_TEXT_DATA"];
    var dict = new Dictionary<string, List<string>>();
    foreach (var child in node.Values)
    {
        if (child["MALE"][1] == "デステンプラー") break;
        foreach (var sub in new string[] { "MALE", "FEMALE" })
        {
            var entry = castTexts(child[sub]);
            var key = entry[1];
            if (dict.ContainsKey(key)) continue;
            if (dictPsp.TryGetValue(key, out var entryPsp))
            {
                entry.AddRange(entryPsp.Skip(3));
            }
            else
            {
                entry.Add("");
                Console.WriteLine(string.Join('|', entry));
            }
            dict.Add(key, entry);
        }
    }
    translations["class"] = dict.Values.ToList();
}
{
    var strongpointIds = deserializer.Deserialize<List<int>>(
        File.ReadAllText(dataPsp + "strongpoints.yaml"));
    var patternHalfwidth = new Regex(@"[0-9]+", RegexOptions.Compiled);
    strongpointIds = strongpointIds.Where(x => x < 78).Concat(strongpointIds).ToList();

    var dictPsp = translationsPsp["strongpoint"].ToDictionary(entry =>
        entry[1].Normalize(NormalizationForm.FormKC).Replace(' ', '　'));

    var node = texts["STRONGHOLD_LC_STRONGHOLD"];
    var dict = new Dictionary<string, List<string>>();
    foreach (var id in strongpointIds)
    {
        var entry = castTexts(node[id.ToString("d3")]["NAME"]).Select(text =>
        {
            var asterisked = patternHalfwidth.Replace(text, "_");
            return asterisked.FirstOrDefault() != '_' ? asterisked : text;
        }).ToList();
        var key = entry[1];
        if (dict.ContainsKey(key)) continue;
        if (dictPsp.TryGetValue(key, out var entryPsp))
        {
            entry.AddRange(entryPsp.Skip(3).Select(x =>
                x.Normalize(NormalizationForm.FormKC).Replace(' ', '　')));
        }
        else
        {
            entry.Add("");
            Console.WriteLine(string.Join('|', entry));
        }
        dict.Add(key, entry);
    }
    translations["strongpoint"] = dict.Values.ToList();
}
{
    var dictPsp = translationsPsp["clan"].ToDictionary(entry => entry[1]);

    var node = texts["_CLAN"];
    var dict = new Dictionary<string, List<string>>();
    foreach (var child in node.Values)
    {
        var entry = castTexts(child["NAME"]);
        var key = entry[1];
        entry.AddRange(dictPsp[key].Skip(3));
        dict.Add(key, entry);
    }
    translations["clan"] = dict.Values.ToList();
}
File.WriteAllText($@"..\translations.yaml", Util.YamlSerializer.Serialize(translations));


// official-chinese.yaml
var offichn = new Dictionary<char, Dictionary<string, string>>();
{
    var node = texts["ORDER_LC_ORDER_MES"];
    var dict = offichn['a'] = new();
    foreach (var kvp in node)
    {
        dict.Add(kvp.Key.TrimStart('0'), kvp.Value["NAME"][2]);
    }
}
{
    var node = texts["UNITNAME0_LC_UNITNAME"];
    var dict = offichn['c'] = new();
    foreach (var kvp in node)
    {
        dict.Add(kvp.Key.TrimStart('0'), kvp.Value["TEXT"][2]);
    }
}
{
    var node = texts["CHARACTER_NAME_LC_CHRNAME"];
    var dict = offichn['d'] = new();
    foreach (var kvp in node)
    {
        dict.Add(kvp.Key.TrimStart('0'), kvp.Value["NAME"][2]);
    }
}
{
    var node = texts["_CLAN"];
    var dict = offichn['l'] = new();
    foreach (var kvp in node)
    {
        dict.Add(kvp.Key.TrimStart('0'), kvp.Value["NAME"][2]);
    }
}
{
    var node = texts["STRONGHOLD_LC_STRONGHOLD"];
    var dict = offichn['p'] = new();
    foreach (var kvp in node)
    {
        dict.Add(kvp.Key.TrimStart('0'), kvp.Value["NAME"][2]);
    }
}
{
    var node = texts["CURRENT_AFFAIRS_TEXT_LC_CURRENT"];
    var dict = offichn['q'] = new();
    foreach (var kvp in node)
    {
        dict.Add(kvp.Key.TrimStart('0'), kvp.Value["TITLE"][2]);
    }
}
{
    var node = texts["RUMOR_TEXT_LC_RUMOR"];
    var dict = offichn['r'] = new();
    foreach (var kvp in node)
    {
        dict.Add(kvp.Key.TrimStart('0'), kvp.Value["TITLE"][2]);
    }
}
{
    var sceneNames = deserializer.Deserialize<Dictionary<string, string>>(
        File.ReadAllText(dataPsp + @"sceneNames.yaml"));
    var sceneNameToId = sceneNames.ToDictionary(kvp => (char.IsDigit(kvp.Value[0]) ? "C" : "") + kvp.Value.ToUpper(), kvp => kvp.Key);

    var node = texts["SCENARIO"];
    var dict = offichn['s'] = new();
    foreach (var kvp1 in node)
    {
        foreach (var kvp2 in kvp1.Value)
        {
            var last = ((IEnumerable<dynamic>)kvp2.Value.Values).Last();
            last = ((IEnumerable<dynamic>)last.Values).Last();
            string text = last["TEXT"][2];
            if (text?.StartsWith("{8C01}") == true)
            {
                var options = text[6..].Split('\n');
                var sceneName = $"{kvp1.Key.Replace("SUB", "")}_{kvp2.Key.TrimStart('0')}";
                var sceneId = sceneNameToId[sceneName];
                dict.Add($"{sceneId}a", options[0]);
                dict.Add($"{sceneId}b", options[1]);
            }
        }
    }
}
offichn['t'] = new()
{
    { "商店", "商店" },
    { "拍卖", "拍卖" },
    { "演习", "演习" },
    { "华伦报告", "沃连报告" },
    { "命运之轮", "命运之轮" },
    { "时事", "时事" },
    { "消息", "新闻" },
    { "穿越点", "锚点" },

    { "DIVA", "歌姬DIVA" },
    { "寻找华伦", "寻找沃连" },
    { "真正的骑士", "真正的骑士" },
    { "十二位勇士", "十二名勇者" },
    { "禁咒探索", "禁咒探索" },
    { "铳的冲击", "枪械冲击" },
    { "封印神殿", "封印神殿" },
    { "启程", "启程" },

    { "蛮族号角", "蛮族号角" },
    { "四风神器", "四风神器" },
    { "南风之剑", "诺托斯" },
    { "北风之斧", "玻瑞阿斯" },
    { "西风之枪", "泽菲罗斯" },
    { "东风之锤", "欧洛斯" },
    { "晶球", "宝珠" },
    { "水晶南瓜", "透明南瓜" },
    { "十二神将的音叉", "十二神将的音叉" },
    { "十二神将", "十二神将" },
    { "音叉", "音叉" },
    { "巫女证", "巫女转职证" },
};
File.WriteAllText(@"..\official-chinese.yaml", Util.YamlSerializer.Serialize(offichn));
