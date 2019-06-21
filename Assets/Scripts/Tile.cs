using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour, IComparable<Tile>
{
    private Text m_textComponent = null;

    private Text TextComponent
    {
        get
        {
            return m_textComponent;
        }
        set
        {
            Assert.IsNotNull(value);
            m_textComponent = value;
        }
    }

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

    public void Awake()
    {
        this.TextComponent = this.GetComponentInChildren<Text>();
    }

    public void SetText(string text)
    {
        Assert.IsNotNull(text);
        this.TextComponent.text = text;
    }

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

    public int CompareTo(Tile other)
    {
        int comparison = this.Weight.CompareTo(other.Weight);

        return comparison;
    }
}

