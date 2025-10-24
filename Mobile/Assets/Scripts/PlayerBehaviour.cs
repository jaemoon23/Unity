using System;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

/// <summary> 
/// Responsible for moving the player automatically and 
/// reciving input. 
/// </summary> 
[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    /// <summary> 
    /// A reference to the Rigidbody component 
    /// </summary> 
    private Rigidbody rb;

    [Tooltip("How fast the ball moves left/right")]
    public float dodgeSpeed = 5;

    [Tooltip("How fast the ball moves forwards automatically")]
    [Range(0, 10)]
    public float rollSpeed = 5;
    private float speed = 0f;

    // Swipe
    public float minSwipeDistance = 0.25f; // Inch
    public float maxSwipeTime = 0.25f; // Second
    private float minSwipeDistancePixelss; // Pixel
    private int fingerId = -1;
    private Vector2 fingerIdTouchStartPosition;
    private float fingerIdTouchStartTime;
    public float sweepDistance = 1f;

    public float zommSpeed = 5f;

    public VirtualJoystick joystick;
    // Start is called before the first frame update
    void Start()
    {
#if DEF_DEV
        Debug.unityLogger.logEnabled = false;
#endif
        // Get access to our Rigidbody component
        rb = GetComponent<Rigidbody>();

        // 인치 -> 픽셀
        minSwipeDistancePixelss = minSwipeDistance * Screen.dpi; // Inch -> Pixel
    }

    private void Update()
    {
        
        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (fingerId == -1) // 핑거 Id가 없을때 
                    {
                        fingerId = touch.fingerId;
                        fingerIdTouchStartPosition = touch.position;
                        fingerIdTouchStartTime = Time.time;
                    }
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:

                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (fingerId == touch.fingerId) // 핑거 Id가 있을때
                    {
                        fingerId = -1;
                        float distance = Vector2.Distance(touch.position, fingerIdTouchStartPosition);
                        float time = Time.time - fingerIdTouchStartTime;
                        if (distance > minSwipeDistancePixelss && time < maxSwipeTime)
                        {
                            Vector2 direction = touch.position - fingerIdTouchStartPosition;
                            direction.Normalize();
                            Vector3 direction3 = direction.x > 0f ? Vector3.right : Vector3.left;
                            if (!rb.SweepTest(direction3, out RaycastHit hit, sweepDistance))
                            {
                                rb.MovePosition(rb.position + direction3 * sweepDistance);
                            }

                            fingerId = -1;
                            fingerIdTouchStartPosition = Vector2.zero;
                            fingerIdTouchStartTime = 0f;

                        }
                    }
                    break;
            }
        }

        if (Input.touchCount == 2)
        {
            // 줌인아웃
            // 터치 구조체에 전 프레임의 포지션이 같이 들어감 deltaPosition << 이거 이용해서 거리가 줄어드는지 늘어나는지 판단하기
            // 공의 스케일을 최소랑 최대값을 정해두고 인아웃때 스케일을 키우기
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // 현재 거리, 이전 거리
            float currentDistance = Vector2.Distance(touch1.position, touch2.position);
            float previousDistance = Vector2.Distance(touch1.position - touch1.deltaPosition,
                                                        touch2.position - touch2.deltaPosition);

            // 거리 차이
            // 차이가 0 보다 크면 손가락 간격 벌어짐
            // 차이가 0 보다 작으면 손가락 간격이 줄어듬
            float deltaDistance = currentDistance - previousDistance;
            // float deltaDistance = (currentDistance - previousDistance) / Screen.dpi;
            // deltaDistance *= Time.deltaTime * zommSpeed;

            // var current = transform.localScale.x;
            // current += deltaDistance;
            // current = Mathf.Clamp(current, 0.5f, 3f);
            // transform.localScale = Vector3.one * current;

            // 새로운 스케일
            Vector3 newScale = transform.localScale * deltaDistance;

            // 최소, 최대 스케일 값
            float minScale = 0.5f;
            float maxScale = 2.0f;

            newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale.y = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale.z = Mathf.Clamp(newScale.x, minScale, maxScale);
            //transform.localScale = newScale;
            transform.localScale = Vector3.Lerp(transform.localScale, newScale, 0.1f) ;
        }

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            var ray = Camera.main.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
            {
                hit.collider.SendMessage("OnTouched", SendMessageOptions.DontRequireReceiver);  // 메서드가 없어도 에러메세지 안뜸
            }
        }
    }
    /// <summary>
    /// FixedUpdate is called at a fixed framerate and is a prime place to put
    /// Anything based on time.
    /// </summary>
    void FixedUpdate()
    {

#if UNITY_EDITOR
        // Check if we're moving to the side 
        var horizontalSpeed = Input.GetAxis("Horizontal") * dodgeSpeed;
#endif

#if UNITY_ANDROID || UNITY_IOS
        // 기울이기 센서
        //horizontalSpeed = Input.acceleration.x * dodgeSpeed;

        // 터치로 이동하는 코드
        // if (Input.touchCount == 1)
        // {
        //     Touch touch = Input.GetTouch(0);
        //     var viewportPos = Camera.main.ScreenToViewportPoint(touch.position);
        //     horizontalSpeed = viewportPos.x < 0.5f ? -1 : 1;
        //     horizontalSpeed *= dodgeSpeed;
        // }

        var direction = joystick.Input.x;

#endif
        rb.AddForce(direction, 0, rollSpeed);
    }
}