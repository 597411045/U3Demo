using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ABSystem : MonoBehaviour
{
    private string url;
    private bool ABDone = false;
    private int fileCount = 0;

    private void Start()
    {
        ABSystem ab = this.GetComponent<ABSystem>();
        ab.Process();
    }

    private void Update()
    {
        if (ABDone)
        {
            SceneManager.LoadScene("Shooter");
            ABDone = false;
        }

        if (Time.time > 2 && fileCount == 0)
        {
            ABDone = true;
        }
    }

    public void Process()
    {
        url = Application.dataPath + "/../AssetBundles/StandaloneWindows/";
        string fileName = "StandaloneWindows";
        Debug.Log("开始AB包流程");
        StartCoroutine(DownloadAB(url, fileName, AnalyseAB));
    }

    IEnumerator DownloadAB(string url, string fileName, Predicate<string> action)
    {
        fileCount++;
        UnityWebRequest request = UnityWebRequest.Get(url + fileName);
        Debug.Log("正在下载：" + fileName);
        yield return request.SendWebRequest();
        Debug.Log("下载后：" + fileName);
        if (request.isDone)
        {
            Debug.Log("下载完成：" + fileName);
            byte[] results = request.downloadHandler.data;
            Stream stream = File.Create(Application.streamingAssetsPath + "/" + fileName + ".tmp");
            stream.Write(results, 0, results.Length);
            stream.Close();
            stream.Dispose();
            action(fileName);
        }

        fileCount--;
    }

    private bool CheckABName(string fileName)
    {
        Debug.Log("检查是否有相同名：" + fileName);
        if (!File.Exists(Application.streamingAssetsPath + "/" + fileName + ".ab"))
        {
            Debug.Log("无相同ab包：" + fileName);
            FileInfo fi = new FileInfo(Application.streamingAssetsPath + "/" + fileName + ".tmp");
            fi.MoveTo(Application.streamingAssetsPath + "/" + fileName + ".ab");
            return true;
        }
        else
        {
            Debug.Log("存在相同ab包：" + fileName);
            return false;
        }
    }

    private bool AnalyseAB(string fileName)
    {
        if (CheckABName(fileName))
        {
            //无相同包
            AssetBundle ab =
                AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + fileName + ".ab");
            AssetBundleManifest abm = ab.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
            string[] childrenAB = abm.GetAllAssetBundles();
            foreach (var item in childrenAB)
            {
                Debug.Log("下载子包：" + item);
                StartCoroutine(DownloadAB(url, item, CheckABName));
            }

            ab.Unload(true);
        }
        else
        {
            //有相同包
            Debug.Log("不同的主包，开始对比");
            AssetBundle newAB =
                AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + fileName + ".tmp");
            AssetBundleManifest newABM = newAB.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
            Dictionary<string, Hash128> newInfo = new Dictionary<string, Hash128>();
            string[] newChildrenAB = newABM.GetAllAssetBundles();

            foreach (var item in newChildrenAB)
            {
                newInfo.Add(item, newABM.GetAssetBundleHash(item));
            }

            newAB.Unload(true);

            AssetBundle oldAB =
                AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + fileName + ".ab");
            AssetBundleManifest oldABM = oldAB.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
            Dictionary<string, Hash128> oldInfo = new Dictionary<string, Hash128>();
            string[] oldChildrenAB = oldABM.GetAllAssetBundles();
            foreach (var item in oldChildrenAB)
            {
                oldInfo.Add(item, oldABM.GetAssetBundleHash(item));
            }

            oldAB.Unload(true);

            bool notingUpdated = true;
            foreach (var item in newInfo)
            {
                Debug.Log("新包:" + item.Key + item.Value);

                if (oldInfo.ContainsKey(item.Key))
                {
                    Debug.Log("有同名旧包");
                    if (oldInfo[item.Key] != item.Value)
                    {
                        Debug.Log("有同名旧包，且HASH不一致");
                        File.Delete(Application.streamingAssetsPath + "/" + item.Key + ".ab");
                        StartCoroutine(DownloadAB(url, item.Key, CheckABName));
                        notingUpdated = false;
                    }
                    else
                    {
                        Debug.Log("有同名旧包，HASH一致");
                    }
                }
                else
                {
                    Debug.Log("无同名旧包");

                    StartCoroutine(DownloadAB(url, item.Key, CheckABName));
                    notingUpdated = false;
                }
            }

            if (notingUpdated)
            {
                File.Delete(Application.streamingAssetsPath + "/" + fileName + ".tmp");
            }
            else
            {
                File.Delete(Application.streamingAssetsPath + "/" + fileName + ".ab");
                CheckABName(fileName);
            }
        }

        return false;
    }
}

// if (File.Exists(Application.streamingAssetsPath + "/StandaloneWindows.assetbundle"))
// {
//     AssetBundle ab =
//         AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/StandaloneWindows.assetbundle");
//     AssetBundleManifest abm = ab.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
//     //string [] a1=abm.GetAllDependencies();
//     string[] a2 = abm.GetAllAssetBundles();
//     foreach (var item in a2)
//     {
//         Hash128 hash = abm.GetAssetBundleHash(item);
//     }
// }