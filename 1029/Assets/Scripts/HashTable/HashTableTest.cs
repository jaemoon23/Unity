using UnityEngine;

public class HashTableTest : MonoBehaviour
{

    void Start()
    {
        ChainingHashTable<string, int> hashTable = new ChainingHashTable<string, int>()
        {
            // 추가
            { "사과", 1 },
            { "바나나", 2 },
            { "오렌지", 3 }
        };

        // 값 검색
        Debug.Log(hashTable["사과"]);
        Debug.Log(hashTable["바나나"]);
        Debug.Log(hashTable["오렌지"]);

        // 키 존재 여부 확인
        Debug.Log(hashTable.ContainsKey("바나나"));
        Debug.Log(hashTable.ContainsKey("포도"));

        // 키-값 쌍 제거
        hashTable.Remove("바나나");
        Debug.Log(hashTable.ContainsKey("바나나"));

        // 리사이즈 테스트
        for (int i = 0; i < 20; i++)
        {
            hashTable.Add("키" + i, i);
        }
    }


    // private void Start()
    // {
    //     var hashTable = new SimpleHashTable<string, int>();
    //     hashTable.Add("one", 1);
    //     hashTable.Add("two", 2);
    //     hashTable.Add("three", 3);

    //     Debug.Log($"Key: one, Value: {hashTable["one"]}");
    //     Debug.Log($"Key: two, Value: {hashTable["two"]}");
    //     Debug.Log($"Key: three, Value: {hashTable["three"]}");

    //     Debug.Log($"Contains Key 'two': {hashTable.ContainsKey("two")}");
    //     Debug.Log($"Contains Key 'four': {hashTable.ContainsKey("four")}");

    //     Debug.Log($"Count: {hashTable.Count}");

    //     Debug.Log($"Remove: {hashTable.Remove("two")}");
    //     Debug.Log($"Contains Key 'two' after removal: {hashTable.ContainsKey("two")}");

    //     foreach (var key in hashTable.Keys)
    //     {
    //         Debug.Log($"Key in HashTable: {key}");
    //     }

    //     // 리사이즈 확인
    //     for (int i = 4; i <= 20; i++)
    //     {
    //         hashTable.Add($"number_{i}", i);
    //     }
    // }




    // private void Start()
    // {
    //     var hashTable = new SimpleHashTable<string, int>();

    //     // 기본 추가
    //     hashTable.Add("one", 1);
    //     hashTable.Add("two", 2);
    //     hashTable.Add("three", 3);

    //     Debug.Log($"Key: one, Value: {hashTable["one"]}");
    //     Debug.Log($"Key: two, Value: {hashTable["two"]}");
    //     Debug.Log($"Key: three, Value: {hashTable["three"]}");

    //     // 포함 여부 확인
    //     Debug.Log($"Contains Key 'two': {hashTable.ContainsKey("two")}");
    //     Debug.Log($"Contains Key 'four': {hashTable.ContainsKey("four")}");

    //     // 현재 개수 확인
    //     Debug.Log($"Count: {hashTable.Count}");

    //     // 삭제 테스트
    //     Debug.Log($"Remove: {hashTable.Remove("two")}");
    //     Debug.Log($"Contains Key 'two' after removal: {hashTable.ContainsKey("two")}");

    //     // 키 출력
    //     foreach (var key in hashTable.Keys)
    //     {
    //         Debug.Log($"Key in HashTable: {key}");
    //     }

    //     // 리사이즈 테스트
    //     for (int i = 4; i <= 20; i++)
    //     {
    //         hashTable.Add($"number_{i}", i);
    //     }

    //     Debug.Log($"리사이즈 후 개수: {hashTable.Count}");

    //     foreach (var kvp in hashTable)
    //     {
    //         Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
    //     }

    //     // TryGetValue 테스트
    //     if (hashTable.TryGetValue("number_10", out int value10))
    //     {
    //         Debug.Log($"TryGetValue 성공: number_10 = {value10}");
    //     }
    //     else
    //     {
    //         Debug.Log("TryGetValue 실패: number_10 없음");
    //     }

    //     // Contains 테스트
    //     Debug.Log($"Contains KeyValuePair(\"one\", 1): {hashTable.Contains(new KeyValuePair<string, int>("one", 1))}");

    //     // CopyTo 테스트
    //     var array = new KeyValuePair<string, int>[hashTable.Count];
    //     hashTable.CopyTo(array, 0);
    //     Debug.Log("CopyTo 결과:");
    //     foreach (var kvp in array)
    //     {
    //         Debug.Log($"{kvp.Key} : {kvp.Value}");
    //     }

    //     // Clear 테스트
    //     hashTable.Clear();
    //     Debug.Log($"Clear 후 Count: {hashTable.Count}");
    // }
}
