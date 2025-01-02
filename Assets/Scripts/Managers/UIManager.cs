using UnityEngine;
using ProjectModels;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public WorldEvents _interfaceWorld;
    public CameraWorld _cameraWorld;
    public bool levelIsLoading = false;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Update()
    {
        RotateCompassByCamera();
    }

    public void DisplayBottomBar(bool show = true)
    {
        _interfaceWorld.DisplayBottomBar(show);
    }

    public void DisplayCompass(bool show = true)
    {
        _interfaceWorld.DisplayCompass(show);
    }

    public void RotateCompassByCamera()
    {
        _interfaceWorld.RotateCompass(-_cameraWorld.transform.eulerAngles.y);
    }

    public void ResetCompass()
    {
        _cameraWorld.Reset();
    }

    public void SetActionData(ActionData data)
    {

        _interfaceWorld.SetActionData(data);
    }

    public void ClearActionData()
    {

        _interfaceWorld.ClearActionData();
    }
}

