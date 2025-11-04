using System.Collections.Generic;

public class BubbleSortStrategy<T> : ISortStrategy<T>
{
    private readonly IComparer<T> comparer = Comparer<T>.Default;

    public BubbleSortStrategy() : this(Comparer<T>.Default)
    {
    }

    public BubbleSortStrategy(IComparer<T> customComparer)
    {
        comparer = customComparer;     
    }

    public void Sort(T[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            for (int j = 0; j < array.Length - i - 1; j++)
            {
                if (comparer.Compare(array[j], array[j + 1]) > 0)
                {
                    T temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;
                }
            }
        }
    }
}
