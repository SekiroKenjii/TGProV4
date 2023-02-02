namespace TGProV4.Application.Serialization.JsonConverters;

public class TimespanJsonConverter : JsonConverter<TimeSpan>
{
    private const string TimeSpanFormat = @"d\.hh\:mm\:ss\:FFF";

    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();

        if (string.IsNullOrWhiteSpace(s))
        {
            return TimeSpan.Zero;
        }

        if (!TimeSpan.TryParseExact(s, TimeSpanFormat, null, out var parsedTimeSpan))
        {
            throw new FormatException($"Input timespan is not in an expected format : expected {Regex
               .Unescape(TimeSpanFormat)}. Please retrieve this key as a string and parse manually.");
        }

        return parsedTimeSpan;
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        var timespanFormatted = $"{value.ToString(TimeSpanFormat)}";

        writer.WriteStringValue(timespanFormatted);
    }
}
