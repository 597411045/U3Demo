using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameMode : MonoBehaviour
{
    public List<Action> OnEnemyClearActions = new List<Action>();

    public void EnterLevel()
    {
        if (SceneManager.GetActiveScene().name.Contains("0"))
        {
        }
        else
        {
        }

        saveManager.ImportData();
    }

    public void OnEnemyClear()
    {
        foreach (var iter in OnEnemyClearActions)
        {
            iter.Invoke();
        }

    }

    public void ChangeScene(string sceneName)
    {
        saveManager.SaveData();
        SceneManager.LoadScene(sceneName);
    }
}