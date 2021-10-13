using System;

public static class GenericLib
{
    public static T As<T>(this object toDownCast) where T : class
    {
        return toDownCast as T;
    }

    public static void As<T>(this object toDownCast, out T returnVariable) where T : struct
    {
        try
        {
            returnVariable = (T)toDownCast;
        }
        catch (InvalidCastException)
        {
            returnVariable = default;
        }
    }
}
