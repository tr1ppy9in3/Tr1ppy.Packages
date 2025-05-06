using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tr1ppy.Result.Types.Markers;

public class SuccessMarker<TValue>
{
    public TValue Value { get; set; }

    public SuccessMarker(TValue value)
    {
        Value = value;
    }
}
