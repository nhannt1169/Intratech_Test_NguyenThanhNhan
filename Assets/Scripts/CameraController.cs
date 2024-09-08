using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 10;
    [SerializeField] private float panSpeed = 10;
    [SerializeField] private float orbitSpeed = 10;
    [SerializeField] private float zoomMin = 0.1f;
    [SerializeField] private float zoomMax = 10f;

    private Vector3 previousMousePosition;

    void LateUpdate()
    {
        // Update previousMousePosition first time to prevent jumping when first click
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            previousMousePosition = Camera.main.orthographic ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : Input.mousePosition;
        }

        Zoom();
        Pan();
        Orbit();

        // Update previousMousePosition when left or right mouse button is pressed
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            previousMousePosition = Camera.main.orthographic ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : Input.mousePosition;
        }
    }

    private void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (Camera.main.orthographic)
            {
                Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - scroll * zoomSpeed, 0.1f);
            }
            transform.Translate(scroll * zoomSpeed * Vector3.forward, Space.Self);
        }
    }

    // Pan with the right mouse button
    void Pan()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 mousePosDiff;
            if (Camera.main.orthographic)
            {
                mousePosDiff = previousMousePosition - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position += mousePosDiff;
            }
            else
            {
                mousePosDiff = Input.mousePosition - previousMousePosition;
                Vector3 panMovement = panSpeed * Time.deltaTime * new Vector3(-mousePosDiff.x, -mousePosDiff.y, 0);
                transform.Translate(panMovement, Space.Self);
            }
        }
    }

    // Orbit with the left mouse button
    void Orbit()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosDiff;
            if (Camera.main.orthographic)
            {
                mousePosDiff = previousMousePosition - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float horizontal = mousePosDiff.x * orbitSpeed * Time.deltaTime;
                float vertical = mousePosDiff.y * orbitSpeed * Time.deltaTime;

                transform.Rotate(Vector3.right, -vertical);
                transform.Rotate(Vector3.up, -horizontal, Space.World);
            }
            else
            {
                mousePosDiff = Input.mousePosition - previousMousePosition;
                float horizontal = mousePosDiff.x * orbitSpeed * Time.deltaTime;
                float vertical = -mousePosDiff.y * orbitSpeed * Time.deltaTime;

                transform.RotateAround(Vector3.zero, Vector3.up, horizontal);
                transform.RotateAround(Vector3.zero, transform.right, vertical);
            }
        }
    }
}
