using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    //[Required]
    [SerializeField] private GameObject _cell;

    [Tooltip("Delay in seconds between display of two generations")]
    [SerializeField] private float DelayBetweenGenerations = 2f;

    [Header("Grid Size")]
    [Range(3, 100)]
    [SerializeField] private int Width = 25;
    [Range(3, 100)]
    [SerializeField] private int Height = 25;



    private Cell[,] _cells;
    private float StartposX;
    private float StartposY;
    private float CellWidth;
    private float CellHeight;

    private int _width, _height;

    private void Awake()
    {
        Manager.GameState = Manager.GameStateEnum.Invalid; // Set Invalid GameState by default
        _width = Width;
        _height = Height;
        if (_width < 3 || _height < 3)
        {
            Debug.LogError("Invalid Width or Height");
            return;
        }
        if (!_cell)
        {
            Debug.LogError("Cell prefab is not properly set");
            return;
        }

        if (Manager.Init())
        {
            Manager.GameState = Manager.GameStateEnum.Initializing;
        }
    }
    private void Start()
    {
        if (Manager.GameState == Manager.GameStateEnum.Invalid) return;
        _cells = new Cell[_width, _height];
        StartposX = transform.position.x - _width / 2;
        StartposY = transform.position.y - _height / 2;
        CellWidth = _cell.GetComponent<SpriteRenderer>().bounds.size.x;
        CellHeight = _cell.GetComponent<SpriteRenderer>().bounds.size.y;
        PopulateGrid();
    }

    private void Update()
    {
        if (Manager.GameState == Manager.GameStateEnum.EditMode && Input.GetKeyDown("enter"))
        {
            Debug.Log("Running Mode");
            Manager.GameState = Manager.GameStateEnum.Running;
            StartCoroutine("Run");
        }
        else if (Manager.GameState == Manager.GameStateEnum.Running && Input.GetKeyDown("enter"))
        {
            Debug.Log("Edit Mode");
            Manager.GameState = Manager.GameStateEnum.EditMode;
        }
    }

    private void PopulateGrid()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Vector2 pos = new Vector2(StartposX + CellWidth * i + CellWidth / 2, StartposY + CellHeight * j + CellHeight / 2);
                Cell tmpCell = Instantiate(_cell, pos, Quaternion.identity, transform).GetComponent<Cell>();
                tmpCell.gameObject.name = "Cell " + i + " " + j;
                tmpCell.Init(i, j, _width, _height);
                _cells[i, j] = tmpCell;
            }
        }
        Manager.GameState = Manager.GameStateEnum.EditMode;
    }

    private void CalcNextCellState()
    { 
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                int neighbours = _cells[i, j].CountNeighbours(_cells);
                bool cellIsAlive = _cells[i, j].IsAlive;
                switch (neighbours)
                {
                    case 2:
                        _cells[i, j].NextState = Cell.NextStateEnum.NoChange;
                        break;
                    case 3:
                        _cells[i, j].NextState = !cellIsAlive ? Cell.NextStateEnum.Alive : Cell.NextStateEnum.NoChange;
                        break;
                    default:
                        _cells[i, j].NextState = cellIsAlive ? Cell.NextStateEnum.Dead : Cell.NextStateEnum.NoChange;
                        break;
                }
            }
        }
    }

    private void UpdateCells()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                
                switch (_cells[i, j].NextState)
                {
                    case Cell.NextStateEnum.Alive:
                        _cells[i, j].IsAlive = true;
                        break;
                    case Cell.NextStateEnum.Dead:
                        _cells[i, j].IsAlive = false;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private IEnumerator Run()
    {
        while (Manager.GameState == Manager.GameStateEnum.Running)
        {
            CalcNextCellState();
            UpdateCells();
            yield return new WaitForSeconds(DelayBetweenGenerations);
        }
        //_hasRunCoroutineFinished = true;
    }
}
