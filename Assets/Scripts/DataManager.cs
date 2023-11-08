using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class DataManager : SimpleSingleton<DataManager>
{

    public string ApiLink;
    public bool IsRequestDone;
    public List<PokeData> CurrentLoadedData = new List<PokeData>();

    private void Start()
    {
        LoadPokeData(1, 100);
    }

    public void LoadPokeData(int start, int end)
    {
        StartCoroutine(GetRequest(ApiLink, start, end));
    }

    IEnumerator GetRequest(string uri, int startId, int endId)
    {
        int id = startId;
        CurrentLoadedData = new List<PokeData>();
        IsRequestDone = false;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    RequestResult result = JsonConvert.DeserializeObject<RequestResult>(webRequest.downloadHandler.text);
                    Debug.Log(result);
                    break;
            }
        }
        id++;
    }
}


[System.Serializable]
public class PokeData
{
    public int id;
    public string name;
    public int height;
    public int weight;
    public int order;
}

public class RequestResult
{
    public int count;
    public List<PokeURL> results;
}

[System.Serializable]
public class PokeURL
{
    public string name;
    public string url;
}