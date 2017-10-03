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
    /// Adding new Properties make sure to Add to the following: Init, LayEgg, IsSameAs, ZeroOutNegativeValues
    /// </summary>

    public bool IsAlive { get; set; }
    public List<string> Ancestors { get; set; } //Chain of Ancestors (Starts blank)
    public bool IsChangingSpecies { get; set; } //Is the creature transitioning to a new species
    public string NewSpeciesName { get; set; } //Name of new species for these offspring
    public int NewSpeciesId { get; set; } //ID for the new species
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
        Ancestors = new List<string>();
    }

    public void InitNewCreature(Random rand, Names names, int speciesId)
    {
        IsAlive = true;
        IsChangingSpecies = false;
        NewSpeciesName = String.Empty;
        SpeciesId = speciesId;
        Species = names.GetRandomName(rand);
        SpeciesStrain = String.Empty;
        BabySpeciesStrainCounter = "A";
        Generation = 0;
        Rotation = MathHelper.ToRadians(rand.Next(0, 360));
        UndigestedFood = 0;
        DigestedFood = 0;
        FoodDigestion = rand.Next((int)FOOD_DIGESTION_INIT_MIN, (int)FOOD_DIGESTION_INIT_MAX);
        TotalFoodEaten = 0;
        TicksSinceLastDigestedFood = 0;
        TicksSinceLastEgg = 0;
        EggInterval = rand.Next((int)EGG_INTERVAL_INIT_MIN, (int)EGG_INTERVAL_INIT_MAX);
        EggIncubation = rand.Next((int)EGG_INCUBATION_INIT_MIN, (int)EGG_INCUBATION_INIT_MAX);
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
        if (UndigestedFood > 0) //only allow digestion once the a food has been eaten
            TicksSinceLastDigestedFood++;

        if (UndigestedFood > 0 && TicksSinceLastDigestedFood >= FoodDigestion)
        {
            TicksSinceLastDigestedFood = 0;
            UndigestedFood--;
            DigestedFood++;
        }
    }
    public Egg LayEgg(Random rand, Names names, List<Creature> gameDataCreatureList)
    {
        Egg egg = new Egg();
        Creature baby = new Creature();

        TicksSinceLastEgg = 0;
        EggsCreated++;

        baby.Ancestors = CopyAncestorList(Ancestors);
        baby.IsAlive = true;
        baby.IsChangingSpecies = false;
        baby.NewSpeciesId = -1;
        baby.NewSpeciesName = String.Empty;
        baby.Rotation = MathHelper.ToRadians(rand.Next(0, 360));
        baby.Position = Position;
        baby.SpeciesId = SpeciesId;
        baby.Species = Species;
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
            if (IsChangingSpecies || SpeciesStrain.Replace(" ", "").Length > 20) //New Species once the Strain goes past 20 different strains
            {
                if (!IsChangingSpecies)
                {
                    NewSpeciesName = names.GetRandomName(rand);
                    NewSpeciesId = gameDataCreatureList.Max(t => t.SpeciesId) + 1;
                    IsChangingSpecies = true;
                }
                baby.Ancestors.Add(Species);
                baby.Species = NewSpeciesName;
                baby.SpeciesId = NewSpeciesId;
                baby.SpeciesStrain = BabySpeciesStrainCounter;

                ///Roman Numeral at the end of Species name does not work
                //int romanNumeralIndex = baby.Species.IndexOf(' ');
                //if (romanNumeralIndex > 0) //Increment Roman numeral of we Detect a space
                //{
                //    string romanNumeral = baby.Species.Substring(romanNumeralIndex + 1);
                //    romanNumeral = Roman.To(Roman.From(romanNumeral) + 1);

                //    baby.Species = baby.Species.Substring(0, romanNumeralIndex) + " " + romanNumeral;
                //}
                //else
                //{
                //    baby.Species = baby.Species + " II";
                //}
            }
            else
            {
                baby.SpeciesStrain = SpeciesStrain + " " + BabySpeciesStrainCounter;
                BabySpeciesStrainCounter = Global.GetNextBase26(BabySpeciesStrainCounter);
            }
        }
        else
        {
            baby.SpeciesStrain = SpeciesStrain;
        }

        ZeroOutNegativeValues(ref baby);

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
        if (Ancestors.Count > 0)
        {
            string ancestorsString = String.Empty;
            if (Ancestors.Count > 5)
            {
                ancestorsString += Ancestors[0] + ", " + Ancestors[1] + "..." + Ancestors[Ancestors.Count - 2] + ", " + Ancestors[Ancestors.Count - 1];
            }
            else
            {
                for (int i = 0; i < Ancestors.Count; i++)
                {
                    ancestorsString += Ancestors[i] + ", ";
                }
            }
            ancestorsString = ancestorsString.Substring(0, ancestorsString.Length - 2);
            info.Add("Ancestors: " + ancestorsString);
        }
        //info.Add("Strain Counter: " + BabySpeciesStrainCounter);
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
        info.Add("Species ID: " + SpeciesId);
        info.Add("Position: {X:" + ((int)Position.X).ToString().PadLeft(4, ' ') + ", Y:" + ((int)Position.Y).ToString().PadLeft(4, ' '));
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
    private List<string> CopyAncestorList(List<string> toCopyList)
    {
        List<string> newList = new List<string>();

        for (int i = 0; i < toCopyList.Count; i++)
        {
            newList.Add(toCopyList[i]);
        }

        return newList;
    }
    private void ZeroOutNegativeValues(ref Creature creature)
    {
        if (creature.EggCamo < 0) creature.EggCamo = 0;
        if (creature.EggIncubation < 0) creature.EggIncubation = 0;
        if (creature.EggInterval < 0) creature.EggInterval = 0;
        if (creature.EggToxicity < 0) creature.EggToxicity = 0;
        if (creature.FoodDigestion < 0) creature.FoodDigestion = 0;
        if (creature.FoodDigestion < 0) creature.FoodDigestion = 0;
        if (creature.Lifespan < 0) creature.Lifespan = 0;
        if (creature.Lifespan < 0) creature.Lifespan = 0;
        if (creature.Sight < 0) creature.Sight = 0;
        if (creature.Attraction < 0) creature.Attraction = 0;
        if (creature.Camo < 0) creature.Camo = 0;
        if (creature.Cloning < 0) creature.Cloning = 0;
        if (creature.ColdClimateTolerance < 0) creature.ColdClimateTolerance = 0;
        if (creature.HotClimateTolerance < 0) creature.HotClimateTolerance = 0;
        if (creature.Herbavore < 0) creature.Herbavore = 0;
        if (creature.Carnivore < 0) creature.Carnivore = 0;
        if (creature.Omnivore < 0) creature.Omnivore = 0;
        if (creature.Scavenger < 0) creature.Scavenger = 0;
    }
}