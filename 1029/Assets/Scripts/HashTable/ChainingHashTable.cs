using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    LinkedList<KeyValuePair<TKey, TValue>>[] table;
    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.75;
    private int size;
    public int Size => size;
    public bool isSizeChanged { get; set; }
    private int count;

    public ChainingHashTable()
    {
        size = DefaultCapacity;
        table = new LinkedList<KeyValuePair<TKey, TValue>>[size];
        count = 0;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            throw new KeyNotFoundException("키를 찾을 수 없습니다.");
        }
        set
        {
            if (key == null)    // 키 널체크
            {
                throw new ArgumentNullException(nameof(key));
            }

            if ((double)count / size >= LoadFactor) // 적재율 초과 시 리사이즈
            {
                Resize();
            }

            int bucket = GetBucket(key);  // 버킷 계산

            if (table[bucket] == null)   // 해당 인덱스에 LinkedList가 없으면 새로 생성
            {
                table[bucket] = new LinkedList<KeyValuePair<TKey, TValue>>();
            }

            // 중복 키 검사
            foreach (var kvp in table[bucket])
            {
                if (kvp.Key.Equals(key))
                {
                    table[bucket].Remove(kvp);
                    table[bucket].AddLast(new KeyValuePair<TKey, TValue>(key, value));
                    return;
                }
            }

            // 중복 키가 없으면 새 키-값 쌍 추가
            table[bucket].AddLast(new KeyValuePair<TKey, TValue>(key, value));
            count++;
        }
    }
    

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>();
            for (int i = 0; i < size; i++)
            {
                if (table[i] != null)   // 해당 인덱스에 LinkedList가 존재하면
                {
                    foreach (var kvp in table[i])
                    {
                        keys.Add(kvp.Key);
                    }
                }
            }
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var values = new List<TValue>();
            for (int i = 0; i < size; i++)
            {
                if (table[i] != null)   // 해당 인덱스에 LinkedList가 존재하면
                {
                    foreach (var kvp in table[i])
                    {
                        values.Add(kvp.Value);
                    }
                }
            }
            return values;
        }
    }

    public int Count => count;

    public bool IsReadOnly => false;

    public LinkedList<KeyValuePair<TKey, TValue>> GetlistForKey(TKey key)
    {
        return table[GetBucket(key)];
    }
    public int GetBucket(TKey key, int size)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        int hash = key.GetHashCode();   // 해시 코드 계산
        return Mathf.Abs(hash) % size;  // 인덱스 계산
    }

    public int GetBucket(TKey key)
    {
        return GetBucket(key, size);
    }

    public void Resize()
    {
        int newSize = size * 2;
        var newTable = new LinkedList<KeyValuePair<TKey, TValue>>[newSize];

        for (int i = 0; i < size; i++)
        {
            if (table[i] != null)   // 해시 테이블 안에 링크드리스트가 존재 하는 경우
            {
                foreach (var kvp in table[i])   // 해당 링크드리스트의 모든 키-값 쌍에 대해
                {
                    int newBucket = GetBucket(kvp.Key, newSize);  // 새로운 테이블의 버킷 계산

                    if (newTable[newBucket] == null) // 해당 인덱스에 LinkedList가 없으면 새로 생성
                    {
                        newTable[newBucket] = new LinkedList<KeyValuePair<TKey, TValue>>();
                    }
                    newTable[newBucket].AddLast(kvp);    // 새로운 테이블의 해당 인덱스(LinkedList)에 키-값 쌍 추가
                }
            }
        }
        table = newTable;
        size = newSize;
        isSizeChanged = true;
    }

    public void Add(TKey key, TValue value)
    {
        if (key == null)    // 키 널체크
        {
            throw new ArgumentNullException(nameof(key));
        }

        if ((double)count / size >= LoadFactor) // 적재율 초과 시 리사이즈
        {
            Resize();
        }

        int bucket = GetBucket(key);  // 버킷 계산

        if (table[bucket] == null)   // 해당 인덱스에 LinkedList가 없으면 새로 생성
        {
            table[bucket] = new LinkedList<KeyValuePair<TKey, TValue>>();
        }

        // 중복 키 검사
        foreach (var kvp in table[bucket])
        {
            if (kvp.Key.Equals(key))
            {
                throw new ArgumentException("키 중복"); // LinkedList안에 같은 키가 있으면 예외 발생
            }
        }

        // 중복 키가 없으면 새 키-값 쌍 추가
        table[bucket].AddLast(new KeyValuePair<TKey, TValue>(key, value));
        count++;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        for (int i = 0; i < size; i++)
        {
            table[i] = null;
        }
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int bucket = GetBucket(item.Key);  // 버킷 계산

        if (table[bucket] != null)    // 해당 인덱스에 LinkedList가 존재하면
        {
            foreach (var kvp in table[bucket])   // 해당 LinkedList의 모든 키-값 쌍 검사
            {
                if (kvp.Key.Equals(item.Key) && kvp.Value.Equals(item.Value))  // 같은 키와 값 발견 시 true 반환
                {
                    return true;
                }
            }
        }

        return false;   // 못찾으면 false 반환
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)    // 키 널체크
        {
            throw new ArgumentNullException(nameof(key));
        }

        int bucket = GetBucket(key);  // 버킷 계산

        if (table[bucket] != null)    // 해당 인덱스에 LinkedList가 존재하면
        {
            foreach (var kvp in table[bucket])   // 해당 LinkedList의 모든 키-값 쌍 검사
            {
                if (kvp.Key.Equals(key))  // 같은 키 발견 시 true 반환
                {
                    return true;
                }
            }
        }

        return false;   // 못찾으면 false 반환
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        // 인덱스 범위 체크
        if (arrayIndex < 0 || arrayIndex > array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        // 배열 크기 체크
        if (array.Length - arrayIndex < count)
        {
            throw new ArgumentException("배열 크기가 부족합니다.");
        }

        int currentIndex = arrayIndex;

        foreach (var kvp in this)
        {
            array[currentIndex++] = kvp;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
        {
            if (table[i] != null)   // 해당 인덱스에 LinkedList가 존재하면
            {
                foreach (var kvp in table[i])
                {
                    yield return kvp;
                }
            }
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)    // 키 널체크
        {
            throw new ArgumentNullException(nameof(key));
        }

        int bucket = GetBucket(key);  // 버킷 계산

        if (table[bucket] != null)    // 해당 인덱스에 LinkedList가 존재하면
        {
            var current = table[bucket].First;
            while (current != null)
            {
                if (current.Value.Key.Equals(key)) // 같은 키 발견 시
                {
                    table[bucket].Remove(current); // 이거 자체가 링크드 리스트에서 노드 삭제하는 거임
                    count--;
                    return true;    // 삭제 성공 반환
                }
                current = current.Next; 
            }
        }
        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int bucket = GetBucket(key);  // 버킷 계산

        if (table[bucket] != null)    // 해당 인덱스에 LinkedList가 존재하면
        {
            foreach (var kvp in table[bucket])   // 해당 LinkedList의 모든 키-값 쌍 검사
            {
                if (kvp.Key.Equals(key))  // 같은 키 발견 시
                {
                    value = kvp.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
