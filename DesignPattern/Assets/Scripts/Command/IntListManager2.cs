using System.Collections.Generic;
using UnityEngine;

public class IntListManager2 : MonoBehaviour
{
    [Header("테스트 설정")]
    [SerializeField] private int numberToAdd = 10;

    private List<int> intList = new List<int>();
    private CommandInvoker invoker = new CommandInvoker();

    private void Start()
    {
        Debug.Log("=== 커맨드 패턴 Undo/Redo 연습 (확장) 시작 ===");
        PrintList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.W))
        {
            ExecutePushBack();  // 뒤 추가
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            ExecutePushFront(); // 앞 추가
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.S))
        {
            ExecutePopBack();   // 뒤 삭제
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ExecutePopFront();  // 앞 삭제
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.D))
        {
            PrintList();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Z))
        {
            ExecuteUndo();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.X))
        {
            ExecuteRedo();
        }
    }

    private void ExecutePushBack()
    {
        var push = new PushBackCommand(intList, numberToAdd);
        numberToAdd += 10;
        invoker.ExecuteCommand(push);
        PrintList();
    }
    private void ExecutePushFront()
    {
        var push = new PushFrontCommand(intList, numberToAdd);
        numberToAdd += 10;
        invoker.ExecuteCommand(push);
        PrintList();
    }

    private void ExecutePopBack()
    {
        var pop = new PopBackCommand(intList);
        invoker.ExecuteCommand(pop);
        PrintList();

    }
    private void ExecutePopFront()
    {
        var pop = new PopFrontCommand(intList);
        invoker.ExecuteCommand(pop);
        PrintList();

    }

    private void ExecuteUndo()
    {
        if (invoker.CanUndo())
        {
            invoker.Undo();
            Debug.Log("<color=cyan>↶ Undo</color>");
            PrintList();
        }
        else
        {
            Debug.LogWarning("Undo할 명령이 없습니다!");
        }
    }

    private void ExecuteRedo()
    {
        if (invoker.CanRedo())
        {
            invoker.Redo();
            Debug.Log("<color=magenta>↷ Redo</color>");
            PrintList();
        }
        else
        {
            Debug.LogWarning("Redo할 명령이 없습니다!");
        }
    }

    private void PrintList()
    {
        Debug.Log($"현재 리스트: [{string.Join(", ", intList)}] (개수: {intList.Count})");
    }
}
