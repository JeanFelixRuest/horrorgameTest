using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Dépendances")]

    [Tooltip("Permet de doter le joueur de fonction de déplacement ainsi que de physiques")]
    [SerializeField] public CharacterController characterController;

    [Tooltip("Permet de modifier certains paramètres de la caméra")]
    [SerializeField] public CameraScript cameraScript;

    [Tooltip("Référence la caméra du joueur")]
    [SerializeField] public Camera camera;

    [Tooltip("Référence le layer utilisé pour être le sol")]
    [SerializeField] LayerMask groundMask;

    [Tooltip("C'est la sensibilité de rotation de la caméra sur l'Axe Y, elle peut-être modifiée dans les paramètres ou dans le GameManager")]
    [SerializeField] private float sensitivityY;

    [Tooltip("C'est la sensibilité de rotation de la caméra sur l'Axe X, elle peut-être modifiée dans les paramètres ou dans le GameManager")]
    [SerializeField] private float sensitivityX;


    [Header("Statistiques")]

    [Tooltip("Vitesse de déplacement minimale du personnage")]
    public float walkSpeed;

    [Tooltip("Vitesse de déplacement maximale du personnage")]
    public float sprintSpeed;

    [Tooltip("Vitesse de déplacement actuelle du personnage")]
    public float currentSpeed;

    [Tooltip("Gravité appliquée au personnage")]
    public float gravity = -9.81f;

    [Tooltip("Hauteur Maximale et Minimale pouvant être atteinte par la caméra depuis son centre")]
    public float xClamp = 90;


    [Header("Autres informations")]
    private Vector2 horizontalInput;
    Vector3 verticalVelocity = Vector3.zero;
    float mouseX, mouseY;
    float sprint;
    float xRotation = 0;

    [Header("PlayerStates")]
    public bool isSprinting = false;


    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraScript = GetComponent<CameraScript>();
        currentSpeed = walkSpeed;
    }

    private void Update()
    {
        Move();
        Gravity();
        RotateCamera();
        UpdateSprint();
    }

    #region MovingRegion
    public void ReceiveMove(Vector2 _horizontalInput)
    {
        horizontalInput = _horizontalInput;
    }

    public void Move()
    {
        Vector3 horizontalVelocity = (transform.right * horizontalInput.x + transform.forward * horizontalInput.y);
        characterController.Move(horizontalVelocity.normalized * currentSpeed * Time.deltaTime);
    }

    public void Gravity() //OH NO NOT GRAVITY
    {
        if (isGrounded())
        {
            verticalVelocity.y = 0;
        }
        verticalVelocity.y += gravity * Time.deltaTime;
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    public bool isGrounded()
    {
        return Physics.CheckSphere(transform.position, 1.10f, groundMask);
    }
    #endregion

    #region LookingRegion
    public void ReceiveLook(Vector2 _mouseInput)
    {
        mouseX = _mouseInput.x * sensitivityX;
        mouseY = _mouseInput.y * sensitivityY;
    }

    void RotateCamera()
    {
        transform.Rotate(Vector3.up, mouseX * Time.deltaTime);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = xRotation;
        camera.transform.eulerAngles = targetRotation;
    }
    #endregion

    #region SprintRegion

    public void ReceiveSprint(float _sprintInput)
    {
        sprint = _sprintInput;
    }

    void UpdateSprint()
    {
        if (isGrounded())
        {
            switch (sprint)
            {
                case 0:
                    currentSpeed = walkSpeed;
                    isSprinting = false;
                    if (camera.fieldOfView >= 70 && camera.fieldOfView <= 70.1f)
                        StartCoroutine(cameraScript.modifyFOV(1));
                    break;
                case 1:
                    currentSpeed = sprintSpeed;
                    isSprinting = true;
                    print(camera.fieldOfView);
                    if (camera.fieldOfView >= 60f && camera.fieldOfView <= 60.1f)
                        StartCoroutine(cameraScript.modifyFOV(0));
                    break;
            }
        }
    }

    #endregion
}
