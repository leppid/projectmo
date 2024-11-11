using UnityEngine;
using Newtonsoft.Json;
using ProjectModels;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public PlayerWorld _player;
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

        if (PlayerPrefs.GetString("playerJson", "null") != "null")
            PlayerData = JsonConvert.DeserializeObject<PlayerData>(PlayerPrefs.GetString("playerJson"));
        else
            PlayerData = new PlayerData { login = "Guest", id = "-1", token = "none" };

        _player.UpdateNickname(PlayerData.login);
    }
}

