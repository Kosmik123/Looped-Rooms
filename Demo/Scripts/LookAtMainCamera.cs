using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (mainCamera)
            transform.LookAt(mainCamera.transform);
    }
}
