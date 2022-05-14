using Assets.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team
{
    public Guid Id;
    public string TeamName;
    public List<User> Users;
    public int Points;
}
