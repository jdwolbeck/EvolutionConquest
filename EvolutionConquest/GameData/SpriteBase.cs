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
        }
    }
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }

    public SpriteBase()
    {
    }
}