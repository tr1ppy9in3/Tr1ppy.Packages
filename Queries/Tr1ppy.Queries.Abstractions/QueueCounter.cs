namespace Tr1ppy.Queries;

public class QueueCounter
{
    private int _counter = 0;

    public int Value => Volatile.Read(ref _counter);

    public void Increment() => 
        Interlocked.Increment(ref _counter);

    public void Decrement() => 
        Interlocked.Decrement(ref _counter);
}
