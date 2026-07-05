using CRM_B.Application.Abstractions.Messaging;

namespace CRM_B.Application.Behaviors.Internal;

internal static class CommandMarker<TRequest>
{
    public static readonly bool IsCommand = Detect();

    private static bool Detect()
    {
        if (typeof(ICommand<>).IsAssignableFrom(typeof(TRequest))) return true;

        foreach (var i in typeof(TRequest).GetInterfaces())
            if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>))
                return true;

        return false;
    }
}