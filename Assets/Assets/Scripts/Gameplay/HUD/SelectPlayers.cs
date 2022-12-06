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
    private TMP_InputField Player1;
    [SerializeField]
    private TMP_InputField Player2;
    [SerializeField]
    private TMP_Dropdown GameType;
    string p1;
    string p2;
    TMP_InputField ratingObject;
    public static BaseGameAccount WhiteCheckersPlayer;
    public static BaseGameAccount BlackCheckersPlayer;
    public static GameType CurrentGameType;
    public static int CurrentRating;

    private Timer timer;
    private bool displayMessage;

    // Start is called before the first frame update
    void Start()
    {
        ratingObject = Instantiate(Rating, FindObjectOfType<SelectPlayers>().gameObject.transform);
        Player1.onValueChanged.AddListener(SetPlayer1);
        Player2.onValueChanged.AddListener(SetPlayer2);
        GameType.onValueChanged.AddListener(SetGameType);
        ratingObject.onValueChanged.AddListener(SetRating);
        timer = gameObject.AddComponent<Timer>();
        timer.Duration = 1;
        displayMessage = false;
    }
    public void SetPlayer1(string p)
    {
        p1 = p;
    }

    public void SetPlayer2(string p)
    {
        p2 = p;
    }

    public void SetGameType(int type)
    {
        switch (type)
        {
            case 0:
                CurrentGameType = GameClasses.GameType.TrainingGame;
                if (ratingObject == null)
                {
                    ratingObject = Instantiate(Rating, FindObjectOfType<SelectPlayers>().gameObject.transform);
                    ratingObject.onValueChanged.AddListener(SetRating);
                }
                break;

            case 1:
                CurrentGameType = GameClasses.GameType.TrainingGame;
                if (ratingObject != null)
                    Destroy(ratingObject.gameObject);
                break;
            case 2:
                CurrentGameType = GameClasses.GameType.AllInRatingGame;
                if (ratingObject != null)
                    Destroy(ratingObject.gameObject);
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
        BaseGameAccount player1 = null;
        BaseGameAccount player2 = null;
        try
        {
            player1 = DataBaseInitializer.singleton.userService.GetPlayerByUsername(p1);
            player2 = DataBaseInitializer.singleton.userService.GetPlayerByUsername(p2);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        if (player1 == null || player2 == null)
        {
            displayMessage = true;
            throw new ArgumentException("One or both players are missing");
        }
        WhiteCheckersPlayer = player1;
        BlackCheckersPlayer = player2;
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
            GUI.ModalWindow(1, new Rect(Screen.width / 2 - 100, Screen.height / 2 - 130, 250, 50), func, "One or both players are missing");
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
