namespace Tr1ppy.Result.Abstractions;

public interface IResult<TFault, TValue>
{
    internal TResult Match<TResult>(Func<TFault, TResult> onFailure, Func<TResult> onSuccess);

    internal TResult Match<TResult>(Func<TResult> onFailure, Func<TValue, TResult> onSuccess);

    internal TResult Match<TResult>(Func<TFault, TResult> onFailure, Func<TValue, TResult> onSuccess);
}