using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kehlet.Functional.Serialization;

public class OptionUnionJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType &&
        typeToConvert.GetGenericTypeDefinition() == typeof(OptionUnion<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(OptionUnionJsonConverter<>).MakeGenericType(valueType);

        return (JsonConverter) Activator.CreateInstance(converterType, [options])!;
    }

    private class OptionUnionJsonConverter<TValue> : JsonConverter<OptionUnion<TValue>>
        where TValue : notnull
    {
        private readonly JsonConverter<TValue> valueConverter;

        public OptionUnionJsonConverter(JsonSerializerOptions options)
        {
            valueConverter = (JsonConverter<TValue>) options.GetConverter(typeof(TValue));
        }

        public override OptionUnion<TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ThrowIfNot(reader, JsonTokenType.StartObject);

            OptionUnion<TValue>? result = null;

            while (reader.Read())
            {
                if (reader.TokenType is JsonTokenType.EndObject)
                {
                    break;
                }

                ThrowIfNot(reader, JsonTokenType.PropertyName);
                var propertyName = reader.GetString()!;

                const string caseName1Raw = nameof(OptionUnion<TValue>.Some);
                var caseName1 = options.PropertyNamingPolicy?.ConvertName(caseName1Raw) ?? caseName1Raw;
                if (string.Equals(caseName1, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    reader.Read();
                    var value = valueConverter.Read(ref reader, typeToConvert, options)!;
                    result = OptionUnion<TValue>.Cons.NewSome(value);
                }

                const string caseName2Raw = nameof(OptionUnion<TValue>.None);
                var caseName2 = options.PropertyNamingPolicy?.ConvertName(caseName2Raw) ?? caseName2Raw;
                if (string.Equals(caseName2, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    reader.Read();
                    result = OptionUnion<TValue>.Cons.NewNone;
                }
            }

            if (result is null)
            {
                throw new JsonException("Expected value.");
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, OptionUnion<TValue> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            switch (value)
            {
                case OptionUnion<TValue>.Some(var someValue):
                {
                    const string caseName1Raw = nameof(OptionUnion<TValue>.Some);
                    var caseName1 = options.PropertyNamingPolicy?.ConvertName(caseName1Raw) ?? caseName1Raw;

                    writer.WritePropertyName(caseName1);
                    valueConverter.Write(writer, someValue, options);
                    break;
                }
                case OptionUnion<TValue>.None:
                {
                    const string caseName2Raw = nameof(OptionUnion<TValue>.None);
                    var caseName2 = options.PropertyNamingPolicy?.ConvertName(caseName2Raw) ?? caseName2Raw;

                    writer.WriteNull(caseName2);
                    break;
                }
                default:
                    throw new JsonException($"Invalid type: {value.GetType()}");
            }

            writer.WriteEndObject();
        }

        private static void ThrowIfNot(Utf8JsonReader reader, JsonTokenType kind)
        {
            if (reader.TokenType != kind)
            {
                throw new JsonException($"Expected {kind}, but received {reader.TokenType}");
            }
        }
    }
}
