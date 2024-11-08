using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using PlayerModels;
using Proyecto26;
using System;

public class LoginEvents : MonoBehaviour
{
    private UIDocument uiDoc;
    private VisualElement _form;
    private Label _loading;
    private Label _error;
    private TextField _loginInput;
    private TextField _passwordInput;
    private Button _authButton;

    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        _form = uiDoc.rootVisualElement.Q<VisualElement>("Form");
        _loading = uiDoc.rootVisualElement.Q<Label>("Loading");
        _error = uiDoc.rootVisualElement.Q<Label>("Error");
        _loginInput = uiDoc.rootVisualElement.Q<TextField>("Login");
        _loginInput.RegisterCallback<ChangeEvent<string>>(OnInputChanged);
        _passwordInput = uiDoc.rootVisualElement.Q<TextField>("Password");
        _passwordInput.RegisterCallback<ChangeEvent<string>>(OnInputChanged);
        _authButton = uiDoc.rootVisualElement.Q<Button>("Auth");
        _authButton.clicked += OnAuthButtonClicked;
    }

    private void Start()
    {
        if (PlayerPrefs.GetString("IsLogOut") == "true")
        {
            PlayerPrefs.DeleteAll();
            return;
        };

        _form.style.display = DisplayStyle.None;
        _loading.style.display = DisplayStyle.Flex;

        ApiManager.instance.Get<Player>("session")
        .Then(res =>
        {
            PlayerPrefs.SetString("playerJson", JsonConvert.SerializeObject(res));
            StartCoroutine(LoadPrototype());
        }).Catch(err =>
        {
            _loading.style.display = DisplayStyle.None;
            _form.style.display = DisplayStyle.Flex;
        });
    }

    private void OnInputChanged(ChangeEvent<string> evt)
    {
        _authButton.SetEnabled(_loginInput.value.Length > 0 && _passwordInput.value.Length > 0);
    }

    void OnAuthButtonClicked()
    {
        SendAuthRequest();
    }

    public class ServerMessage
    {
        public string message;
    }

    void SendAuthRequest()
    {
        _authButton.text = "Authenticating...";
        EnableInputs(false);

        var login = _loginInput.value;
        var password = _passwordInput.value;

        ApiManager.instance.Post<Player>("session", new PlayerData { login = login, password = password })
        .Then(res =>
        {
            PlayerPrefs.SetString("authToken", res.token);
            PlayerPrefs.SetString("playerJson", JsonConvert.SerializeObject(res));
            _form.style.display = DisplayStyle.None;
            _loading.style.display = DisplayStyle.Flex;
            _error.style.display = DisplayStyle.None;
            _authButton.text = "Authorize";
            EnableInputs(true);
            StartCoroutine(LoadPrototype());
        }).Catch((err) =>
        {
            RequestException error = (RequestException)err.GetBaseException();
            if (error.IsNetworkError) _error.text = "Connection error";
            if (error.IsHttpError)
            {
                ServerMessage msg = JsonConvert.DeserializeObject<ServerMessage>(error.Response);
                _error.text = msg.message;
            }
            _error.style.display = DisplayStyle.Flex;
            _authButton.text = "Authorize";
            EnableInputs(true);
        });
    }

    void EnableInputs(bool enable)
    {
        _loginInput.SetEnabled(enable);
        _passwordInput.SetEnabled(enable);
        _authButton.SetEnabled(enable);
    }

    public IEnumerator LoadPrototype()
    {
        yield return new WaitForSeconds(1);
        AsyncOperation async = SceneManager.LoadSceneAsync("World");
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
