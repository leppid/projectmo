using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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

    public void LoadLevel(string name, bool sync = true)
    {
        UIManager.instance.DisplayLoading(true);
        if (sync)
            InventoryManager.instance.SyncInventory(() => StartCoroutine(LoadLevelAsync(name)));
        else
            StartCoroutine(LoadLevelAsync(name));
    }

    IEnumerator LoadLevelAsync(string name)
    {
        yield return new WaitForSeconds(1);
        AsyncOperation async = SceneManager.LoadSceneAsync(name);
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

