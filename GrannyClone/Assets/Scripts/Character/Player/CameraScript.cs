using System.Collections;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Dépendances")]

    [Tooltip("Ici on prend la caméra du joueur")]
    [SerializeField] private Camera camera;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InputManager inputManager;

    [Header("Head Bob")]
    [Range(0.001f, 0.1f)]
    public float amount = 0.002f;
    [Range(1f, 30f)]
    public float frequency = 10f;
    [Range(10f, 100f)]
    public float smooth = 10f;

    Vector3 startPos;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        inputManager = GetComponent<InputManager>();
        camera = playerController.camera;
        startPos = camera.transform.localPosition;
    }

    public IEnumerator modifyFOV(int _option)
    {
        // Ici il y a plusieurs dézoom différent pouvant être pratiqué. Ducoup on vérifie quel option de zoom est affecté en paramètre et on applique ensuite le type de zoom en question
        switch (_option)
        {
            // Ici on augmente le FOV, pour indiquer que le joueur a commencer à sprinter
            case 0:
                for (int i = 0; i < 10; i++)
                {
                    camera.fieldOfView += 1;
                    yield return new WaitForSeconds(.01f);
                }
                break;
            // Ici on diminiue le FOV de 10, soit pour terminer un sprint ou pour indiquer que le joueur est blessé
            case 1:
                for (int i = 0; i < 10; i++)
                {
                    camera.fieldOfView -= 1;
                    yield return new WaitForSeconds(.01f);
                }
                break;
        }
    }

    private void Update()
    {
        CheckForHeadBobTrigger();
        StopHeadBob();
    }

    private void CheckForHeadBobTrigger()
    {
        if (inputManager.horizontalInput.magnitude > 0)
        {
            StartHeadBob();
        }
    }

    private Vector3 StartHeadBob()
    {

        if (playerController.isSprinting)
            frequency = 20f;
        else
            frequency = 10f;
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency) * amount * 1.4f, smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Sin(Time.time * frequency /2f ) * amount * 1.4f, smooth * Time.deltaTime);
        camera.transform.localPosition += pos;

        return pos;
    }

    private void StopHeadBob()
    {
        if (camera.transform.localPosition == startPos) return;
        camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, startPos, 1 * Time.deltaTime);
    }
}
