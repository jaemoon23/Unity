using UnityEngine;
using System.Collections.Generic;
using System;

public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
{
    private List<(TElement Element, TPriority Priority)> heap;

    public PriorityQueue()
    {
        heap = new List<(TElement, TPriority)>();
    }

    public int Count => heap.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        // TODO: 구현
        // 1. 새 요소를 리스트 끝에 추가
        // 2. HeapifyUp으로 힙 속성 복구
        heap.Add((element, priority));
        HeapifyUp(Count - 1);
    }

    public TElement Dequeue()
     {
        // TODO: 구현
        // 1. 빈 큐 체크 및 예외 처리
        // 2. 루트 요소 저장
        // 3. 마지막 요소를 루트로 이동
        // 4. HeapifyDown으로 힙 속성 복구
        // 5. 저장된 루트 요소 반환

        if (Count == 0)
        {
            throw new InvalidOperationException("큐가 비어 있습니다.");
        }

        TElement el = heap[0].Element;

        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];

        heap.RemoveAt(lastIndex);

        if (Count > 0)
        {
            HeapifyDown(0);
        }
        return el;
    }

    public TElement Peek()
    {
        // TODO: 구현
        // 1. 빈 큐 체크 및 예외 처리
        // 2. 루트 요소 반환
        if (Count == 0)
        {
            throw new InvalidOperationException("큐가 비어 있습니다.");
        }
        return heap[0].Element;
    }

    public void Clear()
    {
        // TODO: 구현
        heap.Clear();
    }

    private void HeapifyUp(int index)
    {
        // TODO: 구현
        // 현재 노드가 부모보다 작으면 교환하며 위로 이동
        if (index == 0)
        {
            return;
        }

        int idx = index;

        while (idx > 0)
        {
            int parentIdx = (idx - 1) / 2;

            // 지금 삽입한 값이 부모 노드보다 작을때
            if (heap[idx].Priority.CompareTo(heap[parentIdx].Priority) < 0)
            {
                (TElement, TPriority) temp = heap[parentIdx];
                heap[parentIdx] = heap[idx];
                heap[idx] = temp;

                idx = parentIdx;
            }
            else
            {
                break;
            }
        }
    }

    private void HeapifyDown(int index)
{
    // TODO: 구현
    // 현재 노드가 자식보다 크면 더 작은 자식과 교환하며 아래로 이동

    while (true)
    {
        int left = index * 2 + 1;
        int right = index * 2 + 2;
        int small = index;
        if(left < Count && heap[small].Priority.CompareTo(heap[left].Priority) > 0)
        {
            small = left;
        }
        if (right < Count  && heap[small].Priority.CompareTo(heap[right].Priority) > 0)
        {
            small = right;
        }
        if(small == index)
        {
            break;
        }
        else
        {
            var tmp = heap[index];
            heap[index] = heap[small];
            heap[small] = tmp;
            //(heap[index], heap[small]) = (heap[small], heap[index]);
            index = small;
        }

    }
}
}
