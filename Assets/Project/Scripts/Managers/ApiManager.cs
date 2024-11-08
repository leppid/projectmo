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
    private RequestHelper currentRequest;

    public Promise<T> Get<T>(string path, int id = 0)
    {
        SetAuthorizationHeader();

        if (id != 0)
            return (Promise<T>)RestClient.Get<T>(basePath + path + "/" + id);
        else
            return (Promise<T>)RestClient.Get<T>(basePath + path);
    }

    public Promise<T> Post<T>(string path, object body)
    {
        SetAuthorizationHeader();

        currentRequest = new RequestHelper
        {
            Uri = basePath + path,
            Body = body,
            EnableDebug = false
        };

        return (Promise<T>)RestClient.Post<T>(currentRequest);
    }

    public Promise<T> Put<T>(string path, object body)
    {
        SetAuthorizationHeader();

        currentRequest = new RequestHelper
        {
            Uri = basePath + path,
            Body = body,
            Retries = 5,
            RetrySecondsDelay = 1,
            RetryCallback = (err, retries) =>
            {
                Debug.Log(string.Format("Retry #{0} Status {1}\nError: {2}", retries, err.StatusCode, err));
            }
        };
        return (Promise<T>)RestClient.Put<ResponseHelper>(currentRequest);
    }

    public Promise<T> Delete<T>(string path)
    {
        SetAuthorizationHeader();

        return (Promise<T>)RestClient.Delete(basePath + path);
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

