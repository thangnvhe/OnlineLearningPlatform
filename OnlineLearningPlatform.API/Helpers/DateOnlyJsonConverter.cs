using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnlineLearningPlatform.API.Helpers
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Đọc chuỗi từ JSON và chuyển thành DateOnly
            return DateOnly.ParseExact(reader.GetString()!, Format);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            // Chuyển DateOnly thành chuỗi định dạng yyyy-MM-dd khi trả về Client
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}
