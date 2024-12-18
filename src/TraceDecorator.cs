using System.Diagnostics;
using System.Reflection;

namespace Gainsway.Observability;

public class TraceDecorator<TDecorated> : DispatchProxy
{
    private TDecorated? _decorated = default!;

    public static TDecorated Create(TDecorated decorated)
    {
        object proxy = Create<TDecorated, TraceDecorator<TDecorated>>()!;
        ((TraceDecorator<TDecorated>)proxy!).SetParameters(decorated);

        return (TDecorated)proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null)
        {
            throw new ArgumentNullException(nameof(targetMethod));
        }

        var className = _decorated!.GetType().FullName;
        var activity =
            className?.StartsWith("generated") ?? false
                ? null
                : Activity.Current?.Source.StartActivity($"{className}.{targetMethod.Name}");

        try
        {
            var result = targetMethod.Invoke(_decorated, args);

            if (result is Task task)
            {
                var resultType = targetMethod.ReturnType;

                if (
                    resultType.IsGenericType
                    && resultType.GetGenericTypeDefinition() == typeof(Task<>)
                )
                {
                    var resultTaskType = resultType.GetGenericArguments()[0];

                    var handleAsyncMethodGeneric = typeof(TraceDecorator<TDecorated>)
                        .GetMethod(
                            nameof(HandleAsyncMethod),
                            BindingFlags.NonPublic | BindingFlags.Instance
                        )
                        ?.MakeGenericMethod(resultTaskType);

                    if (handleAsyncMethodGeneric != null)
                    {
                        return handleAsyncMethodGeneric.Invoke(this, [activity, task]);
                    }
                }

                return HandleVoidAsyncMethod(activity, task);
            }

            activity?.Stop();
            return result;
        }
        catch (Exception e)
        {
            activity?.AddException(e);
            throw;
        }
    }

    private async Task HandleVoidAsyncMethod(Activity? activity, Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception e)
        {
            activity?.AddException(e);
            throw;
        }
        finally
        {
            activity?.Stop();
        }
    }

    private async Task<TResult> HandleAsyncMethod<TResult>(Activity? activity, Task<TResult> task)
    {
        try
        {
            TResult result = await task.ConfigureAwait(false);
            return result;
        }
        catch (Exception e)
        {
            activity?.AddException(e);
            throw;
        }
        finally
        {
            activity?.Stop();
        }
    }

    private void SetParameters(TDecorated decorated)
    {
        _decorated = decorated;
    }
}
