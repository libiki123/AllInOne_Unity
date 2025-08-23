using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class ToggleGroupManager : MonoBehaviour
{
    public static event Action<int> OnToggleSelected;

    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private Toggle[] _toggles;

    [SerializeField] private string _borderComponentName = "Border";
    [SerializeField] private string _labelComponentName = "Label";

    private Dictionary<Toggle, ToggleComponents> _toggleComponentsMap = new Dictionary<Toggle, ToggleComponents>();
    private int _currentSelectedIndex;

    [Serializable]
    public class ToggleComponents
    {
        public Image borderImage;
        public TMP_Text label;
    }

    private void Awake()
    {
        if (_toggleGroup == null)
        {
            _toggleGroup = GetComponent<ToggleGroup>();
        }

        InitializeToggles();
    }

    private void InitializeToggles()
    {
        for (int i = 0; i < _toggles.Length; i++)
        {
            var toggle = _toggles[i];
            var toggleIndex = i;

            var toggleComp = new ToggleComponents
            {
                borderImage = FindChildComponent<Image>(toggle.transform, _borderComponentName),
                label = FindChildComponent<TMP_Text>(toggle.transform, _labelComponentName)
            };

            _toggleComponentsMap[toggle] = toggleComp;
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                    OnToggleValueChanged(toggleIndex);

                UpdateToggleVisuals(toggle, isOn);
            });

            UpdateToggleVisuals(toggle, toggle.isOn);

            if (toggle.isOn)
                _currentSelectedIndex = toggleIndex;
        }
    }

    T FindChildComponent<T>(Transform parent, string childName) where T : Component
    {
        Transform child = parent.Find(childName);
        return child != null ? child.GetComponent<T>() : null;
    }

    private void OnToggleValueChanged(int toggleIndex)
    {
        _currentSelectedIndex = toggleIndex;
        OnToggleSelected?.Invoke(toggleIndex);
    }

    private void UpdateToggleVisuals(Toggle toggle, bool isOn)
    {
        if (_toggleComponentsMap.TryGetValue(toggle, out var components))
        {
            if (components.borderImage != null)
            {
                components.borderImage.gameObject.SetActive(isOn);
            }

            if (components.label != null)
            {
                components.label.color = isOn ? Color.red : Color.black;
            }
        }
    }

    private void OnDestroy()
    {
        if (_toggles != null)
        {
            foreach (var toggle in _toggles)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }
        }
    }
}
