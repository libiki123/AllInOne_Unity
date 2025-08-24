using System.Collections.Generic;
using UnityEngine;

public class ScrollListController : MonoBehaviour
{
    [SerializeField] private ScrollListModel _model; // 
    [SerializeField] private ScrollListView _view;

    [SerializeField] private List<SimpleScrollItemModel> _testItems = new List<SimpleScrollItemModel>();

    void Start()
    {
        if (_testItems.Count > 0)
        {
            foreach (var item in _testItems)
            {
                // _model.Add(item);
                _view.CreateItem(item);
            }
        }
    }

}
