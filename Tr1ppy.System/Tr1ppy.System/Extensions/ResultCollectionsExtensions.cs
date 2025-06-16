using Kontur.Results;

using Tr1ppy.System.Abstractions.Errors;

namespace Tr1ppy.System.Extensions;
internal static class ResultCollectionExtensions
{
    internal static Result<Error<TErrorEnum>> ExecuteForEach<TItem, TErrorEnum>(
        this IEnumerable<TItem> items,
        Func<TItem, Result<Error<TErrorEnum>>> action
    )
        where TErrorEnum : Enum
    {
        foreach (TItem item in items)
        {
            Result<Error<TErrorEnum>> result = action(item);
            if (result.TryGetFault(out Error<TErrorEnum>? fault))
            {
                return Result<Error<TErrorEnum>>.Fail(fault);
            }
        }
        return Result<Error<TErrorEnum>>.Succeed();
    }

    internal static async Task<Result<Error<TErrorEnum>>> ExecuteForEachAsync<TItem, TErrorEnum>(
        this IEnumerable<TItem> items,
        Func<TItem, Task<Result<Error<TErrorEnum>>>> action
    )
         where TErrorEnum : Enum
    {
        foreach (TItem item in items)
        {
            Result<Error<TErrorEnum>> result = await action(item);
            if (result.TryGetFault(out Error<TErrorEnum>? fault))
            {
                return Result<Error<TErrorEnum>>.Fail(fault);
            }
        }
        return Result<Error<TErrorEnum>>.Succeed();
    }
}