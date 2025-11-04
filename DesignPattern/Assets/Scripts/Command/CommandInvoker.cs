using System.Collections.Generic;
using UnityEngine;
public class CommandInvoker // 실행기 클래스
{
    private Stack<ICommand> undoStack = new Stack<ICommand>();
    private Stack<ICommand> redoStack = new Stack<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();  // 명령어 실행
        undoStack.Push(command);    // undo 스택에 저장
        redoStack.Clear();  // redo 스택 초기화
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            var command = undoStack.Pop();  // 최근 명령어
            command.Undo();                 // 명령어 취소
            redoStack.Push(command);        // 취소된 명령어를 redo에 저장
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            var command = redoStack.Pop();  // 가장 최근에 취소된 명령어
            command.Execute();              // 명령어 다시 실행
            undoStack.Push(command);        // 실행된 명령어를 undo에 저장
        }
    }

    public bool CanUndo() => undoStack.Count > 0;
    public bool CanRedo() => redoStack.Count > 0;
}
