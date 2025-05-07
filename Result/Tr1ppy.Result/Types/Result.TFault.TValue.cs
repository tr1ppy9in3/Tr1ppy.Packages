using Tr1ppy.Result.Types.Markers;

namespace Tr1ppy.Result.Types;

public class Result<TFault, TValue> : Result<TFault>
{
    protected TValue? _value;

    #region Constructors

    public Result()
    {

    }

    internal Result(TFault fault)
    {
        _fault = fault;
        _success = false;
    }

    internal Result(TValue value)
    {
        _value = value;
        _success = true;
    }

    #endregion

    #region Fabrics

    public new static Result<TFault, TValue> Fail(TFault fault)
    {
        return new Result<TFault, TValue>(fault);
    }

    public static Result<TFault, TValue> Success(TValue value)
    {
        return new Result<TFault, TValue>(value);
    }

    #endregion

    #region Operators

    public static implicit operator Result<TFault, TValue>(TValue value)
    {
        return Success(value);
    }

    public static implicit operator Result<TFault, TValue>(TFault fault)
    {
        return Fail(fault);
    }

    public static implicit operator Result<TFault, TValue>(SuccessMarker<TValue> successMarker)
    {
        return Success(successMarker.Value);
    }

    public static implicit operator Result<TFault, TValue>(FailureMarker<TFault> failureMarker)
    {
        return Fail(failureMarker.Fault);
    }

    #endregion


    public TValue GetValueOrThrow()
    {
        if (_value is null || _value.Equals(default(TValue)))
            throw new ArgumentNullException(nameof(_value));

        return _value;
    }

    public TValue? GetValueOrDefault()
    {
        return _value;
    }
}