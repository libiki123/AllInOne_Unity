using UnityEngine;

public class ScrollListView : MonoBehaviour
{
    [SerializeField] private RectTransform _contentContainer;
    [SerializeField] private ScrollItemView _itemPrefab;

    private ScrollItemView _currentSelectedItem;

    public void CreateItem(SimpleScrollItemModel data)
    {
        var newItem = Instantiate(_itemPrefab, _contentContainer);
        newItem.gameObject.SetActive(true);
        newItem.Init(this, data.Title, data.Color);
    }

    public void OnItemClick(ScrollItemView item)
    {
        if (_currentSelectedItem != null)
        {
            _currentSelectedItem.Deselect();
        }
        _currentSelectedItem = item;
        _currentSelectedItem.Select();

    }
}