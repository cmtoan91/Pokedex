using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PokeInfoPanel : MonoBehaviour
{
    public Button _closeButton;

    public TMP_Text _nameText;
    public TMP_Text _weightText;
    public TMP_Text _heightText;
    public TMP_Text _orderText;


    private void Awake()
    {
        _closeButton.onClick.AddListener(() => Close());    
    }

    public void DisplayData(PokeData data)
    {
        gameObject.SetActive(true);
        _nameText.text = data.name;
        _weightText.text = "Weight: " + data.weight.ToString();
        _heightText.text = "Height: " + data.height.ToString();  
        _orderText.text = "Order: " + data.order.ToString();
    }

    void Close()
    {
        gameObject.SetActive(false);
    }
}
