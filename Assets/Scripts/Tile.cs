using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    public int Row
    {
        get
        {
            return (int)Mathf.Abs(this.transform.localPosition.y);
        }
    }

    public int Column
    {
        get
        {
            return (int)Mathf.Abs(this.transform.localPosition.x);
        }
    }

    public Color Color
    {
        get
        {
            var spriteRenderer = this.GetComponent<SpriteRenderer>();
            Assert.IsNotNull(spriteRenderer);

            return spriteRenderer.color;
        }
        set
        {
            var spriteRenderer = this.GetComponent<SpriteRenderer>();
            Assert.IsNotNull(spriteRenderer);

            spriteRenderer.color = value;
        }
    }

    public uint Weight { get; set; } = 1;

    public bool IsPassable { get; set; } = true;

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int(this.Row, this.Column);
    }

    public override string ToString()
    {
        var coordinates = this.ToVector2Int().ToString();
        var weight = this.Weight.ToString();
        var result = $"{coordinates} : {weight}";

        return result;
    }
}

