using GameClasses;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowGamesHistory : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;
    BaseGameAccount player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerStats.player;
        if (player.GamesCount > 0)
        {
            text.text = player.GetStats();
        }
        else
        {
            text.text = "No games were found";
        }
    }

    public void HandleOnBackButtonEvent()
    {
        SceneManager.LoadScene("PlayerStatsScene");
    }

}
