using UnityEngine;
using ProjectModels;
using TMPro;

public class Location : MonoBehaviour
{
    public string id;
    public string sceneName;
    public Vector3 spawnPosition;
    public string message = null;
    private LocationData data;
    private ActionData actionData;
    private Transform _name;

    void Awake()
    {
        data = new LocationData { id = id, sceneName = sceneName, message = message, spawnPosition = spawnPosition };
        actionData = new ActionData { location = data };
        _name = transform.Find("Name");
        _name.GetComponent<TextMeshProUGUI>().text = sceneName;
    }

    void Update()
    {
        _name.transform.rotation = Camera.main.transform.rotation;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            UIManager.instance.SetActionData(actionData);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            UIManager.instance.ClearActionData();
        }
    }
}
