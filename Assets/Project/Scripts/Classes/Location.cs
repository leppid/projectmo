using UnityEngine;
using ProjectModels;

public class Location : MonoBehaviour
{
    public string id;
    public string sceneName;
    public string message;
    private LocationData data;
    private ActionData actionData;

    void Awake()
    {
        data = new LocationData { id = id, sceneName = sceneName, message = message };
        actionData = new ActionData { location = data };
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            UiManager.instance.SetActionData(actionData);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
           UiManager.instance.ClearActionData();
        }
    }
}
