using UnityEngine;

public class CameraWorld : MonoBehaviour
{
    public float smoothness;
    public Transform targetObject;
    public Joystick Joystick;
    private Vector3 cameraPosition;
    private Vector3 cameraOffset;
    private Vector3 initialCameraOffset;
    private float resetAngle;

    void Start()
    {
        Application.targetFrameRate = 120;
        cameraOffset = transform.position - targetObject.position;
        initialCameraOffset = cameraOffset;
    }

    void Update()
    {
        transform.GetChild(0).gameObject.SetActive(Input.touchCount < 2);

        Folow();

        if (Input.touchCount < 2) return;

        Touch t1 = Input.touches[0];
        Touch t2 = Input.touches[1];

        if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved)
        {
            TouchLogic.Calculate();

            bool playerIsMoving = Joystick.Horizontal != 0 || Joystick.Vertical != 0;

            if (Mathf.Abs(TouchLogic.pinchDistanceDelta) > 0 && Mathf.Abs(TouchLogic.turnAngleDelta) < 0.5f && !playerIsMoving)
            {
                Zoom(TouchLogic.pinchDistanceDelta);
            }

            if (Mathf.Abs(TouchLogic.turnAngleDelta) > 0)
            {
                Rotate(TouchLogic.turnAngleDelta);
            }
        }
    }

    private void Folow()
    {
        // transform.LookAt(targetObject);
        cameraPosition = Vector3.Lerp(transform.position, targetObject.position + cameraOffset, smoothness * Time.deltaTime);
        transform.position = cameraPosition;
    }

    private void Zoom(float pinchDistanceDelta)
    {
        float zoomSpeed = 3f;
        float zoomAmount = pinchDistanceDelta * zoomSpeed * Time.deltaTime;
        Vector3 zoomResult = cameraOffset + transform.forward * zoomAmount;

        if (zoomResult.y > 30 || zoomResult.y < 15) return;

        cameraOffset += transform.forward * zoomAmount;
    }

    private void Rotate(float turnAngleDelta)
    {
        float angle = -turnAngleDelta * 0.6f;
        resetAngle += angle;
        transform.RotateAround(targetObject.position, Vector3.up, angle);
        cameraOffset = Quaternion.Euler(0, angle, 0) * cameraOffset;
    }

    public void Reset()
    {
        transform.RotateAround(targetObject.position, Vector3.up, -resetAngle);
        resetAngle = 0;
        cameraOffset = initialCameraOffset;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}
