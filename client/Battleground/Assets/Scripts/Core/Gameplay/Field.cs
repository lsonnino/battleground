using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Field
{
    private int width, height;
    private int offsetX, offsetY;
    private bool[,] walkable;

    private PathFind.Grid pathFind;

    public Field(Tilemap[] nonCollidingTilemaps, Tilemap[] collidingTilemaps) {
        // Get offset and size
        for (int i=0 ; i < nonCollidingTilemaps.Length ; i++) {
            Vector3 origin = nonCollidingTilemaps[i].origin;
            Vector3 size = nonCollidingTilemaps[i].cellBounds.size;
            if (origin.x < offsetX) {
                offsetX = (int) origin.x;
            }
            if (origin.y < offsetY) {
                offsetY = (int) origin.y;
            }
            if (size.x > width) {
                width = (int) size.x;
            }
            if (size.y > height) {
                height = (int) size.y;
            }
        }
        for (int i=0 ; i < collidingTilemaps.Length ; i++) {
            Vector3 origin = collidingTilemaps[i].origin;
            Vector3 size = collidingTilemaps[i].cellBounds.size;
            if (origin.x < offsetX) {
                offsetX = (int) origin.x;
            }
            if (origin.y < offsetY) {
                offsetY = (int) origin.y;
            }
            if (size.x > width) {
                width = (int) size.x;
            }
            if (size.y > height) {
                height = (int) size.y;
            }
        }

        // Set walkable
        walkable = new bool[width, height];
        for (int i=0 ; i < nonCollidingTilemaps.Length ; i++) {
            AddCollisions(nonCollidingTilemaps[i], false);
        }
        for (int i=0 ; i < nonCollidingTilemaps.Length ; i++) {
            AddCollisions(collidingTilemaps[i], true);
        }

        // Set pathfinder instance
        pathFind = new PathFind.Grid(walkable);
    }

    public bool IsInBounds(int x, int y) {
        return !(x - offsetX < 0 || y - offsetY < 0 ||
            x - offsetX >= width || y - offsetY >= height);
    }
    /*
     * Get the path from one point to another. The returning list is empty
     * if there is none
     */
    public List<PathFind.Point> GetPath(int fromX, int fromY, int toX, int toY) {
        if (!IsInBounds(fromX, fromY) || !IsInBounds(toX, toY)) {
            return null;
        }

        PathFind.Point _from = new PathFind.Point(fromX - offsetX, fromY - offsetY);
        PathFind.Point _to = new PathFind.Point(toX - offsetX, toY - offsetY);

        return PathFind.Pathfinding.FindPath(pathFind, _from, _to, PathFind.Pathfinding.DistanceType.Manhattan);
    }

    /*
     * Update walkable by taking into account a new Tilemap
     * NOTE: non collision tilemaps must be all taken into account before the collision
     * ones are added. Otherwise a non-collision tilemap might override a collision
     */
    private void AddCollisions(Tilemap newMap, bool collision) {
        Vector3 origin = newMap.origin;
        Vector3 size = newMap.cellBounds.size;

        for (int x=0 ; x < width ; x++) {
            for(int y=0 ; y < height ; y++) {
                if (offsetX + x < origin.x || offsetY + y < origin.y ||
                    offsetX + x >= origin.x + size.x || offsetY + y >= origin.y + size.y) {
                    continue;
                }

                bool hasTile = newMap.HasTile(new Vector3Int(offsetX + x, offsetY + y, 0));
                if (collision && hasTile) {
                    walkable[x, y] = false;
                }
                else if (!walkable[x, y] && !collision){
                    walkable[x, y] = true;
                }
            }
        }
    }
}
