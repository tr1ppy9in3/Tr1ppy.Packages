using Tr1ppy.Result.Types.Markers;

namespace Tr1ppy.Result.Types;

public class Result<TFault> 
{
    protected bool _success;
    protected TFault? _fault;

    public bool IsSuccess => _success;
    public bool IsFailure => !_success;


    #region Constructors

    public Result()
    {
        _success = true;
    }

    public Result(TFault fault)
    {
        _fault = fault;
        _success = false;
    }

    #endregion

    #region Operators

    public static implicit operator Result<TFault>(SuccessMarker _)
    {
        return Success();
    }

    public static implicit operator Result<TFault>(TFault fault)
    {
        return Fail(fault);
    }

    public static implicit operator Result<TFault>(FailureMarker<TFault> failureMarker)
    {
        return Fail(failureMarker.Fault);
    }

    #endregion

    #region Fabrics


    public static Result<TFault> Fail(TFault fault)
    {
        return new Result<TFault>(fault);
    }

    public static Result<TFault> Success()
    {
        return new Result<TFault>();
    }


    #endregion

    public TFault GetFaultOrThrow()
    {
        if (_fault is null || _fault.Equals(default(TFault))) 
            throw new ArgumentNullException(nameof(_fault));

        return _fault;
    }

    public TFault? GetFaultOrDefault()
    {
        return _fault;
    }
}