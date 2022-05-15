using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using Assets.Models;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Canvas editUI;
    /// <summary>
    /// Ограничение кадров
    /// </summary>
    public int targetFrameRate = 30;

    /// <summary>
    /// Имя текущего игрока
    /// </summary>
    string CurrentPlayerName
    {
        get
        {
            return CurrentPlayerName;
        }
        set
        {
            CurrentPlayerName = value;
            GameObject.Find("PlayerName").GetComponent<Text>().text = CurrentPlayerName;
        }
    }

    User CurrentPlayer
    {
        get 
        { 
            return CurrentPlayer; 
        } 
        set 
        {
            if (value.role == "Admin")
            {
                editUI.gameObject.SetActive(true);
            }            
            CurrentPlayer = value; 
        } 
    }

    /// <summary>
    /// Участвующие в игре команды
    /// </summary>
    List<Team> Teams
    { 
        get 
        { 
            return Teams; 
        }
        set
        {
            CurrentPlayer = value.Where(t => t.Users.Any(u => u.userName == CurrentPlayerName))
                .SingleOrDefault()
                .Users
                .Where(u => u.userName == CurrentPlayerName)
                .SingleOrDefault();
            Teams = value;
        }
    }

    public void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;

        GetCurrentPlayerName();
        GetCurrentTeams();        
    }

    [DllImport("__Internal")]
    private static extern void GameOver(string userName, int score);
    public void SomeMethod()
    {
    #if UNITY_WEBGL == true && UNITY_EDITOR == false
    GameOver ("Player1", 100);
    #endif
    }
    public void SpawnEnemies(int amount)
    {
        Debug.Log($"Spawning {amount} enemies!");
    }

    [DllImport("__Internal")]
    private static extern void GetAllTeams();

    [DllImport("__Internal")]
    private static extern void GetCurrentUser();

    public void GetCurrentPlayerName()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
				GetCurrentUser();
#endif
    }

    public void GetCurrentTeams()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
				GetAllTeams();
#endif
    }

    public void SetTeams(List<Team> teams)
    {
        Teams = teams;
    }
    public void SetPlayer(string UserName)
    {
        CurrentPlayerName = UserName;
        GetPlayerTeam().Users.Where(u => u.userName == UserName).SingleOrDefault();
    }

    public Team GetPlayerTeam()
    {
        return Teams.Where(t => t.Users.Any(u => u.userName == CurrentPlayerName)).SingleOrDefault();
    }

    public User GetPlayerInfo()
    {
        return CurrentPlayer;
    }

    [DllImport("__Internal")]
    private static extern void SubtractPoints(User currentPlayerState, User newPlayerState);
    public void SubtractPoints(int points)
    {
        User newPlayerState = (User)CurrentPlayer.Clone();
        newPlayerState.points =- points;
#if UNITY_WEBGL == true && UNITY_EDITOR == false
				SubtractPoints(CurrentPlayer, newPlayerState);
#endif
    }    
    public void UpdatePlayerState(User newPlayerState)
    {
        CurrentPlayer = newPlayerState;
    }

}
