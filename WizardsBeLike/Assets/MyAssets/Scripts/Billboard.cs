using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform playerTransform;
    public Camera mainCamera;

    private void LateUpdate()
    {
        transform.position = playerTransform.position;
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }
}