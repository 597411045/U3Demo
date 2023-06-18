using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueSO", menuName = "Dialogue/New Diaglogue", order = 0)]
    public class DialogueSO : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> nodeList = new List<DialogueNode>();
        [NonSerialized] public Dictionary<string, DialogueNode> nodeDic = new Dictionary<string, DialogueNode>();


        public bool NewNode(DialogueNode parent = null)
        {
            DialogueNode tmp = new DialogueNode();
            InsertNode(tmp);

            if (parent != null)
            {
                parent.ChildrenNodes.Add(tmp.Uid, tmp);
                tmp.ParentNodes.Add(parent.Uid, parent);
            }

            return true;
        }

        public bool InsertNode(DialogueNode dn)
        {
            if (nodeDic.ContainsKey(dn.Uid)) return false;
            nodeDic.Add(dn.Uid, dn);
            nodeList.Add(dn);
            return true;
        }

        public bool DeleteNode(DialogueNode dn)
        {
            //删除父亲指向自己的索引
            foreach (var c in dn.ParentNodes)
            {
                c.Value.ChildrenNodes.Remove(dn.Uid);
            }

            //删除孩子指向自己的索引
            foreach (var c in dn.ChildrenNodes)
            {
                c.Value.ParentNodes.Remove(dn.Uid);
            }

            //删除自己
            int i = 0;
            nodeDic.Remove(dn.Uid);

            foreach (var c in nodeList)
            {
                if (c.Uid == dn.Uid)
                {
                    nodeList.RemoveAt(i);
                    return true;
                }

                i++;
            }

            return false;
        }

        public IEnumerable<DialogueNode> GetEnumerable()
        {
            return nodeList;
        }


#if UNITY_EDITOR
        private void OnEnable()
        {
            if (nodeList.Count == 0)
            {
                DialogueNode root = new DialogueNode();
                InsertNode(root);
            }
        }

        private void Awake()
        {
        }
#endif
    }
}