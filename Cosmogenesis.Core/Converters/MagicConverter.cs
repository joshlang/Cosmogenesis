using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cosmogenesis.Core.Converters;

/// <summary>
/// https://github.com/dotnet/corefx/issues/39477
/// </summary>
public class MagicConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        var canI = !typeToConvert.IsAbstract &&
            typeToConvert.GetConstructor(Type.EmptyTypes) != null &&
            !typeToConvert.GetGenericInterfaces(typeof(IDictionary<,>)).Any() &&
            typeToConvert
                .GetProperties()
                .Where(x => !x.CanWrite)
                .Where(x => x.PropertyType.IsGenericType)
                .Select(x => new
                {
                    Property = x,
                    CollectionInterface = x.PropertyType.GetGenericInterfaces(typeof(ICollection<>)).FirstOrDefault()
                })
                .Where(x => x.CollectionInterface != null)
                .Any();

        return canI;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) => (JsonConverter)Activator.CreateInstance(typeof(SuperMagicConverter<>).MakeGenericType(typeToConvert))!;

    public class SuperMagicConverter<T> : JsonConverter<T> where T : new()
    {
        readonly Dictionary<string, (Type PropertyType, Action<T, object?>? Setter, Action<T, object?>? Adder)> PropertyHandlers;
        public SuperMagicConverter()
        {
            PropertyHandlers = typeof(T)
                .GetProperties()
                .Select(x => new
                {
                    Property = x,
                    CollectionInterface = !x.CanWrite && x.PropertyType.IsGenericType ? x.PropertyType.GetGenericInterfaces(typeof(ICollection<>)).FirstOrDefault() : null
                })
                .Select(x =>
                {
                    var tParam = Expression.Parameter(typeof(T));
                    var objParam = Expression.Parameter(typeof(object));
                    Action<T, object?>? setter = null;
                    Action<T, object?>? adder = null;
                    Type? propertyType = null;
                    if (x.Property.CanWrite)
                    {
                        propertyType = x.Property.PropertyType;
                        setter = Expression.Lambda<Action<T, object?>>(
                            Expression.Assign(
                                Expression.Property(tParam, x.Property),
                                Expression.Convert(objParam, propertyType)),
                            tParam,
                            objParam)
                            .Compile();
                    }
                    else
                    {
                        if (x.CollectionInterface != null)
                        {
                            propertyType = x.CollectionInterface.GetGenericArguments()[0];
                            adder = Expression.Lambda<Action<T, object?>>(
                                Expression.Call(
                                    Expression.Property(tParam, x.Property),
                                    x.CollectionInterface.GetMethod("Add")!,
                                    Expression.Convert(objParam, propertyType)),
                                tParam,
                                objParam)
                                .Compile();
                        }
                    }
                    return new
                    {
                        x.Property.Name,
                        setter,
                        adder,
                        propertyType
                    };
                })
                .Where(x => x.propertyType != null)
                .ToDictionary(x => x.Name, x => (x.propertyType!, x.setter, x.adder));
        }
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) => throw new NotImplementedException();
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var item = new T();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    if (PropertyHandlers.TryGetValue(reader.GetString() ?? throw new JsonException($"Bad JSON"), out var handler))
                    {
                        if (!reader.Read())
                        {
                            throw new JsonException($"Bad JSON");
                        }
                        if (handler.Setter != null)
                        {
                            handler.Setter(item, JsonSerializer.Deserialize(ref reader, handler.PropertyType, options));
                        }
                        else
                        {
                            if (reader.TokenType == JsonTokenType.StartArray)
                            {
                                while (true)
                                {
                                    if (!reader.Read())
                                    {
                                        throw new JsonException($"Bad JSON");
                                    }
                                    if (reader.TokenType == JsonTokenType.EndArray)
                                    {
                                        break;
                                    }
                                    handler.Adder!(item, JsonSerializer.Deserialize(ref reader, handler.PropertyType, options));
                                }
                            }
                            else
                            {
                                reader.Skip();
                            }
                        }
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
            }
            return item;
        }
    }
}
