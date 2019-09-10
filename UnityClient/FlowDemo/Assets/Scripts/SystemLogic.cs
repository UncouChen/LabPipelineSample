using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BestHTTP;
using LabData;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;



public enum MotionType
{
    Circle,
    Random
}
public class SystemLogic : MonoSingleton<SystemLogic>
{

    public bool IsConnectToServer = false;
    //UI界面元素
    public Button MarkBtn, ReBuildBtn, TestBtn, CircleBtn, RandomBtn, QuitBtn,GoOnBtn;
    public GameObject  MarkPlane, TestPlane, CircleObject,TestBtnPlane;
   

    public Text ResultText;
    //小球实体
    private GameObject _currentObject;
    //初始位置
    private Vector3 StartPos;
    //是否点击选择类型
    private bool _isChooseType = false,_isGoOn=false;

    private string _scopeId;

    public MotionType Type { get; private set; }
    public GameConfig Config { get; private set; }



  
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr handle, String message, String title, int type);
    void Init()
    {
     
        _currentObject = Instantiate(CircleObject, TestPlane.transform);
        StartPos = _currentObject.transform.position - new Vector3(0, 80, 0);
        Config = LabTools.GetConfig<GameConfig>();
        if (IsConnectToServer)
        {
            new HTTPRequest(new Uri(Config.ServerPath), (request, response) =>
            {
                MessageBox(IntPtr.Zero, response.DataAsText, "Connect", 0);
                Debug.Log(response.DataAsText);
            }).Send();
        }
        #region 按钮事件绑定

        //StartBtn.onClick.AddListener(
        //    (() =>
        //    {
        //        if (ScopeIdInputField.text != "")
        //        {
        //            _scopeId = ScopeIdInputField.text;
        //            InputPlane.SetActive(false);
        //        }
        //        else
        //        {
        //            MessageBox(IntPtr.Zero, "ScopeID is Null!!!", "Error", 0);
        //        }

        //    }));

        MarkBtn.onClick.AddListener(
            () =>
            {
                TestBtnPlane.SetActive(false);
                StopAllCoroutines();
                _currentObject.SetActive(true);
                StartCoroutine(MarkObjectLogic());
                MarkPlane.SetActive(true);
            });

        ReBuildBtn.onClick.AddListener(() =>
        {
            StopAllCoroutines();
            TestBtnPlane.SetActive(false);
            _currentObject.SetActive(false);
            MarkPlane.SetActive(false);
        });

        TestBtn.onClick.AddListener(() =>
        {
            StopAllCoroutines();
            MarkPlane.SetActive(false);
            TestBtnPlane.SetActive(true);
            _currentObject.SetActive(true);
            StartCoroutine(TestObjectLogic());

        });

        //ReturnBtn.onClick.AddListener((() =>
        //{
        //    StopAllCoroutines();
        //    CircleBtn.interactable = false;
        //    RandomBtn.interactable = false;
        //    InputPlane.SetActive(true);

        //}));

        QuitBtn.onClick.AddListener(() =>
        {
            StopAllCoroutines();
            Application.Quit();
        });
        #endregion
    }


    /// <summary>
    /// 圆周和随机运动的逻辑
    /// </summary>
    /// <param name="type"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    IEnumerator ObjectLogic(MotionType type, Vector3 target)
    {
        float time = 0;
        Type = type;
       // Debug.Log(StartPos);
        _currentObject.transform.position = StartPos;
        switch (type)
        {
            case MotionType.Circle:
                Type = type;
                while (time <= 3)
                {
                    _currentObject.transform.RotateAround(target, Vector3.forward, 5f);
                    time += Time.deltaTime;
                    yield return null;
                }
                break;
            case MotionType.Random:
                while (time <= 3)
                {
                    //  _currentObject.transform.LookAt(TriangelList[0]);
                    _currentObject.transform.RotateAround(target + new Vector3(Random.Range(-100, 100), Random.Range(-25, 25)), Vector3.back, 10f);
                    time += Time.deltaTime;
                    yield return null;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        time = 0;
    }
    /// <summary>
    /// 标记的逻辑
    /// </summary>
    /// <returns></returns>
    IEnumerator MarkObjectLogic()
    {

        var postpartum = new Vector3(StartPos.x, StartPos.y + 160);

        CircleBtn.onClick.AddListener(() =>
        {
            Type = MotionType.Circle;
            _isChooseType = true;
        });
        RandomBtn.onClick.AddListener(() =>
        {
            Type = MotionType.Random;
            _isChooseType = true;
        });
        while (true)
        {
            CircleBtn.interactable = false;
            RandomBtn.interactable = false;
            _currentObject.GetComponent<ObjcetLogic>().Init();
            var r = Random.Range(1, 10);
            if (r > 5)
            {
                Type = MotionType.Circle;
                var ie = ObjectLogic(Type, postpartum);
                yield return StartCoroutine(ie);
                _currentObject.GetComponent<ObjcetLogic>().StopCollect();
                CircleBtn.interactable = true;
                RandomBtn.interactable = true;
                yield return new WaitUntil(() => _isChooseType == true);
            }
            else
            {
                Type = MotionType.Random;
                var ie = ObjectLogic(Type, postpartum);
                yield return StartCoroutine(ie);
                _currentObject.GetComponent<ObjcetLogic>().StopCollect();
                CircleBtn.interactable = true;
                RandomBtn.interactable = true;
                yield return new WaitUntil(() => _isChooseType == true);
            }
            _isChooseType = false;
            if (IsConnectToServer)
            {
                _currentObject.GetComponent<ObjcetLogic>().SendDataToServer(Type);
            }
            yield return null;
        }

    }
    /// <summary>
    /// 测试的逻辑
    /// </summary>
    /// <returns></returns>
    IEnumerator TestObjectLogic()
    {
        var postpartum = new Vector3(StartPos.x, StartPos.y + 160);
        GoOnBtn.onClick.AddListener(() => { _isGoOn = true; });
        while (true)
        {
            _isGoOn = false;
            GoOnBtn.interactable = false;
            _currentObject.GetComponent<ObjcetLogic>().Init();
            var r = Random.Range(1, 10);
            if (r > 5)
            {
                Type = MotionType.Circle;
                var ie = ObjectLogic(Type, postpartum);
                yield return StartCoroutine(ie);
                _currentObject.GetComponent<ObjcetLogic>().StopCollect();
            }
            else
            {
                Type = MotionType.Random;
                var ie = ObjectLogic(Type, postpartum);
                yield return StartCoroutine(ie);
                _currentObject.GetComponent<ObjcetLogic>().StopCollect();
            }
            _isChooseType = false;

            if (IsConnectToServer)
            {
                _currentObject.GetComponent<ObjcetLogic>().SendDataToServer(Type);
                yield return new WaitUntil(() =>
                {
                    ResultText.text = "等待结果中。。。。。。。";
                    return _currentObject.GetComponent<ObjcetLogic>().isFinished==true;
                });
                yield return new WaitForSeconds(5f);
                yield return new HTTPRequest(new Uri(Config.ServerPath + "predictResult/" + _currentObject.GetComponent<ObjcetLogic>().Uuid ), (request, response) =>
                {
                    Debug.Log(Config.ServerPath + "predictResult/" + _currentObject.GetComponent<ObjcetLogic>().Uuid);
                    ResultText.text = response.DataAsText;
                    Debug.Log(response.DataAsText);
                }).Send();
            }
           
            GoOnBtn.interactable = true;
            yield return new WaitUntil((() => _isGoOn));
            yield return null;
        }

    }
}
