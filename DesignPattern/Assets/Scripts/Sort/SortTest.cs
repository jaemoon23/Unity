using System;
using System.Collections.Generic;
using UnityEngine;

public class SortingStrategyTest : MonoBehaviour
{
    // 테스트할 문자열 배열
    private string[] wordsToSort = { "banana", "apple", "pear", "orange", "grape", "kiwi" };

    private void Start()
    {
        Debug.Log("===== 문자열 배열 정렬 테스트 =====");

        // 정렬 컨텍스트 생성
        ISortContext<string> sortContext = new SortContext<string>();

        // 정렬 전 배열 출력
        Debug.Log("정렬 전: " + string.Join(", ", wordsToSort));

        // 버블 정렬 테스트
        string[] bubbleSortArray = new string[wordsToSort.Length];
        Array.Copy(wordsToSort, bubbleSortArray, wordsToSort.Length);
        sortContext.SetStrategy(new BubbleSortStrategy<string>());
        sortContext.Sort(bubbleSortArray);
        Debug.Log("버블 정렬 결과: " + string.Join(", ", bubbleSortArray));

        // 삽입 정렬 테스트
        string[] insertionSortArray = new string[wordsToSort.Length];
        Array.Copy(wordsToSort, insertionSortArray, wordsToSort.Length);
        sortContext.SetStrategy(new InsertionSortStrategy<string>());
        sortContext.Sort(insertionSortArray);
        Debug.Log("삽입 정렬 결과: " + string.Join(", ", insertionSortArray));

        // 선택 정렬 테스트
        string[] selectionSortArray = new string[wordsToSort.Length];
        Array.Copy(wordsToSort, selectionSortArray, wordsToSort.Length);
        sortContext.SetStrategy(new SelectionSortStrategy<string>());
        sortContext.Sort(selectionSortArray);
        Debug.Log("선택 정렬 결과: " + string.Join(", ", selectionSortArray));

        // 병합 정렬 테스트
        string[] mergeSortArray = new string[wordsToSort.Length];
        Array.Copy(wordsToSort, mergeSortArray, wordsToSort.Length);
        sortContext.SetStrategy(new MergeSortStrategy<string>());
        sortContext.Sort(mergeSortArray);
        Debug.Log("병합 정렬 결과: " + string.Join(", ", mergeSortArray));

        // 퀵 정렬 테스트
        string[] quickSortArray = new string[wordsToSort.Length];
        Array.Copy(wordsToSort, quickSortArray, wordsToSort.Length);
        sortContext.SetStrategy(new QuickSortStrategy<string>());
        sortContext.Sort(quickSortArray);
        Debug.Log("퀵 정렬 결과: " + string.Join(", ", quickSortArray));

        // 힙 정렬 테스트
        string[] heapSortArray = new string[wordsToSort.Length];
        Array.Copy(wordsToSort, heapSortArray, wordsToSort.Length);
        sortContext.SetStrategy(new HeapSortStrategy<string>());
        sortContext.Sort(heapSortArray);
        Debug.Log("힙 정렬 결과: " + string.Join(", ", heapSortArray));

        // 커스텀 비교자 테스트 (문자열 길이 기준 정렬)
        Debug.Log("\\n===== 커스텀 비교자 테스트 (문자열 길이 기준) =====");
        string[] lengthSortArray = new string[wordsToSort.Length];
        Array.Copy(wordsToSort, lengthSortArray, wordsToSort.Length);
        IComparer<string> lengthComparer = new StringLengthComparer();
        sortContext.SetStrategy(new QuickSortStrategy<string>(lengthComparer));
        sortContext.Sort(lengthSortArray);
        Debug.Log("문자열 길이 기준 정렬 결과: " + string.Join(", ", lengthSortArray));
    }

    // 문자열 길이 기준으로 정렬하는 커스텀 비교자
    private class StringLengthComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return x.Length.CompareTo(y.Length);
        }
    }
}
