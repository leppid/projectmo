using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float smoothness;
    public Transform targetObject;
    private Vector3 cameraPosition;
    private Vector3 cameraOffset;

    void Start()
    {
        Application.targetFrameRate = 120;
        cameraOffset = transform.position - targetObject.position;
    }

    void Update()
    {
        transform.GetChild(0).gameObject.SetActive(Input.touchCount < 2);

        HandleFolow();

        if (Input.touchCount < 2) return;

        HandleRotation();
    }

    void HandleFolow()
    {
        transform.LookAt(targetObject);
        cameraPosition = targetObject.position + cameraOffset;
        transform.position = Vector3.Lerp(transform.position, cameraPosition, smoothness*Time.fixedDeltaTime);
    }

    // void HandleZoom()
    // {
    //     float currentLevel = transform.position.y;
    //     float newLevel;

    //     if (pinchDistanceDelta < 0) {
    //         if (currentLevel > 0 && currentLevel < 19) 
    //         {
    //             newLevel = 25;
    //         } else if (currentLevel > 19 && currentLevel < 26)
    //         {
    //             newLevel = 50;
    //         } else
    //         {
    //             return;
    //         }
    //     }
    //     else
    //     {
    //         if (currentLevel > 26 && currentLevel < 51) 
    //         {
    //             newLevel = 25;
    //         } else if (currentLevel > 19 && currentLevel < 26)
    //         {
    //             newLevel = 18;
    //         } else
    //         {
    //             return;
    //         }
    //     }

    //     Vector3 zoomCameraPosition = new Vector3(transform.position.x, newLevel, transform.position.z);
    //     cameraOffset = zoomCameraPosition - targetObject.position;
    // }


    void HandleRotation()
    {
        Touch touch0 = Input.GetTouch(0);
        switch (touch0.phase) {
            case TouchPhase.Stationary:
                break;
            case TouchPhase.Moved:
                break;
            default: 
                return;
        }

        Touch touch1 = Input.GetTouch(1);
        switch (touch1.phase) {
            case TouchPhase.Stationary:
                break;
            case TouchPhase.Moved:
                break;
            default:
                return;
        }

        var pos1 = touch0.position;
        var pos2 = touch1.position;
        var pos1b = touch0.position - touch0.deltaPosition;
        var pos2b = touch1.position - touch1.deltaPosition;

        var screenCenter = targetObject.position;
    
        transform.position = targetObject.position + cameraOffset;
        transform.RotateAround(screenCenter, Vector3.up, Vector3.SignedAngle(pos2b - pos1b, pos2 - pos1, Vector3.forward));
        cameraOffset = transform.position - targetObject.position;
    }
}
