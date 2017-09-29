using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameData
{
    public List<Creature> Creatures { get; set; } //List of creatures on the map
    public List<Egg> Eggs { get; set; } //Eggs on the map
    public List<Food> Food { get; set; } //Food on the map
    public Creature Focus { get; set; } //Camera focus, the camera class will follow whatever Creature is selected here

    public GameData()
    {
        Creatures = new List<Creature>();
        Eggs = new List<Egg>();
        Food = new List<Food>();
        Focus = null; //Init the focus to null to not follow any creatures
    }
}