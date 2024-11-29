using UnityEngine;
using Proyecto26;
using RSG;

public class ApiManager : MonoBehaviour
{
    private static ApiManager _instance = null;
    public static ApiManager instance
    {
        get { return _instance; }
    }

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private readonly string basePath = "http://91.202.145.155:3000/";
    private RequestHelper currentRequest = new()
    {
        EnableDebug = false,
        Timeout = 5
    };

    public Promise<T> Get<T>(string path, int id = 0)
    {
        SetAuthorizationHeader();


        if (id != 0)
        {
            currentRequest.Uri = basePath + path + "/" + id;
            return (Promise<T>)RestClient.Get<T>(currentRequest);
        }
        else
        {
            currentRequest.Uri = basePath + path;
            return (Promise<T>)RestClient.Get<T>(currentRequest);
        }
    }

    public Promise<T> Post<T>(string path, object body)
    {
        SetAuthorizationHeader();

        currentRequest.Uri = basePath + path;
        currentRequest.Body = body;

        return (Promise<T>)RestClient.Post<T>(currentRequest);
    }

    public Promise<T> Put<T>(string path, object body)
    {
        SetAuthorizationHeader();

        currentRequest.Uri = basePath + path;
        currentRequest.Body = body;
        currentRequest.Retries = 5;
        currentRequest.RetrySecondsDelay = 1;
        currentRequest.RetryCallback = (err, retries) =>
        {
            Debug.Log(string.Format("Retry #{0} Status {1}\nError: {2}", retries, err.StatusCode, err));
        };

        return (Promise<T>)RestClient.Put<T>(currentRequest);
    }

    public Promise<T> Delete<T>(string path)
    {
        SetAuthorizationHeader();

        currentRequest.Uri = basePath + path;

        return (Promise<T>)RestClient.Delete(currentRequest);
    }

    public void AbortRequest()
    {
        if (currentRequest != null)
        {
            currentRequest.Abort();
            currentRequest = null;
        }
    }

    private void SetAuthorizationHeader()
    {
        string token = "Bearer " + PlayerPrefs.GetString("authToken", "none");
        RestClient.DefaultRequestHeaders["Authorization"] = token;
    }
}

