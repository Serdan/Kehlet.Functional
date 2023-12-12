using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kehlet.Functional.Serialization;

public class OptionJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType &&
        typeToConvert.GetGenericTypeDefinition() == typeof(Option<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var type = typeToConvert.GenericTypeArguments[0];
        var converter = typeof(OptionJsonConverter<>).MakeGenericType(type);
        return (JsonConverter) Activator.CreateInstance(converter, [])!;
    }

    private class OptionJsonConverter<T> : JsonConverter<Option<T>>
        where T : notnull
    {
        public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var converter = (JsonConverter<OptionUnion<T>>) options.GetConverter(typeof(OptionUnion<T>));

            var result = converter.Read(ref reader, typeof(OptionUnion<T>), options);
            return result switch
            {
                OptionUnion<T>.Some(var value) => some(value),
                OptionUnion<T>.None => none,
                _ => throw new UnreachableException()
            };
        }

        public override void Write(Utf8JsonWriter writer, Option<T> value, JsonSerializerOptions options)
        {
            var converter = (JsonConverter<OptionUnion<T>>) options.GetConverter(typeof(OptionUnion<T>));

            converter.Write(writer, union(value), options);
        }
    }

}
