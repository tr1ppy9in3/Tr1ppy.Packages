namespace Tr1ppy.Result.Abstractions;

public interface IResult<out TFault>
{
    internal TResult Match<TResult>(Func<TFault, TResult> onFailure, Func<TResult> onSuccess);
}
