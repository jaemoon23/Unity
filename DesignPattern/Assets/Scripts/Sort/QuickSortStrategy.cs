using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickSortStrategy<T> : ISortStrategy<T>
{
    private readonly IComparer<T> comparer = Comparer<T>.Default;

    public QuickSortStrategy() : this(Comparer<T>.Default)
    {
    }

    public QuickSortStrategy(IComparer<T> customComparer)
    {
        comparer = customComparer;     
    }
    public void Sort(T[] array)
    {
        Debug.Log("아직 미구현 quick sort");
    }
}
