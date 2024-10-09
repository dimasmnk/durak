namespace Durak.Core.Common.Helpers;

public static class Guard
{
    public static T EnsureValue<T>(T value, string paramName)
    {
        return value switch
        {
            null => throw new ArgumentException($"{paramName} cannot be null.", paramName),
            not null => value
        };
    }

    public static T EnsureValue<T>(T? value, string paramName)
        where T : struct
    {
        return value switch
        {
            null => throw new ArgumentException($"{paramName} cannot be null.", paramName),
            not null => value.Value
        };
    }

    public static void IsNullOrEmpty(string? value, string paramName)
    {
        ArgumentException.ThrowIfNullOrEmpty(value, paramName);
    }
}