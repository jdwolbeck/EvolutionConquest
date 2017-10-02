using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Creature : SpriteBase
{
    private Vector2 _direction;
    private float _rotation;
    int _undigestedFood;

    /// <summary>
    /// Adding new Properties make sure to Add to the following: Init, LayEgg, IsSame
    /// </summary>

    public bool IsAlive { get; set; }
    public int SpeciesId { get; set; } //Unique ID for species
    public string Species { get; set; } //Random name assigned to specied
    public string SpeciesStrain { get; set; } //This will keep track of specific strains within a species
    public string BabySpeciesStrainCounter { get; set; } //This is used for assigning babies with their strain
    public int Generation { get; set; } //How many generations since the first creature
    public Vector3 WorldPosition { get; set; } //Projected position on the world. Store this here to save processing
    public Vector2 Direction
    {
        get { return _direction; }
        set
        {
            _direction = value;
        }
    }
    public float Rotation
    {
        get { return _rotation; }
        set
        {
            _rotation = value;
            Direction = new Vector2((float)Math.Cos(Rotation - MathHelper.ToRadians(90)), (float)Math.Sin(Rotation - MathHelper.ToRadians(90)));
            Direction.Normalize();
        }
    }
    public Vector2[] Verticies { get; set; } //Design used to draw the creature
    public int UndigestedFood
    {
        get { return _undigestedFood; }
        set
        {
            _undigestedFood = value;
            TotalFoodEaten++;
        }
    } //Food count waiting to be digested
    public int DigestedFood { get; set; } //Food count that has been digested
    public float FoodDigestion { get; set; } //How quickly food can be digested and converted into an egg
    public int TotalFoodEaten { get; set; } //Statistical count of how many food were eaten
    public int TicksSinceLastEgg { get; set; } //The amount of Game Ticks since the last egg was created
    public int TicksSinceLastDigestedFood { get; set; } //The amount of Game Ticks since the last food was digested
    public float EggInterval { get; set; } //How ofter an egg can be output
    public float EggIncubation { get; set; } //How long it takes for the egg to hatch once created
    public float EggCamo { get; set; } //How well the egg is hidden from Scavengers
    public float EggToxicity { get; set; } //How toxic the egg is to other creatures
    public int EggsCreated { get; set; }
    public float Speed { get; set; } //How quickly the creature moved through the world
    public float Lifespan { get; set; } //How long the creature lives
    public float Energy { get; set; } //Energy is spent by moving and earned by eating
    public float ElapsedTicks { get; set; } //How many ticks the creature has been alive
    public float Sight { get; set; } //Allows the creature to adjust path if target is within this many units
    public float Attraction { get; set; }
    public float Herbavore { get; set; } //This is mainly used to create a Herbavore to Carnivore ratio to determine when the creature has become a Carnivore
    public float Carnivore { get; set; } //Can eat other creatures with Carnivore level of (Carnivore lvl / 2 - 5) or less. This will be the only means of food
    public float Omnivore { get; set; } //Can eat both creatures and plants
    public float Scavenger { get; set; } //Can eat other creatures eggs including Scavenger eggs. This will be the only means of food
    public float Camo { get; set; } //Hidden from all creatures with a Camo level less than your level
    public float Cloning { get; set; } //Chance for Spontaneous cloning to occur
    public float ColdClimateTolerance { get; set; } //How well the creature can handle cold parts of the map before needing to move to neutral
    public float HotClimateTolerance { get; set; } //How well the creature can handle hot parts of the map before needing to move to neutral

    public const float EGG_INTERVAL_INIT_MIN = 400;
    public const float EGG_INTERVAL_INIT_MAX = 800;
    public const float EGG_INCUBATION_INIT_MIN = 300;
    public const float EGG_INCUBATION_INIT_MAX = 800;
    public const float FOOD_DIGESTION_INIT_MIN = 50;
    public const float FOOD_DIGESTION_INIT_MAX = 250;
    public const float SPEED_INIT_MIN = 10;
    public const float SPEED_INIT_MAX = 45;
    public const float LIFESPAN_INIT_MIN = 1000;
    public const float LIFESPAN_INIT_MAX = 1200;
    public const float HERBAVORE_INIT_MIN = 3;
    public const float HERBAVORE_INIT_MAX = 20;
    public const float COLD_TOLERANCE_INIT_MIN = 0;
    public const float COLD_TOLERANCE_INIT_MAX = 10;
    public const float HOT_TOLERANCE_INIT_MIN = 0;
    public const float HOT_TOLERANCE_INIT_MAX = 5;
    public const float ENERGY_INIT = 500;

    public Creature()
    {
    }

    public void InitNewCreature(Random rand, Names names, int speciesId)
    {
        IsAlive = true;
        SpeciesId = speciesId;
        Species = names.GetRandomName(rand);
        SpeciesStrain = String.Empty;
        BabySpeciesStrainCounter = "A";
        Generation = 0;
        Rotation = MathHelper.ToRadians(rand.Next(0,360));
        UndigestedFood = 0;
        DigestedFood = 0;
        FoodDigestion = rand.Next((int)FOOD_DIGESTION_INIT_MIN, (int)FOOD_DIGESTION_INIT_MAX);
        TotalFoodEaten = 0;
        TicksSinceLastDigestedFood = 0;
        TicksSinceLastEgg = 0;
        EggInterval = rand.Next((int)EGG_INTERVAL_INIT_MIN, (int)EGG_INTERVAL_INIT_MAX);
        EggIncubation = rand.Next((int)EGG_INCUBATION_INIT_MIN,(int)EGG_INCUBATION_INIT_MAX);
        EggsCreated = 0;
        EggCamo = 0;
        EggToxicity = 0;
        Speed = rand.Next((int)SPEED_INIT_MIN, (int)SPEED_INIT_MAX);
        Lifespan = rand.Next((int)LIFESPAN_INIT_MIN, (int)LIFESPAN_INIT_MAX);
        Energy = ENERGY_INIT; //No mutations or variance on this
        ElapsedTicks = 0;
        Sight = 0;
        Attraction = 0;
        Herbavore = rand.Next((int)HERBAVORE_INIT_MIN, (int)HERBAVORE_INIT_MAX);
        Carnivore = 0;
        Scavenger = 0;
        Omnivore = 0;
        Camo = 0;
        Cloning = 0;
        ColdClimateTolerance = rand.Next((int)COLD_TOLERANCE_INIT_MIN, (int)COLD_TOLERANCE_INIT_MAX);
        HotClimateTolerance = rand.Next((int)HOT_TOLERANCE_INIT_MIN, (int)HOT_TOLERANCE_INIT_MAX);
    }
    public void AdvanceTick()
    {
        ElapsedTicks++;
        TicksSinceLastEgg++;
        if(UndigestedFood > 0) //only allow digestion once the a food has been eaten
            TicksSinceLastDigestedFood++;

        if (UndigestedFood > 0 && TicksSinceLastDigestedFood >= FoodDigestion)
        {
            TicksSinceLastDigestedFood = 0;
            UndigestedFood--;
            DigestedFood++;
        }
    }
    public Egg LayEgg(Random rand)
    {
        Egg egg = new Egg();
        Creature baby = new Creature();

        TicksSinceLastEgg = 0;
        EggsCreated++;

        baby.IsAlive = true;
        baby.Rotation = MathHelper.ToRadians(rand.Next(0, 360));
        baby.Position = Position;
        baby.SpeciesId = SpeciesId;
        baby.Species = Species;        
        BabySpeciesStrainCounter = Global.GetNextBase26(BabySpeciesStrainCounter);
        baby.BabySpeciesStrainCounter = "A";
        baby.Generation = Generation + 1;
        baby.UndigestedFood = 0;
        baby.DigestedFood = 0;
        baby.TotalFoodEaten = 0;
        baby.TicksSinceLastDigestedFood = 0;
        baby.TicksSinceLastEgg = 0;
        baby.ElapsedTicks = 0;

        //Mutations
        baby.EggCamo = EggCamo + Mutation(rand, 5);
        baby.EggIncubation = EggIncubation + (Mutation(rand, 25) * 10);
        baby.EggInterval = EggInterval + (Mutation(rand, 25) * 10);
        baby.EggToxicity = EggToxicity + Mutation(rand, 5);
        baby.FoodDigestion = FoodDigestion + (Mutation(rand, 25) * 10);
        baby.Speed = Speed + Mutation(rand, 10);
        baby.Lifespan = Lifespan + (Mutation(rand, 25) * 10);
        baby.Energy = Energy; //No mutation chance on energy
        baby.Sight = Sight + Mutation(rand, 3);
        baby.Attraction = Attraction + Mutation(rand, 3);
        baby.Camo = Camo + Mutation(rand, 3);
        baby.Cloning = Cloning + Mutation(rand, 3);
        baby.ColdClimateTolerance = ColdClimateTolerance + Mutation(rand, 10 - HotClimateTolerance);
        baby.HotClimateTolerance = HotClimateTolerance + Mutation(rand, 10 - ColdClimateTolerance);
        baby.Herbavore = Herbavore + Mutation(rand, 15);
        baby.Carnivore = Carnivore + Mutation(rand, 10);
        baby.Omnivore = Omnivore + Mutation(rand, 10);
        baby.Scavenger = Scavenger + Mutation(rand, 5);

        //Only iterate the Species/Strain if something that can Mutate has changed
        if (!IsSameAs(baby))
        {
            if (SpeciesStrain.Replace(" ", "").Length > 30) //New Species once the Strain goes past 30 different strains
            {
                int romanNumeralIndex = baby.Species.IndexOf(' ');
                if (romanNumeralIndex > 0) //Increment Roman numeral of we Detect a space
                {
                    string romanNumeral = baby.Species.Substring(romanNumeralIndex + 1);
                    romanNumeral = Roman.To(Roman.From(romanNumeral) + 1);

                    baby.Species = baby.Species.Substring(0, romanNumeralIndex) + " " + romanNumeral;
                }
                else
                {
                    baby.Species = baby.Species + " II";
                }
                baby.SpeciesStrain = String.Empty;
            }
            else
            {
                baby.SpeciesStrain = SpeciesStrain + " " + BabySpeciesStrainCounter;
            }
        }
        else
        {
            baby.SpeciesStrain = SpeciesStrain;
        }

        egg.Position = Position;
        egg.ElapsedTicks = 0;
        egg.TicksTillHatched = (int)Math.Ceiling(EggIncubation);
        egg.Creature = baby;

        return egg;
    }
    public List<string> GetCreatureInformation()
    {
        List<string> info = new List<string>();

        info.Add("Species: " + Species);
        if (String.IsNullOrEmpty(SpeciesStrain))
        {
            info.Add("Strain: Original");
        }
        else
        {
            if (SpeciesStrain.Length > 15)
            {
                info.Add("Strain: " + SpeciesStrain.Replace(" ", ""));
            }
            else if (SpeciesStrain.Length > 30)
            {
                info.Add("Strain: " + SpeciesStrain.Replace(" ", "").Substring(0, 30) + "...");
            }
            else
            {
                info.Add("Strain: " + SpeciesStrain);
            }
        }
        info.Add("Strain Counter: " + BabySpeciesStrainCounter);
        info.Add("Generation: " + Generation);
        info.Add("Lifespan: " + Math.Round(Lifespan / 10.0, 1).ToString());
        info.Add("Age: " + Math.Round(ElapsedTicks / 10.0, 1).ToString());
        info.Add("Energy: " + Energy);
        info.Add(" ");
        info.Add("Food: " + UndigestedFood);
        info.Add("Food Digested: " + DigestedFood);
        info.Add("Last Digested: " + Math.Round(TicksSinceLastDigestedFood / 10.0, 1).ToString());
        info.Add("Food Digestion rate: " + Math.Round(FoodDigestion / 10.0, 1).ToString());
        info.Add("Lifetime Food: " + TotalFoodEaten);
        info.Add(" ");
        info.Add("Egg Interval: " + Math.Round(EggInterval / 10.0, 1).ToString());
        info.Add("Egg Incubation: " + Math.Round(EggIncubation / 10.0, 1).ToString());
        info.Add("Egg Camo: " + EggCamo);
        info.Add("Egg Toxicity: " + EggToxicity);
        info.Add("Last Egg: " + Math.Round(TicksSinceLastEgg / 10.0, 1).ToString());
        info.Add("Eggs Created: " + EggsCreated);
        info.Add(" ");
        info.Add("Herbavore: " + Herbavore);
        info.Add("Carnivore: " + Carnivore);
        info.Add("Omnivore: " + Omnivore);
        info.Add("Scavenger: " + Scavenger);
        info.Add(" ");
        info.Add("Speed: " + Speed);
        info.Add("Sight: " + Sight);
        info.Add("Camo: " + Camo);
        info.Add("Cloning: " + Cloning);
        info.Add("Cold Tolerance: " + ColdClimateTolerance);
        info.Add("Hot Tolerance: " + HotClimateTolerance);
        info.Add(" ");
        info.Add("Position: {X:" + ((int)Position.X).ToString().PadLeft(4,' ') + ", Y:" + ((int)Position.Y).ToString().PadLeft(4, ' '));
        info.Add("Direction: " + Direction);
        info.Add("Rotation: " + Rotation);

        return info;
    }

    //Helper functions
    private float Mutation(Random rand, float mutationChance)
    {
        bool didMutationHappen = false;

        if (mutationChance > 0)
        {
            if (rand.Next(0, 100) >= (100 - mutationChance))
            {
                didMutationHappen = true;
            }

            if (didMutationHappen)
            {
                if (rand.Next(0, 10) > 4)
                {
                    return 1f;
                }
                else
                {
                    return -1f;
                }
            }
        }

        return 0f;
    }
    private float CalculateIntercept(Creature target)
    {
        //Vector2 totarget = target.Position - tower.Position;

        //float a = Vector2.Dot(target.Speed, target.velocity) - (bullet.velocity * bullet.velocity);
        //float b = 2 * Vector2.Dot(target.Speed, totarget);
        //float c = Vector2.Dot(totarget, totarget);

        //float p = -b / (2 * a);
        //float q = (float)Math.Sqrt((b * b) - 4 * a * c) / (2 * a);

        //float t1 = p - q;
        //float t2 = p + q;
        //float t;

        //if (t1 > t2 && t2 > 0)
        //{
        //    t = t2;
        //}
        //else
        //{
        //    t = t1;
        //}

        //Vector aimSpot = target.position + target.velocity * t;
        //Vector bulletPath = aimSpot - tower.position;
        //float timeToImpact = bulletPath.Length() / bullet.speed;//speed must be in units per second
        return 0;
    }
    private bool IsSameAs(Creature compareCreature)
    {
        if (EggCamo != compareCreature.EggCamo || EggInterval != compareCreature.EggInterval || 
            EggIncubation != compareCreature.EggIncubation || EggToxicity != compareCreature.EggToxicity || 
            FoodDigestion != compareCreature.FoodDigestion || Speed != compareCreature.Speed || 
            Lifespan != compareCreature.Lifespan || Sight != compareCreature.Sight || 
            Attraction != compareCreature.Attraction || Camo != compareCreature.Camo || 
            Cloning != compareCreature.Cloning || HotClimateTolerance != compareCreature.HotClimateTolerance || 
            ColdClimateTolerance != compareCreature.ColdClimateTolerance || Herbavore != compareCreature.Herbavore || 
            Carnivore != compareCreature.Carnivore || Scavenger != compareCreature.Scavenger || 
            Omnivore != compareCreature.Omnivore)
            return false;
        
        return true;
    }
}