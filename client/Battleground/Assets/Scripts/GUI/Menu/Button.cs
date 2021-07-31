using UnityEngine;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData) {
        FindObjectOfType<AudioManager>().Play("ButtonHover");
    }

    public void OnPointerClick(PointerEventData eventData) {
        FindObjectOfType<AudioManager>().Play("ButtonPress");
    }
}
