using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using PlayerModels;

public class WorldEvents : MonoBehaviour
{
    private UIDocument uiDoc;
    private Button _logoutButton;
    private VisualElement _loading;

    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        _logoutButton = uiDoc.rootVisualElement.Q<Button>("Logout");
        _loading = uiDoc.rootVisualElement.Q<VisualElement>("Loading");
    }

    private void Start()
    {
        _loading.style.display = DisplayStyle.None;
        _logoutButton.style.display = DisplayStyle.Flex;
        _logoutButton.clicked += OnLogoutButtonClicked;
    }

    private void OnLogoutButtonClicked()
    {
        _loading.style.display = DisplayStyle.Flex;
        _logoutButton.style.display = DisplayStyle.None;
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
