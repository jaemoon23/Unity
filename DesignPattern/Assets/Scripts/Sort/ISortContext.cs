using System;
using UnityEngine;

public interface ISortContext<T>
{
    void SetStrategy(ISortStrategy<T> strategy);
    void Sort(T[] array);
}
