using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using PlayerModels;

public class WorldEvents : MonoBehaviour
{
    private UIDocument uiDoc;
    private VisualElement _bottomBar;
    private VisualElement _menuBlock;
    private Button _menuButton;
    private Button _logoutButton;
    private VisualElement _loading;
    private VisualElement _compass;
    private Button _compassButton;
    public bool IsMenuOpen = false;

    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        _bottomBar = uiDoc.rootVisualElement.Q<VisualElement>("BottomUi");
        _menuBlock = uiDoc.rootVisualElement.Q<VisualElement>("MenuBlock");
        _menuButton = uiDoc.rootVisualElement.Q<Button>("Menu");
        _logoutButton = uiDoc.rootVisualElement.Q<Button>("Logout");
        _loading = uiDoc.rootVisualElement.Q<VisualElement>("Loading");
        _compass = uiDoc.rootVisualElement.Q<VisualElement>("Compass");
        _compassButton = uiDoc.rootVisualElement.Q<Button>("CompassButton");
        _bottomBar.style.bottom = -220f;
        _compass.style.top = -500f;
    }

    private void Start()
    {
        _loading.style.display = DisplayStyle.None;
        _logoutButton.style.display = DisplayStyle.Flex;
        _logoutButton.clicked += LogoutPressed;
        _menuButton.clicked += MenuPressed;
        _compassButton.clicked += CompassPressed;
        ShowBottomBar();
        ShowCompass();
    }

    public void ShowBottomBar()
    {
        StartCoroutine(ShowBottomBarEnum());
    }

    public IEnumerator ShowBottomBarEnum()
    {
        yield return new WaitForSeconds(.2f);
        _bottomBar.style.bottom = 0f;
    }

    public void HideBottomBar()
    {
        _bottomBar.style.bottom = -220f;

        if (IsMenuOpen)
        {
            StartCoroutine(MenuPressedEnum());
        }
    }

    private void MenuPressed()
    {
        StartCoroutine(MenuPressedEnum());
    }

    IEnumerator MenuPressedEnum()
    {
        if (IsMenuOpen)
        {
            IsMenuOpen = false;
            _menuBlock.style.bottom = -400f;
            yield return new WaitForSeconds(.2f);
            _menuBlock.style.display = DisplayStyle.None;
            _menuButton.RemoveFromClassList("menu-button-active");

        }
        else
        {
            IsMenuOpen = true;
            _menuBlock.style.display = DisplayStyle.Flex;
            _menuBlock.style.bottom = 220f;
            _menuButton.AddToClassList("menu-button-active");
        }
    }

    public void ShowCompass()
    {
        StartCoroutine(ShowCompassEnum());
    }

    public IEnumerator ShowCompassEnum()
    {
        _compass.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(.1f);
        _compass.style.top = 0f;
    }

    public void HideCompass()
    {
        StartCoroutine(HideCompassEnum());
    }

    public IEnumerator HideCompassEnum()
    {
        _compass.style.top = -500f;
        yield return new WaitForSeconds(.4f);
        _compass.style.display = DisplayStyle.None;
    }

    public void RotateCompass(float angle)
    {
        _compass.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void CompassPressed()
    {
        UiManager.instance.ResetCompass();
    }

    private void LogoutPressed()
    {
        _loading.style.display = DisplayStyle.Flex;
        _menuBlock.style.display = DisplayStyle.None;
        _bottomBar.style.display = DisplayStyle.None;
        _compass.style.display = DisplayStyle.None;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("IsLogOut", "true");
        StartCoroutine(LoadLogin());
    }

    public IEnumerator LoadLogin()
    {
        yield return new WaitForSeconds(1);
        AsyncOperation async = SceneManager.LoadSceneAsync("Login");
        async.allowSceneActivation = false;

        while (async.isDone == false)
        {
            if (async.progress == .9f)
            {
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }

}
