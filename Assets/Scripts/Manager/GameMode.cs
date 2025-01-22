using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameMode : MonoBehaviour
{
    public static GameMode Instance;
    public int RenderFrame;
    public int FixedFrame;
    private List<Action> AwakeNoOrderList = new List<Action>();
    private List<Action> StartNoOrderList = new List<Action>();
    private List<Action<float>> UpdateNoOrderList = new List<Action<float>>();
    private List<Action> FixedUpdateNoOrderList = new List<Action>();

    private List<Action> AwakeNoOrderListBuffer = new List<Action>();
    private List<Action> StartNoOrderListBuffer = new List<Action>();
    private List<Action<float>> UpdateNoOrderListAddBuffer = new List<Action<float>>();
    private List<Action> FixedUpdateNoOrderListAddBuffer = new List<Action>();
    private List<Action<float>> UpdateNoOrderListRemoveBuffer = new List<Action<float>>();
    private List<Action> FixedUpdateNoOrderListRemoveBuffer = new List<Action>();

    private Dictionary<int, Action> AwakeOrderDic = new Dictionary<int, Action>();
    private Dictionary<int, Action> StartOrderDic = new Dictionary<int, Action>();
    private Dictionary<int, Action> UpdateOrderDic = new Dictionary<int, Action>();
    private Dictionary<int, List<Action>> FixedUpdateOrderDic = new Dictionary<int, List<Action>>();

    public void AddActionToAwakeOrderDic(int i, Action action)
    {
        AwakeOrderDic.Add(i, action);
    }

    public void AddActionToStartOrderDic(int i, Action action)
    {
        StartOrderDic.Add(i, action);
    }

    public void AddActionToUpdateOrderDic(int i, Action action)
    {
        UpdateOrderDic.Add(i, action);
    }

    public void AddActionToFixedUpdateOrderDic(int i, Action action)
    {
        if (FixedUpdateOrderDic.ContainsKey(i) == false)
        {
            FixedUpdateOrderDic.Add(i, new List<Action>());
        }

        FixedUpdateOrderDic[i].Add(action);
    }

    public void AddActionToAwakeNoOrderList(Action action)
    {
        AwakeNoOrderList.Add(action);
    }

    public void AddActionToStartNoOrderList(Action action)
    {
        StartNoOrderList.Add(action);
    }

    public void AddActionToUpdateNoOrderListBuffer(Action<float> action)
    {
        UpdateNoOrderListAddBuffer.Add(action);
    }

    public void AddActionToFixedUpdateNoOrderListBuffer(Action action)
    {
        FixedUpdateNoOrderListAddBuffer.Add(action);
    }

    public void RemoveActionToUpdateNoOrderListBuffer(Action<float> action)
    {
        UpdateNoOrderListRemoveBuffer.Add(action);
    }

    public void RemoveActionToFixedUpdateNoOrderListBuffer(Action action)
    {
        FixedUpdateNoOrderListRemoveBuffer.Add(action);
    }

    void Awake()
    {
        Instance = this;
        entityManager.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        var keys = AwakeOrderDic.Keys;
        var orderKeys = keys.OrderBy(x => x);
        foreach (var iter in orderKeys)
        {
            AwakeOrderDic[iter].Invoke();
        }

        foreach (var iter in AwakeNoOrderList)
        {
            iter.Invoke();
        }

        keys = StartOrderDic.Keys;
        orderKeys = keys.OrderBy(x => x);
        foreach (var iter in orderKeys)
        {
            StartOrderDic[iter].Invoke();
        }

        foreach (var iter in StartNoOrderList)
        {
            iter.Invoke();
        }

        EnterLevel();
    }

    // Update is called once per frame
    void Update()
    {
        RenderFrame++;
        var keys = UpdateOrderDic.Keys;
        var orderKeys = keys.OrderBy(x => x);
        foreach (var iter in orderKeys)
        {
            UpdateOrderDic[iter].Invoke();
        }

        foreach (var iter in UpdateNoOrderList)
        {
            iter.Invoke(Time.deltaTime);
        }

        foreach (var iter in UpdateNoOrderListAddBuffer)
        {
            iter.Invoke(Time.deltaTime);
            UpdateNoOrderList.Add(iter);
        }

        UpdateNoOrderListAddBuffer.Clear();

        foreach (var iter in UpdateNoOrderListRemoveBuffer)
        {
            UpdateNoOrderList.Remove(iter);
        }

        UpdateNoOrderListRemoveBuffer.Clear();
    }

    private void FixedUpdate()
    {
        FixedFrame++;
        var keys = FixedUpdateOrderDic.Keys;
        var orderKeys = keys.OrderBy(x => x);
        foreach (var iter in orderKeys)
        {
            foreach (var iter2 in FixedUpdateOrderDic[iter])
            {
                iter2.Invoke();
            }
        }

        foreach (var iter in FixedUpdateNoOrderList)
        {
            iter.Invoke();
        }

        foreach (var iter in FixedUpdateNoOrderListAddBuffer)
        {
            iter.Invoke();
            FixedUpdateNoOrderList.Add(iter);
        }

        FixedUpdateNoOrderListAddBuffer.Clear();

        foreach (var iter in FixedUpdateNoOrderListRemoveBuffer)
        {
            FixedUpdateNoOrderList.Remove(iter);
        }

        FixedUpdateNoOrderListRemoveBuffer.Clear();
    }
}