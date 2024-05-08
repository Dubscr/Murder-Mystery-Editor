using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
public class DownloadAssetBundles : MonoBehaviour
{
    public WorkshopHandler workshopHandler;
    private void Start()
    {
    }
    public static void LoadAssetBundleLevel(string path)
    {
        List<string> filePaths = new();
        foreach (var file in Directory.GetFiles(path))
        {
            if (!file.Contains("manifest") && !file.Contains("AssetBundles"))
            {

                if (file.Contains("\\"))
                {
                    file.Replace("\\", "/");
                }

                filePaths.Add(file);
            }
        }

        foreach (string filePath in filePaths)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            var go = bundle.LoadAsset(bundle.GetAllAssetNames()[0]) as GameObject;
            InstantiateGameObjectFromAssetBundles(go);
        }
    }
    public static IEnumerator DownloadAssetBundleFromServer(string url)
    {
        GameObject go = null;

        //string url = "https://drive.usercontent.google.com/u/0/uc?id=1sIYnwmaA1T-5r3m9kPsOYcGAq3T6oUcz&export=download";

        UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(webRequest.error);
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);
            go = bundle.LoadAsset(bundle.GetAllAssetNames()[0]) as GameObject;
            bundle.Unload(false);
            yield return new WaitForEndOfFrame();
        }
        webRequest.Dispose();
        InstantiateGameObjectFromAssetBundles(go);
    }

    private static void InstantiateGameObjectFromAssetBundles(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("Asset bundle is null...");
            return;
        }

        GameObject instanceGO = Instantiate(go);
        instanceGO.transform.position = Vector3.zero;
    }
}
