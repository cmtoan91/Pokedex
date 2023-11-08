using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PokeDisplaySlot : MonoBehaviour
{
    [SerializeField]
    Image _backgroundImage;

    [SerializeField]
    Image _pokemonImage;

    [SerializeField]
    Button _button;

    PokeData _pokeData;
    UI_PokeDisplayCanvas _controller;

    public void SetController(UI_PokeDisplayCanvas controller)
    {
        _controller = controller;
    }
    public void Init(PokeData data)
    {
        Color bgColor = DataManager.Instance.GetColor(data.species.name);
        _backgroundImage.color = bgColor;
        if (DataManager.Instance.TryGetSprite(data.id, out var sprite))
            _pokemonImage.sprite = sprite;
        _pokeData = data;
        _button.onClick.AddListener(() => DisplayInfo());
    }

    void DisplayInfo() 
    {
        _controller?.ShowInfo(_pokeData);
    }

}
