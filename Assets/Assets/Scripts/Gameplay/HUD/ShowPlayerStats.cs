using GameClasses;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowPlayerStats : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Text;
    private StringBuilder Stats;
    private BaseGameAccount player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerStats.player;
        Stats = new StringBuilder();
        Stats.AppendLine($"Name: {player.Username}");
        Stats.AppendLine($"ID: {player.ID}");
        Stats.AppendLine($"Account type: {player.GetType().Name}");
        Stats.AppendLine($"Rating: {player.CurrentRating}");
        Stats.AppendLine($"Games count: {player.GamesCount}");

        Text.text = Stats.ToString();
    }

    public void HandlOnReturnButtonEvent()
    {
        SceneManager.LoadScene("PlayerStatsScene");
    }
}
