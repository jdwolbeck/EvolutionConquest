using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace EvolutionConquest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //Framework variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private InputState _inputState;
        private Player _player;
        private SpriteFont _diagFont;
        //Game variables
        private GameData _gameData;
        private Texture2D _blackPixel;
        private Texture2D _whitePixel;
        private Texture2D _basicCreatureTexture;
        private Texture2D _foodTexture;
        private Texture2D _eggTexture;
        private Random _rand;
        private CreatureShapeGenerator _creatureGenerator;
        private FoodShapeGenerator _foodGenerator;
        private EggShapeGenerator _eggGenerator;
        private Names _names;
        private Borders _borders;
        private List<string> _creatureStats;
        private double _elapsedSecondsSinceTick;
        private double _elapsedTimeSinceFoodGeneration;
        private float _currentTicksPerSecond = 30;
        private float _tickSeconds;
        //Constants
        private const float TICKS_PER_SECOND = 30;
        private const int BORDER_WIDTH = 10;
        private const float INIT_FOOD_RATIO = 0.0001f;
        private const float INIT_STARTING_CREATURE_RATIO = 0.00005f;
        private const float FOOD_GENERATION_INTERVAL_SECONDS = 0.5f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _inputState = new InputState();
            _player = new Player();
            _elapsedSecondsSinceTick = 0;
            _elapsedTimeSinceFoodGeneration = 0;

            IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            Global.Camera.ViewportWidth = _graphics.GraphicsDevice.Viewport.Width;
            Global.Camera.ViewportHeight = _graphics.GraphicsDevice.Viewport.Height;
            Global.Camera.CenterOn(new Vector2(Global.Camera.ViewportWidth / 2, Global.Camera.ViewportHeight / 2));

            base.Initialize();
        }
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _diagFont = Content.Load<SpriteFont>("DiagnosticsFont");
            _tickSeconds = TICKS_PER_SECOND;

            _blackPixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            Color[] color = new Color[1];
            color[0] = Color.Black;
            _blackPixel.SetData(color);

            _whitePixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            color = new Color[1];
            color[0] = Color.White;
            _whitePixel.SetData(color);

            _gameData = new GameData();
            _rand = new Random();
            _names = new Names();
            _creatureStats = new List<string>();
            _creatureGenerator = new CreatureShapeGenerator();
            _foodGenerator = new FoodShapeGenerator();
            _eggGenerator = new EggShapeGenerator();
            _basicCreatureTexture = _creatureGenerator.CreateCreatureTexture(_graphics.GraphicsDevice);
            _foodTexture = _foodGenerator.CreateFoodTexture(_graphics.GraphicsDevice);
            _eggTexture = _eggGenerator.CreateEggTexture(_graphics.GraphicsDevice, Color.Black, Color.White);

            //Generate the Map
            _borders = new Borders();
            _borders.Texture = _blackPixel;
            _borders.LeftWall = new Vector2(0, 0);
            _borders.RightWall = new Vector2(Global.WORLD_SIZE, 0);
            _borders.TopWall = new Vector2(0, 0);
            _borders.BottomWall = new Vector2(0, Global.WORLD_SIZE);

            //Load in random food
            int amountOfFood = (int)(((Global.WORLD_SIZE * Global.WORLD_SIZE) / _foodTexture.Width) * INIT_FOOD_RATIO);
            for (int i = 0; i < amountOfFood; i++)
            {
                SpawnFood();
            }

            //Game start, load in starting population of creatures
            int startingCreatureAmount = (int)(((Global.WORLD_SIZE * Global.WORLD_SIZE) / ((_basicCreatureTexture.Width + _basicCreatureTexture.Height) / 2)) * INIT_STARTING_CREATURE_RATIO);
            for (int i = 0; i < startingCreatureAmount; i++)
            {
                SpawnStartingCreature();
            }
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        protected override void Update(GameTime gameTime)
        {
            bool tick = false;
            if (_inputState.IsExitGame(PlayerIndex.One))
            {
                Exit();
            }
            else
            {
                _player.HandleInput(_inputState);
            }

            _tickSeconds = 1 / _currentTicksPerSecond;

            _elapsedSecondsSinceTick += gameTime.ElapsedGameTime.TotalSeconds;
            _elapsedTimeSinceFoodGeneration += gameTime.ElapsedGameTime.TotalSeconds;
            if (_elapsedSecondsSinceTick > _tickSeconds)
            {
                _elapsedSecondsSinceTick = _elapsedSecondsSinceTick - _tickSeconds; //Start the next tick with the overage
                tick = true;
            }

            //During a tick do all creature processing
            if (tick)
            {
                for (int i = 0; i < _gameData.Creatures.Count; i++)
                {
                    if (_gameData.Creatures[i].IsAlive)
                    {
                        _gameData.Creatures[i].AdvanceTick();

                        //Check if the creature has died
                        if (_gameData.Creatures[i].ElapsedTicks > _gameData.Creatures[i].Lifespan)
                        {
                            _gameData.Creatures[i].IsAlive = false;
                            //Drop all food on the ground randomly around the area
                            for (int k = 0; k < _gameData.Creatures[k].UndigestedFood; k++)
                            {
                                SpawnFood(new Vector2(_gameData.Creatures[k].Position.X + _rand.Next(-5, 5), _gameData.Creatures[k].Position.Y + _rand.Next(-5, 5)));
                            }
                        }
                        //Check if we can lay a new egg
                        if (_gameData.Creatures[i].DigestedFood > 0 && _gameData.Creatures[i].TicksSinceLastEgg >= _gameData.Creatures[i].EggInterval)
                        {
                            _gameData.Creatures[i].DigestedFood--;
                            //Lay new egg logic
                        }
                    }
                }
            }
            else //Off tick processing
            {
                //Spawn new food
                if (_elapsedTimeSinceFoodGeneration > FOOD_GENERATION_INTERVAL_SECONDS)
                {
                    _elapsedTimeSinceFoodGeneration -= FOOD_GENERATION_INTERVAL_SECONDS;
                    SpawnFood();
                }

                //CollisionDetection
                //Border Collision Detection
                for (int i = 0; i < _gameData.Creatures.Count; i++)
                {
                    bool collide = false;
                    if (_gameData.Creatures[i].Position.X <= 0 || _gameData.Creatures[i].Position.X >= Global.WORLD_SIZE)
                    {
                        collide = true;
                        //if (_gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y >= 0)
                        //{
                        //    _gameData.Creatures[i].Rotation = MathHelper.ToDegrees(_gameData.Creatures[i].Rotation) + MathHelper.ToRadians(90);
                        //}
                        //else if (_gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y < 0)
                        //{
                        //    _gameData.Creatures[i].Rotation = _gameData.Creatures[i].Rotation + MathHelper.ToRadians(270);
                        //}
                        //else if (_gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y >= 0)
                        //{
                        //    _gameData.Creatures[i].Rotation = _gameData.Creatures[i].Rotation + MathHelper.ToRadians(270);
                        //}
                        //else if (_gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y < 0)
                        //{
                        //    _gameData.Creatures[i].Rotation = _gameData.Creatures[i].Rotation + MathHelper.ToRadians(90);
                        //}
                        if (_gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y >= 0)
                        {
                            _gameData.Creatures[i].Rotation = MathHelper.ToRadians((MathHelper.ToDegrees(_gameData.Creatures[i].Rotation) + 90) % 360);
                        }
                        else if (_gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y < 0)
                        {
                            _gameData.Creatures[i].Rotation = MathHelper.ToRadians((MathHelper.ToDegrees(_gameData.Creatures[i].Rotation) + 270) % 360);
                        }
                        else if (_gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y >= 0)
                        {
                            _gameData.Creatures[i].Rotation = MathHelper.ToRadians((MathHelper.ToDegrees(_gameData.Creatures[i].Rotation) + 270) % 360);
                        }
                        else if (_gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y < 0)
                        {
                            _gameData.Creatures[i].Rotation = MathHelper.ToRadians((MathHelper.ToDegrees(_gameData.Creatures[i].Rotation) + 90) % 360);
                        }
                    }
                    if (_gameData.Creatures[i].Position.Y <= 0 || _gameData.Creatures[i].Position.Y >= Global.WORLD_SIZE)
                    {
                        collide = true;
                        if (_gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y >= 0)
                        {
                            _gameData.Creatures[i].Rotation = _gameData.Creatures[i].Rotation + MathHelper.ToRadians(270);
                        }
                        else if (_gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y < 0)
                        {
                            _gameData.Creatures[i].Rotation = _gameData.Creatures[i].Rotation + MathHelper.ToRadians(90);
                        }
                        else if (_gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y >= 0)
                        {
                            _gameData.Creatures[i].Rotation = _gameData.Creatures[i].Rotation + MathHelper.ToRadians(90);
                        }
                        else if (_gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y < 0)
                        {
                            _gameData.Creatures[i].Rotation = _gameData.Creatures[i].Rotation + MathHelper.ToRadians(270);
                        }
                    }
                    if (collide)
                    {

                    }
                }

                //Movement
                for (int i = 0; i < _gameData.Creatures.Count; i++)
                {
                    if (_gameData.Creatures[i].IsAlive)
                    {
                        //Move the creature
                        _gameData.Creatures[i].Position += _gameData.Creatures[i].Direction * ((_gameData.Creatures[i].Speed / 10f) * (_currentTicksPerSecond / TICKS_PER_SECOND)) * TICKS_PER_SECOND * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }

            _inputState.Update();
            Global.Camera.HandleInput(_inputState, PlayerIndex.One, ref _gameData);
            if (_gameData.Focus != null)
            {
                Global.Camera.CenterOn(_gameData.Focus.Position);
            }

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // === DRAW WITHIN THE WORLD ===
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
            //Draw food
            for(int i = 0; i < _gameData.Food.Count; i++)
            {
                _spriteBatch.Draw(_gameData.Food[i].Texture, _gameData.Food[i].Position, null, Color.White, 0f, _gameData.Food[i].Origin, 1f, SpriteEffects.None, 1f);
            }

            //Draw Creatures
            for (int i = 0; i < _gameData.Creatures.Count; i++)
            {
                if (_gameData.Creatures[i].IsAlive)
                {
                    _spriteBatch.Draw(_gameData.Creatures[i].Texture, _gameData.Creatures[i].Position, null, Color.White, _gameData.Creatures[i].Rotation, _gameData.Creatures[i].Origin, 1f, SpriteEffects.None, 1f);
                }
            }

            //Draw Borders
            _spriteBatch.Draw(_borders.Texture, new Rectangle((int)_borders.LeftWall.X - BORDER_WIDTH, (int)_borders.LeftWall.Y, BORDER_WIDTH, Global.WORLD_SIZE + BORDER_WIDTH), Color.Black);
            _spriteBatch.Draw(_borders.Texture, new Rectangle((int)_borders.RightWall.X, (int)_borders.RightWall.Y - BORDER_WIDTH, BORDER_WIDTH, Global.WORLD_SIZE + BORDER_WIDTH), Color.Black);
            _spriteBatch.Draw(_borders.Texture, new Rectangle((int)_borders.TopWall.X - BORDER_WIDTH, (int)_borders.TopWall.Y - BORDER_WIDTH, Global.WORLD_SIZE + BORDER_WIDTH, BORDER_WIDTH), Color.Black);
            _spriteBatch.Draw(_borders.Texture, new Rectangle((int)_borders.BottomWall.X - BORDER_WIDTH, (int)_borders.BottomWall.Y, Global.WORLD_SIZE + (BORDER_WIDTH * 2), BORDER_WIDTH), Color.Black);
            _spriteBatch.End();

            //=== DRAW HUD INFORMATION ===
            _spriteBatch.Begin();
            //Draw Creature information
            //Draw panel boxes
            if (_gameData.Focus != null)
            {
                int startingX = 10, startingY = 100, borderDepth = 5, width = 0, height = 0, textHeight = 0, textSpacing = 5;
                int currentX = startingX + borderDepth + textSpacing;
                int currentY = startingY + borderDepth + textSpacing;

                _creatureStats = _gameData.Focus.GetCreatureInformation();
                textHeight = (int)Math.Ceiling(_diagFont.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZ").Y);
                height = (borderDepth * 2) + _creatureStats.Count * (textHeight + textSpacing);
                width = (borderDepth * 2) + (int)Math.Ceiling(_diagFont.MeasureString("Position: {X:-100.000000, Y:-100.000000}").X) + (textSpacing * 2);

                _spriteBatch.Draw(_blackPixel, new Rectangle(startingX, startingY, width, height), Color.Black);
                _spriteBatch.Draw(_whitePixel, new Rectangle(startingX + borderDepth, startingY + borderDepth, width - borderDepth * 2, height - borderDepth * 2), Color.White);
                for (int i = 0; i < _creatureStats.Count; i++)
                {
                    _spriteBatch.DrawString(_diagFont, _creatureStats[i], new Vector2(currentX, currentY), Color.Black);
                    currentY += textHeight + textSpacing;
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        //Helper functions
        private void SpawnFood()
        {
            SpawnFood(new Vector2(_rand.Next(_foodTexture.Width, Global.WORLD_SIZE - _foodTexture.Width), _rand.Next(_foodTexture.Height, Global.WORLD_SIZE - _foodTexture.Height)));
        }
        private void SpawnFood(Vector2 position)
        {
            Food food = new Food();
            food.Texture = _foodTexture;
            food.Position = position;

            _gameData.Food.Add(food);
        }
        private void SpawnStartingCreature()
        {
            Creature creature = new Creature();
            creature.InitNewCreature(_rand, _names);
            creature.Texture = _basicCreatureTexture;
            creature.Position = new Vector2(_rand.Next(creature.Texture.Width, Global.WORLD_SIZE - creature.Texture.Width), _rand.Next(creature.Texture.Height, Global.WORLD_SIZE - creature.Texture.Height));
            _gameData.Creatures.Add(creature);
        }
    }
}
