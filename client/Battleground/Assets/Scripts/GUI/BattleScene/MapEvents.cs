using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEvents : MonoBehaviour
{
    public BattleScene gameMaster;
    public GameObject field;

    // TODO: remove "once"
    public bool once = false;

    // Pads
    public Tilemap padsTilemap;
    public Tile padTileModel;
    public List<Vector3Int> padTiles;

    // Zoom state
    public const float mouseScrollSensitivity = 0.1f;
    private float currentScroll;

    // Drag state
    private bool dragging;
    private float dragX, dragY;

    void Start() {
        currentScroll = 0;
        dragging = false;
    }
    void Update() {
        // TODO: remove "once"
        if (!once) {
            once = true;
            Warrior dino = new Warrior(Warrior.DINO);
            dino.x = 0;
            dino.y = 0;
            ShowPositions(dino);
        }

        if (!gameMaster.isLooking) {
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

    public void ShowPositions(Warrior warrior) {
        int wd = warrior.GetWalkingDistance();
        int x = warrior.x; int y = warrior.y;

        padsTilemap.SetTile(new Vector3Int(x, y, 0), padTileModel);

        for (int dx=-wd ; dx <= wd ; dx++) {
            for (int dy=-wd + Mathf.Abs(dx) ; dy <= wd - Mathf.Abs(dx) ; dy++) {
                if (dx == 0 && dy == 0) {
                    continue;
                }

                // TODO: check if there is another player there

                List<PathFind.Point> path = gameMaster.field.GetPath(x, y, x + dx, y + dy);
                if (path != null && path.Count > 0 && path.Count <= wd) {
                    Vector3Int pos = new Vector3Int(x + dx, y + dy, 0);
                    padsTilemap.SetTile(pos, padTileModel);
                    padTiles.Add(pos);
                }
            }
        }
    }
    public void RemovePads() {
        while (padTiles.Count > 0) {
            padsTilemap.SetTile(padTiles[0], null);
            padTiles.RemoveAt(0);
        }
    }
}
