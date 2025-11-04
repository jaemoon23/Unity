using System.Collections.Generic;
using UnityEngine;

public class PushFrontCommand : ICommand
{
    private List<int> list;
    private int value;
    public PushFrontCommand(List<int> list, int value)
    {
        this.list = list;
        this.value = value;
    }
    public void Execute()
    {
        list.Insert(0, value);
        Debug.Log($"> 앞에서 추가: {value}");
    }

    public void Undo()
    {
        list.RemoveAt(0);
    }
}
