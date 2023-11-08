using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
public class UI_PokeDisplayCanvas : MonoBehaviour
{
    public int StartIndex = 0;
    public int EntryPerIndex = 6;
    public int CurrentPage;
    public UI_PokeDisplaySlot SlotPrefab;

    //public Transform Content;
    public ScrollRect Scroll;

    private void Start()
    {
        Init();
    }

    async void Init()
    {
        while (!DataManager.Instance.IsRequestDone)
        {
            await Task.Delay(100);
        }
        for(int i = 0; i < DataManager.Instance.CurrentLoadedData.Count; i++)
        {
            UI_PokeDisplaySlot slot = Instantiate(SlotPrefab, Scroll.content);
        }
    }

    void LoadPage()
    {

    }
}
