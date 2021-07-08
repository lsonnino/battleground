using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEvents : MonoBehaviour
{
    public BattleScene gameMaster;
    public GameObject field;

    public const float mouseScrollSensitivity = 0.1f;
    private float currentScroll;

    private bool dragging;
    private float dragX, dragY;

    void Start() {
        currentScroll = 0;
        dragging = false;
    }
    void Update() {
        if (!gameMaster.isLooking) {
            return;
        }

        float scrollDelta = Input.mouseScrollDelta.y * mouseScrollSensitivity;
        if (Mathf.Abs(scrollDelta) >= 0.1f) {
            currentScroll += scrollDelta;
            float scale = 1f;

            if (currentScroll > 0.1f) {
                scale = 0.9f;
                currentScroll = 0;
            }
            else if (currentScroll < -0.1f) {
                scale = 1/0.9f;
                currentScroll = 0;
            }

            field.transform.localScale = new Vector3(
                field.transform.localScale.x * scale,
                field.transform.localScale.y * scale,
                field.transform.localScale.z
            );

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
