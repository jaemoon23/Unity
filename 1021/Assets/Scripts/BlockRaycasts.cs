using Mono.Cecil.Cil;
using UnityEngine;

public class BlockRaycasts : MonoBehaviour
{
    [SerializeField] CanvasGroup can;

    private void OnEnable()
    {
        can.blocksRaycasts = true;
    }
    private void OnDisable()
    {
        can.blocksRaycasts = false;
    }
}
