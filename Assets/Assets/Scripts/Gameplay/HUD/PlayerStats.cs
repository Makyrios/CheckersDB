using GameClasses;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    public static BaseGameAccount player;
    private string input;

    // Start is called before the first frame update
    void Start()
    {
        inputField.onValueChanged.AddListener(SetPlayer);
    }

    public void SetPlayer(string field)
    {
        input = field;
    }

    public void HandleOnPlayerStatsButtonEvent()
    {
        if (int.TryParse(input, out int result))
        {
            player = DataBaseInitializer.singleton.userService.GetPlayerByID(result);
        }
        else
        {
            player = DataBaseInitializer.singleton.userService.GetPlayerByUsername(input);
        }
        if (player != null)
        {
            SceneManager.LoadScene("ShowPlayerStatsScene");
        }
        else
        {
            Debug.Log("Couldn't find the player");
        }
    }

    public void HandleOnGamesHistoryButtonEvent()
    {
        if (int.TryParse(input, out int result))
        {
            player = DataBaseInitializer.singleton.userService.GetPlayerByID(result);
        }
        else
        {
            player = DataBaseInitializer.singleton.userService.GetPlayerByUsername(input);
        }
        if (player != null)
        {
            SceneManager.LoadScene("GamesHistoryScene");
        }
        else
        {
            Debug.Log("Couldn't find the player");
        }
    }

    public void HandleOnReturnButtonEvent()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

}
