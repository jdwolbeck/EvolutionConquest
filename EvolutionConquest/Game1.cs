using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace EvolutionConquest
{
    public class Game1 : Game
    {
        //Framework variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private InputState _inputState;
        private Player _player;
        private SpriteFont _diagFont;
        private SpriteFont _panelHeaderFont;
        private int _diagTextHeight;
        //Game variables
        private GameData _gameData;
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
        private int _uniqueSpeciesCount;
        private double _elapsedSecondsSinceTick;
        private double _elapsedTimeSinceFoodGeneration;
        private float _currentTicksPerSecond = 30;
        private float _tickSeconds;
        private float _elapsedTicksSinceSecondProcessing;
        private int _speciesIdCounter;
        private Chart _chart;
        private List<string> _controlsListText;
        //Constants
        private const float SPRITE_FONT_SCALE = 0.5f;
        private const float TICKS_PER_SECOND = 30;
        private const int BORDER_WIDTH = 10;
        private const float INIT_FOOD_RATIO = 0.0001f;
        private const float INIT_STARTING_CREATURE_RATIO = 0.00001f;
        private const float FOOD_GENERATION_INTERVAL_SECONDS = 0.05f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.PreferredBackBufferWidth = 1600;
            _inputState = new InputState();
            _player = new Player();
            _elapsedSecondsSinceTick = 0;
            _elapsedTimeSinceFoodGeneration = 0;
            _elapsedTicksSinceSecondProcessing = 0;
            _speciesIdCounter = 0;

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

            _panelHeaderFont = Content.Load<SpriteFont>("ArialBlack");
            _diagFont = Content.Load<SpriteFont>("DiagnosticsFont");
            _diagTextHeight = (int)Math.Ceiling(_diagFont.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZ[]").Y);
            _tickSeconds = TICKS_PER_SECOND;

            _whitePixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            Color[] color = new Color[1];
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
            _borders.Texture = _whitePixel;
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
            Creature creature = new Creature();
            creature.InitNewCreature(_rand, ref _names, _speciesIdCounter);
            _speciesIdCounter++;
            creature.Texture = _basicCreatureTexture;
            creature.Position = new Vector2(500, 100);
            creature.Rotation = MathHelper.ToRadians(-30);
            _gameData.Creatures.Add(creature);

            Global.Camera.AdjustZoom(500);
            _gameData.Focus = creature;
            _gameData.FocusIndex = _gameData.Creatures.Count - 1;

            //Game Controls list for HUD
            _controlsListText = new List<string>();
            //Controls: [W][A][S][D] Camera Pan, [PageUp][PageDown] Iterate Creatures, [Shift] + [PageUp][PageDown] Iterate Species, [F12] Toggle Chart";
            _controlsListText.Add("[W][A][S][D] Camera Pan");
            _controlsListText.Add("[Scroll Wheel] Zoom");
            _controlsListText.Add("[PageUp][PageDown] Cycle Creatures");
            _controlsListText.Add("[Shift] + [PageUp][PageDown] Cycle Species");
            _controlsListText.Add("[F10] Toggle Creature Statistics");
            _controlsListText.Add("[F11] Toggle Chart");
            _controlsListText.Add("[Left Click] Follow/Unfollow Creature");
            _controlsListText.Add("[H] Highlight Species");
            _controlsListText.Add(" ");
            _controlsListText.Add("[F12] Toggle Control Menu");

            //Create the chart
            _chart = new Chart();
            _chart.Width = 600;
            _chart.Height = 300;
            _chart.Location = new System.Drawing.Point(_graphics.PreferredBackBufferWidth - _chart.Width - 2, _graphics.PreferredBackBufferHeight - _chart.Height - 2);
            _chart.Text = "Test";
            _chart.Visible = false;
            ChartArea chartArea1 = new ChartArea();
            chartArea1.Name = "ChartArea1";
            _chart.ChartAreas.Add(chartArea1);
            Legend legend = new Legend();
            _chart.Legends.Add(legend);

            Control.FromHandle(Window.Handle).Controls.Add(_chart);
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
                _inputState.Update();
                _player.HandleInput(_inputState, PlayerIndex.One, ref _gameData);
                Global.Camera.HandleInput(_inputState, PlayerIndex.One, ref _gameData);
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
                _elapsedTicksSinceSecondProcessing++;

                //Check eggs before creatures so that the baby can follow cretaure update logic when born such as eating food
                for (int i = _gameData.Eggs.Count - 1; i >= 0; i--)
                {
                    _gameData.Eggs[i].AdvanceTick();
                    //Check to egg hatched
                    if (_gameData.Eggs[i].ElapsedTicks >= _gameData.Eggs[i].TicksTillHatched)
                    {
                        //TODO change texture based on creature properties dynamically
                        //Assign texture to the creature
                        _gameData.Eggs[i].Creature.Texture = _basicCreatureTexture;
                        _gameData.Creatures.Add(_gameData.Eggs[i].Creature);
                        _gameData.Eggs.RemoveAt(i);
                    }
                }
                for (int i = _gameData.Creatures.Count - 1; i >= 0; i--)
                {
                    _gameData.Creatures[i].AdvanceTick();

                    //Check if the creature has died
                    if (_gameData.Creatures[i].ElapsedTicks > _gameData.Creatures[i].Lifespan)
                    {
                        _gameData.Creatures[i].IsAlive = false;
                        _gameData.DeadCreatures.Add(_gameData.Creatures[i]);
                        //REMOVED To help with performance issues
                        ////Drop all food on the ground randomly around the area
                        //for (int k = 0; k < _gameData.Creatures[k].UndigestedFood; k++)
                        //{
                        //    SpawnFood(new Vector2(_gameData.Creatures[k].Position.X + _rand.Next(-5, 5), _gameData.Creatures[k].Position.Y + _rand.Next(-5, 5)));
                        //}
                        if (_gameData.Focus == _gameData.Creatures[i])
                        {
                            _gameData.Focus = null;
                            _gameData.FocusIndex = -1;
                        }
                        _gameData.Creatures.RemoveAt(i);
                    }
                    //Check if we can lay a new egg
                    if (_gameData.Creatures[i].DigestedFood > 0 && _gameData.Creatures[i].TicksSinceLastEgg >= _gameData.Creatures[i].EggInterval)
                    {
                        _gameData.Creatures[i].DigestedFood--; //Costs one digested food to lay an egg
                        Egg egg = _gameData.Creatures[i].LayEgg(_rand, ref _names, _gameData.Creatures);
                        //TODO handle this maybe in the Creature class
                        egg.Texture = _eggTexture;
                        _gameData.Eggs.Add(egg); //Add the new egg to gameData, the LayEgg function will calculate the Mutations
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
                    if (_gameData.Creatures[i].Position.X - (_gameData.Creatures[i].Texture.Width / 2) <= 0 || _gameData.Creatures[i].Position.X + (_gameData.Creatures[i].Texture.Width / 2) >= Global.WORLD_SIZE)
                    {
                        if (_gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y >= 0 ||
                            _gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y < 0 ||
                            _gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y >= 0 ||
                            _gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y < 0)
                        {
                            _gameData.Creatures[i].Rotation = (((float)Math.PI * 2) - _gameData.Creatures[i].Rotation);
                        }
                    }
                    if (_gameData.Creatures[i].Position.Y - (_gameData.Creatures[i].Texture.Height / 2) <= 0 || _gameData.Creatures[i].Position.Y + (_gameData.Creatures[i].Texture.Height / 2) >= Global.WORLD_SIZE)
                    {
                        if (_gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y >= 0 ||
                            _gameData.Creatures[i].Direction.X >= 0 && _gameData.Creatures[i].Direction.Y < 0 ||
                            _gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y >= 0 ||
                            _gameData.Creatures[i].Direction.X < 0 && _gameData.Creatures[i].Direction.Y < 0)
                        {
                            _gameData.Creatures[i].Rotation = (((float)Math.PI) - _gameData.Creatures[i].Rotation);
                        }
                    }

                    //Food collision
                    bool creatureBoundsCalculated = false;
                    for (int k = 0; k < _gameData.Food.Count; k++)
                    {
                        if (Vector2.Distance(_gameData.Food[k].Position, _gameData.Creatures[i].Position) <= _gameData.Creatures[i].TextureCollideDistance)
                        {
                            if (!creatureBoundsCalculated) //Only calculate the bounds for the creature once
                            {
                                creatureBoundsCalculated = true;
                                _gameData.Creatures[i].CalculateBounds();
                            }
                            if (_gameData.Creatures[i].Bounds.Intersects(_gameData.Food[k].Bounds))
                            {
                                _gameData.Creatures[i].UndigestedFood++;
                                _gameData.Food.RemoveAt(k);
                            }
                        }
                    }

                    if (_gameData.Creatures[i].IsAlive)
                    {
                        //Move the creature
                        _gameData.Creatures[i].Position += _gameData.Creatures[i].Direction * ((_gameData.Creatures[i].Speed / 10f) * (_currentTicksPerSecond / TICKS_PER_SECOND)) * TICKS_PER_SECOND * (float)gameTime.ElapsedGameTime.TotalSeconds;
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

                //Every second processing only when it is not a TICK
                if (_elapsedTicksSinceSecondProcessing >= TICKS_PER_SECOND * 5)
                {
                    _elapsedTicksSinceSecondProcessing = 0;

                    //For the HUD calculate the unique number of species. Do this in the "Per Second" code so that it is not a big performance hit
                    _uniqueSpeciesCount = _gameData.GetUniqueSpeciesCount();

                    //Generate Graph data
                    _gameData.CalculateChartData(); //This will populat the Chart Data in _gameData. Even if we hide the chart we need to keep track of ChartData

                    if (_gameData.ShowChart)
                    {
                        if (!_chart.Visible && _gameData.ChartDataTop.Count > 0)
                        {
                            _chart.Visible = true;
                        }

                        _chart.Series.Clear();
                        for (int i = 0; i < _gameData.ChartDataTop.Count; i++)
                        {
                            int? count = _gameData.ChartDataTop[i].CountsOverTime[_gameData.ChartDataTop[i].CountsOverTime.Count - 1];
                            string name = String.Empty;
                            if (count != null)
                            {
                                name = _gameData.ChartDataTop[i].Name + "(" + count + ")";
                            }
                            else
                            {
                                name = _gameData.ChartDataTop[i].Name;
                            }

                            _chart.Series.Add(name);
                            _chart.Series[name].XValueType = ChartValueType.Int32;
                            _chart.Series[name].ChartType = SeriesChartType.StackedArea100;
                            _chart.Series[name].BorderWidth = 3;
                            for (int k = 0; k < _gameData.ChartDataTop[i].CountsOverTime.Count; k++)
                            {
                                _chart.Series[name].Points.AddXY(k, _gameData.ChartDataTop[i].CountsOverTime[k]);
                            }
                        }
                    }
                }

                //Display chart logic here as well so that we do not need to wait 5 seconds
                if (_gameData.ChartDataTop.Count > 0)
                {
                    _chart.Visible = _gameData.ShowChart;
                }
            }

            //This must be after movement caluclations occur for the creatures otherwise the camera will glitch back and forth
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
            DrawWorldObjects();
            //=== DRAW HUD INFORMATION, DOES NOT DRAW TO WORLD SCALE ===
            DrawHUD(); 

            base.Draw(gameTime);
        }

        //Draw World Functions
        private void DrawWorldObjects()
        {
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);

            DrawFood();
            DrawEggs();
            DrawCreatures();
            DrawBorders();
            DrawHighlightCreatures();

            _spriteBatch.End();
        }
        private void DrawCreatures()
        {
            for (int i = 0; i < _gameData.Creatures.Count; i++)
            {
                if (_gameData.Creatures[i].IsAlive)
                {
                    _spriteBatch.Draw(_gameData.Creatures[i].Texture, _gameData.Creatures[i].Position, null, Color.White, _gameData.Creatures[i].Rotation, _gameData.Creatures[i].Origin, 1f, SpriteEffects.None, 1f);
                }
            }
        }
        private void DrawEggs()
        {
            for (int i = 0; i < _gameData.Eggs.Count; i++)
            {
                _spriteBatch.Draw(_gameData.Eggs[i].Texture, _gameData.Eggs[i].Position, null, Color.White, 0f, _gameData.Eggs[i].Origin, 1f, SpriteEffects.None, 1f);
            }
        }
        private void DrawFood()
        {
            for (int i = 0; i < _gameData.Food.Count; i++)
            {
                _spriteBatch.Draw(_gameData.Food[i].Texture, _gameData.Food[i].Position, null, Color.White, 0f, _gameData.Food[i].Origin, 1f, SpriteEffects.None, 1f);
            }
        }
        private void DrawBorders()
        {
            _spriteBatch.Draw(_borders.Texture, new Rectangle((int)_borders.LeftWall.X - BORDER_WIDTH, (int)_borders.LeftWall.Y, BORDER_WIDTH, Global.WORLD_SIZE + BORDER_WIDTH), Color.SaddleBrown);
            _spriteBatch.Draw(_borders.Texture, new Rectangle((int)_borders.RightWall.X, (int)_borders.RightWall.Y - BORDER_WIDTH, BORDER_WIDTH, Global.WORLD_SIZE + BORDER_WIDTH), Color.SaddleBrown);
            _spriteBatch.Draw(_borders.Texture, new Rectangle((int)_borders.TopWall.X - BORDER_WIDTH, (int)_borders.TopWall.Y - BORDER_WIDTH, Global.WORLD_SIZE + BORDER_WIDTH, BORDER_WIDTH), Color.SaddleBrown);
            _spriteBatch.Draw(_borders.Texture, new Rectangle((int)_borders.BottomWall.X - BORDER_WIDTH, (int)_borders.BottomWall.Y, Global.WORLD_SIZE + (BORDER_WIDTH * 2), BORDER_WIDTH), Color.SaddleBrown);
        }
        private void DrawHighlightCreatures()
        {
            if (_gameData.HighlightSpecies && _gameData.Focus != null)
            {
                int followedSpecies = _gameData.Focus.SpeciesId;
                int borderWidth = 3;

                for (int i = 0; i < _gameData.Creatures.Count; i++)
                {
                    if (_gameData.Creatures[i].SpeciesId == followedSpecies)
                    {
                        int diagnolLength = (int)Math.Ceiling(Math.Sqrt((_gameData.Creatures[i].Texture.Width * _gameData.Creatures[i].Texture.Width) + (_gameData.Creatures[i].Texture.Height * _gameData.Creatures[i].Texture.Height)));
                        float upperLeftX = _gameData.Creatures[i].Position.X - (diagnolLength / 2), upperLeftY = _gameData.Creatures[i].Position.Y - (diagnolLength / 2);

                        _spriteBatch.Draw(_whitePixel, new Rectangle((int)upperLeftX - borderWidth, (int)upperLeftY - borderWidth, diagnolLength + borderWidth * 2, borderWidth), Color.Red);
                        _spriteBatch.Draw(_whitePixel, new Rectangle((int)upperLeftX - borderWidth, (int)upperLeftY + diagnolLength, diagnolLength + borderWidth * 2, borderWidth), Color.Red);
                        _spriteBatch.Draw(_whitePixel, new Rectangle((int)upperLeftX + diagnolLength, (int)upperLeftY - borderWidth, borderWidth, diagnolLength + borderWidth * 2), Color.Red);
                        _spriteBatch.Draw(_whitePixel, new Rectangle((int)upperLeftX - borderWidth, (int)upperLeftY - borderWidth, borderWidth, diagnolLength + borderWidth * 2), Color.Red);
                    }
                }
            }
        }

        //Draw HUD Functions
        private void DrawHUD()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            DrawCreatureStatsPanel();
            DrawMapStatisctics();
            DrawControlsPanel();
            DrawChartBorder();

            _spriteBatch.End();
        }
        private void DrawCreatureStatsPanel()
        {
            if (_gameData.ShowCreatureStats && _gameData.Focus != null)
            {
                DrawPanelWithText(_panelHeaderFont, "Creature Statistics", _diagFont, _gameData.Focus.GetCreatureInformation(), Global.Anchor.LeftCenter, (int)Math.Ceiling(_diagFont.MeasureString("Position: {X:-100.000000, Y:-100.000000}").X), 0, 20);
            }
        }
        private void DrawMapStatisctics()
        {
            string deadCreatures = String.Empty;

            if (_gameData.DeadCreatures.Count >= 1000)
            {
                deadCreatures = Math.Round(_gameData.DeadCreatures.Count / 1000.0, 2).ToString("#,##0.00") + "k";
            }
            else if (_gameData.DeadCreatures.Count >= 1000000)
            {
                deadCreatures = Math.Round(_gameData.DeadCreatures.Count / 1000000.0, 2).ToString("#,##0.00") + "m";
            }
            else
            {
                deadCreatures = _gameData.DeadCreatures.Count.ToString("#,##0");
            }

            string mapStats = "Alive Creatures: " + _gameData.Creatures.Count.ToString("#,##0") + ", Unique Species: " +
                _uniqueSpeciesCount.ToString("#,##0") + ", Dead Creatures: " + deadCreatures + ", Eggs: " +
                _gameData.Eggs.Count.ToString("#,##0") + ", Map Food: " + _gameData.Food.Count.ToString("#,##0");

            Vector2 textSize = _diagFont.MeasureString(mapStats);
            Vector2 drawPos = new Vector2((_graphics.PreferredBackBufferWidth / 2) - (_diagFont.MeasureString(mapStats).X / 2), 10);

            _spriteBatch.Draw(_whitePixel, new Rectangle((int)drawPos.X, (int)drawPos.Y, (int)textSize.X, (int)textSize.Y), Color.White);
            _spriteBatch.DrawString(_diagFont, mapStats, drawPos, Color.Black);//, 0f, Vector2.Zero, SPRITE_FONT_SCALE, SpriteEffects.None, 1f);
        }
        private void DrawControlsPanel()
        {
            if (_gameData.ShowControls)
            {
                DrawPanelWithText(_panelHeaderFont, "Controls", _diagFont, _controlsListText, Global.Anchor.TopRight, 0, 0, 20);
            }
            else
            {
                DrawPanelWithText(_panelHeaderFont, String.Empty, _diagFont, new List<string> { "[F11] Show Controls" }, Global.Anchor.TopRight, 0, 0, 20);
            }
        }
        private void DrawChartBorder()
        {
            if (_gameData.ShowChart && _gameData.ChartDataTop.Count > 0)
            {
                int borderDepth = 2;

                _spriteBatch.Draw(_whitePixel, new Rectangle(_chart.Location.X - borderDepth, _chart.Location.Y - borderDepth, _chart.Width + (borderDepth * 2), _chart.Height + (borderDepth * 2)), Color.Black);
            }
        }
        private void DrawPanelWithText(SpriteFont headerFont, string header, SpriteFont textFont, List<string> text, Global.Anchor anchor, int lockedWidthValue, int lockedHeightValue, int screenBuffer)
        {
            Color borderColor = Color.Black;
            Color headerBackgroundColor = Color.LightGreen;
            Color headerTextColor = Color.DarkGreen;
            Color textColor = Color.DarkGreen;
            Color textBackgroundColor = Color.LightBlue;
            int startingX = 0, startingY = 0, width = 0, height = 0, headerHeight = 0, textHeight = 0, textWidth = 0, headerTextHeight = 0;
            int borderDepth = 2, textSpacing = 5;
            Vector2 headerSize;
            bool drawHeader = true;

            if (String.IsNullOrEmpty(header))
                drawHeader = false;

            if (drawHeader)
            {
                headerSize = headerFont.MeasureString(header);
                headerTextHeight = (int)Math.Ceiling(headerSize.Y);
                textWidth = (int)Math.Ceiling(headerSize.Y);
            }

            if (lockedWidthValue == 0)
            {
                foreach (string s in text)
                {
                    int tmpSize = (int)Math.Ceiling(textFont.MeasureString(s).X);
                    if (tmpSize > textWidth)
                    {
                        textWidth = tmpSize;
                    }
                }
            }
            else
            {
                textWidth = lockedWidthValue;
            }

            if (lockedHeightValue == 0)
            {
                textHeight = _diagTextHeight;
            }
            else
            {
                textHeight = lockedHeightValue;
            }

            if (drawHeader)
                headerHeight = (textSpacing * 4) + headerTextHeight;
            height = headerHeight + (borderDepth * 2) + (text.Count * (textHeight + textSpacing)) + textSpacing;
            width = (borderDepth * 2) + textWidth + (textSpacing * 2);

            switch (anchor)
            {
                case Global.Anchor.TopCenter:
                    startingX = (_graphics.PreferredBackBufferWidth / 2) - (width / 2);
                    startingY = screenBuffer;
                    break;
                case Global.Anchor.BottomCenter:
                    startingX = (_graphics.PreferredBackBufferWidth / 2) - (width / 2);
                    startingY = _graphics.PreferredBackBufferHeight - height - screenBuffer;
                    break;
                case Global.Anchor.RightCenter:
                    startingX = _graphics.PreferredBackBufferWidth - width - screenBuffer;
                    startingY = (_graphics.PreferredBackBufferHeight / 2) - (height / 2);
                    break;
                case Global.Anchor.LeftCenter:
                    startingX = screenBuffer;
                    startingY = (_graphics.PreferredBackBufferHeight / 2) - (height / 2);
                    break;
                case Global.Anchor.TopLeft:
                    startingX = screenBuffer;
                    startingY = screenBuffer;
                    break;
                case Global.Anchor.TopRight:
                    startingX = _graphics.PreferredBackBufferWidth - width - screenBuffer;
                    startingY = screenBuffer;
                    break;
                case Global.Anchor.BottomLeft:
                    startingX = screenBuffer;
                    startingY = _graphics.PreferredBackBufferHeight - height - screenBuffer;
                    break;
                case Global.Anchor.BottomRight:
                    startingX = _graphics.PreferredBackBufferWidth - width - screenBuffer;
                    startingY = _graphics.PreferredBackBufferHeight - height - screenBuffer;
                    break;
            }

            int currentX = startingX + borderDepth + textSpacing;
            int currentY = startingY + borderDepth + textSpacing;

            _spriteBatch.Draw(_whitePixel, new Rectangle(startingX, startingY, width, height), borderColor);
            _spriteBatch.Draw(_whitePixel, new Rectangle(startingX + borderDepth, startingY + borderDepth, width - borderDepth * 2, height - borderDepth * 2), textBackgroundColor);
            if (drawHeader)
                _spriteBatch.Draw(_whitePixel, new Rectangle(startingX + borderDepth, startingY + borderDepth, width - borderDepth * 2, headerHeight), headerBackgroundColor);

            if (drawHeader)
            {
                currentY += textSpacing;
                _spriteBatch.DrawString(_panelHeaderFont, header, new Vector2(currentX, currentY), headerTextColor);
                currentY += headerTextHeight + (textSpacing * 3);
            }

            for (int i = 0; i < text.Count; i++)
            {
                _spriteBatch.DrawString(textFont, text[i], new Vector2(currentX, currentY), textColor);
                currentY += textHeight + textSpacing;
            }
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
            food.CalculateBounds();

            _gameData.Food.Add(food);
        }
        private void SpawnStartingCreature()
        {
            Creature creature = new Creature();
            creature.InitNewCreature(_rand, ref _names, _speciesIdCounter);
            creature.Texture = _basicCreatureTexture;
            creature.Position = new Vector2(_rand.Next(creature.Texture.Width, Global.WORLD_SIZE - creature.Texture.Width), _rand.Next(creature.Texture.Height, Global.WORLD_SIZE - creature.Texture.Height));
            _gameData.Creatures.Add(creature);

            _speciesIdCounter++;
        }
    }
}
