using System.Collections;
using System.Collections.Generic;
using RPG.Dialogue;
using UnityEngine;

public class DialogTest : MonoBehaviour
{
    public DialogueSO dso;
    public TaskSO tso;

    void Start()
    {
        foreach (var so in tso.GetTasks())
        {
            Debug.Log(so);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}