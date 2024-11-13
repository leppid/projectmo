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
    private bool IsMenuOpen = false;

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
        _logoutButton.clicked += StartLogout;
        _menuButton.clicked += HandleMenu;
        _compassButton.clicked += ResetCompass;
        _actionButton.clicked += HandleAction;
        _messageBlock.clicked += CloseMessage;
        DisplayBottomBar(true);
        DisplayCompass(true);
    }

    public void DisplayBottomBar(bool show = true)
    {
        if (show)
        {
            StartCoroutine(ShowBottomBarEnum());
        }
        else
        {
            StartCoroutine(HideBottomBarEnum());
        }
    }

    private IEnumerator ShowBottomBarEnum()
    {
        yield return new WaitForSeconds(.2f);
        _bottomBar.style.bottom = 0f;
    }

    private IEnumerator HideBottomBarEnum()
    {
        DisplayMenu(false);
        yield return new WaitForSeconds(.2f);
        _bottomBar.style.bottom = -220f;
    }

    public void HandleMenu()
    {
        DisplayMenu(!IsMenuOpen);
    }

    public void DisplayMenu(bool show = true)
    {
        if (show)
        {

            StartCoroutine(ShowMenuBlockEnum());
        }
        else
        {
            StartCoroutine(HideMenuBlockEnum());
        }
    }

    private IEnumerator ShowMenuBlockEnum()
    {
        if (!IsMenuOpen)
        {
            IsMenuOpen = true;
            _menuBlock.style.display = DisplayStyle.Flex;
            _menuBlock.style.bottom = 220f;
            _menuButton.AddToClassList("menu-button-active");
            yield return null;
        }
    }

    private IEnumerator HideMenuBlockEnum()
    {
        if (IsMenuOpen)
        {
            IsMenuOpen = false;
            _menuBlock.style.bottom = -400f;
            yield return new WaitForSeconds(.2f);
            _menuBlock.style.display = DisplayStyle.None;
            _menuButton.RemoveFromClassList("menu-button-active");

        }
    }

    public void DisplayCompass(bool show = true)
    {
        if (show)
        {
            StartCoroutine(ShowCompassEnum());
        }
        else
        {
            StartCoroutine(HideCompassEnum());
        }
    }

    private IEnumerator ShowCompassEnum()
    {
        _compassBlock.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(.1f);
        _compassBlock.style.top = 0f;

    }

    private IEnumerator HideCompassEnum()
    {
        _compassBlock.style.top = -500f;
        yield return new WaitForSeconds(.3f);
        _compassBlock.style.display = DisplayStyle.None;
    }

    public void RotateCompass(float angle)
    {
        _compassBlock.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void ResetCompass()
    {
        UIManager.instance.ResetCompass();
    }

    public void StartLogout()
    {
        _mainBlock.style.display = DisplayStyle.None;
        _loadingBlock.style.display = DisplayStyle.Flex;

        PlayerManager.instance.SaveLastPosition();
        PlayerPrefs.SetString("IsLogOut", "true");

        StartCoroutine(LoadLogin());
    }

    private IEnumerator LoadLogin()
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

    public void HandleAction()
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
            StartCoroutine(ShowMessageEnum());
        }
    }

    public void CloseMessage()
    {
        DisplayMessage(false);
    }

    public void DisplayMessage(bool show = true)
    {
        if (show)
        {
            StartCoroutine(ShowMessageEnum());
        }
        else
        {
            StartCoroutine(HideMessageEnum());
        }
    }

    private IEnumerator ShowMessageEnum()
    {
        _actionButton.style.display = DisplayStyle.None;
        _messageBlock.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(5f);
        _messageBlock.style.display = DisplayStyle.None;
    }

    private IEnumerator HideMessageEnum()
    {
        _messageBlock.style.display = DisplayStyle.None;
        yield return null;
        StopAllCoroutines();
    }

}
