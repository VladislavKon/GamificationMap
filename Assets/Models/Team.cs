using Assets.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Team
{
    public string id;
    public string teamName;
    public List<User> users;
    public int points;
    public Color colorIndex;
}

[Serializable]
public class Teams
{
    public List<Team> teams;
}
