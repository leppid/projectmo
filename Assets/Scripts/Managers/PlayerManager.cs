using UnityEngine;
using Newtonsoft.Json;
using ProjectModels;
using UnityEngine.SceneManagement;
using Proyecto26;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public PlayerWorld _playerWorld;
    public CameraWorld _cameraWorld;
    public PlayerData PlayerData = new() { displayName = "Guest", id = "-1", token = "none", location = "Hills", position = "(250.34, 0.57, 250.84)", bagPages = 2 };
    string currentLocation;
    string currentPosition;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _playerWorld = GameObject.Find("PlayerWorld").GetComponent<PlayerWorld>();
        _cameraWorld = GameObject.Find("CameraWorld").GetComponent<CameraWorld>();

        FetchPlayerPrefs();
        SpawnPlayer();

        _playerWorld.UpdateNickname(PlayerData.displayName);
    }

    public void Start()
    {
        SyncPlayerLocation();
    }

    public void FetchPlayerData()
    {
        if (PlayerPrefs.GetString("authToken", "null") == "null") return;

        ApiManager.instance.Get<PlayerData>("session")
        .Then(res =>
        {
            PlayerData = res;
            PlayerPrefs.SetString("playerJson", JsonConvert.SerializeObject(res));
        });
    }


    public void FetchPlayerPrefs()
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

    public void SyncPlayerData()
    {
        if (PlayerPrefs.GetString("authToken", "null") == "null") return;

        currentLocation = SceneManager.GetActiveScene().name;
        currentPosition = _playerWorld.transform.position.ToString();

        ApiManager.instance.Post<ResponseHelper>("player/sync", new PlayerParams { location = currentLocation, position = currentPosition, inventory = InventoryManager.instance.inventoryData });
    }

    public void SyncPlayerLocation()
    {
        if (PlayerPrefs.GetString("authToken", "null") == "null") return;

        currentLocation = SceneManager.GetActiveScene().name;
        currentPosition = _playerWorld.transform.position.ToString();

        ApiManager.instance.Post<ResponseHelper>("player/sync", new PlayerParams { location = currentLocation, position = currentPosition });
    }

    public void SpawnPlayer()
    {
        string forceSpawnString = PlayerPrefs.GetString("forceSpawnCords", "null");

        if (forceSpawnString == "null")
        {
            string playerLastPosString = PlayerData.position;

            if (playerLastPosString.Length > 0)
            {
                Vector3 playerLastPos = ProjectUtils.StringToVector3(playerLastPosString);
                SetPlayerPosition(playerLastPos);
            }
        }
        else
        {
            Vector3 forceSpawnCords = ProjectUtils.StringToVector3(forceSpawnString);
            SetPlayerPosition(forceSpawnCords);
        }

        PlayerPrefs.DeleteKey("forceSpawnCords");
    }

    public void SetPlayerPosition(Vector3 position)
    {
        _playerWorld.SetPosition(position);
        _cameraWorld.SetPosition(new Vector3(position.x, position.y + 26, position.z - 17));
    }

    public void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SyncPlayerData();
    }
}

