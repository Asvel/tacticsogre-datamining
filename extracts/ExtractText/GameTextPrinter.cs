namespace TacticsogreExtracts
{
    public class GameTextPrinter : IFormatProvider, ICustomFormatter
    {
        public object? GetFormat(Type? type) => type == typeof(ICustomFormatter) ? this : null;
        public string Format(string? fmt, object? arg, IFormatProvider? formatProvider) => fmt switch
        {
            "+-" => FormatPlusMinus(arg),
            "ap" => Convert.ToUInt16(arg) > 100 ? arg!.ToString()! : GetText("appellation", arg),
            "bo" => arg?.Equals(true) == true ? "true" : "false",
            "ch" => GetText("character", arg, unitNameMap),
            "cl" => GetText("clan", arg),
            "it" => GetText("item", arg),
            "op" => compareOperators[Convert.ToByte(arg)],
            "pe" => $"{"LNC"[Convert.ToUInt16(arg)]}",
            "sp" => GetText("strongpoint", arg),
            "sm" => GetText("strongpoint", arg, strongpointMiniNameMap),
            "wr" => GetText("wr_rumor", arg),
            _ when fmt?[0] == '!' => arg?.ToString()?.Equals(fmt?[1..]) == true ? "" : "<error>",
            _ => arg is IFormattable arg_
                ? arg_.ToString(fmt, formatProvider)
                : arg?.ToString() ?? "",
        };

        readonly Dictionary<string, Dictionary<ushort, string>> texts = new();
        readonly Dictionary<ushort, ushort> unitNameMap;
        readonly Dictionary<ushort, ushort> strongpointMiniNameMap;
        public GameTextPrinter(int langIndex)
        {
            var deserializer = new YamlDotNet.Serialization.Deserializer();
            void loadYaml<T>(string path, out T v) => v = deserializer.Deserialize<T>(File.ReadAllText(path));
            loadYaml(@"..\texts.yaml", out dynamic textsRaw);
            loadYaml(@"..\_unitNameIds.yaml", out unitNameMap);
            loadYaml(@"..\_strongpointMiniNameIds.yaml", out strongpointMiniNameMap);

            void loadText(string type, string prefix, string suffix)
            {
                var dict = texts[type] = new();
                foreach (var kvp in textsRaw[prefix])
                {
                    if (!kvp.Value.ContainsKey(suffix)) continue;
                    dict.Add(Convert.ToUInt16(kvp.Key), kvp.Value[suffix][langIndex]);
                }
            }
            loadText("appellation", "ORDER_LC_ORDER_MES", "NAME");
            loadText("character", "UNITNAME0_LC_UNITNAME", "TEXT");
            loadText("clan", "_CLAN", "NAME");
            loadText("strongpoint", "STRONGHOLD_LC_STRONGHOLD", "NAME");
            loadText("wr_rumor", "RUMOR_TEXT_LC_RUMOR", "TITLE");
            loadText("item", "ARMSTEXT_LC_ARMS", "NAME");
            loadText("item1000", "COMMODITYTEXT_LC_COMMODITY", "NAME");
            foreach (var (id, name) in texts["item1000"]) texts["item"].Add((ushort)(id + 1000), name);
        }

        string GetText(string type, object? arg, Dictionary<ushort, ushort>? idMap = null)
        {
            var id = Convert.ToUInt16(arg);
            return idMap?.TryGetValue(id, out var nameId) == true
                ? $"{id}->{nameId}:{texts[type][nameId]}"
                : $"{id}:{(texts[type].TryGetValue(id, out var text) ? text : "Unknown")}";
        }

        static readonly string[] compareOperators = { "==", "!=", "<", "<=", ">", ">=" };

        static string FormatPlusMinus(object? arg)
        {
            var n = Convert.ToInt32(arg);
            return n >= 0 ? $"+= {n}" : $"-= {-n}";
        }

    }
}
