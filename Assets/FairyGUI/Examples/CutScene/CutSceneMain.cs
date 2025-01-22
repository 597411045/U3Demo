using System.Collections;
using UnityEngine;
using FairyGUI;

/// <summary>
/// Demonstrated the simple flow of a game.
/// </summary>
public class CutSceneMain : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
        Stage.inst.onKeyDown.Add(OnKeyDown);

        GameMode.uIManager.LoadUIResourceByBundle("cutscene");

        LevelManager.inst.Init();
        LevelManager.inst.LoadLevel("scene1");
    }

    void OnKeyDown(EventContext context)
    {
        if (context.inputEvent.keyCode == KeyCode.Escape)
        {
            Application.Quit();
        }
    }
}