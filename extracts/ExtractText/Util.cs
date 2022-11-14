using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace TacticsogreExtracts
{
    public class Util
    {
        public static Dictionary<int, byte[]> LoadPakd(string inputPath, Func<int, bool>? idFilter = null)
        {
            using var inputFile = File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            var reader = new BinaryReader(inputFile);
            if (reader.ReadUInt32() != 0x646B6170/*pakd*/) throw new InvalidDataException();

            var count = reader.ReadInt32();
            var offsets = new int[count + 1];
            for (var i = 0; i < count + 1; i++) offsets[i] = reader.ReadInt32();
            var ids = new int[count];
            var hasId = offsets[0] >= inputFile.Position + count * 4;
            for (var i = 0; i < count; i++)
            {
                ids[i] = hasId ? reader.ReadInt32() : i;
                if (i == 1 && ids[i] == 0)
                {
                    ids[i] = 1;
                    hasId = false;
                }
            }

            var output = new Dictionary<int, byte[]>();
            for (var i = 0; i < count; i++)
            {
                if (idFilter != null && !idFilter(ids[i])) continue;
                inputFile.Position = offsets[i];
                output.Add(ids[i], reader.ReadBytes(offsets[i + 1] - offsets[i]));
            }
            return output;
        }
        
        public static IEnumerable<Memory<byte>> GetXlceEntries(byte[] xlce, int assertEntrySize = 0)
        {
            if (BitConverter.ToInt32(xlce, 0) != 0x65636c78/*xlce*/) throw new InvalidDataException();
            var count = BitConverter.ToInt32(xlce, 4);
            var bodyStart = BitConverter.ToInt32(xlce, 8);
            var entrySize = BitConverter.ToInt32(xlce, 12);
            if (assertEntrySize != 0 && entrySize != assertEntrySize) throw new InvalidDataException();
            for (var i = 0; i < count; i++)
            {
                var entryStart = bodyStart + i * entrySize;
                yield return xlce.AsMemory(entryStart, entrySize);
            }
        }

        public static readonly ISerializer YamlSerializer = new SerializerBuilder()
            .WithEventEmitter(nextEmitter => new MultilineScalarFlowStyleEmitter(nextEmitter))
            .Build();
    }

    class MultilineScalarFlowStyleEmitter : ChainedEventEmitter
    {
        public MultilineScalarFlowStyleEmitter(IEventEmitter nextEmitter) : base(nextEmitter) { }

        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            if (typeof(string).IsAssignableFrom(eventInfo.Source.Type))
            {
                string? value = eventInfo.Source.Value as string;
                if (!string.IsNullOrEmpty(value))
                {
                    bool isMultiLine = value.IndexOfAny(new char[] { '\r', '\n', '\x85', '\x2028', '\x2029' }) >= 0;
                    if (isMultiLine)
                    {
                        eventInfo = new ScalarEventInfo(eventInfo.Source)
                        {
                            Style = ScalarStyle.Literal
                        };
                    }
                }
            }
            nextEmitter.Emit(eventInfo, emitter);
        }
    }
}
