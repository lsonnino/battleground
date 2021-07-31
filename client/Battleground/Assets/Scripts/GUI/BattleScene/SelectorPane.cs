using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectorPane : MonoBehaviour
{
    public RectTransform rectTransform;
    public SelectorPaneEntry[] entries;
    public System.Action<int> callback;

    public void Select(int index) {
        callback(index);
    }

    public void SetEntry(int index, Sprite sprite) {
        if (index < 0 || index >= entries.Length) { return; }
        entries[index].Init(sprite);
    }
    public bool ContainsMouse() {
        Vector2 localpoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out localpoint);

        return rectTransform.rect.Contains(localpoint);
    }

    /*
     * Deprecated code in case it is needed
     */
    /*
    private int GetOnWhich() {
        Vector2 localpoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out localpoint);
        float x = Rect.PointToNormalized(rectTransform.rect, localpoint).x;

        x = x * indicators.Length;
        return (int) x;
    }
    */
}
