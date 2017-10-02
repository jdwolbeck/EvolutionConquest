using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameData
{
    public List<Creature> Creatures { get; set; } //List of creatures on the map
    public List<Creature> DeadCreatures { get; set; } //Used for writing stats at the end
    public List<Egg> Eggs { get; set; } //Eggs on the map
    public List<Food> Food { get; set; } //Food on the map
    public Creature Focus { get; set; } //Camera focus, the camera class will follow whatever Creature is selected here
    public int FocusIndex { get; set; } //Camera focus index, this value is used when Paging between Creatures
    public List<SpeciesToCount> ChartData { get; set; }
    public List<SpeciesToCount> ChartDataTop { get; set; }

    public GameData()
    {
        ChartData = new List<SpeciesToCount>();
        ChartDataTop = new List<SpeciesToCount>();
        Creatures = new List<Creature>();
        DeadCreatures = new List<Creature>();
        Eggs = new List<Egg>();
        Food = new List<Food>();
        Focus = null; //Init the focus to null to not follow any creatures
        FocusIndex = -1;
    }

    public int GetUniqueSpeciesCount()
    {
        return Creatures.Select(o => o.Species).Distinct().Count();
    }
    public void InitializeChartData()
    {
        //Initialization
        for (int i = 0; i < Creatures.Count; i++)
        {
            SpeciesToCount sc = new SpeciesToCount();
            sc.Name = Creatures[i].Species;
            sc.Id = Creatures[i].SpeciesId;
            sc.CountsOverTime.Add(1);

            ChartData.Add(sc);
        }
    }
    public void CalculateChartData()
    {
        if (ChartData.Count == 0)
        {
            InitializeChartData();
        }
        else
        {
            //Expand the CountsOverTime variable in ChartData
            foreach (SpeciesToCount sc in ChartData)
            {
                sc.CountsOverTime.Add(0);
            }

            List<SpeciesDistinct> newList = new List<SpeciesDistinct>();
            int preXcount = ChartData[ChartData.Count - 1].CountsOverTime.Count;
            foreach (Creature c in Creatures)
            {
                bool found = false;
                foreach (SpeciesToCount sc in ChartData)
                {
                    if (c.SpeciesId == sc.Id)
                    {
                        found = true;
                        sc.CountsOverTime[sc.CountsOverTime.Count - 1]++;
                        break;
                    }
                }
                if (!found) //New Species was introduced, we need to fill the data so the StackedArea chart does not complain
                {
                    SpeciesToCount sc = new SpeciesToCount();
                    sc.Name = c.Species;
                    sc.Id = c.SpeciesId;

                    ChartData.Add(sc);

                    for (int i = 0; i < preXcount; i++)
                    {
                        sc.CountsOverTime.Add(0); //Fill in all the rows first
                    }
                    sc.CountsOverTime[sc.CountsOverTime.Count - 1]++;
                }
            }

            ChartDataTop.Clear();
            List<SpeciesToCount> topFive = ChartData.Where(t => t.CountsOverTime[t.CountsOverTime.Count - 1] > 0).OrderByDescending(t => t.CountsOverTime[t.CountsOverTime.Count - 1]).ToList();
            int countToGet = topFive.Count();
            if (countToGet > 5)
                countToGet = 5;

            for (int i = 0; i < countToGet; i++)
            {
                ChartDataTop.Add(topFive[i]);
            }
        }
    }
}