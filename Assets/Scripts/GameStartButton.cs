using System;
using System.Collections;
using System.Collections.Generic;
using PRG.Network;
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
            this.GetComponent<Button>().onClick.AddListener(() =>
            {
                this.GetComponent<Button>().interactable = false;
            });
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
            this.GetComponent<Button>().onClick.AddListener(() =>
            {
                this.GetComponent<Button>().interactable = false;
            });
        }
    }
}