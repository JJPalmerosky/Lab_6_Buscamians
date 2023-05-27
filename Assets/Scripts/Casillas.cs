using UnityEngine;

public struct Casillas 
{
    public enum Type
    {
        Invalid,
        Empty,
        Mine,
        Number,
    }

    public Vector3Int position; //Para trabajar con el TileMap
    public Type type;
    public int number;
    public bool reveald; //Esta o no revelada la casilla
    public bool flagged; //Bandera
    public bool exploded;
}
