using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class DataManager : SimpleSingleton<DataManager>
{
    public string PokeDataApiLink;
    public string PokeColorApiLink;

    public string SpriteDirectory;

    public bool IsDataInitDone;
    public bool IsRequestDone;
    public List<PokeData> CurrentLoadedData = new List<PokeData>();

    [SerializeField]
    SerializedDictionary<string, Color> _colorDict = new SerializedDictionary<string, Color>();

    [HideInInspector]
    public int TotalEntry;

    [HideInInspector]
    public List<PokeURL> AllEntryUrl;

    Dictionary<int, Sprite> _spriteHash = new Dictionary<int, Sprite>();
    Dictionary<string, string> _colorHash = new Dictionary<string, string>();
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(GetInitialData());
    }

    public void LoadPokeData(int start, int end)
    {
        StartCoroutine(LoadData(start, end));
    }

    IEnumerator GetInitialData()
    {
        IsDataInitDone = false;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(PokeDataApiLink))
        {
            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    RequestResult result = JsonConvert.DeserializeObject<RequestResult>(webRequest.downloadHandler.text);
                    TotalEntry = result.count;
                    AllEntryUrl = result.results;
                    break;
            }
        }
        for (int i = 0; i < 11; i++)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(PokeColorApiLink + i))
            {
                yield return webRequest.SendWebRequest();
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        PokeColor colorData = JsonConvert.DeserializeObject<PokeColor>(webRequest.downloadHandler.text);
                        foreach(PokeSpecies specie in colorData.pokemon_species)
                        {
                            _colorHash[specie.name] = colorData.name;
                        }
                        break;
                }
            }

        }
        IsDataInitDone = true;
    }

    IEnumerator LoadData(int startIdx, int endIdx)
    {
        int idx = startIdx;
        IsRequestDone = false;
        CurrentLoadedData = new List<PokeData>();
        while (idx <= endIdx)
        {
            if(idx >= TotalEntry)
            {
                IsRequestDone = true;
                yield break;
            }
            using (UnityWebRequest webRequest = UnityWebRequest.Get(AllEntryUrl[idx].url))
            {
                yield return webRequest.SendWebRequest();
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        PokeData pokeData = JsonConvert.DeserializeObject<PokeData>(webRequest.downloadHandler.text);
                        CurrentLoadedData.Add(pokeData);
                        _spriteHash[pokeData.id] = Resources.Load<Sprite>(SpriteDirectory + pokeData.id);
                        break;
                }
            }
            idx++;
        }
        IsRequestDone = true;
    }

    public bool TryGetData(int idx, out PokeData pokeData)
    {
        if (idx >= CurrentLoadedData.Count)
        {
            pokeData = null;
            return false; 
        }
        else
        {
            pokeData = CurrentLoadedData[idx];
            return true;
        }
    }

    public bool TryGetSprite(int id, out Sprite sprite)
    {
        return _spriteHash.TryGetValue(id, out sprite);
    }

    public Color GetColor(string speciesName)
    {
        if(_colorHash.TryGetValue(speciesName, out var colorName))
        {
            if (_colorDict.TryGetValue(colorName, out Color color)) return color;
            return Color.gray;
        }
        return Color.gray;
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
    public PokeSpecies species;
}

public class RequestResult
{
    public int count;
    public List<PokeURL> results;
}

public class PokeURL
{
    public string name;
    public string url;
}

public class PokeSpecies
{
    public string name;
    public string url;
}

public class PokeColor
{
    public string name;
    public List<PokeSpecies> pokemon_species;
}