using Cinemachine;
using UnityEngine;

using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float panSpeed = 20f;
    public float zoomSpeed = 1500f;
    public Vector2 clampMin;
    public Vector2 clampMax;
    public float zoomMin = -5f;
    public float zoomMax = -25f;

    private void Update()
    {
        // Panning
        if (Input.GetMouseButton(1))
        {
            PanCamera();
        }

        // Zooming
        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > Mathf.Epsilon)
        {
            ZoomCamera();
        }
    }

    private void PanCamera()
    {
        float x = -Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
        float y = -Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;

        Vector3 newPosition = transform.position + new Vector3(x, y, 0);
        float zoomFactor = Mathf.InverseLerp(zoomMax, zoomMin, transform.position.z);

        // Clamping X and Y based on the zoom factor.
        // Adjust clamping logic if necessary
        newPosition.x = Mathf.Clamp(newPosition.x, clampMin.x, clampMax.x);
        newPosition.y = Mathf.Clamp(newPosition.y, clampMin.y, clampMax.y);

        transform.position = newPosition;
    }

    private void ZoomCamera()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        float newZoom = Mathf.Clamp(transform.position.z + zoomDelta, zoomMax, zoomMin);

        Vector3 position = transform.position;
        position.z = newZoom;
        transform.position = position;
    }
}

