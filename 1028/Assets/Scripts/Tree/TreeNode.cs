using UnityEngine;

/// <summary>
/// 이진 탐색 트리 노드 클래스
/// </summary>
/// <typeparam name="TKey">키</typeparam>
/// <typeparam name="TValue">값</typeparam>
public class TreeNode<TKey, TValue>
{
    public TKey Key { get; set; }   // 키
    public TValue Value { get; set; } // 값

    public int Height { get; set; }  // 노드의 높이

    /// <summary>
    /// 왼쪽 자식 노드
    /// </summary>
    public TreeNode<TKey, TValue> Left { get; set; }

    /// <summary>
    /// 오른쪽 자식 노드
    /// </summary>
    public TreeNode<TKey, TValue> Right { get; set; }

    public TreeNode(TKey key, TValue value)
    {
        Key = key;
        Value = value;
        Height = 1;
    }
}
