using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ObjectData
{
    public string Type { get; set; }
    public double X{ get; set; }
    public double Y { get; set; }
    public DateTime TimeStamp { get; set; }

    public string ScopeId { get; set; }

}


