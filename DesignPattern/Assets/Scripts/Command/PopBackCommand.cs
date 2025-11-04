using System.Collections.Generic;
using UnityEngine;

public class PopBackCommand : ICommand
{
    private List<int> list;
    private int removedValue;
    public PopBackCommand(List<int> list)
    {
        this.list = list;
    }
    public void Execute()
    {
        if (list.Count > 0)
        {
            removedValue = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            Debug.Log($"< 뒤에서 삭제");
        }
    }

    public void Undo()
    {
        list.Add(removedValue);
    }
}

