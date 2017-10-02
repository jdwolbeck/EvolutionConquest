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
            TextureCollideDistance = _texture.Width + _texture.Height;
        }
    }
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }
    public Rectangle Bounds { get; set; } //Food will auto set this but Creature will not for performance reasons.
    public int TextureCollideDistance { get; set; } 

    public SpriteBase()
    {
    }

    public void CalculateBounds()
    {
        Bounds = new Rectangle((int)(Position.X - (Texture.Width / 2)), (int)(Position.Y - (Texture.Height / 2)), Texture.Width, Texture.Height);
    }
}