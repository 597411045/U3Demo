using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "TaskSO", menuName = "Dialogue/New Task", order = 0)]
    public class TaskSO : ScriptableObject
    {
        public IEnumerable<string> GetTasks()
        {
            yield return "Task 1";
            yield return "Task 2";
            yield return "Task 3";
        }
    }
}