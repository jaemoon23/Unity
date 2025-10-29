using System;
using UnityEngine;

public class AVLTree<TKey, TValue> : BinarySearchTree<TKey, TValue> where TKey : IComparable<TKey>
{
    public AVLTree() : base()
    {

    }

    /// <summary>
    /// 노드 추가
    /// </summary>
    protected override TreeNode<TKey, TValue> Add(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        node = base.Add(node, key, value);
        return Balance(node);
    }

    /// <summary>
    /// 노드 추가 또는 업데이트
    /// </summary>
    protected override TreeNode<TKey, TValue> AddOrUpdate(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        node = base.AddOrUpdate(node, key, value);
        return Balance(node);
    }

    /// <summary>
    /// 노드 삭제
    /// </summary>
    protected override TreeNode<TKey, TValue> Remove(TreeNode<TKey, TValue> node, TKey key)
    {
        node = base.Remove(node, key);

        if (node == null)
        {
            return null;
        }

        return Balance(node);
    }


    /// <summary>
    /// 노드의 균형 계산
    /// </summary>
    protected int BalanceFactor(TreeNode<TKey, TValue> node)
    {
        return node == null ? 0 : GetNodeHeight(node.Left) - GetNodeHeight(node.Right);
    }

    /// <summary>
    /// 트리 균형 맞추기
    /// </summary>
    protected TreeNode<TKey, TValue> Balance(TreeNode<TKey, TValue> node)
    {
        int balanceFactor = BalanceFactor(node);

        // 왼쪽이 더 무거움 (LL)
        if (balanceFactor > 1)
        {
            // 왼쪽 자식에서 오른쪽이 더 무거움 (LR)
            if (BalanceFactor(node.Left) < 0)
            {
                node.Left = RotateLeft(node.Left);
            }

            return RotateRight(node);
        }

        // 오른쪽이 더 무거움 (RR)
        if (balanceFactor < -1)
        {
            // 오른쪽 자식에서 왼쪽이 더 무거움 (RL)
            if (BalanceFactor(node.Right) > 0)
            {
                node.Right = RotateRight(node.Right);
            }
            return RotateLeft(node);
        }

        return node;
    }

    /// <summary>
    ///  LL 회전 (왼쪽이 무거움)
    /// </summary>
    protected TreeNode<TKey, TValue> RotateRight(TreeNode<TKey, TValue> node)
    {
        var leftChild = node.Left;  // 지금 노드의 왼쪽 자식
        var rightSubtreeOfLeftChild = leftChild.Right;  // 왼쪽 자식의 오른쪽 자식

        leftChild.Right = node; 
        node.Left = rightSubtreeOfLeftChild;    

        // 노드 높이 갱신
        UpdateHeight(node);
        UpdateHeight(leftChild);

        return leftChild; // 새로운 루트 반환
    }

    /// <summary>
    /// RR 회전 (오른쪽이 무거움)
    /// </summary>
    protected TreeNode<TKey, TValue> RotateLeft(TreeNode<TKey, TValue> node)
    {
        var rightChild = node.Right;
        var leftSubtreeOfRightChild = rightChild.Left;

        rightChild.Left = node; // 오른쪽 자식이 새로운 루트가 됨
        node.Right = leftSubtreeOfRightChild;    // 왼쪽 서브트리를 원래 노드의 오른쪽에 연결

        // 노드 높이 갱신
        UpdateHeight(node);
        UpdateHeight(rightChild);

        return rightChild; // 새로운 루트 반환
    }
}
