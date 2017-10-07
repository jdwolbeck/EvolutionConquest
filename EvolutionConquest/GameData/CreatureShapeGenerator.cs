using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CreatureShapeGenerator
{
    public CreatureShapeGenerator()
    {
    }

    public List<Vector2> CreateCreature()
    {
        List<Vector2> verticies = new List<Vector2>();

        verticies.Add(new Vector2(-1, 1));
        verticies.Add(new Vector2(1, 1));
        verticies.Add(new Vector2(1, -1));
        verticies.Add(new Vector2(-1, -1));

        return verticies;
    }
    public Texture2D CreateCreatureTexture(GraphicsDevice device)
    {
        Texture2D texture;
        int IMAGE_WIDTH = 12;
        int IMAGE_HEIGHT = 12;
        //float alpha = 1f;

        texture = new Texture2D(device, IMAGE_WIDTH, IMAGE_HEIGHT);
        Color[] colors = new Color[IMAGE_WIDTH * IMAGE_HEIGHT];

        Color SPINE = Color.Black;

        List<Color> colorList = new List<Color>();
        //Layer1
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer2
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer3
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer4
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer5
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(SPINE);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer6
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(SPINE);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer7
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(SPINE);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer8
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(SPINE);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer9
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(SPINE);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer10
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.White);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        //Layer11
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Black);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        //Layer12
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);
        colorList.Add(Color.Transparent);

        colors = colorList.ToArray();

        texture.SetData(colors);

        return texture;
    }
}