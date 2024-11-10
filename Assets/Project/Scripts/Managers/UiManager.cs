using System;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    public GameObject _UI;
    public CameraController _camera;

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
        _UI.GetComponent<WorldEvents>().RotateCompass(-Camera.main.transform.eulerAngles.y);
    }

    public void ShowBottomBar()
    {
        _UI.GetComponent<WorldEvents>().ShowBottomBar();
    }

    public void HideBottomBar()
    {
        _UI.GetComponent<WorldEvents>().HideBottomBar();
    }

        public void ShowCompass()
    {
        _UI.GetComponent<WorldEvents>().ShowCompass();
    }

    public void HideCompass()
    {
        _UI.GetComponent<WorldEvents>().HideCompass();
    }

    public void ResetCompass()
    {
        _camera.Reset();
    }
}

