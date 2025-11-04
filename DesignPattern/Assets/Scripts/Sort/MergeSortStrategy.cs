using System;
using System.Collections.Generic;
using UnityEngine;

public class MergeSortStrategy<T> : ISortStrategy<T>
{
    private readonly IComparer<T> comparer = Comparer<T>.Default;

    public MergeSortStrategy() : this(Comparer<T>.Default)
    {
    }

    public MergeSortStrategy(IComparer<T> customComparer)
    {
        comparer = customComparer;     
    }
    public void Sort(T[] array)
    {
        Debug.Log("아직 미구현 merge sort");
    }
    
}
