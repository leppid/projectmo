using UnityEngine;
using Newtonsoft.Json;
using ProjectModels;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public PlayerWorld _playerWorld;
    public CameraWorld _cameraWorld;
    public PlayerData PlayerData = new() { displayName = "Guest", id = "-1", token = "none", location = "Hills", position = "(250.34, 0.57, 250.84)", bagPages = 1 };

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (_playerWorld == null)
            _playerWorld = GameObject.Find("PlayerWorld").GetComponent<PlayerWorld>();

        if (_cameraWorld == null)
            _cameraWorld = GameObject.Find("MainCamera").GetComponent<CameraWorld>();

        SetPrefsData();
        RestoreLastPosition();
        _playerWorld.UpdateNickname(PlayerData.displayName);
    }

    public void SetPrefsData()
    {
        string guestDataString = PlayerPrefs.GetString("guestJson", "null");
        string playerDataString = PlayerPrefs.GetString("playerJson", "null");

        if (playerDataString != "null")
        {
            PlayerData = JsonConvert.DeserializeObject<PlayerData>(playerDataString);
        }
        else if (guestDataString != "null")
        {
            PlayerData = JsonConvert.DeserializeObject<PlayerData>(guestDataString);
        }
    }

    public void FetchPlayer()
    {
        if (PlayerPrefs.GetString("authToken", "null") == "null") return;

        ApiManager.instance.Get<PlayerData>("session")
        .Then(res =>
        {
            PlayerData = res;
            PlayerPrefs.SetString("playerJson", JsonConvert.SerializeObject(res));
        });
    }

    public void SaveLastPosition()
    {
        if (PlayerPrefs.GetString("authToken", "null") == "null") return;

        string playerPos = _playerWorld.transform.position.ToString();
        string playerLoc = SceneManager.GetActiveScene().name;

        ApiManager.instance.Post<PlayerData>("sync_position", new PlayerLocationParams { location = playerLoc, position = playerPos });
    }

    public void RestoreLastPosition()
    {
        string playerLastPosString = PlayerData.position;

        if (playerLastPosString.Length > 0)
        {
            Vector3 playerLastPos = ProjectUtils.StringToVector3(playerLastPosString);
            Vector3 cameraLastPos = new Vector3(playerLastPos.x, playerLastPos.y + 26, playerLastPos.z - 17);

            _playerWorld.SetPosition(playerLastPos);
            _cameraWorld.SetPosition(cameraLastPos);
        }
    }

    public void OnApplicationPause()
    {
        SaveLastPosition();
    }
}

