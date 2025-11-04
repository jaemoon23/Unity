using System.Collections.Generic;
using UnityEngine;

public class HeapSortStrategy<T> : ISortStrategy<T>
{
    private readonly IComparer<T> comparer = Comparer<T>.Default;
    public HeapSortStrategy() : this(Comparer<T>.Default)
    {
    }
    public HeapSortStrategy(IComparer<T> customComparer)
    {
        comparer = customComparer;     
    }
    public void Sort(T[] array)
    {


        // 힙 생성
        for (int i = array.Length / 2 - 1; i >= 0; i--)
        {
            Heapify(array, array.Length, i);
        }

        // 힙에서 요소를 하나씩 추출
        for (int i = array.Length - 1; i > 0; i--)
        {
            // 현재 루트(최대값)를 끝으로 이동
            T temp = array[0];
            array[0] = array[i];
            array[i] = temp;

            // 감소된 힙에 대해 힙 속성 재적용
            Heapify(array, i, 0);
        }
    }
    
    private void Heapify(T[] array, int n, int i)
    {
        int largest = i; // 루트를 가장 큰 값으로 초기화
        int left = 2 * i + 1; // 왼쪽 자식 인덱스
        int right = 2 * i + 2; // 오른쪽 자식 인덱스

        // 왼쪽 자식이 루트보다 크면 largest 업데이트
        if (left < n && comparer.Compare(array[left], array[largest]) > 0)
        {
            largest = left;
        }

        // 오른쪽 자식이 현재 largest보다 크면 largest 업데이트
        if (right < n && comparer.Compare(array[right], array[largest]) > 0)
        {
            largest = right;
        }

        
        if (largest != i)   // largest가 루트가 아니면 교환
        {
            T swap = array[i];
            array[i] = array[largest];
            array[largest] = swap;

            Heapify(array, n, largest);
        }
    }
}
