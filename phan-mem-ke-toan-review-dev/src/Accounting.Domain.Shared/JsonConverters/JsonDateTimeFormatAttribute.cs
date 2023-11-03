using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Accounting.JsonConverters
{
    public sealed class JsonDateTimeFormatAttribute : JsonConverterAttribute
    {
        private readonly string format;

        public JsonDateTimeFormatAttribute(string format)
        {
            this.format = format;
        }

        public string Format => this.format;

        public override JsonConverter? CreateConverter(Type typeToConvert)
        {
            return new DatetimeFormatConverter(this.format);
        }
    }
}
