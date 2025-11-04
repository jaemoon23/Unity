using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{   
    private const int DefaultCapacity = 16; // 기본 용량
    private const double LoadFactor = 0.75; // 최대 적재율

    private KeyValuePair<TKey, TValue>[] table; // 해시 테이블
    private bool[] occupied; // 슬롯 점유 여부

    private int size; // 테이블 용량
    private int count; // 현재 요소 개수
    public SimpleHashTable()
    {
        size = DefaultCapacity;
        table = new KeyValuePair<TKey, TValue>[size];
        occupied = new bool[size];
        count = 0;
    }

    private int GetIndex(TKey key, int size)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        int hash = key.GetHashCode();   // 해시 코드 계산
        return Mathf.Abs(hash) % size;  // 인덱스 계산
    }
    private int GetIndex(TKey key)
    {
        return GetIndex(key, size);
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
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            int index = GetIndex(key);  // 인덱스 계산
            if (occupied[index] && !table[index].Key.Equals(key))
            {
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else if (!occupied[index])
            {
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
                occupied[index] = true;
                count++;
            }
            else
            {
                throw new InvalidOperationException("해시 충돌");
            }
        }
    }

    public ICollection<TKey> Keys => Enumerable.Range(0, size).Where(i => occupied[i]).Select(i => table[i].Key).ToList();
    // {
    //     get
    //     {
    //         List<TKey> keys = new List<TKey>();
    //         for (int i = 0; i < size; i++)
    //         {
    //             if (occupied[i])
    //             {
    //                 keys.Add(table[i].Key);
    //             }
    //         }
    //         return keys;
    //     }
    // }
    public ICollection<TValue> Values => Enumerable.Range(0, size).Where(i => occupied[i]).Select(i => table[i].Value).ToList();
    // {
    //     get
    //     {
    //         List<TValue> values = new List<TValue>();
    //         for (int i = 0; i < size; i++)
    //         {
    //             if (occupied[i])
    //             {
    //                 values.Add(table[i].Value);
    //             }
    //         }
    //         return values;
    //     }
    // }
    public int Count => count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        // TODO: null 체크 필요
        if ((double)count / size >= LoadFactor) // 적재율 초과 시 리사이즈
        {
            Resize();
        }

        int index = GetIndex(key);  // 인덱스 계산

        // 배열에 요소가 있는지 검사
        if (!occupied[index])
        {
            table[index] = new KeyValuePair<TKey, TValue>(key, value);  // 요소 추가
            occupied[index] = true; // 슬롯 점유 표시
            count++;    // 요소 개수 증가
        }
        else if (table[index].Key.Equals(key))
        {
            // 중복 키가 있는 경우
            throw new ArgumentException("키 중복");
        }
        else
        {
            // 해시 충돌 발생
            throw new InvalidOperationException("해시 충돌");
        }
    }

    public void Resize()
    {
        int newSize = size * 2; // 용량 두 배 증가

        var newTable = new KeyValuePair<TKey, TValue>[newSize]; // 새 테이블 생성
        var newOccupied = new bool[newSize]; // 새 점유 배열 생성

        for (int i = 0; i < size; i++)
        {
            if (!occupied[i])
            {
                continue;
            }

            int newIndex = GetIndex(table[i].Key, newSize); // 새 인덱스 계산

            if (newOccupied[newIndex])
            {
                // 해시 충돌 발생
                throw new InvalidOperationException("해시 충돌");
            }

            newTable[newIndex] = table[i];
            newOccupied[newIndex] = true;
        }

        table = newTable;
        occupied = newOccupied;
        size = newSize;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Array.Clear(table, 0, size);
        Array.Clear(occupied, 0, size);
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (TryGetValue(item.Key, out TValue value))
        {
            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        return false;
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        int index = GetIndex(key);  // 인덱스 계산

        return occupied[index] && table[index].Key.Equals(key); // 키 존재 여부 반환
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }
        if (arrayIndex < 0 || arrayIndex > array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }
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
            if (occupied[i])
            {
                yield return table[i];
            }
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        int index = GetIndex(key);  // 인덱스 계산
        if (occupied[index] && table[index].Key.Equals(key))
        {
            occupied[index] = false;    // 슬롯 비우기
            table[index] = default;     // 요소 제거
            count--;
            return true;
        }
        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
        // if (item.Key == null)
        // {
        //     throw new ArgumentNullException(nameof(item.Key));
        // }
        // int index = GetIndex(item.Key);  // 인덱스 계산
        // if (occupied[index] && Contains(item))
        // {
        //     occupied[index] = false;    // 슬롯 비우기
        //     table[index] = default;     // 요소 제거
        //     count--;
        //     return true;
        // }
        // return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        int index = GetIndex(key);  // 인덱스 계산

        if (occupied[index] && table[index].Key.Equals(key))
        {
            value = table[index].Value;
            return true;
        }

        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
