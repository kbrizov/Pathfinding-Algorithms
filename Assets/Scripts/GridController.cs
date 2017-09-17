using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

public class GridController : MonoBehaviour
{
    private const int DEFAULT_ROWS = 10;
    private const int DEFAULT_COLUMNS = 10;
    private const float DEFAULT_WAIT = 0.0f;

    [SerializeField]
    private int rows = DEFAULT_ROWS;

    [SerializeField]
    private int columns = DEFAULT_COLUMNS;

    [SerializeField]
    private GameObject tilePrefab = null;

    [SerializeField]
    private Color unvisitedTilesColor = new Color();

    [SerializeField]
    private Color visitedTilesColor = new Color();

    [SerializeField]
    private Color frontierColor = new Color();

    private Grid grid;

    protected virtual void Awake()
    {
        this.grid = new Grid(rows, columns, this.transform, tilePrefab, unvisitedTilesColor);
        InitializePathCosts(grid);
    }

    protected virtual void Start()
    {
        StartCoroutine(DisplayBreathFirstSearch(grid[11, 15], grid[0, 16]));
    }

    protected virtual void Update()
    {
    }

    //private IEnumerable FindCheapestPath(Tile startTile, Tile endTile)
    //{

    //}

    private IEnumerator DisplayBreathFirstSearch(Tile startTile, Tile endTile)
    {
        startTile.Color = Color.green;
        endTile.Color = Color.red;

        TileDictionary visitedTiles = new TileDictionary();
        visitedTiles.Add(startTile);

        Queue<Tile> frontier = new Queue<Tile>();
        frontier.Enqueue(startTile);

        Tile currentTile = null;
        while (frontier.Count > 0)
        {
            currentTile = frontier.Dequeue();

            if (currentTile != startTile && currentTile != endTile)
            {
                currentTile.Color = visitedTilesColor;
            }

            if (currentTile == endTile)
            {
                var path = this.GetPathTo(endTile, visitedTiles);
                path.RemoveFirst();
                path.RemoveLast();

                this.DisplayPath(path);

                yield break;
            }

            IEnumerable<Tile> tileNeighbors = ShuffleTiles(GetTileNeighbors(currentTile)).Where(tile => tile.IsPassable);
            foreach (var tile in tileNeighbors)
            {
                if (!visitedTiles.Contains(tile))
                {
                    frontier.Enqueue(tile);
                    visitedTiles.Add(tile, currentTile);

                    if (tile != endTile)
                    {
                        tile.Color = frontierColor;
                    }
                }
            }

            yield return new WaitForSeconds(DEFAULT_WAIT);
        }
    }

    private IEnumerator DisplayDepthFirstSearch(Tile startTile, Tile endTile)
    {
        startTile.Color = Color.green;
        endTile.Color = Color.red;

        TileDictionary visitedTiles = new TileDictionary();
        visitedTiles.Add(startTile);

        Stack<Tile> frontier = new Stack<Tile>();
        frontier.Push(startTile);

        Tile currentTile = null;
        while (frontier.Count > 0)
        {
            currentTile = frontier.Pop();

            if (currentTile != startTile && currentTile != endTile)
            {
                currentTile.Color = visitedTilesColor;
            }

            if (currentTile == endTile)
            {
                var path = this.GetPathTo(endTile, visitedTiles);
                path.RemoveFirst();
                path.RemoveLast();

                this.DisplayPath(path);

                yield break;
            }

            IEnumerable<Tile> tileNeighbors = ShuffleTiles(GetTileNeighbors(currentTile)).Where(tile => tile.IsPassable);
            foreach (var tile in tileNeighbors)
            {
                if (!visitedTiles.Contains(tile) && !frontier.Contains(tile))
                {
                    frontier.Push(tile);
                    visitedTiles.Add(tile, currentTile);

                    if (tile != endTile)
                    {
                        tile.Color = frontierColor;
                    }
                }
            }

            yield return new WaitForSeconds(DEFAULT_WAIT);
        }
    }

    private void DisplayPath(LinkedList<Tile> path)
    {
        foreach (var tile in path)
        {
            tile.Color = Color.yellow;
        }
    }

    private IEnumerator DisplayBreathFirstTraversal(Tile startTile)
    {
        var visitedTiles = new HashSet<Tile>();
        visitedTiles.Add(startTile);

        var frontier = new Queue<Tile>();
        frontier.Enqueue(startTile);

        while (frontier.Count > 0)
        {
            var currentTile = frontier.Dequeue();
            currentTile.Color = visitedTilesColor;

            var tileNeighbors = GetTileNeighbors(currentTile);
            foreach (var tile in tileNeighbors)
            {
                if (!visitedTiles.Contains(tile))
                {
                    frontier.Enqueue(tile);
                    visitedTiles.Add(tile);
                    tile.Color = frontierColor;
                }
            }

            yield return new WaitForSeconds(DEFAULT_WAIT);
        }
    }

    private IEnumerator DisplayDepthFirstTraversal(Tile startTile)
    {
        var visitedTiles = new HashSet<Tile>();
        visitedTiles.Add(startTile);

        var frontier = new Stack<Tile>();
        frontier.Push(startTile);

        while (frontier.Count > 0)
        {
            var currentTile = frontier.Pop();
            currentTile.Color = visitedTilesColor;

            var tileNeighbors = GetTileNeighbors(currentTile);
            foreach (var tile in tileNeighbors)
            {
                if (!visitedTiles.Contains(tile))
                {
                    frontier.Push(tile);
                    visitedTiles.Add(tile);
                    tile.Color = frontierColor;
                }
            }

            yield return new WaitForSeconds(DEFAULT_WAIT);
        }
    }

