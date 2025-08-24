using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SimpleScrollItemModel
{
    public string Title;
    public Color Color;
}

public class ScrollListModel
{
    public List<SimpleScrollItemModel> Items { get; private set; }

    public ScrollListModel(SimpleScrollItemModel[] items)
    {
        Items = items.ToList();
    }

    public SimpleScrollItemModel Get(int index) => Items[index];
    public void Clear() => Items.Clear();
    public void Add(SimpleScrollItemModel item) => Items.Add(item);
    public void Remove(SimpleScrollItemModel item) => Items.Remove(item);

    public void Swap(int indexA, int indexB)
    {
        (Items[indexA], Items[indexB]) = (Items[indexB], Items[indexA]);
    }
}
