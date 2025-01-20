using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField] private Image JoyStickCenter;
    [SerializeField] private Image JoyStickButton;
    [NonSerialized] public AIController currentChaAic;
    public PositionConstraint pcs;

    private Vector3 _jsInitCenter;
    private Vector3 _jsCenter;
    private Vector3 _jsButton;
    private bool _isLeftButtonDown;
    private bool _isTouchDown;
    private int chaIndex = 0;

    private bool Flag = true;
    private Vector3 dir;

    //摇杆的最大偏移量
    public float joyStickBtnMaxDistance = 0.1f;

    private void Awake()
    {
        GameMode.Instance.AddActionToStartNoOrderList(M_Start);
        GameMode.Instance.AddActionToFixedUpdateOrderDic(99, M_FixedUpdate);
        Instance = this;
    }

    // Start is called before the first frame update
    void M_Start()
    {
        if (GameMode.Instance.entityManager.playerList.Count > 0)
        {
            SetModel(GameMode.Instance.entityManager.playerList[0]);
        }

        if (JoyStickCenter != null)
        {
            _jsInitCenter = JoyStickCenter.transform.position;
        }

        _isLeftButtonDown = false;
        _isTouchDown = false;
    }

    public void SetModel(AIController param)
    {
        if (param != null)
        {
            if (currentChaAic != null && currentChaAic.stateData)
            {
                Vector3 pos = currentChaAic.gameObject.transform.position;
                currentChaAic.gameObject.transform.position = new Vector3(-10, 10, -10);
                param.gameObject.transform.position = pos;
                param.gameObject.transform.rotation = currentChaAic.gameObject.transform.rotation;
            }

            pcs.SetSource(0, new ConstraintSource() { sourceTransform = param.gameObject.transform, weight = 1 });
            currentChaAic = param;
            this.transform.position = param.gameObject.transform.position + new Vector3(0, 10, -10);
        }
    }

    public void ChangeCha()
    {
        for (int i = 0; i < GameMode.Instance.entityManager.playerList.Count; i++)
        {
            chaIndex = (++chaIndex) % GameMode.Instance.entityManager.playerList.Count;
            if (GameMode.Instance.entityManager.playerList[chaIndex].characterData.currentHealth > 0 &&
                GameMode.Instance.entityManager.playerList[chaIndex] != currentChaAic)
            {
                SetModel(GameMode.Instance.entityManager.playerList[chaIndex]);
                Flag = true;
                return;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeCha();
        }

        dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            dir += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            dir -= Vector3.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            dir -= Vector3.right;
        }

        if (Input.GetKey(KeyCode.D))
        {
            dir += Vector3.right;
        }
    }

    private void M_FixedUpdate()
    {
        if (currentChaAic == null)
        {
            return;
        }

        if (currentChaAic.stateData.IfAnimDie() && Flag)
        {
            Flag = false;
            ChangeCha();
        }

        if (dir != Vector3.zero)
        {
            currentChaAic?.Move(dir);
        }
        else
        {
            currentChaAic?.StopMove();
            currentChaAic.stateData.AttackAble = true;
        }
    }

    public string ExportData()
    {
        StringBuilder data = new StringBuilder();
        data.Append("<PlayerData>");
        data.Append("<当前角色>" + currentChaAic.gameObject.name + "<当前角色>");
        data.Append("<PlayerData>");
        return data.ToString();
    }

    public void ImportData(string str)
    {
        var data = str.Split("<当前角色>");
        foreach (var iter in data)
        {
            if (iter != "")
            {
                chaIndex = -1;
                foreach (var iter2 in GameMode.Instance.entityManager.playerList)
                {
                    if (iter2.gameObject.name == iter)
                    {
                        chaIndex++;
                        SetModel(iter2);
                    }
                }
            }
        }
    }
}