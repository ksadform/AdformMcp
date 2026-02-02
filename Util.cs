namespace AdformMcp;

public static class Util
{
    public static (T? data, Exception? error) Unwrap<T>(Func<T> func)
    {
        try
        {
            var result = func();
            return (result, null);
        }
        catch (Exception ex)
        {
            return (default(T), ex);
        }
    }

    public static async Task<(T? data, Exception? error)> Unwrap<T>(Func<Task<T>> func)
    {
        try
        {
            var result = await func();
            return (result, null);
        }
        catch (Exception ex)
        {
            return (default(T), ex);
        }
    }

    public static bool HasError(Exception? exception)
    {
        return exception != null;
    }

}
