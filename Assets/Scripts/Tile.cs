using UnityEngine;
using UnityEngine.Assertions;

internal class Tile
{
    private const float DEFAULT_WEIGHT = 1f;

    private int row;
    private int column;
    private SpriteRenderer spriteRenderer;
    private bool isPassable;
    private float weight;

    public Tile(int row, int column, SpriteRenderer spriteRenderer, bool isPassable = true, float weight = DEFAULT_WEIGHT)
    {
        this.Row = row;
        this.Column = column;
        this.SpriteRenderer = spriteRenderer;
        this.IsPassable = isPassable;
        this.Cost = weight;
    }

    public int Row
    {
        get
        {
            return this.row;
        }
        private set
        {
            Assert.IsTrue(value >= 0);
            this.row = value;
        }
    }

    public int Column
    {
        get
        {
            return this.column;
        }
        private set
        {
            Assert.IsTrue(value >= 0);
            this.column = value;
        }
    }

    private SpriteRenderer SpriteRenderer
    {
        get
        {
            return this.spriteRenderer;
        }
        set
        {
            Assert.IsNotNull(value);
            this.spriteRenderer = value;
        }
    }

    public bool IsPassable
    {
        get
        {
            return this.isPassable;
        }
        set
        {
            this.isPassable = value;
        }
    }

    public float Cost
    {
        get
        {
            return this.weight;
        }
        set
        {
            Assert.IsTrue(value >= 0);
            this.weight = value;
        }
    }

    public Color Color
    {
        get
        {
            return this.spriteRenderer.color;
        }
        set
        {
            this.spriteRenderer.color = value;
        }
    }
        
    public override string ToString()
    {
        var result = string.Format("({0}, {1})", row, column);

        return result;
    }

    public override bool Equals(object obj)
    {
        var tile = obj as Tile;

        if (tile == null)
        {
            return false;
        }

        if (!(this.Row == tile.Row && this.Column == tile.Column))
        {
            return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        // Using prime numbers to generate hash.
        var hash = ((row * 53 + column) * 91) * 127;

        return hash;
    }
}

