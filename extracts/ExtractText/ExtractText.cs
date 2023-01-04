using TacticsogreExtracts;
using Node = System.Collections.Generic.Dictionary<string, object>;

var getDataPath = (string path) => Path.Combine(args[0], "Data", path);
var gameTextEncoding = new GameTextEncoding(true);

var texts = new Node();
var langs = new string[] { "en-us", "ja-jp", "zh-cn" };
for (var langIndex = 0; langIndex < langs.Length; langIndex++)
{
    var localizeText = Util.LoadPakd(getDataPath(@$"localize\LocalizeText_{langs[langIndex]}.pack"));

    foreach (var data in localizeText.Values)
    {
        var reader = new BinaryReader(new MemoryStream(data));
        if (reader.ReadUInt32() != 0x65747874/*txte*/) throw new InvalidDataException();

        var count = reader.ReadInt32();
        var bodyStart = reader.ReadInt32();
        var entrySize = reader.ReadInt32();
        if (bodyStart != 0x10 || entrySize != 0x20) throw new InvalidDataException();

        string readString()
        {
            var offset = Convert.ToInt32(reader.ReadInt64());
            var length = 0;
            while (data[offset + length] != 0) length++;
            return gameTextEncoding.GetString(data, offset, length);
        }

        for (var i = 0; i < count; i++)
        {
            var key = readString()
                .Replace("_NAME_FULL_OR_AGE", "_NAME-FULL-OR-AGE")
                .Replace("_NAME_2LINES", "_NAME-2LINES");
            var value = readString();
            if (value != "" && value != "nothing" && value != "Not Use" && value != "NOT USED" && value != "（断末魔）" && !key.EndsWith("_LINECHECK"))
            {
                var parts = key.Split('_');
                var node = texts;
                foreach (var part in parts.Take(parts.Length - 1))
                {
                    node.TryAdd(part, new Node());
                    node = (Node)node[part];
                }
                var lastPart = parts.Last().Replace('-', '_');
                node.TryAdd(lastPart, new string[langs.Length]);
                ((string[])node[lastPart])[langIndex] = value;
            }
            reader.ReadInt64();  // gender, only used in de-de/es-es/fr-fr
            reader.ReadInt64();
        }
    }
}

texts = texts
    .Select(kvp =>
    {
        var key = kvp.Key;
        var value = kvp.Value;
        while (value is Node node && node.Count == 1)
        {

            key = $"{key}_{node.Keys.First()}";
            value = node.Values.First();
        }
        return KeyValuePair.Create(key, value);
    })
    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

((Node)texts["STRONGHOLD_LC_STRONGHOLD"]).Remove("110");  // dummy data, part of it has been stripped

var clan = new Node();
foreach (var kvp in ((dynamic)texts)["WR_TERM_LC"]["WR"]["CHAOS"])
{
    var names = ((string[])kvp.Value["TEXT"]).Select(n => n.TrimEnd('人')).ToArray();
    clan[((string)kvp.Key)[..^1]] = new Node() { { "NAME", names } };
}
texts["_CLAN"] = clan;

File.WriteAllText(@"..\texts.yaml", Util.YamlSerializer.Serialize(texts)
    .Replace("-{8401FF0101} LUCK", "'-{8401FF0101} LUCK'")  // these break parsing (might YamlDotNet bug)
    .Replace("-{8401FF0101} LOYALTY", "'-{8401FF0101} LOYALTY'"));
