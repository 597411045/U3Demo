using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using RPG.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartButton : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        if (this.gameObject.name == "StartGame")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().StartAsClient);
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
        }

        if (this.gameObject.name == "StopValidCenter")
        {
        }

        if (this.gameObject.name == "StartManagerCenter")
        {
        }

        if (this.gameObject.name == "StopManagerCenter")
        {
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
        }

        if (this.gameObject.name == "ClientSendSome")
        {
        }

        if (this.gameObject.name == "StartAsServer")
        {
            this.GetComponent<Button>().onClick.AddListener(GameObject.FindObjectOfType<NetworkCenter>().StartAsServer);
        }
    }

}