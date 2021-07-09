using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEvents : MonoBehaviour
{
    public BattleScene gameMaster;
    public GameObject field;

    // Pads
    public Tilemap padsTilemap;
    public Tile padTileModel, selectedPadTileModel;
    public List<Vector3Int> padTiles;

    // Zoom state
    public const float mouseScrollSensitivity = 0.1f;
    private float currentScroll;

    // Drag state
    private bool pressed;
    private float dragX, dragY;

    void Start() {
        currentScroll = 0;
        pressed = false;
    }
    void Update() {
        if (!gameMaster.isLooking) {
            if (!gameMaster.gameState.IsThisPlayerTurn()) {
                return;
            }

            // Handle events as a function of the game Phase


            return;
        }


        // === Handle zoom =========================

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


        // === Handle drag =========================

        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        if (!pressed && Input.GetMouseButtonDown(0)) {
            dragX = mousePos.x - field.transform.position.x;
            dragY = mousePos.y - field.transform.position.y;
            pressed = true;
        }
        else if (pressed && Input.GetMouseButtonUp(0)) {
            pressed = false;
        }

        if (pressed) {
            field.transform.position = new Vector3(mousePos.x - dragX, mousePos.y - dragY, field.transform.position.z);
        }
    }

    public void ShowPositions(Warrior warrior) {
        int wd = warrior.GetWalkingDistance();
        int x = warrior.GetX(); int y = warrior.GetY();

        Vector3Int pos = new Vector3Int(x, y, 0);
        AddPad(pos, true);

        for (int dx=-wd ; dx <= wd ; dx++) {
            for (int dy=-wd + Mathf.Abs(dx) ; dy <= wd - Mathf.Abs(dx) ; dy++) {
                if (dx == 0 && dy == 0) {
                    continue;
                }

                List<PathFind.Point> path = gameMaster.field.GetPath(x, y, x + dx, y + dy);
                if (path != null && path.Count > 0 && path.Count <= wd) {
                    pos.x = x + dx;
                    pos.y = y + dy;
                    AddPad(pos, false);
                }
            }
        }
    }
    public void AddPad(Vector3Int pos, bool selected) {
        if (!padsTilemap.HasTile(pos)) {
            padTiles.Add(pos);
        }
        padsTilemap.SetTile(pos, selected ? selectedPadTileModel : padTileModel);
    }
    public void RemovePads() {
        while (padTiles.Count > 0) {
            padsTilemap.SetTile(padTiles[0], null);
            padTiles.RemoveAt(0);
        }
    }
}
