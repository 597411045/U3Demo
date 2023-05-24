using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class JsonSaveableEntity : MonoBehaviour
    {
        [SerializeField] private string uniqueIdentifier = "";

        private static Dictionary<string, JsonSaveableEntity> globalLookup =
            new Dictionary<string, JsonSaveableEntity>();

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            foreach (var child in GetComponents<IJsonSaveable>())
            {
                JToken token = child.CaptureASJToken();
                string component = child.GetType().ToString();
                Debug.Log($"{name} Capture {component} ={token.ToString()}");
                stateDict[child.GetType().ToString()] = token;
            }

            return state;
        }

        public void RestoreFromJToken(JToken s)
        {
            JObject state = s.ToObject<JObject>();
            IDictionary<string, JToken> stateDict = state;
            foreach (var child in GetComponents<IJsonSaveable>())
            {
                string component = child.GetType().ToString();
                if (stateDict.ContainsKey(component))
                {
                    Debug.Log($"{name} Restore {component} =>{stateDict[component].ToString()}");
                    child.RestoreFormJTkoen(stateDict[component]);
                }
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            SerializedObject so = new SerializedObject(this);
            SerializedProperty sp = so.FindProperty("uniqueIdentifier");

            if (string.IsNullOrEmpty(sp.stringValue) || !IsUnique(sp.stringValue))
            {
                sp.stringValue = System.Guid.NewGuid().ToString();
                so.ApplyModifiedProperties();
            }

            globalLookup[sp.stringValue] = this;
        }
#endif

        private bool IsUnique(string spStringValue)
        {
            if (!globalLookup.ContainsKey(spStringValue)) return true;

            if (globalLookup[spStringValue] == this) return true;

            if (globalLookup[spStringValue] == null)
            {
                globalLookup.Remove(spStringValue);
                return true;
            }

            if (globalLookup[spStringValue].GetUniqueIdentifier() != spStringValue)
            {
                globalLookup.Remove(spStringValue);
                return true;
            }

            return false;
        }
    }
}