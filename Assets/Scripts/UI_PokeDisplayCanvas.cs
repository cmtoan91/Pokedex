using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class UI_PokeDisplayCanvas : MonoBehaviour
{
    public Button NextPage;
    public Button PrevPage;
    public Button CloseApp;

    public UI_PokeInfoPanel InfoPanel;
    public TMP_Text PageTextDisplay;
    public GameObject LoadingPanel;

    public int EntryPerPage = 20;
    public int CurrentPage = 1;
    
    public UI_PokeDisplaySlot SlotPrefab;
    int _maximumPage;
    
    //public Transform Content;
    public ScrollRect Scroll;
    List<UI_PokeDisplaySlot> _allSlot = new List<UI_PokeDisplaySlot>();
    bool _isload;
    private void Start()
    {
        Screen.SetResolution(675, 1080, true);
        Init();
        CurrentPage = 1;
        LoadCurrentPage();
        NextPage.onClick.AddListener(() => OnNextPage());
        PrevPage.onClick.AddListener(() => OnPrevPage());
        CloseApp.onClick.AddListener(() => OnCloseApp());
    }

    void Init()
    { 
        for(int i = 0; i < EntryPerPage; i++)
        {
            UI_PokeDisplaySlot slot = Instantiate(SlotPrefab, Scroll.content);
            slot.SetController(this);
            _allSlot.Add(slot);
        }
        _maximumPage = DataManager.Instance.TotalEntry / EntryPerPage;
        if (DataManager.Instance.TotalEntry % EntryPerPage > 0) _maximumPage++;
    }

    async void LoadCurrentPage()
    {
        PageTextDisplay.text = string.Format("{0}/{1}", CurrentPage, _maximumPage);
        _isload = true;
        LoadingPanel.SetActive(true);
        while (!DataManager.Instance.IsDataInitDone)
        {
            await Task.Delay(100);
        }
        int startIndex = (CurrentPage - 1) * EntryPerPage;
        int endIndex = startIndex + EntryPerPage - 1;
        DataManager.Instance.LoadPokeData(startIndex, endIndex);
        while (!DataManager.Instance.IsRequestDone)
        {
            await Task.Delay(100);
        }
        for(int i = 0; i < EntryPerPage; i++)
        {
            if(DataManager.Instance.TryGetData(i, out var pokeData))
            {
                _allSlot[i].gameObject.SetActive(true);
                _allSlot[i].Init(pokeData);
            }
            else
            {
                _allSlot[i].gameObject.SetActive(false);
            }
        }
        _isload = false;
        LoadingPanel.SetActive(false);
    }

    public void ShowInfo(PokeData pokeData)
    {
        InfoPanel.DisplayData(pokeData);
    }

    void OnNextPage()
    {
        if (CurrentPage >= _maximumPage) return;
        if (_isload) return;
        CurrentPage++;
        LoadCurrentPage();
    }

    void OnPrevPage()
    {
        if (CurrentPage <= 1) return;
        if (_isload) return;
        CurrentPage--;
        LoadCurrentPage();
    }

    void OnCloseApp()
    {
        Application.Quit();
    }
}
