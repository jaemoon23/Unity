using System.Collections.Generic;

public class SelectionSortStrategy<T> : ISortStrategy<T>
{
    private readonly IComparer<T> comparer = Comparer<T>.Default;

    public SelectionSortStrategy() : this(Comparer<T>.Default)
    {
    }

    public SelectionSortStrategy(IComparer<T> customComparer)
    {
        comparer = customComparer;     
    }

    public void Sort(T[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            int minIndex = i;  // 최솟값의 인덱스를 현재 위치로 초기화
            
            // i+1부터 끝까지 탐색하며 최솟값 찾기
            for (int j = i + 1; j < array.Length; j++)
            {
                if (comparer.Compare(array[j], array[minIndex]) < 0)
                {
                    minIndex = j;  // 더 작은 값 발견 시 인덱스 갱신
                }
            }
            
            // 최솟값을 현재 위치와 교환
            if (minIndex != i)
            {
                T temp = array[i];
                array[i] = array[minIndex];
                array[minIndex] = temp;
            }
        }
    }
}
