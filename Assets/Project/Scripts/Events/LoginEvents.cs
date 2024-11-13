using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using ProjectModels;
using Proyecto26;

public class LoginEvents : MonoBehaviour
{
    private UIDocument uiDoc;
    private Button _logoButton;
    private VisualElement _form;
    private Label _loading;
    private Label _error;
    private TextField _loginInput;
    private TextField _passwordInput;
    private Button _authButton;
    private int logoClickedTimes = 0;
    private string loadLocation = "Hills";

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        _logoButton = uiDoc.rootVisualElement.Q<Button>("Logo");
        _logoButton.clicked += LogoPressed;
        _form = uiDoc.rootVisualElement.Q<VisualElement>("Form");
        _loading = uiDoc.rootVisualElement.Q<Label>("Loading");
        _error = uiDoc.rootVisualElement.Q<Label>("Error");
        _loginInput = uiDoc.rootVisualElement.Q<TextField>("Login");
        _passwordInput = uiDoc.rootVisualElement.Q<TextField>("Password");
        _loginInput.RegisterCallback<ChangeEvent<string>>(HandleButton);
        _passwordInput.RegisterCallback<ChangeEvent<string>>(HandleButton);
        _authButton = uiDoc.rootVisualElement.Q<Button>("Auth");
        _authButton.clicked += AuthPressed;
    }

    void Start()
    {
        PlayerPrefs.DeleteKey("playerJson");

        if (PlayerPrefs.GetString("IsLogOut") == "true")
        {
            PlayerPrefs.DeleteKey("authToken");
            PlayerPrefs.DeleteKey("IsLogOut");
            return;
        };

        FetchSession();
    }

    void FetchSession()
    {
        _form.style.display = DisplayStyle.None;
        _loading.style.display = DisplayStyle.Flex;

        ApiManager.instance.Get<PlayerData>("session")
        .Then(res =>
        {
            loadLocation = res.location;
            PlayerPrefs.SetString("playerJson", JsonConvert.SerializeObject(res));
            StartCoroutine(LoadPrototype());
        }).Catch(err =>
        {
            _loading.style.display = DisplayStyle.None;
            _form.style.display = DisplayStyle.Flex;
        });
    }

    void LogoPressed()
    {
        if (logoClickedTimes < 3)
        {
            logoClickedTimes++;
        }
        else
        {
            logoClickedTimes = 0;
            StartCoroutine(LoadPrototype());
        }
    }

    void AuthPressed()
    {
        CreateSession();
    }

    void CreateSession()
    {
        EnableInputs(false);
        _authButton.text = "Authenticating...";

        var login = _loginInput.value;
        var password = _passwordInput.value;

        ApiManager.instance.Post<PlayerData>("session", new PlayerSessionParams { login = login, password = password })
        .Then(res =>
        {
            PlayerPrefs.SetString("authToken", res.token);
            PlayerPrefs.SetString("playerJson", JsonConvert.SerializeObject(res));
            _error.style.display = DisplayStyle.None;
            loadLocation = res.location ?? "Hills";
            StartCoroutine(LoadPrototype());
        }).Catch((err) =>
        {
            RequestException error = (RequestException)err.GetBaseException();
            if (error.IsNetworkError) _error.text = "Connection error";
            if (error.IsHttpError)
            {
                ServerError se = JsonConvert.DeserializeObject<ServerError>(error.Response);
                _error.text = se.message;
            }
            _error.style.display = DisplayStyle.Flex;
        }).Finally(() =>
        {
            _authButton.text = "Authorize";
            EnableInputs(true);
        });
    }

    void HandleButton(ChangeEvent<string> evt)
    {
        _authButton.SetEnabled(_loginInput.value.Length > 0 && _passwordInput.value.Length > 0);
    }


    void EnableInputs(bool enable)
    {
        _loginInput.SetEnabled(enable);
        _passwordInput.SetEnabled(enable);
        _authButton.SetEnabled(enable);
    }

    public IEnumerator LoadPrototype()
    {
        _form.style.display = DisplayStyle.None;
        _loading.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(1);
        AsyncOperation async = SceneManager.LoadSceneAsync(loadLocation);
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
