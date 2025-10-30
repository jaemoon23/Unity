using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 이진 탐색 트리
/// (키 중복을 허용하지 않음)
/// </summary>
public class BinarySearchTree<TKey, TValue> : IDictionary<TKey, TValue> where TKey : IComparable<TKey>
{
    /// <summary>
    /// 트리의 루트 노드
    /// </summary>
    protected TreeNode<TKey, TValue> rootNode;

    public BinarySearchTree()
    {
        rootNode = null;
    }

    public TValue this[TKey key]
    {
        get
        {
            // 키에 해당하는 값을 반환
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            else
            {
                // 키가 존재하지 않을 때 예외 발생
                throw new KeyNotFoundException($"키 {key}를 찾을 수 없습니다.");
            }
        }
        set
        {
            // 키가 존재하면 값을 업데이트, 없으면 새 노드 추가
            rootNode = AddOrUpdate(rootNode, key, value);
        }
    }

    /// <summary>
    /// 키가 존재하면 값을 업데이트, 없으면 새 노드 추가
    /// </summary>
    protected virtual TreeNode<TKey, TValue> AddOrUpdate(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        // 탐색을 하다가 null 노드를 만나면 새 노드 생성
        if (node == null)
        {
            return new TreeNode<TKey, TValue>(key, value);
        }

        int compare = key.CompareTo(node.Key);  // 키 비교

        if (compare < 0)
        {
            node.Left = AddOrUpdate(node.Left, key, value); // 왼쪽 서브트리에 삽입
        }
        else if (compare > 0)
        {
            node.Right = AddOrUpdate(node.Right, key, value); // 오른쪽 서브트리에 삽입
        }
        else
        {
            node.Value = value; // 키가 이미 존재하면 값 업데이트
        }

        UpdateHeight(node);

        return node;    // 변경된 노드 반환
    }

    public ICollection<TKey> Keys => InOrderTraversal().Select(KVPair => KVPair.Key).ToList();

    public ICollection<TValue> Values => InOrderTraversal().Select(KVPair => KVPair.Value).ToList();

    public int Count => CountNodes(rootNode);   // 노드 개수

