using UnityEngine;
using UnityEngine.InputSystem;
public class CubeInput : MonoBehaviour
{
    private Vector3 move;
    private Vector3 rotate;

    public float moveSpeed = 5f;
    public float rotateSpeed = 100f;
    public Renderer cubeRenderer;

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        move = new Vector3(input.x, input.y, 0f);
        Debug.Log($"이동: {move}");
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        rotate = new Vector3(input.y, input.x, 0);
        Debug.Log($"회전: {rotate}");
    }

    public void OnColor(InputAction.CallbackContext context)
    {
        string button = context.control.name;
        Debug.Log($"버튼누름: {button}");

        switch (button)
        {
            case "buttonSouth": // A 버튼
                cubeRenderer.material.color = Color.red;
                break;
            case "buttonEast": // B 버튼
                cubeRenderer.material.color = Color.blue;
                break;
            case "buttonWest": // X 버튼
                cubeRenderer.material.color = Color.green;
                break;
            case "buttonNorth": // Y 버튼
                cubeRenderer.material.color = Color.yellow;
                break;
        }
        
    }

    private void Update()
    {
        
        transform.Translate(move * moveSpeed * Time.deltaTime);    // 이동 함수
        transform.Rotate(rotate * rotateSpeed * Time.deltaTime);   // 회전 함수
    }
}
