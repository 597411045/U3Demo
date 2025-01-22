using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameMode : MonoBehaviour
{
    public static UIManager uIManager
    {
        get { return UIManager.Instance; }
    }

    public static EntityManager entityManager
    {
        get { return EntityManager.Instance; }
    }
}