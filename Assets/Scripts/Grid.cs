using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

internal class Grid : IEnumerable<Tile>
{
    private Tile[,] grid;

    public Grid(int rows, int columns, Transform parent, GameObject tilePrefab, Color tileColor)
    {
        this.grid = new Tile[rows, columns];

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                // Mapping the vector coordinates to their positions in a matrix.
                var position = new Vector2(parent.position.x + column, parent.position.y - row);

                var prefabClone = Object.Instantiate(tilePrefab, position, Quaternion.identity);
                Assert.IsNotNull(prefabClone);
                prefabClone.transform.parent = parent;

                var spriteRenderer = prefabClone.GetComponent<SpriteRenderer>();
                Assert.IsNotNull(spriteRenderer);
                spriteRenderer.color = tileColor;

                grid[row, column] = new Tile(row, column, spriteRenderer);
            }
        }

        //this.CreateUnpassableTiles();
    }

    public int Rows
    {
        get
        {
            return this.grid.GetLength(0);
        }
    }

    public int Columns
    {
        get
        {
            return this.grid.GetLength(1);
        }
    }

    public Tile this[int row, int column]
    {
        get
        {
            return this.grid[row, column];
        }
    }

    public IEnumerable<Tile> Tiles
    {
        get
        {
            var allTiles = new List<Tile>();

            foreach (var tile in this)
            {
                allTiles.Add(tile);
            }

            return allTiles;
        }
    }

    private void CreateUnpassableTiles()
    {
        // Unpassable tiles will be 1/5 of the total number of cells.
        int unpassableTiles = (this.Rows * this.Columns) / 5;

        for (int i = 0; i < unpassableTiles; i++)
        {
            var randomRow = Random.Range(0, Rows);
            var randomColumn = Random.Range(0, Columns);

            var randomTile = this.grid[randomRow, randomColumn];
            randomTile.IsPassable = false;
            randomTile.Color = Color.black;
        }
    }

    public IEnumerator<Tile> GetEnumerator()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                yield return this.grid[row, column];
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
