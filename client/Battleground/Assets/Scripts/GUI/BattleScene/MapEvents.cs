using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEvents : MonoBehaviour
{
    public BattleScene gameMaster;
    public GameObject field;

    private bool dragging;
    private float dragX, dragY;

    void Start() {
        dragging = false;
    }
    void Update() {
        if (!gameMaster.isLooking && !dragging) {
            return;
        }

        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        if (!dragging && Input.GetMouseButtonDown(0)) {
            dragX = mousePos.x - field.transform.position.x;
            dragY = mousePos.y - field.transform.position.y;
            dragging = true;
        }
        else if (dragging && Input.GetMouseButtonUp(0)) {
            dragging = false;
        }

        if (dragging) {
            field.transform.position = new Vector3(mousePos.x - dragX, mousePos.y - dragY, field.transform.position.z);
        }
    }
}
