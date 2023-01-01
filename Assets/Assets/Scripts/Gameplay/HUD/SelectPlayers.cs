using GameClasses;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectPlayers : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField Rating;
    [SerializeField]
    private TMP_InputField Opponent;
    [SerializeField]
    private TMP_Dropdown GameType;


    private string opponent;
    public static BaseGameAccount WhiteCheckersPlayer;
    public static BaseGameAccount BlackCheckersPlayer;
    public static GameType CurrentGameType;
    public static int CurrentRating;

    private string message;
    private Timer timer;
    private bool displayMessage;

    // Start is called before the first frame update
    void Start()
    {
        Rating.onValueChanged.AddListener(SetRating);
        Rating.gameObject.SetActive(true);
        Opponent.onValueChanged.AddListener(SetOpponent);
        GameType.onValueChanged.AddListener(SetGameType);
        timer = gameObject.AddComponent<Timer>();
        timer.Duration = 1;
        displayMessage = false;
    }

    public void SetOpponent(string player)
    {
        opponent = player;
    }

    public void SetGameType(int type)
    {
        switch (type)
        {
            case 0:
                CurrentGameType = GameClasses.GameType.StandartGame;
                Rating.gameObject.SetActive(true);
                break;

            case 1:
                CurrentGameType = GameClasses.GameType.TrainingGame;
                Rating.gameObject.SetActive(false);
                break;
            case 2:
                CurrentGameType = GameClasses.GameType.AllInRatingGame;
                Rating.gameObject.SetActive(false);
                break;

        }
    }

    public void SetRating(string rating)
    {
        if (int.TryParse(rating, out int result))
        {
            CurrentRating = result;
        }
    }

    public void HandleOnPlayButtonClick()
    {
        BaseGameAccount opp = null;
        try
        {
            opp = DataBaseInitializer.singleton.userService.GetPlayerByUsername(opponent);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        if (opp == null)
        {
            message = "Opponent was not found";
            displayMessage = true;
            throw new ArgumentException("Opponent was not found");
        }
        else if (opp == DataBaseInitializer.singleton.CurrentPlayer)
        {
            message = "You cannot play with yourself";
            displayMessage = true;
            throw new ArgumentException("You cannot play with yourself");
        }
        else if (CurrentGameType == GameClasses.GameType.StandartGame && CurrentRating == 0)
        {
            message = "Enter rating in Rating field";
            displayMessage = true;
            throw new ArgumentException("Enter rating");
        }

        // Random game sides
        int side = UnityEngine.Random.Range(0, 2);
        if (side == 0)
        {
            WhiteCheckersPlayer = DataBaseInitializer.singleton.CurrentPlayer;
            BlackCheckersPlayer = opp;
        }
        else
        {
            WhiteCheckersPlayer = opp;
            BlackCheckersPlayer = DataBaseInitializer.singleton.CurrentPlayer;
        }

        SceneManager.LoadScene("GameScene");
    }

    public void HandleOnReturnButtonEvent()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void func(int id)
    {
    }

    void OnGUI()
    {
        if (displayMessage)
        {
            GUI.ModalWindow(1, new Rect(Screen.width / 2 - 100, Screen.height / 2 - 130, 250, 50), func, message);
            if (timer.Finished)
            {
                displayMessage = false;
                timer.Stop();
            }
            else
            {
                timer.Run();
            }
        }
    }
}
