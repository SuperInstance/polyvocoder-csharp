using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// Polyvocoder C#/.NET 9 port
// Verifies the Quilt fleet canary: fnv1a-64("café Δ 日本語") = 0x024a555471370b18d

namespace Polyvocoder;

public static class Fnv1a
{
    private const ulong FNV_OFFSET_BASIS = 0xcbf29ce484222325;
    private const ulong FNV_PRIME = 0x100000001b3;
    public const string CANARY_STRING = "café Δ 日本語";

    public static ulong Hash(string s)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        ulong h = FNV_OFFSET_BASIS;
        foreach (byte b in bytes)
        {
            h ^= b;
            h = unchecked(h * FNV_PRIME);
        }
        return h;
    }

    public static ulong CanaryHash() => Hash(CANARY_STRING);

    public static bool Verify() => CanaryHash() == 0x024a555471370b18d;
}

// Schema-parity types — match Python and Rust with bidirectional field naming
public class JEVFeatures
{
    [JsonPropertyName("canon_worthy")] public double CanonWorthy { get; set; }
    [JsonPropertyName("distinct_voice")] public double DistinctVoice { get; set; }
    [JsonPropertyName("doctrine_anchor")] public double DoctrineAnchor { get; set; }
    [JsonPropertyName("voice_fit")] public double VoiceFit { get; set; }
    [JsonPropertyName("novelty")] public double Novelty { get; set; }
    [JsonPropertyName("density")] public double Density { get; set; }
}

public class DecodedSample
{
    [JsonPropertyName("latent")] public double[] Latent { get; set; } = Array.Empty<double>();
    [JsonPropertyName("text")] public string Text { get; set; } = "";
    [JsonPropertyName("image")] public string Image { get; set; } = "";
    [JsonPropertyName("audio")] public float[] Audio { get; set; } = Array.Empty<float>();
}

// A custom converter that emits both snake_case and camelCase aliases
public class PolyvocoderResultConverter : JsonConverter<PipelineResult>
{
    public override PipelineResult? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<PipelineResult>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, PipelineResult value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("features");
        JsonSerializer.Serialize(writer, value.Features, options);
        // decoded_samples and decodedSamples both
        writer.WritePropertyName("decoded_samples");
        JsonSerializer.Serialize(writer, value.DecodedSamples, options);
        writer.WritePropertyName("decodedSamples");
        JsonSerializer.Serialize(writer, value.DecodedSamples, options);
        writer.WriteEndObject();
    }
}

public class PipelineResult
{
    [JsonPropertyName("features")] public JEVFeatures Features { get; set; } = new();
    [JsonPropertyName("decoded_samples")]
    public DecodedSample[] DecodedSamples { get; set; } = Array.Empty<DecodedSample>();
}

public static class Program
{
    public static void Main()
    {
        var h = Fnv1a.CanaryHash();
        var verified = Fnv1a.Verify();

        Console.WriteLine($"fnv1a-64('{Fnv1a.CANARY_STRING}') = 0x{h:x16}");
        Console.WriteLine($"verify: {(verified ? "PASS" : "FAIL")}");

        // Demo: serialize a sample result
        var result = new PipelineResult
        {
            Features = new JEVFeatures
            {
                CanonWorthy = 0.46,
                DistinctVoice = 0.53,
                DoctrineAnchor = 0.67,
                VoiceFit = 0.71,
                Novelty = 0.84,
                Density = 1.04,
            },
            DecodedSamples = new[]
            {
                new DecodedSample
                {
                    Latent = new[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6 },
                    Text = "Cells are scars. The witness log accumulates.",
                    Image = "* * * * *",
                    Audio = new float[] { 0.1f, -0.2f, 0.3f, -0.4f },
                },
            },
        };

        var opts = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        Console.WriteLine("\nSample pipeline result (schema-parity):");
        Console.WriteLine(JsonSerializer.Serialize(result, opts));

        Environment.Exit(verified ? 0 : 1);
    }
}
