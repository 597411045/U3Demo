using System;
using TMPro;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    public bool TimeTest;
    public bool inputTest;

    public TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeTest)
        {
            text.text = DateTime.Now.ToString();
            return;
        }

        if (inputTest)
        {
            text.text = "";
            var tmp = Input.touches;
            foreach (var iter in tmp)
            {
                text.text += iter.fingerId.ToString() + "\r\n";
                text.text += iter.position.ToString() + "\r\n";
                text.text += iter.deltaPosition.ToString() + "\r\n";
                text.text += iter.deltaTime.ToString() + "\r\n";
                text.text += iter.tapCount.ToString() + "\r\n";
                text.text += iter.phase.ToString() + "\r\n";
                text.text += "\r\n";
            }
        }
    }
}