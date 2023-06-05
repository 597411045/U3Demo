using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using RPG.Saving;
using UnityEngine;
using UnityEngine.UI;

public class GameStartButton : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        if (this.gameObject.name == "StartGame")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<SavingWrapper>().StartGame);
        }
        if (this.gameObject.name == "ServerAccept")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<SocketTest>().AcceptStart);
        }
        if (this.gameObject.name == "ServerValid")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<SocketTest>().ValidStart);
        }
        if (this.gameObject.name == "ClientConnect")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<SocketTest>().ClientConnect);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}