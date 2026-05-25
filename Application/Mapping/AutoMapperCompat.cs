using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper;

public interface IMapper
{
    TDestination Map<TDestination>(object source);
    object? Map(object? source, Type sourceType, Type destinationType);
}

public interface IMapperConfigurationExpression
{
    string? LicenseKey { get; set; }
}

public abstract class Profile
{
    internal List<MapDefinition> Definitions { get; } = new();

    protected MappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
    {
        var definition = new MapDefinition(typeof(TSource), typeof(TDestination));
        Definitions.Add(definition);
        return new MappingExpression<TSource, TDestination>(definition, Definitions);
    }

    protected MappingExpression CreateMap(Type sourceType, Type destinationType)
    {
        var definition = new MapDefinition(sourceType, destinationType);
        Definitions.Add(definition);
        return new MappingExpression(definition, Definitions);
    }
}

public sealed class MapperConfigurationExpression : IMapperConfigurationExpression
{
    public string? LicenseKey { get; set; }
}

public sealed class MappingExpression<TSource, TDestination>
{
    private readonly MapDefinition _definition;
    private readonly List<MapDefinition> _definitions;

    internal MappingExpression(MapDefinition definition, List<MapDefinition> definitions)
    {
        _definition = definition;
        _definitions = definitions;
    }

    public MappingExpression<TSource, TDestination> ForMember(
        Expression<Func<TDestination, object?>> destinationMember,
        Action<MemberOptions<TSource>> memberOptions)
    {
        var memberName = ReflectionHelpers.GetMemberName(destinationMember);
        var options = new MemberOptions<TSource>(_definition, memberName);
        memberOptions(options);
        return this;
    }

    public MappingExpression<TSource, TDestination> ReverseMap()
    {
        var reverse = _definition.CreateReverse();
        _definitions.Add(reverse);
        return this;
    }
}

public sealed class MappingExpression
{
    private readonly MapDefinition _definition;
    private readonly List<MapDefinition> _definitions;

    internal MappingExpression(MapDefinition definition, List<MapDefinition> definitions)
    {
        _definition = definition;
        _definitions = definitions;
    }

    public MappingExpression ForMember(string destinationMember, Action<MemberOptions> memberOptions)
    {
        var options = new MemberOptions(_definition, destinationMember);
        memberOptions(options);
        return this;
    }

    public MappingExpression ReverseMap()
    {
        var reverse = _definition.CreateReverse();
        _definitions.Add(reverse);
        return this;
    }
}

public sealed class MemberOptions<TSource>
{
    private readonly MapDefinition _definition;
    private readonly string _memberName;

    internal MemberOptions(MapDefinition definition, string memberName)
    {
        _definition = definition;
        _memberName = memberName;
    }

    public void Ignore()
    {
        _definition.IgnoredMembers.Add(_memberName);
    }

    public void MapFrom(Expression<Func<TSource, object?>> sourceMember)
    {
        _definition.CustomMappings[_memberName] = new MemberMap(
            ValueFactory: source => sourceMember.Compile().Invoke((TSource)source!),
            SourcePath: ReflectionHelpers.GetMemberPath(sourceMember));
    }
}

public sealed class MemberOptions
{
    private readonly MapDefinition _definition;
    private readonly string _memberName;

    internal MemberOptions(MapDefinition definition, string memberName)
    {
        _definition = definition;
        _memberName = memberName;
    }

    public void Ignore()
    {
        _definition.IgnoredMembers.Add(_memberName);
    }

    public void MapFrom(string sourceMember)
    {
        _definition.CustomMappings[_memberName] = new MemberMap(
            ValueFactory: source => ReflectionHelpers.GetValueByPath(source, sourceMember),
            SourcePath: sourceMember);
    }
}

internal sealed class RuntimeMapper : IMapper
{
    private readonly List<MapDefinition> _definitions;

    public RuntimeMapper(IEnumerable<MapDefinition> definitions)
    {
        _definitions = definitions.ToList();
    }

    public TDestination Map<TDestination>(object source)
    {
        return (TDestination)Map(source, source.GetType(), typeof(TDestination))!;
    }

    public object? Map(object? source, Type sourceType, Type destinationType)
    {
        if (source is null)
        {
            return null;
        }

