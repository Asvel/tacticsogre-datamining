using TacticsogreExtracts;
using Node = System.Collections.Generic.Dictionary<string, object>;

var gameTextEncoding = new GameTextEncoding(true);
var gamePath = args[0];

var texts = new Node();
var langs = new string[] { "en-us", "ja-jp", "zh-cn" };
for (var langIndex = 0; langIndex < langs.Length; langIndex++)
{
    var localizeText = Util.LoadPakd(Path.Combine(gamePath, @$"Data\localize\LocalizeText_{langs[langIndex]}.pack"));

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
            if (value != "" && value != "nothing" && value != "Not Use" && value != "NOT USED" && value != "（断末魔）")
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
            reader.ReadInt64();  // contents of these two fields are cross language identical
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

File.WriteAllText(@"..\texts.yaml", Util.YamlSerializer.Serialize(texts));
