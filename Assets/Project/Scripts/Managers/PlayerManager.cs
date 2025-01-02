using UnityEngine;
using Newtonsoft.Json;
using ProjectModels;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public PlayerWorld _playerWorld;
    public CameraWorld _cameraWorld;
    public PlayerData PlayerData = new() { displayName = "Guest", id = "-1", token = "none", location = "Hills", position = "(250.34, 0.57, 250.84)", bagPages = 2 };

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
        SpawnPlayer();
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

    public void SpawnPlayer()
    {
        string forceSpawnString = PlayerPrefs.GetString("forceSpawnCords", "null");

        if (forceSpawnString == "null")
        {
            RestoreLastPosition();
        }
        else
        {
            Vector3 forceSpawnCords = ProjectUtils.StringToVector3(forceSpawnString);
            SetPlayerPosition(forceSpawnCords);
            SaveLastPosition();
        }

        PlayerPrefs.DeleteKey("forceSpawnCords");
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
            SetPlayerPosition(playerLastPos);
        }
    }

    public void SetPlayerPosition(Vector3 position)
    {
        _playerWorld.SetPosition(position);
        _cameraWorld.SetPosition(new Vector3(position.x, position.y + 26, position.z - 17));
    }

    public void OnApplicationPause()
    {
        SaveLastPosition();
    }
}

