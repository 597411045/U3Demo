using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PRG.Cmd;
using PRG.Network;
using RPG.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class CmdManagement : TaskPipelineBase, ILocalCompute
    {
        public static CmdManagement Ins;
        private CommandExecuter ce;


        [SerializeField] public GameObject _canvas;
        [SerializeField] public InputField _inputField;
        [SerializeField] public GameObject _content;
        private string lastCmd;

        private void Awake()
        {
            if (Ins == null)
            {
                Debug.LogError(this.ToString() + " Awake");
                Ins = this;
            }
            else
            {
                Debug.LogError("For Now, Only One " + this.ToString() + " Allowed");
                Destroy(this);
            }

            ce = new CommandExecuter();
        }

        protected override void Start()
        {
            base.Start();
            _canvas.SetActive(false);
        }

        private bool lastIsFocused;

        public void LocalCompute()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                _canvas.SetActive(!_canvas.activeInHierarchy);
                _inputField.ActivateInputField();
            }

            if (lastIsFocused && Input.GetKeyDown(KeyCode.Return))
            {
                NetworkManagement.Ins.cmdTunnel.Enqueue(Encoding.UTF8.GetBytes(_inputField.text));
                lastCmd = _inputField.text;
                _inputField.text = "";
                _inputField.ActivateInputField();
            }

            if (lastIsFocused && Input.GetKeyDown(KeyCode.UpArrow))
            {
                _inputField.text = lastCmd;
                _inputField.ActivateInputField();
            }

            lastIsFocused = _inputField.isFocused;

            if (lastIsFocused && Input.GetKeyDown(KeyCode.Tab))
            {
                _inputField.text = CommandExecuter.Ins.GetHint(_inputField.text);
                _inputField.ActivateInputField();
            }
            
            if (lastIsFocused && Input.GetKeyDown(KeyCode.DownArrow))
            {
                _inputField.text = CommandExecuter.Ins.GetNextHint();
                _inputField.ActivateInputField();
            }
        }

        private int logCount;

        public void LogOnScreen(string log)
        {
            logCount++;
            GameObject go = Instantiate(Resources.Load<GameObject>("CMDText"), _content.transform);
            go.GetComponent<Text>().text = DateTime.Now.ToString("HH:mm:ss") + " : " + log;
            _content.GetComponent<RectTransform>().sizeDelta = new Vector2(20, logCount * 50);
        }
    }
}