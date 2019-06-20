using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class TileGridController : MonoBehaviour
{
    private const float DEFAULT_WAIT = 0.0f;

    [SerializeField]
    private Color m_startColor = Color.green;

    [SerializeField]
    private Color m_endColor = Color.red;

    [SerializeField]
    private Color m_pathColor = new Color(0.73f, 0.0f, 1.0f);

    [SerializeField]
    private Color m_visitedTilesColor = new Color(0.8f, 0.75f, 0.7f);

    [SerializeField]
    private Color m_frontierColor = new Color(0.4f, 0.53f, 0.8f);

    [SerializeField]
    private TileGrid m_grid = null;

    protected virtual void Start()
    {
        Assert.IsNotNull(m_grid);

        var start = m_grid[11, 15];
        var end = m_grid[0, 23];

        StartCoroutine(DisplayDepthFirstSearch(start, end));
    }

    private IEnumerator DisplayBreathFirstSearch(Tile start, Tile end)
    {
        Assert.IsNotNull(m_grid);
        Assert.IsNotNull(start);
        Assert.IsNotNull(end);

        start.Color = m_startColor;
        end.Color = m_endColor;

        var visitedTiles = new Dictionary<Tile, Tile>();
        visitedTiles.Add(start, null);

        var frontier = new Queue<Tile>();
        frontier.Enqueue(start);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current != start && current != end)
            {
                current.Color = m_visitedTilesColor;
            }

            if (current == end)
            {
                var path = new LinkedList<Tile>(this.GetPathTo(end, visitedTiles));
                path.RemoveFirst();
                path.RemoveLast();

                this.DisplayPath(path);

                yield break;
            }

            var neighbors = m_grid.GetNeighbors(current).Where(tile => tile.IsPassable);
            foreach (var tile in neighbors)
            {
                if (!visitedTiles.ContainsKey(tile))
                {
                    frontier.Enqueue(tile);
                    visitedTiles.Add(tile, current);

                    if (tile != end)
                    {
                        tile.Color = m_frontierColor;
                    }
                }
            }

            yield return new WaitForSeconds(DEFAULT_WAIT);
        }
    }

    private IEnumerator DisplayDepthFirstSearch(Tile start, Tile end)
    {
        Assert.IsNotNull(m_grid);
        Assert.IsNotNull(start);
        Assert.IsNotNull(end);

        start.Color = Color.green;
        end.Color = Color.red;

        var visitedTiles = new Dictionary<Tile, Tile>();
        visitedTiles.Add(start, null);

        var frontier = new Stack<Tile>();
        frontier.Push(start);

        while (frontier.Count > 0)
        {
            var current = frontier.Pop();

            if (current != start && current != end)
            {
                current.Color = m_visitedTilesColor;
            }

            if (current == end)
            {
                var path = new LinkedList<Tile>(this.GetPathTo(end, visitedTiles));
                path.RemoveFirst();
                path.RemoveLast();

                this.DisplayPath(path);

                yield break;
            }

            var neighbors = ShuffleTiles(m_grid.GetNeighbors(current).Where(tile => tile.IsPassable).ToList());
            foreach (var tile in neighbors)
            {
                if (!visitedTiles.ContainsKey(tile))
                {
                    frontier.Push(tile);
                    visitedTiles.Add(tile, current);

                    if (tile != end)
                    {
                        tile.Color = m_frontierColor;
                    }
                }
            }

            yield return new WaitForSeconds(DEFAULT_WAIT);
        }
    }

    private IEnumerator DisplayBreathFirstTraversal(Tile start)
    {
        Assert.IsNotNull(m_grid);
        Assert.IsNotNull(start);

        start.Color = m_startColor;

        var visitedTiles = new HashSet<Tile>();
        visitedTiles.Add(start);

        var frontier = new Queue<Tile>();
        frontier.Enqueue(start);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            current.Color = m_visitedTilesColor;

            var neighbors = m_grid.GetNeighbors(current);
            foreach (var tile in neighbors)
            {
                if (!visitedTiles.Contains(tile))
                {
                    frontier.Enqueue(tile);
                    visitedTiles.Add(tile);
                    tile.Color = m_frontierColor;
                }
            }

            yield return new WaitForSeconds(DEFAULT_WAIT);
        }
    }

    private IEnumerator DisplayDepthFirstTraversal(Tile start)
    {
        Assert.IsNotNull(m_grid);
        Assert.IsNotNull(start);

        start.Color = m_startColor;

        var visitedTiles = new HashSet<Tile>();
        visitedTiles.Add(start);

        var frontier = new Stack<Tile>();
        frontier.Push(start);

        while (frontier.Count > 0)
        {
            var current = frontier.Pop();

            if (current != start)
            {
                current.Color = m_visitedTilesColor;
            }

            var neighbors = ShuffleTiles(m_grid.GetNeighbors(current).Where(tile => tile.IsPassable).ToList());
            foreach (var tile in neighbors)
            {
                if (!visitedTiles.Contains(tile))
                {
                    frontier.Push(tile);
                    visitedTiles.Add(tile);
                    tile.Color = m_frontierColor;
                }
            }

            yield return new WaitForSeconds(DEFAULT_WAIT);
        }
    }

    private void DisplayPath(IEnumerable<Tile> path)
    {
        Assert.IsNotNull(path);

        foreach (var tile in path)
        {
            tile.Color = m_pathColor;
        }
    }

    private IEnumerable<Tile> GetPathTo(Tile end, IDictionary<Tile, Tile> visitedTiles)
    {
        var path = new LinkedList<Tile>();

        var current = end;
        var previous = visitedTiles[current];

        while (previous != null)
        {
            path.AddFirst(current);

            current = previous;
            previous = visitedTiles[current];
        }

        path.AddFirst(current);

        return path;
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

    private IDictionary<Tile, float> InitializePathCosts(TileGrid grid)
    {
        var costs = new Dictionary<Tile, float>();

        foreach (var tile in grid)
        {
            costs.Add(tile, float.PositiveInfinity);
        }

        return costs;
    }
}
