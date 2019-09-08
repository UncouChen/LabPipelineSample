using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig
{
    public string ServerPath { get; set; }

    public GameConfig()
    {
        ServerPath = "http://127.0.0.1:5000/";
    }
}
