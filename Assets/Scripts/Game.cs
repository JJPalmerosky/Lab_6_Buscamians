using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public int width = 16;
    public int height = 16;
    public int mineCount = 32;
    public Image happy;
    public Image glasses;
    public Image shitty;


    private Board board;
    private Casillas[,] state;
    private bool gameover;
    private TimeControl time;
    private bool pausa;

    public void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }

    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        NewGame();
        time = GetComponent<TimeControl>();
        glasses.enabled = false;
        shitty.enabled = false;
    }

    public void NewGame()
    {
        state = new Casillas[width, height];
        gameover = false;

        GenerateCells();
        GenerateMines();
        GenerateNumbers();

        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        board.Draw(state);
        

    }

    public void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Casillas casillas = new Casillas();
                casillas.position = new Vector3Int(x, y, 0);
                casillas.type = Casillas.Type.Empty;
                state[x, y] = casillas;
            }
        }
    }

    public void GenerateMines()
    {
        for (int i = 0; i < mineCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            while (state[x, y].type == Casillas.Type.Mine) // Sigue poniendo minas mientras esto no sea verdad.
            {
                x++;

                if (x >= width) //Si estas en la esquina o  borde del tablero.
                {
                    x = 0;
                    y++;
                    
                    if (y >= height) //Si estas en el final del tablero.
                    {
                        y = 0;
                    }
                }
            }

            state[x, y].type = Casillas.Type.Mine;
        }
    }

    public void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Casillas casillas = state[x, y];

                if (casillas.type == Casillas.Type.Mine)
                {
                    continue;
                }

                casillas.number = CountMines(x, y);

                if (casillas.number > 0)
                {
                    casillas.type = Casillas.Type.Number;
                }

                state[x, y] = casillas;
            }
        }
    }

    private int CountMines(int casillaX, int casillaY) // Cuenta las minas al rededor de una casilla 
    {
        int count = 0;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)
                {
                    continue;
                }

                int x = casillaX + adjacentX; 
                int y = casillaY + adjacentY;

                if (GetCasillas(x, y).type == Casillas.Type.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void Update()
    {
        if (!gameover)
        {
            if (Input.GetMouseButtonDown(1)) //Boton derecho
            {
                Flag();
            }
            else if (Input.GetMouseButtonDown(0)) //Botón izquierdo
            {
                Reveal();
            }
        }
        
    }

    private void Flag()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Casillas casillas = GetCasillas(cellPosition.x, cellPosition.y);

        if (casillas.type == Casillas.Type.Invalid || casillas.reveald)
        {
            return;
        }

        casillas.flagged = !casillas.flagged;
        state[cellPosition.x, cellPosition.y] = casillas;
        board.Draw(state);
    }

    private void Reveal()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Casillas casillas = GetCasillas(cellPosition.x, cellPosition.y);

        if (casillas.type == Casillas.Type.Invalid || casillas.reveald || casillas.flagged)
        {
            return;
        }

        switch (casillas.type)
        {
            case Casillas.Type.Mine:
                Explode(casillas);
                break;

            case Casillas.Type.Empty:
                Flood(casillas);
                CheckWinCondition();
                break;

            default:
                casillas.reveald = true;
                state[cellPosition.x, cellPosition.y] = casillas;
                CheckWinCondition();
                break;

        }

        board.Draw(state);
    }

    private void Flood(Casillas casillas) // Si se revela una casilla vacia, revela las casillas vacias proximas a esta casilla
    {
        if (casillas.reveald) return; // para si ya se ha revelado la casilla
        if (casillas.type == Casillas.Type.Mine || casillas.type == Casillas.Type.Invalid) return; //para si la casilla es una mina o si es una casilla invalida

        casillas.reveald = true;
        state[casillas.position.x, casillas.position.y] = casillas; // revela las casillas

        if (casillas.type == Casillas.Type.Empty) // Si se encuentra con otra casilla vacia, vuelve a llamarse a si mismo.
        {
            Flood(GetCasillas(casillas.position.x - 1, casillas.position.y)); //Casilla vacia hacia la izquierda
            Flood(GetCasillas(casillas.position.x + 1, casillas.position.y)); // hacia la derecha
            Flood(GetCasillas(casillas.position.x, casillas.position.y - 1)); // hacia abajo
            Flood(GetCasillas(casillas.position.x, casillas.position.y + 1)); // hacia arriba 
        }
    }

    public void Explode(Casillas casillas)
    {
        shitty.enabled = true;
        happy.enabled = false;
        Debug.Log("Game Over!");
        gameover = true;

        casillas.reveald = true;
        casillas.exploded = true;
        state[casillas.position.x, casillas.position.y] = casillas;

        for (int x = 0; x < width; x++) // Revela todas las minas del juego.
        {
            for (int y = 0; y < height; y++)
            {
                casillas = state[x, y];

                if (casillas.type == Casillas.Type.Mine)
                {
                    casillas.reveald = true;
                    state[x, y] = casillas;
                }
            }
        }
    }

    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < height; y++)
            {
                Casillas casillas = state[x, y];

                if (casillas.type != Casillas.Type.Mine && !casillas.reveald)
                {
                    return;
                }
            }
        }

        happy.enabled = false;
        glasses.enabled = true;
        Debug.Log("Winner");
        gameover = true;

        for (int x = 0; x < width; x++) // Cambia las minas que hay en el juego por una bandera, para indicar donde estaban
        {
            for (int y = 0; y < height; y++)
            {
                Casillas casillas = state[x, y];

                if (casillas.type == Casillas.Type.Mine)
                {
                    casillas.flagged = true;
                    state[x, y] = casillas;
                }
            }
        }
    }

    private Casillas GetCasillas(int x, int y)
    {
        if (IsValid(x, y))
        {
            return state[x, y];
        } 
        else
        {
            return new Casillas();
        }
    }

    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void Salir()
    {
        Application.Quit();
    }
}
