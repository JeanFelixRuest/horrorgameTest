using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] PlayerController controller;

    PlayerControls controls;
    PlayerControls.PlayerActions playerActions;

    public Vector2 horizontalInput;
    Vector2 mouseInput;
    float sprintInput;

    private void Awake()
    {
        controls = new PlayerControls();
        playerActions = controls.Player;

        playerActions.Move.performed += context => horizontalInput = context.ReadValue<Vector2>();
        playerActions.Move.canceled += context => horizontalInput = Vector2.zero;

        playerActions.Look.performed += context => mouseInput = context.ReadValue<Vector2>();
        playerActions.Look.canceled += context => mouseInput = Vector2.zero;

        playerActions.Sprint.performed += context => sprintInput = context.ReadValue<float>();
        playerActions.Sprint.canceled += context => sprintInput = 0;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        controller.ReceiveMove(horizontalInput);
        controller.ReceiveLook(mouseInput);
        controller.ReceiveSprint(sprintInput);
    }
}
