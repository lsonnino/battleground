using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Field
{
    private int width, height;
    private bool[,] walkable;

    private PathFind.Grid pathFind;

    public Field(Tilemap[] nonCollidingTilemaps, Tilemap[] collidingTilemaps) {
        for (int i=0 ; i < nonCollidingTilemaps.Length ; i++) {
            Vector3 size = nonCollidingTilemaps[i].cellBounds.size;
            if (size.x > width) {
                width = (int) size.x;
            }
            if (size.y > height) {
                height = (int) size.y;
            }
        }
        for (int i=0 ; i < collidingTilemaps.Length ; i++) {
            Vector3 size = collidingTilemaps[i].cellBounds.size;
            if (size.x > width) {
                width = (int) size.x;
            }
            if (size.y > height) {
                height = (int) size.y;
            }
        }

        walkable = new bool[width, height];
        for (int x=0 ; x < width ; x++) {
            for(int y=0 ; y < height ; y++) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                bool found = false;

                // Check if there is a collision
                for (int i=0 ; i < collidingTilemaps.Length ; i++) {
                    Vector3 size = collidingTilemaps[i].cellBounds.size;
                    if (x < size.x && y < size.y && collidingTilemaps[i].HasTile(pos)) {
                        walkable[x, y] = false;
                        found = true;
                        break;
                    }
                }

                if (found) { continue; }

                // Check if there is a walkable tile
                for (int i=0 ; i < nonCollidingTilemaps.Length ; i++) {
                    Vector3 size = nonCollidingTilemaps[i].cellBounds.size;
                    if (x < size.x && y < size.y && nonCollidingTilemaps[i].HasTile(pos)) {
                        walkable[x, y] = true;
                        found = true;
                        break;
                    }
                }

                if (found) { continue; }

                // There is no tile there
                walkable[x, y] = false;
            }
        }

        pathFind = new PathFind.Grid(walkable);
    }

    /*
     * Get the path from one point to another. The returning list is empty
     * if there is none
     */
    public List<PathFind.Point> GetPath(int fromX, int fromY, int toX, int toY) {
        if (fromX < 0 || fromX > width || fromY < 0 || fromY > height ||
            toX < 0 || toX > width || toY < 0 || toY > height) {
            return null;
        }

        PathFind.Point _from = new PathFind.Point(fromX, fromY);
        PathFind.Point _to = new PathFind.Point(toX, toY);

        return PathFind.Pathfinding.FindPath(pathFind, _from, _to, PathFind.Pathfinding.DistanceType.Manhattan);
    }

    /*
     * Update the pathfinding to take new collisions into account
     * Takes a tilemap with collisions enabled smaller or of the same size
     * as the largest tilemap taken into account when instantiating this object
     */
    public void AddCollisions(Tilemap newMap) {
        Vector3 size = newMap.cellBounds.size;
        bool updated = false;

        for (int x=0 ; x < width ; x++) {
            for(int y=0 ; y < height ; y++) {
                if (x < size.x && y < size.y && newMap.HasTile(new Vector3Int(x, y, 0))) {
                    // If the place was walkable, it is not anymore: update the
                    // grid at the end of the function
                    updated |= walkable[x, y];
                    walkable[x, y] = false;
                }
            }
        }

        if (updated) {
            pathFind.UpdateGrid(walkable);
        }
    }
}
