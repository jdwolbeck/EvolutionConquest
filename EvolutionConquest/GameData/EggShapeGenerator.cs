using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EggShapeGenerator
{
    public EggShapeGenerator()
    { }

    public Texture2D CreateEggTexture(GraphicsDevice device, Color eggOutlineColor, Color eggInnerColor)
    {
        Texture2D texture;
        int IMAGE_WIDTH = 6;
        int IMAGE_HEIGHT = 6;

        texture = new Texture2D(device, IMAGE_WIDTH, IMAGE_HEIGHT);
        Color[] colors = new Color[IMAGE_WIDTH * IMAGE_HEIGHT];

        List<Color> colorList = new List<Color>();
        //Layer1
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        //Layer2
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        //Layer3
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggInnerColor);
        colorList.Add(eggInnerColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        //Layer4
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggInnerColor);
        colorList.Add(eggInnerColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        //Layer5
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        //Layer6
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);
        colorList.Add(eggOutlineColor);

        colors = colorList.ToArray();

        texture.SetData(colors);

        return texture;
    }
}