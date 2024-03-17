using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ObserverController : MonoBehaviour
{
    [SerializeField]
    private Transform head;

    [SerializeField]
    private float speed = 5;

    private CharacterController _characterController;
    public CharacterController CharacterController
    {
        get
        {
            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();
            return _characterController;
        }
    }

    private float currentPitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float vertical = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        float yaw = 1.8f * Input.GetAxis("Mouse X");
        float pitch = 1.8f * Input.GetAxis("Mouse Y");
        currentPitch -= pitch;
        currentPitch = Mathf.Clamp(currentPitch, -90, 90);

        transform.Rotate(Vector3.up, yaw);
        CharacterController.Move(transform.forward * vertical + transform.right * horizontal);
        head.localRotation = Quaternion.AngleAxis(currentPitch, Vector3.right);    
    }
}
