using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private GameObject _border;

    [Header("Effects")]
    [SerializeField, Range(0f, 1f)] private float _hoverDarkenAmount = 0.7f;

    private ScrollListView _scrollListView;
    private Color _originalColor;
    private bool _isSelected = false;

    public void Init(ScrollListView view, string title, Color color)
    {
        _scrollListView = view;
        _titleText.text = title;
        _backgroundImage.color = color;
        _originalColor = color;
        _border.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isSelected) return;
        _backgroundImage.color = new Color(
            _originalColor.r * _hoverDarkenAmount,
            _originalColor.g * _hoverDarkenAmount,
            _originalColor.b * _hoverDarkenAmount,
            _originalColor.a
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _backgroundImage.color = _originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _scrollListView.OnItemClick(this);
    }

    public void Select()
    {
        _isSelected = true;
        _border.SetActive(true);
    }

    public void Deselect()
    {
        _isSelected = false;
        _border.SetActive(false);
    }


}
