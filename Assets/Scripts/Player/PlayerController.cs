using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;

    private Rigidbody _rigidbody;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate() {
        Move();
    }
    private void LateUpdate() {
        CameraLook();
    }
    void Move(){
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y;
        _rigidbody.velocity = dir;
    }
    public void OnMove(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Performed){
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled){
            curMovementInput = Vector2.zero;
        }
    }

    void CameraLook(){
        camCurXRot += mouseDelta.y*lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot,minXLook,maxXLook); 
        //Mathf.Clamp camCurXRot이 최솟값보다 작아지면 최솟값을 반환하고 최댓값 보다 커지면 최댓값을 반환한다.
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot,0,0); // 왜 음수일까

        transform.eulerAngles += new Vector3(0,mouseDelta.x*lookSensitivity,0);
    }

    public void OnLook(InputAction.CallbackContext context){
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context){
        if(context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up* jumpPower, ForceMode.Impulse);
        }
    }

    bool IsGrounded(){
        Ray[] rays = new Ray[4]{
            new Ray(transform.position + (transform.forward *0.2f)+ (transform.up*0.01f),Vector3.down),
            new Ray(transform.position + (-transform.forward *0.2f)+ (transform.up*0.01f),Vector3.down),
            new Ray(transform.position + (transform.forward *0.2f)+ (transform.up*0.01f),Vector3.down),
            new Ray(transform.position + (-transform.forward *0.2f)+ (transform.up*0.01f),Vector3.down),
        };
        for(int i=0; i<rays.Length; i++)
        {
         if(Physics.Raycast(rays[i],0.1f,groundLayerMask)){
            return true;
         }   
        }
        return false;
    }
}
