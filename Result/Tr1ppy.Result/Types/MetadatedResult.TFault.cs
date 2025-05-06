namespace Tr1ppy.Result.Types;

public class MetadatedResult<TMetadata, TFault> : Result<TFault> 
    where TMetadata: class
{
}