        return MapInternal(source, sourceType, destinationType);
    }

    private object? MapInternal(object? source, Type sourceType, Type destinationType)
    {
        if (source is null)
        {
            return null;
        }

        var underlyingDestinationType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;

        if (underlyingDestinationType.IsAssignableFrom(sourceType))
        {
            return source;
        }

        if (TryConvertSimpleValue(source, underlyingDestinationType, out var converted))
        {
            return converted;
        }

        if (TryMapEnumerable(source, sourceType, underlyingDestinationType, out var mappedEnumerable))
        {
            return mappedEnumerable;
        }

        var definition = FindDefinition(sourceType, underlyingDestinationType);
        var destination = Activator.CreateInstance(underlyingDestinationType)
            ?? throw new InvalidOperationException($"No se pudo crear {underlyingDestinationType.FullName}.");

        foreach (var destinationProperty in underlyingDestinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!destinationProperty.CanWrite || definition.IgnoredMembers.Contains(destinationProperty.Name))
            {
                continue;
            }

            object? sourceValue = null;
            if (definition.CustomMappings.TryGetValue(destinationProperty.Name, out var memberMap))
            {
                sourceValue = memberMap.ValueFactory(source);
            }
            else
            {
                sourceValue = ReflectionHelpers.GetValueByPath(source, destinationProperty.Name);
            }

            if (sourceValue is null)
            {
                continue;
            }

            var mappedValue = MapValue(sourceValue, destinationProperty.PropertyType);
            destinationProperty.SetValue(destination, mappedValue);
        }

        return destination;
    }

    private object? MapValue(object sourceValue, Type destinationType)
    {
        var sourceType = sourceValue.GetType();

        if (destinationType.IsAssignableFrom(sourceType))
        {
            return sourceValue;
        }

        if (TryConvertSimpleValue(sourceValue, Nullable.GetUnderlyingType(destinationType) ?? destinationType, out var converted))
        {
            return converted;
        }

        if (TryMapEnumerable(sourceValue, sourceType, destinationType, out var mappedEnumerable))
        {
            return mappedEnumerable;
        }

        return MapInternal(sourceValue, sourceType, destinationType);
    }

    private bool TryMapEnumerable(object sourceValue, Type sourceType, Type destinationType, out object? mapped)
    {
        mapped = null;

        if (sourceValue is string || !typeof(IEnumerable).IsAssignableFrom(sourceType) || destinationType == typeof(string))
        {
            return false;
        }

        var destinationElementType = ReflectionHelpers.GetCollectionElementType(destinationType);
        if (destinationElementType is null)
        {
            return false;
        }

        var listType = typeof(List<>).MakeGenericType(destinationElementType);
        var list = (IList)Activator.CreateInstance(listType)!;

        foreach (var item in (IEnumerable)sourceValue)
        {
            list.Add(item is null ? null! : MapValue(item, destinationElementType));
        }

        if (destinationType.IsArray)
        {
            var array = Array.CreateInstance(destinationElementType, list.Count);
            list.CopyTo(array, 0);
            mapped = array;
            return true;
        }

        if (destinationType.IsAssignableFrom(listType))
        {
            mapped = list;
            return true;
        }

        if (destinationType.IsGenericType)
        {
            var genericDefinition = destinationType.GetGenericTypeDefinition();
            if (genericDefinition == typeof(IReadOnlyList<>) ||
                genericDefinition == typeof(IEnumerable<>) ||
                genericDefinition == typeof(ICollection<>) ||
                genericDefinition == typeof(IList<>))
            {
                mapped = list;
                return true;
            }
        }

        if (!destinationType.IsAbstract && destinationType.GetConstructor(Type.EmptyTypes) is not null)
        {
            var instance = Activator.CreateInstance(destinationType);
            if (instance is IList destinationList)
            {
                foreach (var item in list)
                {
                    destinationList.Add(item);
                }

                mapped = instance;
                return true;
            }
        }

        return false;
    }

    private static bool TryConvertSimpleValue(object sourceValue, Type destinationType, out object? converted)
    {
        converted = null;

        if (destinationType == typeof(string))
        {
            converted = sourceValue.ToString();
            return true;
        }

        if (destinationType.IsEnum)
        {
            if (sourceValue is string enumName)
            {
                converted = Enum.Parse(destinationType, enumName, ignoreCase: true);
                return true;
            }

            converted = Enum.ToObject(destinationType, sourceValue);
            return true;
        }

        if (destinationType == typeof(DateOnly) && sourceValue is DateTime dateTime)
        {
            converted = DateOnly.FromDateTime(dateTime);
            return true;
        }

        if (destinationType == typeof(DateOnly?) && sourceValue is DateTime nullableDateTime)
        {
            converted = DateOnly.FromDateTime(nullableDateTime);
            return true;
        }

        if (sourceValue is IConvertible && typeof(IConvertible).IsAssignableFrom(destinationType))
        {
            converted = Convert.ChangeType(sourceValue, destinationType);
            return true;
        }

        return false;
    }

    private MapDefinition FindDefinition(Type sourceType, Type destinationType)
    {
        var exact = _definitions.LastOrDefault(d => d.SourceType == sourceType && d.DestinationType == destinationType);
        if (exact is not null)
        {
            return exact;
        }

        if (sourceType.IsGenericType && destinationType.IsGenericType)
        {
            var sourceGeneric = sourceType.GetGenericTypeDefinition();
            var destinationGeneric = destinationType.GetGenericTypeDefinition();
            var openGeneric = _definitions.LastOrDefault(d => d.SourceType == sourceGeneric && d.DestinationType == destinationGeneric);
            if (openGeneric is not null)
            {
                return openGeneric;
            }
        }

        return new MapDefinition(sourceType, destinationType);
    }
}

