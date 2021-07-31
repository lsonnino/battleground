using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectorPaneEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image entry, indicator;
    public Sprite hoverSprite, selectedSprite, transparentSprite;
    public int index;
    public SelectorPane selectorPane;
    public bool selected, containsElement;

    public void Init(Sprite sprite) {
        if (sprite == null) {
            indicator.sprite = transparentSprite;
            containsElement = false;
        }
        else {
            indicator.sprite = sprite;
            containsElement = true;
        }

        entry.sprite = transparentSprite;
        selected = false;
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
        if (!selected && containsElement) {
            entry.sprite = hoverSprite;
        }
    }
    public void OnPointerExit(PointerEventData pointerEventData) {
        if (!selected && containsElement) {
            entry.sprite = transparentSprite;
        }
    }
    public void OnPointerClick(PointerEventData pointerEventData) {
        if (containsElement) {
            selected = true;
            selectorPane.Select(index);
        }
    }
}
