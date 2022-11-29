using GameClasses;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GetPlayerData : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_Dropdown accountDropdown;
    public string Username;
    public GameAccount accountType;

    // Start is called before the first frame update
    void Start()
    {
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
        DataBaseInitializer.singleton.userService.CreateUser(newPlayer);
    }

    public void HandleOnReturnButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
