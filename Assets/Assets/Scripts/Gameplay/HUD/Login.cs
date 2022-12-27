using GameClasses;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField usernameInput;
    [SerializeField]
    private TMP_Dropdown typeDropdown;

    // GUI support
    private bool displayMessage;
    private bool isLogged;
    private string message;
    private Timer timer;

    private string username;
    private GameAccount accountType;


    // Start is called before the first frame update
    void Start()
    {
        // Database initializing support
        DataBaseInitializer.singleton.userService.ReadToDB();

        displayMessage = false;
        isLogged = false;

        timer = gameObject.AddComponent<Timer>();
        timer.Duration = 1;

        usernameInput.onValueChanged.AddListener(SetUsername);
        typeDropdown.onValueChanged.AddListener(SetAccountType);

    }

    public void SetUsername(string input)
    {
        username = input;
    }

    public void SetAccountType(int type)
    {
        switch (type)
        {
            case 1:
                accountType = GameAccount.Bonus;
                break;
            case 2:
                accountType = GameAccount.Streak;
                break;
            default:
                accountType = GameAccount.Standart;
                break;
        }
    }

    public void HandleOnLoginClickEvent()
    {
        foreach (var user in DataBaseInitializer.singleton.userService.SelectAllUsers())
        {
            if (user.Username == username)
            {
                DataBaseInitializer.singleton.CurrentPlayer = user;
                SceneManager.LoadScene("MainMenuScene");
                return;
            }
        }

        string pattern = @"^[a-zA-Z][a-zA-Z0-9]{2,9}$";
        if (!Regex.IsMatch(username, pattern))
        {
            message = "Invalid username";
            displayMessage = true;
            return;
        }

        BaseGameAccount newPlayer;
        switch (accountType)
        {
            case GameAccount.Standart:
                newPlayer = new BaseGameAccount(username);
                break;
            case GameAccount.Bonus:
                newPlayer = new BonusGameAccount(username);
                break;
            case GameAccount.Streak:
                newPlayer = new StreakGameAccount(username);
                break;
            default:
                newPlayer = new BaseGameAccount(username);
                break;
        }
        DataBaseInitializer.singleton.userService.CreateUser(newPlayer);
        DataBaseInitializer.singleton.CurrentPlayer = newPlayer;

        SceneManager.LoadScene("MainMenuScene");
    }

    private void func(int id)
    {
        if (!isLogged)
        {
            print(message);
        }
    }

    private void OnGUI()
    {
        if (displayMessage)
        {
            GUI.ModalWindow(1, new Rect(Screen.width / 2 - 100, Screen.height / 2 - 130, 250, 50), func, message);
            isLogged = true;
            if (timer.Finished)
            {
                displayMessage = false;
                isLogged = false;
                timer.Stop();
            }
            else
            {
                timer.Run();
            }
        }
    }
}
