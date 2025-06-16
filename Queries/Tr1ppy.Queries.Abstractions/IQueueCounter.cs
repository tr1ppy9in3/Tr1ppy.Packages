namespace Tr1ppy.Queries.Abstractions;

public interface IQueueCounter
{
    int Value { get; }

    void Increment();

    void Decrement();
}
