using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class JsonSavingSystem : MonoBehaviour
    {
        private const string extensin = ".json";

        public IEnumerator LoadLastScene(string saveFile)
        {
            JObject state = LoadJsonFromFile(saveFile);
            IDictionary<string, JToken> stateDict = state;
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (stateDict.ContainsKey("lastSceneBuildIndex"))
            {
                buildIndex = stateDict["lastSceneBuildIndex"].ToObject<int>();
            }

            if (buildIndex == 2) buildIndex = 0;
            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreFromToken(state);
        }

        public void Save(string saveFile)
        {
            JObject state = LoadJsonFromFile(saveFile);
            CaptureAsTokenInSystem(state);
            SaveFileAsJson(saveFile, state);
        }

        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        public void Load(string saveFile)
        {
            RestoreFromToken(LoadJsonFromFile(saveFile));
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (var child in Directory.EnumerateFiles(Application.persistentDataPath))
            {
                if (Path.GetExtension(child) == extensin)
                {
                    yield return Path.GetFileNameWithoutExtension(child);
                }
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + extensin);
        }

        private void SaveFileAsJson(string saveFile, JObject state)
        {
            string path = GetPathFromSaveFile(saveFile);
            Debug.Log("Saving to " + path);
            using (var textWriter = File.CreateText(path))
            {
                using (var writer = new JsonTextWriter(textWriter))
                {
                    writer.Formatting = Formatting.Indented;
                    state.WriteTo(writer);

                    // string a = state.ToString();
                    // byte[] buffer = Encoding.ASCII.GetBytes(a);
                    //
                    // JObject b = JObject.Parse(a);
                    // IDictionary<string, JToken> stateDict = b;
                    //
                    // int c = 0;
                }
            }
        }

        private void CaptureAsTokenInSystem(JObject state)
        {
            IDictionary<string, JToken> stateDict = state;
            JsonSaveableEntity[] tmp = FindObjectsOfType<JsonSaveableEntity>();

            foreach (var child in tmp)
            {
                stateDict[child.GetUniqueIdentifier()] = child.CaptureAsJTokenInEntity();
            }

            stateDict["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreFromToken(JObject state)
        {
            IDictionary<string, JToken> stateDict = state;
            foreach (var child in FindObjectsOfType<JsonSaveableEntity>())
            {
                string id = child.GetUniqueIdentifier();
                if (stateDict.ContainsKey(id))
                {
                    child.RestoreFromJToken(stateDict[id]);
                }
            }
        }

        private JObject LoadJsonFromFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new JObject();
            }

            using (var textReader = File.OpenText(path))
            {
                using (var reader = new JsonTextReader(textReader))
                {
                    reader.FloatParseHandling = FloatParseHandling.Double;

                    return JObject.Load(reader);
                }
            }
        }
    }
}