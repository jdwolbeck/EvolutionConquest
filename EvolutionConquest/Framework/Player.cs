using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Player
{
    public Player()
    {
    }

    public void HandleInput(InputState inputState, PlayerIndex? controllingPlayer, ref GameData gameData)
    {
        PlayerIndex playerIndex;

        if (inputState.IsNewKeyPress(Keys.F12, controllingPlayer, out playerIndex))
        {
            gameData.ShowChart = !gameData.ShowChart;
        }
    }
}