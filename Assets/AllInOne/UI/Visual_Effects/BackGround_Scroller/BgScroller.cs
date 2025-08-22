using UnityEngine;
using UnityEngine.UI;

public class BgScroller : MonoBehaviour
{
    [SerializeField] private RawImage _img;
    [SerializeField] private Vector2 _scrollDir = new Vector2(0.1f, 0.1f);

    void Update()
    {
        _img.uvRect = new Rect(_img.uvRect.position + new Vector2(_scrollDir.x, _scrollDir.y) * Time.deltaTime, _img.uvRect.size);
    }
}

