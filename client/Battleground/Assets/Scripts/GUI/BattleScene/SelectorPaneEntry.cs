using UnityEngine;
using UnityEngine.UI;

public class SelectorPaneEntry : MonoBehaviour
{
    public RectTransform rectTransform;
    public EventsHandler handler;

    void Start() {}
    void Update() {}

    public void OnClick() {
        Vector2 localpoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out localpoint);
        float x = Rect.PointToNormalized(rectTransform.rect, localpoint).x;

        x = x * this.transform.childCount;
        handler.SelectorPaneEntrySelected(((int) x));
    }
}
