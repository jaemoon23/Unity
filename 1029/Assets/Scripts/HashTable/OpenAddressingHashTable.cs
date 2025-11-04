using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ProbingStrategy
{
    Linear, // 선형 탐사
    Quadratic, // 제곱 탐사
    DoubleHash // 이중 해싱
}

public class OpenAddressingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.6;
    private KeyValuePair<TKey, TValue>[] table; // 해시 테이블
    private bool[] occupied; // 점유 상태
    private bool[] deleted; // 삭제 상태
    private int size; // 현재 크기
    public int Size => size;
    private int count; // 요소 수
    private ProbingStrategy probingStrategy;
    public ProbingStrategy ProbingStrategy { get { return probingStrategy; } set { probingStrategy = value; }}
    public bool isSizeChanged { get; set; }
    public OpenAddressingHashTable(ProbingStrategy strategy = ProbingStrategy.Linear)
    {
        size = DefaultCapacity;
        table = new KeyValuePair<TKey, TValue>[size];
        occupied = new bool[size];
        deleted = new bool[size];
        count = 0;

        probingStrategy = strategy;
    }

    public int GetPrimaryHash(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        int hash = key.GetHashCode();
        return Mathf.Abs(hash) % size;
    }
    
     public int GetSecondaryHash(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        
        int hash = key.GetHashCode();
        
        // 0이 반환되지 않도록 1을 더함
        return 1 + (Math.Abs(hash) % (size - 1));
    }

    public int GetProbeIndex(TKey key, int attempt)
    {
        int primaryIndex = GetPrimaryHash(key); // 기본 해시 인덱스

        switch (probingStrategy)
        {
            case ProbingStrategy.Linear:    // 선형 탐사
                return (primaryIndex + attempt) % size;
            case ProbingStrategy.Quadratic: // 제곱 탐사
                return (primaryIndex + attempt * attempt) % size;
            case ProbingStrategy.DoubleHash: // 이중 해싱
                return (GetPrimaryHash(key) + attempt * GetSecondaryHash(key)) % size;
        }
        throw new ArgumentException(nameof(probingStrategy));
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

            if ((double)(count + 1) / size > LoadFactor)
            {
                Resize();
            }

            int attempt = 0; // 탐사 시도 횟수
            int index; // 해시 테이블 인덱스

            do
            {
                index = GetProbeIndex(key, attempt);
                if (!occupied[index] || deleted[index]) // 빈 슬롯 또는 삭제된 슬롯 발견
                {
                    table[index] = new KeyValuePair<TKey, TValue>(key, value);
                    occupied[index] = true; // 슬롯 점유 표시
                    deleted[index] = false; // 삭제 상태 해제
                    count++;
                    return;
                }

                if (table[index].Key.Equals(key)) // 중복 키 발견
                {
                    // 값 교체
                    table[index] = new KeyValuePair<TKey, TValue>(key, value);
                    return;
                }

                attempt++;

                if (attempt > size) // 전체 슬롯을 탐사했으면 리사이즈
                {
                    Resize();
                    attempt = 0; // 리사이즈 후 탐사 시도 횟수 초기화
                }
            } while (true);
        }
    }

    public ICollection<TKey> Keys => Enumerable.Range(0, size).Where(i => occupied[i] && !deleted[i]).Select(i => table[i].Key).ToList();

    public ICollection<TValue> Values => Enumerable.Range(0, size).Where(i => occupied[i] && !deleted[i]).Select(i => table[i].Value).ToList();

    public int Count => count;

    public bool IsReadOnly => false;

    public void Resize()
    {
        var oldTable = table;
        var oldOccupied = occupied;
        var oldDeleted = deleted;
        var oldSize = size;

        size *= 2;
        table = new KeyValuePair<TKey, TValue>[size];
        occupied = new bool[size];
        deleted = new bool[size];
        count = 0;

        for (int i = 0; i < oldSize; i++)
        {
            if (oldOccupied[i] && !oldDeleted[i])   // 예전 테이블에서 사용 중인 슬롯이면 재해싱
            {
                Add(oldTable[i].Key, oldTable[i].Value);
            }
        }
        isSizeChanged = true;
    }

    public int FindIndex(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        int attempt = 0;
        do
        {
            int index = GetProbeIndex(key, attempt);
            if (!occupied[index] && !deleted[index])
            {
                return -1; // 빈 슬롯이면서 삭제된 적이 없는 슬롯이면 탐사 종료
            }

            if (occupied[index] && !deleted[index] && table[index].Key.Equals(key))
            {
                return index; // 키 발견
            }

            attempt++;
            
        } while (attempt < size);
            
        return -1;
    }

    public void Add(TKey key, TValue value)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if ((double)(count + 1) / size > LoadFactor)
        {
            Resize();
        }

        int attempt = 0; // 탐사 시도 횟수
        int index; // 해시 테이블 인덱스

        do
        {
            index = GetProbeIndex(key, attempt);
            if (!occupied[index] || deleted[index]) // 빈 슬롯 또는 삭제된 슬롯 발견
            {
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
                occupied[index] = true; // 슬롯 점유 표시
                deleted[index] = false; // 삭제 상태 해제
                count++;
                return;
            }

            if (table[index].Key.Equals(key)) // 중복 키 발견
            {
                throw new ArgumentException("키 중복");
            }

            attempt++;

            if (attempt > size) // 전체 슬롯을 탐사했으면 리사이즈
            {
                Resize();
                attempt = 0; // 리사이즈 후 탐사 시도 횟수 초기화
            }
        } while (true);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Array.Clear(table, 0, size);
        Array.Clear(occupied, 0, size);
        Array.Clear(deleted, 0, size);
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int index = FindIndex(item.Key);

        if (index != -1)
        {
            return table[index].Value.Equals(item.Value);
        }

        return false;
    }

    public bool ContainsKey(TKey key)
    {
        return FindIndex(key) != -1;
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

        int index = arrayIndex;
        
        foreach (var kvp in this)
        {
            array[index++] = kvp;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
        {
            if (occupied[i] && !deleted[i])
            {
                yield return table[i];
            }
        }
    }

    public bool Remove(TKey key)
    {
        int index = FindIndex(key);

        if (index != -1)
        {
            deleted[index] = true; // 삭제 표시
            count--;
            return true;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        int index = FindIndex(item.Key);
        
        // 키가 존재하고 값도 일치하면 삭제
        if (index != -1 && table[index].Value.Equals(item.Value))
        {
            deleted[index] = true; // 삭제 표시
            count--;
            return true;
        }

        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int index = FindIndex(key);
        if (index != -1)
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
