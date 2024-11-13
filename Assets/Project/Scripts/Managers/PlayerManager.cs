using UnityEngine;
using Newtonsoft.Json;
using ProjectModels;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public PlayerWorld _playerWorld;
    public CameraWorld _cameraWorld;
    public PlayerData PlayerData;

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

        FetchPlayerData();
        RestoreLastPosition();
        UpdateNickname(PlayerData.displayName);
    }

    public void FetchPlayerData()
    {
        if (PlayerPrefs.GetString("playerJson", "null") != "null")
            PlayerData = JsonConvert.DeserializeObject<PlayerData>(PlayerPrefs.GetString("playerJson"));
        else
            PlayerData = new PlayerData { displayName = "Guest", id = "-1", token = "none", location = "Hills", position = "(250.34, 0.57, 250.84)" };
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

    public void SaveLastPosition()
    {
        if (PlayerPrefs.GetString("authToken", "null") == "null") return;
        
        string playerPos = _playerWorld.transform.position.ToString();
        string playerLoc = SceneManager.GetActiveScene().name;

        ApiManager.instance.Put<PlayerData>("player/me", new PlayerLocationParams { location = playerLoc, position = playerPos });
    }

    public void UpdateNickname(string text)
    {
        _playerWorld.UpdateNickname(text);
    }

    public void OnApplicationPause()
    {
        SaveLastPosition();
    }
}

