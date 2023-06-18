using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        [SerializeField] public string Uid;
        [SerializeField] public string Text;
        [SerializeField] public Rect NodeRect;
        [NonSerialized] public Dictionary<string, DialogueNode> ChildrenNodes;
        [NonSerialized] public Dictionary<string, DialogueNode> ParentNodes;

        public DialogueNode()
        {
            ChildrenNodes = new Dictionary<string, DialogueNode>();
            ParentNodes = new Dictionary<string, DialogueNode>();
            NodeRect = new Rect(0, 0, 200, 200);
            Uid = System.Guid.NewGuid().ToString();
        }
    }
}