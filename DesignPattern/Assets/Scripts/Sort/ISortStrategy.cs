using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 공통 인터페이스
public interface ISortStrategy<T>
{
    public void Sort(T[] array);
}
