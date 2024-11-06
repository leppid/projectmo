using UnityEngine;
using Proyecto26;
using RSG;

public class ApiManager : MonoBehaviour
{
    public static ApiManager instance;
    
    private readonly string basePath = "http://localhost:3000/";
    private RequestHelper currentRequest;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public Promise<T> Get<T>(string path, int id = -1)
    {
        if (id != -1)
            return (Promise<T>)RestClient.Get(basePath + path + "/" + id);
        else
            return (Promise<T>)RestClient.Get(basePath + path);
    }

    public Promise<T> Post<T>(string path, object body)
    {
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
}

