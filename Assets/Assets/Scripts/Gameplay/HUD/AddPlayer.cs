using GameClasses;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddPlayer : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_Dropdown accountDropdown;
    public string Username;
    public GameAccount accountType;
    private bool displayMessage;
    private bool isLogged;
    private string message;
    Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        message = "";
        isLogged = false;
        displayMessage = false;
        timer = gameObject.AddComponent<Timer>();
        timer.Duration = 1;
        usernameInputField.onValueChanged.AddListener(SetUsername);
        accountDropdown.onValueChanged.AddListener(SetAccountType);
    }

    public void SetUsername(string username)
    {
        Username = username;
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

    public void HandleOnAddUserButton()
    {
        if (DataBaseInitializer.singleton.userService.SelectAllUsers().Find(x => x.Username == Username) != null)
        {
            message = "This username is already taken";
            displayMessage = true;
            return;
        }

        string pattern = @"^[a-zA-Z][a-zA-Z0-9]{2,9}$";
        if (!Regex.IsMatch(Username, pattern))
        {
            message = "Invalid username";
            displayMessage = true;
            return;
        }

        BaseGameAccount newPlayer;
        switch (accountType)
        {
            case GameAccount.Standart:
                newPlayer = new BaseGameAccount(Username);
                break;
            case GameAccount.Bonus:
                newPlayer = new BonusGameAccount(Username);
                break;
            case GameAccount.Streak:
                newPlayer = new StreakGameAccount(Username);
                break;
            default:
                newPlayer = new BaseGameAccount(Username);
                break;
        }
        message = "Player has been successfully added";
        displayMessage = true;
        DataBaseInitializer.singleton.userService.CreateUser(newPlayer);
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

    public void HandleOnReturnButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
