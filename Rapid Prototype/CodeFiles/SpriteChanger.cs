using UnityEngine;
using UnityEngine.Tilemaps;

public class SpriteChanger : MonoBehaviour
{
    public TileBase[] oldTiles;
    public TileBase[] newTiles;

    void Start()
    {


        BeginTilechange();
        
    }

    public void BeginTilechange() {

        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();

        if (tilemaps.Length > 0)
        {
            
            foreach (Tilemap tilemap in tilemaps)
            {
                // tauscht die Tiles aus
                ChangeTiles(tilemap, oldTiles, newTiles);
            }
        }
        else
        {
            Debug.LogError("No Tilemap components found on the GameObject.");
        }

    }

    void ChangeTiles(Tilemap tilemap, TileBase[] fromTiles, TileBase[] toTiles)
    {
        // Debug.Log("Changing tiles in Tilemap: " + tilemap.gameObject.name);

        // Holt alle Tileposis aus der tilemap
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            // prüft, ob es das richtige Tile an der Position gibt
            TileBase currentTile = tilemap.GetTile(position);

            for (int i = 0; i < fromTiles.Length; i++)
            {
                if (currentTile == fromTiles[i])
                {
                    // Debug.Log("Tile found at position. Changing.");

                    // Neues Tile wird dort gesetzt
                    tilemap.SetTile(position, toTiles[i]);
                    break; // Verlasse die Schleife, sobald das Tile gefunden wurde
                }
            }
        }
    }
}