using UnityEngine.Tilemaps;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }

    public Tile tileUnknown;
    public Tile tileEmpty;
    public Tile tileMine;
    public Tile tileExploded;
    public Tile tileFlag;
    public Tile tileNum1;
    public Tile tileNum2;
    public Tile tileNum3;
    public Tile tileNum4;
    public Tile tileNum5;
    public Tile tileNum6;
    public Tile tileNum7;
    public Tile tileNum8;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(Casillas[,] state)
    {
        int width = state.GetLength(0);
        int height = state.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Casillas casillas = state[x, y];
                tilemap.SetTile(casillas.position, GetTile(casillas));
            }
        }
    }

    private Tile GetTile(Casillas casillas)
    {
        if (casillas.reveald)
        {
            return GetRevealedTile(casillas);
        }
        else if (casillas.flagged)
        {
            return tileFlag;
        }
        else { return tileUnknown; }
    }

    private Tile GetRevealedTile(Casillas casillas)
    {
        switch (casillas.type)
        {
            case Casillas.Type.Empty: return tileEmpty;
            case Casillas.Type.Mine: return casillas.exploded ? tileExploded : tileMine; // Si la mina explotó, devuelve tileExploded, sino, devuelve tileMine
            case Casillas.Type.Number: return GetNumberTile(casillas);
            default: return null;
        }
    }

    private Tile GetNumberTile(Casillas casillas)
    {
        switch (casillas.number)
        {
            case 1: return tileNum1;
            case 2: return tileNum2;
            case 3: return tileNum3;
            case 4: return tileNum4;
            case 5: return tileNum5;
            case 6: return tileNum6;      
            case 7: return tileNum7;
            case 8: return tileNum8;
            default: return null;
        }
    }
}
