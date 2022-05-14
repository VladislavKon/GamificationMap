using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using Assets.Models;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
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
            Teams = value;
            if (CurrentPlayerName != null) 
            {
                GameObject.Find("Points").GetComponent<Text>().text = GetPlayerInfo().Points.ToString();
            }
        }
    }

    public void Start()
    {
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
    }

    public Team GetPlayerTeam()
    {
        return Teams.Where(t => t.Users.Any(u => u.UserName == CurrentPlayerName)).SingleOrDefault();
    }

    public User GetPlayerInfo()
    {
        return GetPlayerTeam().Users.Where(u => u.UserName == CurrentPlayerName).SingleOrDefault();
    }

    [DllImport("__Internal")]
    private static extern void SubtractPoints(User player);
    public void SubtractPoints(int points)
    {
        var player = GetPlayerInfo();
        player.Points =- points;
#if UNITY_WEBGL == true && UNITY_EDITOR == false
				SubtractPoints(player);
#endif
    }
}
