using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SpriteBase
{
    public Texture2D Texture { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }

    public SpriteBase()
    {
    }
}