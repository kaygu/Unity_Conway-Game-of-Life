using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Cell: MonoBehaviour
{
    [SerializeField] private Color AliveColor;
    [SerializeField] private Color DeadColor;

    private Index _me;
    private Index[] _neighbours;
    private const int _maxNeighbours = 8;

    private bool _isAlive;
    private int _state;
    private SpriteRenderer _sr;
    public NextStateEnum NextState;

    public void Init(int _x, int _y, int _width, int _height)
    {
        _sr = GetComponent<SpriteRenderer>();
        IsAlive = false;
        NextState = NextStateEnum.NoChange;
        _me = new Index { x = _x, y = _y };
        GenerateNeighbours(_x, _y, _width, _height);
    }


    public int CountNeighbours(Cell[,] cells)
    {
        return _neighbours.Sum(neigbour => cells[neigbour.x, neigbour.y].GetState());
    }

    private int GetState()
    {
        return _state;
    }

    public bool IsAlive
    {
        get 
        { 
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            _state = _isAlive ? 1 : 0;
            _sr.color = _isAlive ? AliveColor : DeadColor;
        }
    }


    private void OnMouseDown()
    {
        if (Manager.GameState == Manager.GameStateEnum.EditMode)
        {
            IsAlive = !IsAlive;
        }
    }

    private struct Index
    {
        internal int x;
        internal int y;
        public override string ToString()
        {
            return "Cell (" + x + "," + y +")";
        }
    }
    public enum NextStateEnum
    {
        NoChange,
        Dead,
        Alive
    }

    private void GenerateNeighbours(int x, int y, int width, int height)
    {
        // Generate neighbours, spacial cases for borders (5) & corners (3)
        if (x > 0 && y > 0 && x < width -1 && y < height -1)
        {
            _neighbours = new Index[_maxNeighbours];
            _neighbours[0] = new Index
            {
                // bottom left
                x = x - 1,
                y = y - 1
            };
            _neighbours[1] = new Index
            {
                // middle left
                x = x - 1,
                y = y
            };
            _neighbours[2] = new Index
            {
                // top left
                x = x - 1,
                y = y + 1
            };
            _neighbours[3] = new Index
            {
                // bottom middle
                x = x,
                y = y - 1
            };
            _neighbours[4] = new Index
            {
                // top middle
                x = x,
                y = y + 1
            };
            _neighbours[5] = new Index
            {
                // bottom right
                x = x + 1,
                y = y - 1
            };
            _neighbours[6] = new Index
            {
                // middle right
                x = x + 1,
                y = y
            };
            _neighbours[7] = new Index
            {
                // top right
                x = x + 1,
                y = y + 1
            };
        }
        else
        {
            _neighbours = new Index[0];
        }
        
    }

}
