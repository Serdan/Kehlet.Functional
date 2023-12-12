using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kehlet.Functional.Serialization;

public class ResultUnionJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType &&
        typeToConvert.GetGenericTypeDefinition() == typeof(ResultUnion<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(ResultUnionJsonConverter<>).MakeGenericType(valueType);

        return (JsonConverter) Activator.CreateInstance(converterType, [options])!;
    }

    private class ResultUnionJsonConverter<TValue> : JsonConverter<ResultUnion<TValue>>
        where TValue : notnull
    {
        private readonly JsonConverter<TValue> valueConverter;

        public ResultUnionJsonConverter(JsonSerializerOptions options)
        {
            valueConverter = (JsonConverter<TValue>) options.GetConverter(typeof(TValue));
        }

        public override ResultUnion<TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ThrowIfNot(reader, JsonTokenType.StartObject);
            ResultUnion<TValue>? result = null;

            while (reader.Read())
            {
                if (reader.TokenType is JsonTokenType.EndObject)
                {
                    break;
                }

                ThrowIfNot(reader, JsonTokenType.PropertyName);
                var propertyName = reader.GetString()!;

                const string caseName1Raw = nameof(ResultUnion<TValue>.Ok);
                var caseName1 = options.PropertyNamingPolicy?.ConvertName(caseName1Raw) ?? caseName1Raw;
                if (string.Equals(caseName1, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    reader.Read();
                    var v = valueConverter.Read(ref reader, typeof(TValue), options)
                        ?? throw new JsonException("Expected value, received null.");
                    result = ResultUnion<TValue>.Cons.NewOk(v);
                }

                const string caseName2Raw = nameof(ResultUnion<TValue>.Error);
                var caseName2 = options.PropertyNamingPolicy?.ConvertName(caseName2Raw) ?? caseName2Raw;
                if (string.Equals(caseName2, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    reader.Read();
                    var v = reader.GetString();
                    result = ResultUnion<TValue>.Cons.NewError(new(v));
                }
            }

            if (result is null)
            {
                throw new JsonException("Expected value.");
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, ResultUnion<TValue> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            switch (value)
            {
                case ResultUnion<TValue>.Ok(var okValue):
                {
                    const string caseName1Raw = nameof(ResultUnion<TValue>.Ok);
                    var caseName1 = options.PropertyNamingPolicy?.ConvertName(caseName1Raw) ?? caseName1Raw;

                    writer.WritePropertyName(caseName1);
                    valueConverter.Write(writer, okValue, options);
                    break;
                }
                case ResultUnion<TValue>.Error(var error):
                {
                    const string caseName2Raw = nameof(ResultUnion<TValue>.Error);
                    var caseName2 = options.PropertyNamingPolicy?.ConvertName(caseName2Raw) ?? caseName2Raw;

                    writer.WriteString(caseName2, error.Message);
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
