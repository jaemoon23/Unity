using System.Collections.Generic;
using UnityEngine;

public class PopFrontCommand : ICommand
{
    private List<int> list;
    private int removedValue;
    public PopFrontCommand(List<int> list)
    {
        this.list = list;
    }
    public void Execute()
    {
        if (list.Count > 0)
        {
            removedValue = list[0];
            list.RemoveAt(0);
            Debug.Log($"> 앞에서 삭제");
        }
    }

    public void Undo()
    {
        list.Insert(0, removedValue);
    }
}