internal sealed class MapDefinition
{
    public Type SourceType { get; }
    public Type DestinationType { get; }
    public HashSet<string> IgnoredMembers { get; } = new(StringComparer.Ordinal);
    public Dictionary<string, MemberMap> CustomMappings { get; } = new(StringComparer.Ordinal);

    public MapDefinition(Type sourceType, Type destinationType)
    {
        SourceType = sourceType;
        DestinationType = destinationType;
    }

    public MapDefinition CreateReverse()
    {
        var reverse = new MapDefinition(DestinationType, SourceType);
        foreach (var ignored in IgnoredMembers)
        {
            reverse.IgnoredMembers.Add(ignored);
        }

        foreach (var mapping in CustomMappings)
        {
            if (!string.IsNullOrWhiteSpace(mapping.Value.SourcePath))
            {
                reverse.CustomMappings[mapping.Value.SourcePath!] = new MemberMap(
                    ValueFactory: source => ReflectionHelpers.GetValueByPath(source, mapping.Key),
                    SourcePath: mapping.Key);
            }
        }

        return reverse;
    }
}

internal sealed record MemberMap(Func<object?, object?> ValueFactory, string? SourcePath);

internal static class ReflectionHelpers
{
    public static string GetMemberName(LambdaExpression expression)
    {
        return expression.Body switch
        {
            MemberExpression member => member.Member.Name,
            UnaryExpression { Operand: MemberExpression member } => member.Member.Name,
            _ => throw new NotSupportedException($"Expresión no soportada: {expression}.")
        };
    }

    public static string? GetMemberPath(LambdaExpression expression)
    {
        var members = new Stack<string>();
        Expression? current = expression.Body;

        if (current is UnaryExpression unary)
        {
            current = unary.Operand;
        }

        while (current is MemberExpression memberExpression)
        {
            members.Push(memberExpression.Member.Name);
            current = memberExpression.Expression;
        }

        return members.Count == 0 ? null : string.Join(".", members);
    }

    public static object? GetValueByPath(object? source, string path)
    {
        if (source is null)
        {
            return null;
        }

        object? current = source;
        foreach (var segment in path.Split('.'))
        {
            if (current is null)
            {
                return null;
            }

            var property = current.GetType().GetProperty(segment, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property is null)
            {
                return null;
            }

            current = property.GetValue(current);
        }

        return current;
    }

    public static Type? GetCollectionElementType(Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        if (type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            if (genericArgs.Length == 1 && typeof(IEnumerable).IsAssignableFrom(type))
            {
                return genericArgs[0];
            }
        }

        var enumerableInterface = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        return enumerableInterface?.GetGenericArguments()[0];
    }
}
