using UnityEngine;

public class SortContext<T> : ISortContext<T>
{
    private ISortStrategy<T> strategy;
    public void SetStrategy(ISortStrategy<T> strategy)
    {
        this.strategy = strategy;
    }

    public void Sort(T[] array)
    {
        strategy.Sort(array);
    }
}