    private LinkedList<Tile> GetPathTo(Tile end, TileDictionary visitedTiles)
    {
        var path = new LinkedList<Tile>();

        var current = end;
        var previous = visitedTiles.GetPrevious(current);

        while (previous != null)
        {
            path.AddFirst(current);

            current = previous;
            previous = visitedTiles.GetPrevious(current);
        }

        path.AddFirst(current);

        return path;
    }

    private IList<Tile> GetTileNeighbors(Tile tile)
    {
        return this.GetTileNeighbors(tile.Row, tile.Column);
    }

    private IList<Tile> GetTileNeighbors(int row, int column)
    {
        var neighbors = new List<Tile>();

        bool canGetUpperNeighbor = CanGetTile(row - 1, column);
        //bool canGetUpperRightNeighbor = CanGetTile(row - 1, column + 1);
        bool canGetRightNeighbor = CanGetTile(row, column + 1);
        //bool canGetLowerRightNeighbor = CanGetTile(row + 1, column + 1); //
        bool canGetLowerNeighbor = CanGetTile(row + 1, column);
        //bool canGetLowerLeftNeighbor = CanGetTile(row + 1, column - 1); //
        bool canGetLeftNeighbor = CanGetTile(row, column - 1);
        //bool canGetUpperLeftNeighbor = CanGetTile(row - 1, column - 1); //

        if (canGetUpperNeighbor)
        {
            var upperNeighbor = grid[row - 1, column];
            neighbors.Add(upperNeighbor);
        }

        //if (canGetUpperRightNeighbor) //
        //{
        //    var upperRightNeighbor = grid[row - 1, column + 1];
        //    neighbors.Add(upperRightNeighbor);
        //}

        if (canGetRightNeighbor)
        {
            var rightNeighbor = grid[row, column + 1];
            neighbors.Add(rightNeighbor);
        }

        //if (canGetLowerRightNeighbor) //
        //{
        //    var lowerRightNeighbor = grid[row + 1, column + 1];
        //    neighbors.Add(lowerRightNeighbor);
        //}

        if (canGetLowerNeighbor)
        {
            var lowerNeighbor = grid[row + 1, column];
            neighbors.Add(lowerNeighbor);
        }

        //if (canGetLowerLeftNeighbor) //
        //{
        //    var lowerLeftNeighbor = grid[row + 1, column - 1];
        //    neighbors.Add(lowerLeftNeighbor);
        //}

        if (canGetLeftNeighbor)
        {
            var leftNeighbor = grid[row, column - 1];
            neighbors.Add(leftNeighbor);
        }

        //if (canGetUpperLeftNeighbor) //
        //{
        //    var upperLeftNeighbor = grid[row - 1, column - 1];
        //    neighbors.Add(upperLeftNeighbor);
        //}

        return neighbors;
    }

    private IList<Tile> ShuffleTiles(IList<Tile> tiles)
    {
        // Fisher-Yates shuffle algorithm.
        var count = tiles.Count;
        for (var index = 0; index < count; index++)
        {
            int randomIndex = index + Random.Range(0, count - index);
            var temp = tiles[index];
            tiles[index] = tiles[randomIndex];
            tiles[randomIndex] = temp;
        }

        return tiles;
    }

    private Dictionary<Tile, float> InitializePathCosts(Grid grid)
    {
        var costs = new Dictionary<Tile, float>();

        foreach (var tile in grid)
        {
            costs.Add(tile, float.PositiveInfinity);
        }

        return costs;
    }

    private bool CanGetTile(int row, int column)
    {
        int minIndex = 0;
        int maxRowIndex = this.grid.Rows - 1;
        int maxColumnIndex = this.grid.Columns - 1;

        if (row < minIndex || row > maxRowIndex)
        {
            return false;
        }

        if (column < minIndex || column > maxColumnIndex)
        {
            return false;
        }

        return true;
    }

    private void PositionGridAccordingToCamera()
    {
        Vector3 tileSize = this.tilePrefab.GetComponent<SpriteRenderer>().sprite.bounds.size;
        Vector3 cameraTileOffset = new Vector3(tileSize.x, -tileSize.y) / 2f;

        Vector3 gridTopLeft = this.transform.position;
        Vector3 gridBottomRight = new Vector3(columns, -rows);
        Vector3 gridCenter = (gridTopLeft + gridBottomRight) / 2;

        Vector3 cameraCenter = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0f);
        Vector3 cameraPositionRelativeToGridCenter = (cameraCenter - gridCenter) + cameraTileOffset;

        this.transform.position += cameraPositionRelativeToGridCenter;
    }

    private class TileDictionary
    {
        private Dictionary<Tile, Tile> dictionary;

        public TileDictionary()
        {
            this.dictionary = new Dictionary<Tile, Tile>();
        }

        public Tile this[Tile tile]
        {
            get
            {
                return this.dictionary[tile];
            }
            set
            {
                this.dictionary[tile] = value;
            }
        }

        public IEnumerable<Tile> Tiles
        {
            get
            {
                return this.dictionary.Keys;
            }
        }

        public void Add(Tile tile, Tile previous = null)
        {
            this.dictionary.Add(tile, previous);
        }

        public bool Remove(Tile tile)
        {
            bool isRemoved = this.dictionary.Remove(tile);

            return isRemoved;
        }

        public bool Contains(Tile tile)
        {
            bool isNodeContained = this.dictionary.ContainsKey(tile);

            return isNodeContained;
        }

        public Tile GetPrevious(Tile tile)
        {
            return this[tile];
        }
    }
}
