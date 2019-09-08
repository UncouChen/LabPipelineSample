using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;
using Newtonsoft.Json;
using BestHTTP;

public class HttpTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TestServerConnect()
    {
        new HTTPRequest(new Uri("http://127.0.0.1:5000/"), (request, response) => Debug.Log(response.DataAsText)).Send();
    }

    public void SendDataBtn()
    {
        List<ObjectData> objectDatas = new List<ObjectData>();
        for (int i = 0; i < 500; i++)
        {
            objectDatas.Add(new ObjectData()
            {
                ScopeId = "test01",
                TimeStamp = DateTime.Now,
                Type = "Test",
                X = Random.Range(-1, 2),
                Y = Random.Range(-1, 2),
            });
        }

        var o = JsonConvert.SerializeObject(objectDatas);
        var h = new HTTPRequest(new Uri("http://127.0.0.1:5000/uploadData"), HTTPMethods.Post,OnRequestFinished)
        {
            RawData = Encoding.UTF8.GetBytes(o)
        };
        if (!h.HasHeader("Content-Type"))
        {
            h.SetHeader("Content-Type","application/json");
        }

        h.Send();
    }

    void OnRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        Debug.Log(response.DataAsText);
    }
}
