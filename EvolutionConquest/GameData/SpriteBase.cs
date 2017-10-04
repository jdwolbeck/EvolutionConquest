using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SpriteBase
{
    private Texture2D _texture;
    private Vector2 _position;
    private Rectangle _bounds;
    private List<Point> _gridPositions;

    public Texture2D Texture
    {
        get
        {
            return _texture;
        }
        set
        {
            _texture = value;
            Origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
            TextureCollideDistance = (int)Math.Ceiling(Math.Sqrt(_texture.Width * _texture.Width + _texture.Height * _texture.Height));
            CalculateBounds();
            _bounds.X = (int)(_position.X - (Texture.Width / 2));
            _bounds.Y = (int)(_position.Y - (Texture.Height / 2));
        }
    }
    public Vector2 Position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
            if (Texture != null)
            {
                _bounds.X = (int)(_position.X - (Texture.Width / 2));
                _bounds.Y = (int)(_position.Y - (Texture.Height / 2));
            }
        }
    }
    public Vector2 Origin { get; set; }
    public Rectangle Bounds
    { 
        get
        {
            return _bounds;
        }
        set
        {
            _bounds = value;
        }
    }
    public int TextureCollideDistance { get; set; } 
    public List<Point> GridPositions
    {
        get
        {
            return _gridPositions;
        }
        set
        {
            OldGridPositions = _gridPositions;
            _gridPositions = value;
        }
    } //The list of grid positions for the creature
    public List<Point> OldGridPositions { get; set; } //The list of grid positions for the creature
    public string CurrentGridPositionsForCompare { get; set; }
    public string OldGridPositionsForCompare { get; set; }

    public SpriteBase()
    {
        _position = Vector2.Zero;
        GridPositions = new List<Point>();
        OldGridPositions = new List<Point>();
    }

    public void CalculateBounds()
    {
        Bounds = new Rectangle((int)(Position.X - (Texture.Width / 2)), (int)(Position.Y - (Texture.Height / 2)), Texture.Width, Texture.Height);
    }
    public Vector2 CalculateGridPositionVector(int cellSize)
    {
        Vector2 pos = new Vector2();

        //Divide the position by the cell size then cast to int which does the same as Math.Floor. 
        //Multiply by the cell size to find the upper left cell corner. 
        //This only tells you the cell the object is in centered over not accounting for texture height/width
        pos.X = (int)(Position.X / cellSize) * cellSize;
        pos.Y = (int)(Position.Y / cellSize) * cellSize;

        return pos;
    }
    public Point CalculateGridPosition(int cellSize)
    {
        Point pos = new Point();

        //Divide the position by the cell size then cast to int which does the same as Math.Floor. 
        //Multiply by the cell size to find the upper left cell corner. 
        //This only tells you the cell the object is in centered over not accounting for texture height/width
        pos.X = (int)(Position.X / cellSize);
        pos.Y = (int)(Position.Y / cellSize);

        return pos;
    }
    public void SetOldGridPos(string s)
    {
        OldGridPositionsForCompare = CurrentGridPositionsForCompare;
        CurrentGridPositionsForCompare = s;
    }
    public List<Point> GetGridDelta()
    {
        List<Point> delta = new List<Point>();

        foreach (Point p in GridPositions)
        {
            bool found = false;

            foreach (Point o in OldGridPositions)
            {
                if (p.X == o.X && p.Y == o.Y)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                delta.Add(p);
            }
        }

        return delta;
    }
}