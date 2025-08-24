using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TabGroup : MonoBehaviour
{
    public static event Action<int> OnTabSelected;

    [SerializeField] private List<TabButton> _tabButtons = new List<TabButton>();
    [SerializeField] private List<GameObject> _pages = new List<GameObject>();
    [SerializeField] private Sprite _tabIdleSprite;
    [SerializeField] private Sprite _tabHoverSprite;
    [SerializeField] private Sprite _tabActiveSprite;

    private TabButton _selectedTab;
    private GameObject _currentPage;

    private void Start()
    {
        foreach (var tabButton in _tabButtons)
        {
            tabButton.Init(this);
            tabButton.BackgroundImg.sprite = _tabIdleSprite;
        }

        if (_tabButtons.Count > 0)         // Select first tab by default
        {
            OnTabClick(_tabButtons[0]);
        }
    }

    public void OnTabEnter(TabButton tabButton)
    {
        if (tabButton != _selectedTab)
        {
            tabButton.BackgroundImg.sprite = _tabHoverSprite;
        }
    }

    public void OnTabExit(TabButton tabButton)
    {
        if (tabButton != _selectedTab)
        {
            tabButton.BackgroundImg.sprite = _tabIdleSprite;
        }
    }

    public void OnTabClick(TabButton tabButton)
    {
        if (_selectedTab != null)
        {
            _selectedTab.BackgroundImg.sprite = _tabIdleSprite;
            _selectedTab.Deselect();
        }
        _selectedTab = tabButton;
        _selectedTab.Select();
        _selectedTab.BackgroundImg.sprite = _tabActiveSprite;

        int index = _tabButtons.IndexOf(tabButton);
        if (_currentPage != null)
        {
            _currentPage.SetActive(false);
        }
        _currentPage = _pages[index];
        _currentPage.SetActive(true);

        OnTabSelected?.Invoke(index);
    }
}
