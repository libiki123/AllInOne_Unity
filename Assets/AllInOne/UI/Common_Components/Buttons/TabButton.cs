using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;


public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image _backgroundImage;
    public Image BackgroundImg => _backgroundImage;

    private TabGroup _tabGroup;

    public void Init(TabGroup tabGroup)
    {
        _tabGroup = tabGroup;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tabGroup.OnTabExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _tabGroup.OnTabClick(this);
    }

    public void Deselect()
    {
        // Add any selection logic here (animations, sound effects, etc.)
    }

    public void Select()
    {
        // Add any delection logic here (animations, sound effects, etc.)
    }
}

