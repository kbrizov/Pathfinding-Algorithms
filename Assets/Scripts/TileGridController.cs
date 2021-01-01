using System;
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
    private Color m_expensiveAreaColor = new Color(0.19f, 0.65f, 0.43f);

    [SerializeField]
    private Color m_pathColor = new Color(0.73f, 0.0f, 1.0f);

    [SerializeField]
    private Color m_visitedTilesColor = new Color(0.75f, 0.55f, 0.38f);

    [SerializeField]
    private Color m_frontierColor = new Color(0.4f, 0.53f, 0.8f);

    [SerializeField]
    private TileGrid m_grid = null;

    protected virtual void Start()
    {
        Assert.IsNotNull(m_grid);

        var start = m_grid[9, 3];
        var end = m_grid[3, 33];

        this.CreateExpensiveArea(m_grid[0, 15], m_grid[18, 20], 10);

        this.StartCoroutine(DisplayAStarSearch(start, end, CalculateEuclideanDistance));
    }

    private IEnumerator DisplayDepthFirstSearch(Tile start, Tile end)
    {
        Assert.IsNotNull(m_grid);
        Assert.IsNotNull(start);
        Assert.IsNotNull(end);

        start.Color = Color.green;
        end.Color = Color.red;

        var visited = new Dictionary<Tile, Tile>();
        visited.Add(start, null);

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
                this.DisplayPath(start, end, visited);

                yield break;
            }

            var neighbors = ShuffleTiles(m_grid.GetNeighbors(current).Where(tile => tile.IsPassable).ToList());
            foreach (var tile in neighbors)
            {
                if (!visited.ContainsKey(tile))
                {
                    frontier.Push(tile);
                    visited.Add(tile, current);

                    if (tile != end)
                    {
                        tile.Color = m_frontierColor;
                    }
                }
            }

            yield return new WaitForSeconds(DEFAULT_WAIT);
        }
    }

    private IEnumerator DisplayBreathFirstSearch(Tile start, Tile end)
    {
        Assert.IsNotNull(m_grid);
        Assert.IsNotNull(start);
        Assert.IsNotNull(end);

        start.Color = m_startColor;
        end.Color = m_endColor;

        var visited = new Dictionary<Tile, Tile>();
        visited.Add(start, null);

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
                this.DisplayPath(start, end, visited);

                yield break;
            }

            var neighbors = m_grid.GetNeighbors(current).Where(tile => tile.IsPassable);
            foreach (var tile in neighbors)
            {
                if (!visited.ContainsKey(tile))
                {
                    frontier.Enqueue(tile);
                    visited.Add(tile, current);

                    if (tile != end)
                    {
                        tile.Color = m_frontierColor;
                    }
                }
            }

            yield return new WaitForSeconds(DEFAULT_WAIT);
        }
    }

    private IEnumerator DisplayUniformCostSearch(Tile start, Tile end)
    {
        // Uniform Cost Search = Dijkstra Search with specific (start, end).

        Assert.IsNotNull(m_grid);
        Assert.IsNotNull(start);
        Assert.IsNotNull(end);

        start.Color = m_startColor;
        start.SetText("0");

        end.Color = m_endColor;
        end.SetText("X");

        var costs = InitializePathCosts(m_grid);
        costs[start] = 0.0f;

        var visited = new Dictionary<Tile, Tile>();
        visited.Add(start, null);

        var frontier = new PriorityQueue<Tile>((a, b) => costs[a].CompareTo(costs[b])); // Stable priority queue.
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
                this.DisplayPath(start, end, visited);

                yield break;
            }

            var neighbors = m_grid.GetNeighbors(current).Where(tile => tile.IsPassable);
            foreach (var tile in neighbors)
            {
                var currentCost = costs[tile];
                var newCost = costs[current] + tile.Weight;

                if (newCost < currentCost)
                {
                    costs[tile] = newCost;

                    if (visited.ContainsKey(tile))
                    {
                        visited[tile] = current;
                    }

                    tile.SetText(newCost.ToString());
                }

                if (!visited.ContainsKey(tile))
                {
                    frontier.Enqueue(tile);
                    visited.Add(tile, current);

                    if (tile != end)
                    {
                        tile.Color = m_frontierColor;
                    }
                }
            }

            yield return new WaitForSeconds(DEFAULT_WAIT);
        }
    }

    private IEnumerator DisplayBestFirstSearch(Tile start, Tile end, Func<Tile, Tile, float> heuristic)
    {
        Assert.IsNotNull(m_grid);
        Assert.IsNotNull(start);
        Assert.IsNotNull(end);

        start.Color = m_startColor;
        start.SetText("0");

        end.Color = m_endColor;
        end.SetText("X");

        var costs = InitializePathCosts(m_grid);
        costs[start] = 0.0f;

        Comparison<Tile> heuristicComparison = (a, b) =>
        {
            var aPriority = heuristic(a, end);
            var bPriority = heuristic(b, end);

            return aPriority.CompareTo(bPriority);
        };

        var visited = new Dictionary<Tile, Tile>();
        visited.Add(start, null);

        var frontier = new PriorityQueue<Tile>(heuristicComparison); // Stable priority queue.
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
                this.DisplayPath(start, end, visited);

                yield break;
            }

            var neighbors = m_grid.GetNeighbors(current).Where(tile => tile.IsPassable);
            foreach (var tile in neighbors)
            {
                var currentCost = costs[tile];
                var newCost = costs[current] + tile.Weight;

                if (newCost < currentCost)
                {
                    costs[tile] = newCost;

                    if (visited.ContainsKey(tile))
                    {
                        visited[tile] = current;
                    }

                    tile.SetText(newCost.ToString());
                }

                if (!visited.ContainsKey(tile))
                {
                    frontier.Enqueue(tile);
                    visited.Add(tile, current);

                    if (tile != end)
                    {
                        tile.Color = m_frontierColor;
                    }
                }
            }

            yield return new WaitForSeconds(DEFAULT_WAIT);
        }
    }

    private IEnumerator DisplayAStarSearch(Tile start, Tile end, Func<Tile, Tile, float> heuristic)
    {
        Assert.IsNotNull(m_grid);
        Assert.IsNotNull(start);
        Assert.IsNotNull(end);

        start.Color = m_startColor;
        start.SetText("0");

        end.Color = m_endColor;
        end.SetText("X");

        var costs = InitializePathCosts(m_grid);
        costs[start] = 0.0f;

        Comparison<Tile> heuristicComparison = (a, b) =>
        {
            var aPriority = costs[a] + heuristic(a, end);
            var bPriority = costs[b] + heuristic(b, end);

            return aPriority.CompareTo(bPriority);
        };

        var visited = new Dictionary<Tile, Tile>();
        visited.Add(start, null);

        var frontier = new PriorityQueue<Tile>(heuristicComparison); // Stable priority queue.
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
                this.DisplayPath(start, end, visited);

                yield break;
            }

            var neighbors = m_grid.GetNeighbors(current).Where(tile => tile.IsPassable);
            foreach (var tile in neighbors)
            {
                var currentCost = costs[tile];
                var newCost = costs[current] + tile.Weight;

                if (newCost < currentCost)
                {
                    costs[tile] = newCost;

                    if (visited.ContainsKey(tile))
                    {
                        visited[tile] = current;
                    }

                    tile.SetText(newCost.ToString());
                }

                if (!visited.ContainsKey(tile))
                {
                    frontier.Enqueue(tile);
                    visited.Add(tile, current);

                    if (tile != end)
                    {
                        tile.Color = m_frontierColor;
                    }
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

    private IEnumerator DisplayDijkstraAlgorithm(Tile start)
    {
        Assert.IsNotNull(m_grid);
        Assert.IsNotNull(start);

        var costs = InitializePathCosts(m_grid);
        costs[start] = 0.0f;

        var visited = new HashSet<Tile>();
        visited.Add(start);

        var frontier = new PriorityQueue<Tile>((a, b) => costs[a].CompareTo(costs[b])); // Stable priority queue.
        frontier.Enqueue(start);

        start.Color = m_startColor;
        start.SetText("0");

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            if (current != start)
            {
                current.Color = m_visitedTilesColor;
            }

            var neighbors = m_grid.GetNeighbors(current);
            foreach (var tile in neighbors)
            {
                var currentCost = costs[tile];
                var newCost = costs[current] + tile.Weight;

                if (newCost < currentCost)
                {
                    costs[tile] = newCost;

                    tile.SetText(newCost.ToString());
                }

                if (!visited.Contains(tile))
                {
                    frontier.Enqueue(tile);
                    visited.Add(tile);

                    tile.Color = m_frontierColor;
                }
            }

            yield return new WaitForSeconds(DEFAULT_WAIT);
        }
    }

    private void CreateExpensiveArea(Tile topLeft, Tile bottomRight, uint weight)
    {
        Assert.IsNotNull(topLeft);
        Assert.IsNotNull(bottomRight);

        int width = Mathf.Abs(topLeft.Column - bottomRight.Column);
        int height = Mathf.Abs(topLeft.Row - bottomRight.Row);

        int startRow = topLeft.Row;
        int endRow = topLeft.Row + height;

        int startColumn = topLeft.Column;
        int endColumn = topLeft.Column + width;

        for (int row = startRow; row <= endRow; row++)
        {
            for (int column = startColumn; column <= endColumn; column++)
            {
                var tile = m_grid[row, column];
                tile.Weight = weight;
                tile.Color = m_expensiveAreaColor;
            }
        }
    }

    private void DisplayPath(Tile start, Tile end, IDictionary<Tile, Tile> visitedTiles)
    {
        Assert.IsNotNull(start);
        Assert.IsNotNull(end);
        Assert.IsNotNull(visitedTiles);

        int count = 0;
        var path = this.GetPathTo(end, visitedTiles);

        foreach (var tile in path)
        {
            tile.SetText(count.ToString());
            count++;

            if (tile == start)
            {
                tile.Color = m_startColor;
            }
            else if (tile == end)
            {
                tile.Color = m_endColor;
            }
            else
            {
                tile.Color = m_pathColor;
            }
        }
    }

    private IEnumerable<Tile> GetPathTo(Tile end, IDictionary<Tile, Tile> visitedTiles)
    {
        Assert.IsNotNull(end);
        Assert.IsNotNull(visitedTiles);

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
        Assert.IsNotNull(tiles);

        // Fisher-Yates shuffle algorithm.
        var count = tiles.Count;
        for (var index = 0; index < count; index++)
        {
            int randomIndex = index + UnityEngine.Random.Range(0, count - index);
            var temp = tiles[index];
            tiles[index] = tiles[randomIndex];
            tiles[randomIndex] = temp;
        }

        return tiles;
    }

    private float CalculateManhattanDistance(Tile a, Tile b)
    {
        Assert.IsNotNull(a);
        Assert.IsNotNull(b);

        float manhattanDistance = Mathf.Abs(a.Row - b.Row) + Mathf.Abs(a.Column - b.Column);

        return manhattanDistance;
    }

    private float CalculateEuclideanDistance(Tile a, Tile b)
    {
        Assert.IsNotNull(a);
        Assert.IsNotNull(b);

        float euclideanDistance = Vector2Int.Distance(a.ToVector2Int(), b.ToVector2Int());

        return euclideanDistance;
    }

    private IDictionary<Tile, float> InitializePathCosts(TileGrid grid)
    {
        Assert.IsNotNull(grid);

        var costs = new Dictionary<Tile, float>();

        foreach (var tile in grid)
        {
            costs.Add(tile, float.PositiveInfinity);
        }

        return costs;
    }
}
