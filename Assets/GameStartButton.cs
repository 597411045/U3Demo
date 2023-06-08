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
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().StartAsClient);
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<SavingWrapper>().StartGame);
        }

        if (this.gameObject.name == "StartAcceptCenter")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().AcceptStart);
        }

        if (this.gameObject.name == "StopAcceptCenter")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().AcceptStop);
        }

        if (this.gameObject.name == "StartValidCenter")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().ValidStart);
        }

        if (this.gameObject.name == "StopValidCenter")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().ValidStop);
        }

        if (this.gameObject.name == "StartManagerCenter")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().ManagerStart);
        }

        if (this.gameObject.name == "StopManagerCenter")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().ManagerStop);
        }
        if (this.gameObject.name == "StartClient")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().ConnectStart);
        }
        if (this.gameObject.name == "StartCommCenter")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().CommStart);
        }
        if (this.gameObject.name == "StopCommCenter")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().CommStop);
        }
        if (this.gameObject.name == "ClientSendValid")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().ClientSendValid);
        }
        if (this.gameObject.name == "ClientSendSome")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().ClientSendSome);
        }
        if (this.gameObject.name == "StartAsServer")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().StartAsServer);
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<SavingWrapper>().StartGame);

        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}