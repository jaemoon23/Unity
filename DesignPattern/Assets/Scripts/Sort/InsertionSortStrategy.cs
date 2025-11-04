using System;
using System.Collections.Generic;

public class InsertionSortStrategy<T> : ISortStrategy<T>
{
    private readonly IComparer<T> comparer = Comparer<T>.Default;

    public InsertionSortStrategy() : this(Comparer<T>.Default)
    {
    }
    public InsertionSortStrategy(IComparer<T> customComparer)
    {
        comparer = customComparer;     
    }
    public void Sort(T[] array)
    {   
        // array {5, 2, 9, 1, 5, 6} 가정하고 보면
        // 1 사이클 array {2, 5, 9, 1, 5, 6}
        // 2 사이클 array {2, 5, 9, 1, 5, 6}    9는 5보다 크니까 while문 조건 아님
        // 3 사이클 array {1, 2, 5, 9, 5, 6}

        for (int i = 1; i < array.Length; i++)
        {
            T key = array[i];   // key = 2 (idx 1) > 9 (idx 2) > 1 (idx 3)
            int j = i - 1;      // j = 0 (idx 0) > 1 (idx 1) > 2 (idx 2)

            while (j >= 0 && comparer.Compare(array[j], key) > 0)
            {
                array[j + 1] = array[j];
                j--;
            }
            array[j + 1] = key;
        }
    }
}
