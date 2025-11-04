using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PushBackCommand : ICommand
{
    private List<int> list;
    private int value;
    public PushBackCommand(List<int> list, int value)
    {
        this.list = list;
        this.value = value;
    }
    public void Execute()
    {
        list.Add(value);
        Debug.Log($"< 뒤에서 추가 {value}");
    }

    public void Undo()
    {
        list.RemoveAt(list.Count - 1);
    }
}
