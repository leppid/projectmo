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
        Application.targetFrameRate = 120;
        uiDoc = GetComponent<UIDocument>();
        _loading = uiDoc.rootVisualElement.Q<Label>("Loading");
        _error = uiDoc.rootVisualElement.Q<Label>("Error");
        _logoButton = uiDoc.rootVisualElement.Q<Button>("Logo");
        _logoButton.clicked += LogoPressed;
        _form = uiDoc.rootVisualElement.Q<VisualElement>("Form");
        _loginInput = uiDoc.rootVisualElement.Q<TextField>("Login");
        _passwordInput = uiDoc.rootVisualElement.Q<TextField>("Password");
        _loginInput.RegisterCallback<ChangeEvent<string>>(DisplaySubmit);
        _passwordInput.RegisterCallback<ChangeEvent<string>>(DisplaySubmit);
        _authButton = uiDoc.rootVisualElement.Q<Button>("Auth");
        _authButton.clicked += CreateSession;
    }

    void Start()
    {
        PlayerPrefs.DeleteKey("playerJson");
        PlayerPrefs.DeleteKey("guestJson");
        PlayerPrefs.DeleteKey("inventoryJson");

        if (PlayerPrefs.GetString("isLogout") == "true")
        {
            PlayerPrefs.DeleteKey("isLogout");

            string errorPref = PlayerPrefs.GetString("errorMessage", "null");

            if (errorPref != "null")
            {
                DisplayError(true, errorPref);
                PlayerPrefs.DeleteKey("errorMessage");
            }
            else
                PlayerPrefs.DeleteKey("authToken");

            return;
        };

        FetchSession();
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
            PlayerData guest = new() { displayName = "Guest", id = "-1", token = "none", location = "Hills", position = "(250.34, 0.57, 250.84)", bagPages = 2 };
            PlayerPrefs.SetString("guestJson", JsonConvert.SerializeObject(guest));
            PlayerPrefs.DeleteKey("authToken");
            StartCoroutine(LoadPrototype());
        }
    }

    void CreateSession()
    {
        DisplayInputs(false);

        _authButton.text = "Authenticating...";

        var login = _loginInput.value;
        var password = _passwordInput.value;

        ApiManager.instance.Post<PlayerData>("session", new SessionParams { login = login, password = password })
        .Then(res =>
        {
            PlayerPrefs.SetString("authToken", res.token);
            PlayerPrefs.SetString("playerJson", JsonConvert.SerializeObject(res));
            DisplayError(false);
            loadLocation = res.location?.Length > 0 ? res.location : "Hills";
            FetchInventory();
        }).Catch((err) =>
        {
            RequestException error = (RequestException)err.GetBaseException();

            if (error.IsNetworkError) DisplayError(true, "Connection error.");

            if (error.IsHttpError)
            {
                ServerError se = JsonConvert.DeserializeObject<ServerError>(error.Response);
                DisplayError(true, se.message);
            }

            DisplayInputs(true);

            _authButton.text = "Authorize";
        });
    }

    void FetchSession()
    {
        if (PlayerPrefs.GetString("authToken", "null") == "null") return;

        DisplayLoading(true);

        ApiManager.instance.Get<PlayerData>("session")
        .Then(res =>
        {
            loadLocation = res.location?.Length > 0 ? res.location : "Hills";
            PlayerPrefs.SetString("playerJson", JsonConvert.SerializeObject(res));
            FetchInventory();
        }).Catch(err =>
        {
            RequestException error = (RequestException)err.GetBaseException();

            if (error.IsNetworkError) DisplayError(true, "Connection error.");

            if (error.IsHttpError)
            {
                ServerError se = JsonConvert.DeserializeObject<ServerError>(error.Response);
                DisplayError(true, se.message);
            }

            DisplayLoading(false);
        });
    }

    void FetchInventory()
    {
        ApiManager.instance.GetArray<ItemData>("player/inventory").Then(res =>
        {
            PlayerPrefs.SetString("inventoryJson", JsonConvert.SerializeObject(res));
            StartCoroutine(LoadPrototype());
        }).Catch(err =>
        {
            StartCoroutine(LoadPrototype());
        });
    }

    void DisplaySubmit(ChangeEvent<string> evt)
    {
        _authButton.SetEnabled(_loginInput.value.Length > 0 && _passwordInput.value.Length > 0);
    }


    void DisplayInputs(bool enable)
    {
        _loginInput.SetEnabled(enable);
        _passwordInput.SetEnabled(enable);
        _authButton.SetEnabled(enable);
    }

    void DisplayLoading(bool loading)
    {
        _form.style.display = loading ? DisplayStyle.None : DisplayStyle.Flex;
        _loading.style.display = loading ? DisplayStyle.Flex : DisplayStyle.None;
    }

    void DisplayError(bool show = true, string error = "Something went wrong.")
    {
        StartCoroutine(DisplayErrorEnum(show, error));
    }

    IEnumerator DisplayErrorEnum(bool show, string error)
    {
        yield return new WaitForSeconds(0.01f);
        _error.text = show ? error : "";
        _error.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public IEnumerator LoadPrototype()
    {
        DisplayLoading(true);
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
