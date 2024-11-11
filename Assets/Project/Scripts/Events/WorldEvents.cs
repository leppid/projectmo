using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using ProjectModels;

public class WorldEvents : MonoBehaviour
{
    private ActionData actionData;
    private UIDocument uiDoc;
    private VisualElement _loadingBlock;
    private VisualElement _mainBlock;
    private Button _actionButton;
    private Button _messageBlock;
    private VisualElement _compassBlock;
    private Button _compassButton;
    private VisualElement _bottomBar;
    private Button _menuButton;
    private VisualElement _menuBlock;
    private Button _logoutButton;
    public bool IsMenuOpen = false;

    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        _loadingBlock = uiDoc.rootVisualElement.Q<VisualElement>("LoadingBlock");
        _mainBlock = uiDoc.rootVisualElement.Q<VisualElement>("MainBlock");
        _messageBlock = uiDoc.rootVisualElement.Q<Button>("MessageBlock");
        _compassBlock = uiDoc.rootVisualElement.Q<VisualElement>("CompassBlock");
        _compassButton = uiDoc.rootVisualElement.Q<Button>("CompassButton");
        _actionButton = uiDoc.rootVisualElement.Q<Button>("ActionButton");
        _bottomBar = uiDoc.rootVisualElement.Q<VisualElement>("BottomBar");
        _menuButton = uiDoc.rootVisualElement.Q<Button>("MenuButton");
        _menuBlock = uiDoc.rootVisualElement.Q<VisualElement>("MenuBlock");
        _logoutButton = uiDoc.rootVisualElement.Q<Button>("LogoutButton");

        _bottomBar.style.bottom = -220f;
        _compassBlock.style.top = -500f;
    }

    private void Start()
    {
        _loadingBlock.style.display = DisplayStyle.None;
        _mainBlock.style.display = DisplayStyle.Flex;
        _logoutButton.clicked += LogoutPressed;
        _menuButton.clicked += MenuPressed;
        _compassButton.clicked += CompassPressed;
        _actionButton.clicked += ActionPressed;
        _messageBlock.clicked += MessagePressed;
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
        _compassBlock.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(.1f);
        _compassBlock.style.top = 0f;
    }

    public void HideCompass()
    {
        StartCoroutine(HideCompassEnum());
    }

    public IEnumerator HideCompassEnum()
    {
        _compassBlock.style.top = -500f;
        yield return new WaitForSeconds(.4f);
        _compassBlock.style.display = DisplayStyle.None;
    }

    public void RotateCompass(float angle)
    {
        _compassBlock.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void CompassPressed()
    {
        UiManager.instance.ResetCompass();
    }

    private void LogoutPressed()
    {
        _mainBlock.style.display = DisplayStyle.None;
        _loadingBlock.style.display = DisplayStyle.Flex;
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

    public void SetActionData(ActionData data)
    {
        actionData = data;
        _actionButton.text = data.buttonText();
        _actionButton.style.display = DisplayStyle.Flex;
    }

    public void ClearActionData()
    {
        _actionButton.style.display = DisplayStyle.None;
        actionData = null;
    }

    public void ActionPressed()
    {
        if (actionData != null)
        {
            switch (actionData.type())
            {
                case "location":
                    LoadLocation();
                    break;
                case "battle":
                    // TODO
                    break;
            }
        }
    }

    private void LoadLocation()
    {
        if (actionData.location.message != null)
        {
            _messageBlock.text = actionData.location.message;
            StartCoroutine(DisplayMessageEnum());
        }
    }

    private IEnumerator DisplayMessageEnum()
    {
        _actionButton.style.display = DisplayStyle.None;
        _messageBlock.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(5f);
        _messageBlock.style.display = DisplayStyle.None;
    }

    private void MessagePressed()
    {
        _messageBlock.style.display = DisplayStyle.None;
        StopAllCoroutines();
    }

}
