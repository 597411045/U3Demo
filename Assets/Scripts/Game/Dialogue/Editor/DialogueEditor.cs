using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueEditor : EditorWindow
    {
        static DialogueSO selectedSO;

        [NonSerialized] private GUIStyle nodeStyle;
        [NonSerialized] private DialogueNode dragedDN;
        [NonSerialized] private Vector2 deltaVector;
        [NonSerialized] private Action actionForNode;
        [NonSerialized] private DialogueNode firstParam;

        [MenuItem("Window/Dialogue/Dialogue Window")]
        public static void ShowDialogueWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Window");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int id, int line)
        {
            selectedSO = EditorUtility.InstanceIDToObject(id) as DialogueSO;
            if (selectedSO == null) return false;
            GetWindow(typeof(DialogueEditor), false, "Dialogue Window");
            return true;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += () =>
            {
                selectedSO = Selection.activeObject as DialogueSO;
                Repaint();
            };
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            //nodeStyle.normal.background = Texture2D.whiteTexture;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(10, 10, 10, 10);
        }

        private void OnGUI()
        {
            //EditorGUIL.LabelField(new Rect(10,10,100,100),"Test");


            //执行一次的委托
            if (actionForNode != null)
            {
                actionForNode.Invoke();
                actionForNode = null;
            }

            if (selectedSO == null) return;

            //拖动
            if (Event.current.type == EventType.MouseDown && dragedDN == null)
            {
                foreach (var eachNode in selectedSO.GetEnumerable())
                {
                    if (eachNode.NodeRect.Contains(Event.current.mousePosition))
                    {
                        dragedDN = eachNode;
                        deltaVector = dragedDN.NodeRect.position - Event.current.mousePosition;
                    }
                }
            }

            if (Event.current.type == EventType.MouseUp && dragedDN != null)
            {
                dragedDN = null;
            }

            if (Event.current.type == EventType.MouseDrag && dragedDN != null)
            {
                Undo.RecordObject(selectedSO, "Drag Dialogue Node");
                dragedDN.NodeRect.position = Event.current.mousePosition + deltaVector;
                GUI.changed = true;
            }


            //画曲线
            foreach (var eachNode in selectedSO.GetEnumerable())
            {
                Vector2 start = new Vector2(eachNode.NodeRect.xMax, eachNode.NodeRect.center.y);
                foreach (var childNode in eachNode.ChildrenNodes)
                {
                    Vector2 end = new Vector2(childNode.Value.NodeRect.xMin, childNode.Value.NodeRect.center.y);
                    Vector2 offset = end - start;
                    offset.y = 0;
                    offset.x *= 0.8f;
                    Handles.DrawBezier(start,
                        end,
                        start + offset, end - offset, Color.white, null, 5);
                }
            }

            //主要内容
            foreach (var eachNode in selectedSO.GetEnumerable())
            {
                GUILayout.BeginArea(eachNode.NodeRect, nodeStyle);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Node:");
                string tmp2 = EditorGUILayout.TextField(eachNode.Text);
                //if (c.text != cloneText)
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(selectedSO, "Update Dialogue Text");
                    eachNode.Text = tmp2;
                    EditorUtility.SetDirty(selectedSO);
                }

                //显示child内容
                foreach (var childNode in eachNode.ChildrenNodes)
                {
                    EditorGUILayout.LabelField(childNode.Value.Text);
                }

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("-"))
                {
                    actionForNode += () =>
                    {
                        Undo.RecordObject(selectedSO, "Delete New Node");
                        selectedSO.DeleteNode(eachNode);
                    };
                }

                if (GUILayout.Button("+"))
                {
                    actionForNode += () =>
                    {
                        Undo.RecordObject(selectedSO, "Add New Node");
                        selectedSO.NewNode(eachNode);
                    };
                }

                if (firstParam == null)
                {
                    if (GUILayout.Button("Link"))
                    {
                        firstParam = eachNode;
                    }
                }
                else
                {
                    if (firstParam.Uid == eachNode.Uid)
                    {
                        if (GUILayout.Button("UnLink"))
                        {
                            firstParam = null;
                        }
                    }
                    else
                    {
                        if (firstParam.ChildrenNodes.ContainsKey(eachNode.Uid))
                        {
                            GUILayout.Label("Already Child");
                        }
                        else
                        {
                            if (GUILayout.Button("Child"))
                            {
                                actionForNode += () =>
                                {
                                    firstParam.ChildrenNodes.Add(eachNode.Uid, eachNode);
                                    eachNode.ParentNodes.Add(firstParam.Uid, firstParam);
                                    firstParam = null;
                                };
                            }
                        }
                    }
                }


                GUILayout.EndHorizontal();


                GUILayout.EndArea();
            }
        }
    }
}