using System.Text.Json;
using System.Text.Json.Serialization;
using CRM_B.Domain.Kernel.Entities;

namespace CRM_B.Infrastructure.Jobs.Outbox.Serialization;

internal static class OutboxEventSerializer
{
    public static readonly JsonSerializerOptions Options = BuildOptions();

    public static string Serialize(object @event) =>
        JsonSerializer.Serialize(@event, @event.GetType(), Options);

    public static object Deserialize(string payload, Type type) =>
        JsonSerializer.Deserialize(payload, type, Options)
        ?? throw new InvalidOperationException($"Outbox payload deserialized to null for type {type.Name}.");

    private static JsonSerializerOptions BuildOptions()
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            WriteIndented = false,
        };

        options.Converters.Add(new StronglyTypedIdJsonConverterFactory());
        return options;
    }
}

internal sealed class StronglyTypedIdJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsValueType) return false;

        return typeToConvert.GetInterfaces().Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityId<>));
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(StronglyTypedIdJsonConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

internal sealed class StronglyTypedIdJsonConverter<TId> : JsonConverter<TId>
    where TId : struct, IEntityId<TId>
{
    public override TId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        TId.From(reader.GetGuid());

    public override void Write(Utf8JsonWriter writer, TId value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}