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

        player = JsonConvert.DeserializeObject<Player>(PlayerPrefs.GetString("playerJson"));
        _player.UpdateNickname(player.login);
    }
}

