using Tr1ppy.Result.Types.Markers;

namespace Tr1ppy.Result.Types;

public static class Result
{
    public static FailureMarker<TFault> Fail<TFault>(TFault fault)
    {
        return new FailureMarker<TFault>(fault);    
    }

    public static SuccessMarker Success()
    {
        return new SuccessMarker();
    }

    public static SuccessMarker<TValue> Success<TValue>(TValue value)
    {
        return new SuccessMarker<TValue>(value);
    }
}