    /// <summary>
    /// 트리의 노드 개수 계산 (재귀 함수)
    /// </summary>
    protected virtual int CountNodes(TreeNode<TKey, TValue> node)
    {
        if (node == null)
        {
            return 0;
        }
        return 1 + CountNodes(node.Left) + CountNodes(node.Right);
    }

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        rootNode = Add(rootNode, key, value);
    }

    /// <summary>
    /// 키-값 쌍에 해당하는 노드 추가
    /// </summary>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    /// <summary>
    /// 키에 해당하는 노드 추가 (재귀 함수)
    /// </summary>
    protected virtual TreeNode<TKey, TValue> Add(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        // 탐색을 하다가 null 노드를 만나면 새 노드 생성
        if (node == null)
        {
            return new TreeNode<TKey, TValue>(key, value);
        }

        int compare = key.CompareTo(node.Key);  // 키 비교

        if (compare < 0)
        {
            node.Left = Add(node.Left, key, value); // 왼쪽 서브트리에 삽입
        }
        else if (compare > 0)
        {
            node.Right = Add(node.Right, key, value); // 오른쪽 서브트리에 삽입
        }
        else
        {
            throw new ArgumentException($"키 {key}는 이미 존재합니다.");    // 중복 키 허용 안 함
        }

        // 노드 높이 갱신
        UpdateHeight(node);

        return node;
    }

    public void Clear()
    {
        rootNode = null;   // 루트 노드를 null로 설정하여 트리 초기화
    }

    /// <summary>
    /// 키-값 쌍이 트리에 존재하는지 확인
    /// </summary>
    /// <param name="item"></param>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ContainsKey(item.Key);
    }

    /// <summary>
    /// 키가 트리에 존재하는지 확인
    /// </summary>
    /// <param name="key"></param>
    public bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out _);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        // 예외
        foreach (var item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return InOrderTraversal().GetEnumerator();
    }

    /// <summary>
    /// 키에 해당하는 노드 삭제 (매개변수가 키인 경우)
    /// </summary>
    public bool Remove(TKey key)
    {
        int initialCount = Count;
        rootNode = Remove(rootNode, key);   // 키에 해당하는 노드 삭제

        return Count < initialCount;    // 노드 개수가 줄어들었으면 삭제 성공
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    /// <summary>
    /// 키에 해당하는 노드 삭제 (재귀 함수)
    /// </summary>
    protected virtual TreeNode<TKey, TValue> Remove(TreeNode<TKey, TValue> node, TKey key)
    {
        if (node == null)
        {
            return node;    // 키가 존재하지 않음
        }

        int compare = key.CompareTo(node.Key);  // 키 비교

        if (compare < 0)
        {
            node.Left = Remove(node.Left, key); // 왼쪽 서브트리에서 삭제
        }
        else if (compare > 0)
        {
            node.Right = Remove(node.Right, key); // 오른쪽 서브트리에서 삭제
        }
        else
        {
            if (node.Left == null)
            {
                return node.Right; // 오른쪽 자식 반환
            }
            else if (node.Right == null)
            {
                return node.Left; // 왼쪽 자식 반환
            }

            TreeNode<TKey, TValue> minNode = FindMin(node.Right); // 오른쪽 서브트리에서 최소 노드 찾기

            node.Key = minNode.Key;       // 현재 노드의 키를 최소 노드의 키로 교체
            node.Value = minNode.Value;   // 현재 노드의 값을 최소 노드의 값으로 교체

            node.Right = Remove(node.Right, minNode.Key); // 오른쪽 서브트리에서 최소 노드 삭제
        }

        // 노드 높이
        UpdateHeight(node);

        return node;    // 변경된 노드 반환
    }

    /// <summary>
    /// 가장 작은 키를 가진 노드 찾기
    /// </summary>
    protected virtual TreeNode<TKey, TValue> FindMin(TreeNode<TKey, TValue> node)
    {
        // 왼쪽 자식이 없을 때까지 이동
        while (node.Left != null)
        {
            node = node.Left;
        }
        return node;
    }

    /// <summary>
    /// 키에 해당하는 값을 찾기
    /// </summary>
    public bool TryGetValue(TKey key, out TValue value)
    {
        return TryGetValue(rootNode, key, out value);
    }

    /// <summary>
    /// 키에 해당하는 값을 찾기 (재귀 함수)
    /// </summary>
    protected bool TryGetValue(TreeNode<TKey, TValue> node, TKey key, out TValue value)
    {
        if (node == null)
        {
            value = default(TValue);
            return false;
        }

        // 키 비교
        int compare = key.CompareTo(node.Key);

        if (compare == 0)
        {
            value = node.Value;
            return true;
        }
        else if (compare < 0)
        {
            return TryGetValue(node.Left, key, out value);  // 왼쪽 서브트리 탐색
        }
        else
        {
            return TryGetValue(node.Right, key, out value);  // 오른쪽 서브트리 탐색
        }
        
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// 중위 순회 (In-Order Traversal)
    /// </summary>
    public virtual IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal()
    {
        return InOrderTraversal(rootNode);  // 루트 노드부터 시작하여 중위 순회 수행
    }

    protected virtual IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal(TreeNode<TKey, TValue> node)
    {
        if (node != null)
        {
            foreach (var kvp in InOrderTraversal(node.Left))
            {
                yield return kvp;   // 왼쪽 서브트리 방문
            }

            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);  // 현재 노드 방문

            foreach (var kvp in InOrderTraversal(node.Right))
            {
                yield return kvp;   // 오른쪽 서브트리 방문
            }
        }
        
    }

    /// <summary>
    /// 전위 순회 (Pre-Order Traversal)
    /// </summary>
    public virtual IEnumerable<KeyValuePair<TKey, TValue>> PreOrderTraversal()
    {
        return PreOrderTraversal(rootNode);  // 루트 노드부터 시작하여 전위 순회 수행
    }

    protected virtual IEnumerable<KeyValuePair<TKey, TValue>> PreOrderTraversal(TreeNode<TKey, TValue> node)
    {
        if (node != null)
        {
            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);  // 현재 노드 방문
            
            foreach (var kvp in PreOrderTraversal(node.Left))
            {
                yield return kvp;   // 왼쪽 서브트리 방문
            }

            foreach (var kvp in PreOrderTraversal(node.Right))
            {
                yield return kvp;   // 오른쪽 서브트리 방문
            }
        }
    }

    /// <summary>
    /// 후위 순회 (Post-Order Traversal)
    /// </summary>
    public virtual IEnumerable<KeyValuePair<TKey, TValue>> PostOrderTraversal()
    {
        return PostOrderTraversal(rootNode);  // 루트 노드부터 시작하여 후위 순회 수행
    }

    protected virtual IEnumerable<KeyValuePair<TKey, TValue>> PostOrderTraversal(TreeNode<TKey, TValue> node)
    {
        foreach (var kvp in PostOrderTraversal(node.Left))
        {
            yield return kvp;
        }

        foreach (var kvp in PostOrderTraversal(node.Right))
        {
            yield return kvp;
        }

        yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
    }

    /// <summary>
    /// 레벨 순회 (Level-Order Traversal)
    /// </summary>
    public virtual IEnumerable<KeyValuePair<TKey, TValue>> LevelOrderTraversal()
    {
        return LevelOrderTraversal(rootNode);  // 루트 노드부터 시작하여 레벨 순회 수행
    }

    protected virtual IEnumerable<KeyValuePair<TKey, TValue>> LevelOrderTraversal(TreeNode<TKey, TValue> node)
    {
        if (node == null)
        {
            yield break;
        }

        Queue<TreeNode<TKey, TValue>> queue = new Queue<TreeNode<TKey, TValue>>();
        queue.Enqueue(node);

        while (queue.Count > 0)
        {
            TreeNode<TKey, TValue> current = queue.Dequeue();
            yield return new KeyValuePair<TKey, TValue>(current.Key, current.Value);

            if (current.Left != null)
            {
                queue.Enqueue(current.Left);
            }
            
            if (current.Right != null)
            {
                queue.Enqueue(current.Right);
            }

        }
    }

    protected virtual void UpdateHeight(TreeNode<TKey, TValue> node)
    {
        if (node != null)
        {
            node.Height = Mathf.Max(GetNodeHeight(node.Left), GetNodeHeight(node.Right)) + 1;
        }
    }

    /// <summary>
    /// 노드 높이를 반환
    /// </summary>
    protected virtual int GetNodeHeight(TreeNode<TKey, TValue> node)
    {
        // if (node == null)
        // {
        //     return 0;
        // }
        return node == null ? 0 : node.Height;
    }
    
}
