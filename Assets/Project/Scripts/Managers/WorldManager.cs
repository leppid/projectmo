using UnityEngine;
using PlayerModels;
using Newtonsoft.Json;
using TMPro;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    public PlayerOpenWorld _player;
    public Player player;

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
            player = JsonConvert.DeserializeObject<Player>(PlayerPrefs.GetString("playerJson"));
        else
            player = new Player { login = "Guest", id = "-1", token = "none" };

        _player.UpdateNickname(player.login);
    }
}

