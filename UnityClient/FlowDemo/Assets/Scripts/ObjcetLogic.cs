using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using BestHTTP;
using Newtonsoft.Json;

public class ObjcetLogic : MonoBehaviour{

    readonly List<ObjectData> _dataList = new List<ObjectData>();

    public string Uuid { get; private set; }

    public bool isFinished { get; private set; }
    public void Init()
    {
       StartCoroutine(Logic());
    }

    public void StopCollect()
    {
       this.StopAllCoroutines();
    }
    IEnumerator Logic()
    {
        Uuid = Guid.NewGuid().ToString();
        _dataList?.Clear();
        while (true)
        {
            _dataList?.Add(new ObjectData()
            {
                scopeId = Uuid,
                timeStamp = DateTime.Now,
                x= this.gameObject.transform.position.x,
                y=this.gameObject.transform.position.y
            });
            yield return null;
        }
    }

    public void SendDataToServer(MotionType type)
    {
        isFinished = false;
        _dataList.ForEach(p=>p.type=type.ToString());
        var o = JsonConvert.SerializeObject(_dataList);
       // Debug.Log(o);
        var request = new HTTPRequest(new Uri(SystemLogic.Instance.Config.ServerPath+"uploadData"), HTTPMethods.Post, OnRequestFinished)
        {
            RawData = Encoding.UTF8.GetBytes(o)
        };
        if (!request.HasHeader("Content-Type"))
        {
            request.SetHeader("Content-Type", "application/json");
        }
        request.Send();
       // _dataList.Clear();
    }

    public void DataDispose()
    {
        _dataList.Clear();
    }
    
    void OnRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        isFinished = true;
        Debug.Log(response.DataAsText);
    }

}
