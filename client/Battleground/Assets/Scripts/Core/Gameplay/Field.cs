using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Field
{
    // Intrinsic parameters of the field
    private int width, height;
    private int offsetX, offsetY;
    private bool[,] walkable;

    private PathFind.Grid pathFind;

    // Field state
    private bool[,] walkableState;

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

        walkableState = (bool[,]) walkable.Clone();

        // Set pathfinder instance
        pathFind = new PathFind.Grid(walkableState);
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

    /*
     * Remove a Warrior from the field (if it died for instance)
     */
    public void RemoveWarrior(Warrior warrior) {
        if (warrior.IsPlaced()) {
            walkableState[warrior.GetX() - offsetX, warrior.GetY() - offsetY] =
                walkable[warrior.GetX() - offsetX, warrior.GetY() - offsetY];
        }

        warrior.Place(-1, -1);
    }
    /*
     * Move a warrior to a given position or place it for the first time at a
     * given position
     */
    public void MoveWarrior(Warrior warrior, int x, int y) {
        // Reset where the warrior was
        if (warrior.IsPlaced()) {
            RemoveWarrior(warrior);
        }
        else {
            // The warrior is beeing summoned: cannot attack
            warrior.attacked = true;
        }

        // Set the warrior to the new position
        warrior.Place(x ,y);
        warrior.moved = true;

        // Update the walkable state for path finding
        walkableState[x - offsetX, y - offsetY] = false;
        pathFind.UpdateGrid(walkableState);
    }

    /*
     * Get if a given coordinate is within the bounds of the map
     */
    public bool IsInBounds(int x, int y) {
        return !(x - offsetX < 0 || y - offsetY < 0 ||
            x - offsetX >= width || y - offsetY >= height);
    }
    public bool IsWalkable(int x, int y) {
        if (!IsInBounds(x, y)) { return false; }
        else {
            return this.walkableState[x - offsetX, y - offsetY];
        }
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

        List<PathFind.Point> path = PathFind.Pathfinding.FindPath(pathFind, _from, _to, PathFind.Pathfinding.DistanceType.Manhattan);

        List<PathFind.Point> convertedPath = new List<PathFind.Point>();
        // Convert path to world coordinates
        for (int i=0 ; i < path.Count ; i++) {
            convertedPath.Add(new PathFind.Point(path[i].x + offsetX, path[i].y + offsetY));
        }

        return convertedPath;
    }
}
