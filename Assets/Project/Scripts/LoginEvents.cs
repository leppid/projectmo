using UnityEngine;
using UnityEngine.UIElements;

public class LoginEvents : MonoBehaviour
{
    private UIDocument uiDoc;
    private VisualElement _form;
    private TextField _loginInput;
    private TextField _passwordInput;
    private Button _authButton;

    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        _form = uiDoc.rootVisualElement.Q<VisualElement>("Form");
        _loginInput = uiDoc.rootVisualElement.Q<TextField>("Login");
        _loginInput.RegisterCallback<ChangeEvent<string>>(OnInputChanged);
        _passwordInput = uiDoc.rootVisualElement.Q<TextField>("Password");
        _passwordInput.RegisterCallback<ChangeEvent<string>>(OnInputChanged);
        _authButton = uiDoc.rootVisualElement.Q<Button>("Auth");
        _authButton.clicked += OnAuthButtonClicked;
    }

    private void OnInputChanged(ChangeEvent<string> evt)
    {
        _authButton.SetEnabled(_loginInput.value.Length > 0 && _passwordInput.value.Length > 0);
    }

    void OnAuthButtonClicked()
    {
        SendAuthRequest();
    }

    public class Player
    {
        public string id;
        public string login;
    }

    public class PlayerData
    {
        public string login;
        public string password;
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
            _authButton.text = "Authorize";
            EnableInputs(true);
            Debug.Log(JsonUtility.ToJson(res, true));
        }).Catch(err =>
        {
            _authButton.text = "Authorize";
            EnableInputs(true);
            Debug.Log(err.Message);
        });
    }

    void EnableInputs(bool enable)
    {
        _loginInput.SetEnabled(enable);
        _passwordInput.SetEnabled(enable);
        _authButton.SetEnabled(enable);
    }

}
