using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TileGrid : MonoBehaviour, IEnumerable<Tile>
{
    private Tile[,] m_grid = null;

    [SerializeField]
    private uint m_rows = 8;

    [SerializeField]
    private uint m_columns = 8;

    [SerializeField]
    private GameObject m_tilePrefab = null;

    public Tile this[int row, int column]
    {
        get
        {
            Assert.IsTrue(IsRowInRange(row), $"Invalid row: {row}");
            Assert.IsTrue(IsColumnInRange(column), $"Invalid column: {column}");

            return m_grid[row, column];
        }
        private set
        {
            Assert.IsTrue(IsRowInRange(row), $"Invalid row: {row}");
            Assert.IsTrue(IsColumnInRange(column), $"Invalid column: {column}");
            Assert.IsNotNull(value);

            m_grid[row, column] = value;
        }
    }

    public uint Rows => m_rows;

    public uint Columns => m_columns;

    public void Awake()
    {
        this.InitializeGrid();
    }

    public IEnumerator<Tile> GetEnumerator()
    {
        foreach (var tile in m_grid)
        {
            yield return tile;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public IEnumerable<Tile> GetNeighbors(int row, int column)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            //new Vector2Int(1, 1),
            new Vector2Int(0, 1),
            //new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),
            //new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            //new Vector2Int(1, -1)
        };

        var position = this[row, column].ToVector2Int();
        var neighbors = new List<Tile>(directions.Length);

        foreach (var direction in directions)
        {
            var currentPosition = position + direction;

            if (IsInRange(currentPosition.x, currentPosition.y))
            {
                neighbors.Add(this[currentPosition.x, currentPosition.y]);
            }
        }

        return neighbors;
    }

    public IEnumerable<Tile> GetNeighbors(Tile tile)
    {
        Assert.IsNotNull(tile);

        return this.GetNeighbors(tile.Row, tile.Column);
    }

    private void InitializeGrid()
    {
        Assert.IsNotNull(m_tilePrefab);

        m_grid = new Tile[Rows, Columns];

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                // Map the new square's vector coordinates to their positions in a matrix (XY plane).
                var position = new Vector2(this.transform.position.x + column, this.transform.position.y - row);

                var tilePrefabClone = Instantiate(m_tilePrefab, position, m_tilePrefab.transform.rotation, this.transform);
                var tile = tilePrefabClone.GetComponent<Tile>();

                this[row, column] = tile;
            }
        }
    }

    private bool IsInRange(int row, int column)
    {
        bool isRowValid = this.IsRowInRange(row);
        bool isColumnValid = this.IsColumnInRange(column);

        bool isInRange = isRowValid && isColumnValid;

        return isInRange;
    }

    private bool IsRowInRange(int row)
    {
        bool isInRange = (0 <= row && row < Rows);

        return isInRange;
    }

    private bool IsColumnInRange(int column)
    {
        bool isInRange = (0 <= column && column < Columns);

        return isInRange;
    }
}
