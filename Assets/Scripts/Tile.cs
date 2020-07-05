using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour, IComparable<Tile>
{
    private Text m_textComponent = null;

    [SerializeField]
    private Color m_passableColor = new Color(0.86f, 0.83f, 0.83f);

    [SerializeField]
    private Color m_unpassableColor = new Color(0.37f, 0.37f, 0.37f);

    [SerializeField]
    private bool m_isPassable = true;

    [SerializeField]
    private uint m_weight = 1;

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

    public bool IsPassable
    {
        get
        {
            return m_isPassable;
        }
        set
        {
            m_isPassable = value;
            this.Color = m_isPassable ? m_passableColor : m_unpassableColor;
        }
    }

    public uint Weight
    {
        get => m_weight;
        set => m_weight = value;
    }

    public int Row
    {
        get
        {
            int row = Convert.ToInt32(Mathf.Abs(this.transform.localPosition.y));

            return row;
        }
    }

    public int Column
    {
        get
        {
            int column = Convert.ToInt32(Mathf.Abs(this.transform.localPosition.x));

            return column;
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

    public void Awake()
    {
        this.TextComponent = this.GetComponentInChildren<Text>();
        this.IsPassable = m_isPassable;
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

