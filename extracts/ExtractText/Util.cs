using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace TacticsogreExtracts
{
    public class Util
    {
        public static Dictionary<int, byte[]> LoadPakd(string inputPath)
        {
            using var inputFile = File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            var reader = new BinaryReader(inputFile);
            if (reader.ReadUInt32() != 0x646B6170/*pakd*/) throw new InvalidDataException();

            var count = reader.ReadInt32();
            var offsets = new int[count + 1];
            for (var i = 0; i < count + 1; i++) offsets[i] = reader.ReadInt32();
            var ids = new int[count];
            var hasId = offsets[0] >= inputFile.Position + count * 4;
            for (var i = 0; i < count; i++) ids[i] = hasId ? reader.ReadInt32() : i;

            var output = new Dictionary<int, byte[]>();
            for (var i = 0; i < count; i++)
            {
                inputFile.Position = offsets[i];
                output.Add(ids[i], reader.ReadBytes(offsets[i + 1] - offsets[i]));
            }
            return output;
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
