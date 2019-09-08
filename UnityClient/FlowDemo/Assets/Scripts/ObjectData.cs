using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ObjectData
{
    public string type { get; set; }
    public double x{ get; set; }
    public double y { get; set; }
    public DateTime timeStamp { get; set; }

    public string scopeId { get; set; }

}


