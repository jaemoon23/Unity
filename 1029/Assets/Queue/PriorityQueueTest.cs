using UnityEngine;

public class PriorityQueueTest : MonoBehaviour
{
    private PriorityQueue<string, int> pq;

    void Start()
    {
        pq = new PriorityQueue<string, int>();

        Debug.Log("Priority Queue Test Start");

        TestBasicOperations();
        TestPriorityOrder();
        TestEmptyQueue();
    }

    void TestBasicOperations()
    {
        Debug.Log("Enqueue/Dequeue Test");
        pq.Enqueue("Task1", 3);
        Debug.Log($"Enqueue: Task1 (우선순위: 3), Count: {pq.Count}");

        pq.Enqueue("Task2", 1);
        Debug.Log($"Enqueue: Task2 (우선순위: 1), Count: {pq.Count}");

        pq.Enqueue("Task3", 5);
        Debug.Log($"Enqueue: Task3 (우선순위: 5), Count: {pq.Count}");

        Debug.Log($"Peek: {pq.Peek()}");

        while (pq.Count > 0)
        {
            Debug.Log($"Dequeue: {pq.Dequeue()}");
        }
    }

    void TestPriorityOrder()
    {
        Debug.Log("\nPriority Test");
        pq.Clear();

        pq.Enqueue("낮은우선순위", 10);
        pq.Enqueue("높은우선순위", 1);
        pq.Enqueue("중간우선순위", 5);

        Debug.Log("예상: 1 -> 5 -> 10");
        while (pq.Count > 0)
        {
            Debug.Log($"  - {pq.Dequeue()}");
        }
    }

    void TestEmptyQueue()
    {
        Debug.Log("Empty Queue Test");
        pq.Clear();

        try
        {
            pq.Dequeue();
        }
        catch (System.InvalidOperationException ex)
        {
            Debug.Log($"✓ 예외 발생: {ex.Message}");
        }
    }
}