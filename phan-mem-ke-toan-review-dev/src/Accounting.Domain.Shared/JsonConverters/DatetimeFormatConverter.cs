using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Accounting.JsonConverters
{
    public class DatetimeFormatConverter : JsonConverter<DateTime>
    {
		private readonly string Format;
		public DatetimeFormatConverter(string format)
		{
			Format = format;
		}
		public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
		{
			writer.WriteStringValue(date.ToString(Format));
		}
		public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return DateTime.ParseExact(reader.GetString(), Format, null);
		}
	}
}
