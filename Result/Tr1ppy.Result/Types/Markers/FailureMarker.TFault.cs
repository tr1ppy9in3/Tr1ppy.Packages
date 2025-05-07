namespace Tr1ppy.Result.Types.Markers;

public class FailureMarker<TFault>
{
    public TFault Fault { get; }

    public FailureMarker(TFault fault)
    {
        Fault = fault;
    }
}
