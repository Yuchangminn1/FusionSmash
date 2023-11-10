using UnityEngine;
using UnityEngine.InputSystem;

public class NewInputTest : MonoBehaviour
{

    public PlayerInputAction playerControls;

    private InputAction move;
    private InputAction jump;
    private InputAction fire;
    private InputAction chat;
    private InputAction look;


    bool isis = false;
    Vector2 moveDirection = Vector2.zero;
    Vector2 lookVec = Vector2.zero;
    Vector2 dir;
    private void Awake()
    {
        playerControls = new PlayerInputAction();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        look = playerControls.Player.Look;
        look.Enable();

        jump = playerControls.Player.Jump;
        jump.Enable();

        fire = playerControls.Player.Fire;
        fire.Enable();

        chat = playerControls.Player.Chat;
        chat.Enable();


        
        jump.performed += Jump;
        fire.performed += Fire;
        chat.performed += Chat;
    }
    private void OnDisable()
    {
        move.Disable();
        fire.Disable();
        jump.Disable();

    }


    // Update is called once per frame

    void FixedUpdate()
    {
        dir = Vector2.Lerp(dir, move.ReadValue<Vector2>(), 0.1f);
        dir.x = MYCut(dir.x);
        dir.y = MYCut(dir.y);
        Debug.Log(dir);
        lookVec = look.ReadValue<Vector2>();
        Debug.Log($"lookVec  = " + lookVec);
    }

    private float MYCut(float _float )
    {
        if (Mathf.Abs(_float) > 0.9f)
            _float = 1 * _float/ Mathf.Abs(_float);
        else if (Mathf.Abs(_float) < 0.1)
            _float = 0;
        return _float;
    }

    //public void OnMove(InputValue value)
    //{
    //    Debug.Log("move È£Ãâ");
    //    dir = value.Get<Vector2>();

    //}
    //public void OnJump()
    //{
    //    Debug.Log("Jump Jump Jump");

    //}
    public void Fire(InputAction.CallbackContext context)
    {
        Debug.Log("Fire Fire Fire");

    }
    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump Jump Jump");
    }
    private void Chat(InputAction.CallbackContext context)
    {
        Debug.Log("Chat Chat Chat");
    }
}
