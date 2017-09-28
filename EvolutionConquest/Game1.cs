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
        private Texture2D _blackPixel;
        private Texture2D _borderTexture;
        private Random _rand;
        private CreatureShapeGenerator _creatureGenerator;
        private Texture2D _basicCreatureTexture;
        private List<Creature> _creatures;
        private List<Egg> _eggs;
        private Names _names;
        private Borders _borders;
        private double elapsedSecondsSinceTick;
        //Constants
        private const float TICKS_PER_SECOND = 20;
        private const float TICK_SECONDS = 1 / TICKS_PER_SECOND;
        private const int BORDER_WIDTH = 10;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Content.RootDirectory = "Content";
            _inputState = new InputState();
            _player = new Player();
            elapsedSecondsSinceTick = 0;
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

            _blackPixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            Color[] color = new Color[1];
            color[0] = Color.Black;
            _blackPixel.SetData(color);

            //_borderTexture = new Texture2D(_graphics.GraphicsDevice, BORDER_WIDTH, Global.WORLD_SIZE);
            //Color[] colorBorder = new Color[_borderTexture.Width * _borderTexture.Height];
            //for (int i = 0; i < colorBorder.Length; i++)
            //{
            //    colorBorder[i] = Color.Black;
            //}
            //_borderTexture.SetData(colorBorder);

            _rand = new Random();
            _names = new Names();
            _creatureGenerator = new CreatureShapeGenerator();
            _basicCreatureTexture = _creatureGenerator.CreateCreatureTexture(_graphics.GraphicsDevice);

            //Generate the Map
            _borders = new Borders();
            _borders.LeftWall = new Vector2(0, 0);
            _borders.RightWall = new Vector2(Global.WORLD_SIZE, 0);
            _borders.TopWall = new Vector2(0, 0);
            _borders.BottomWall = new Vector2(0, Global.WORLD_SIZE);
            //Anytime the View changes we need to re-calculate the Border WorldPosition
            _borders.Texture = _borderTexture;

            //Game start, load in starting population of creatures
            _creatures = new List<Creature>();
            _eggs = new List<Egg>();

            Creature creature = new Creature();
            creature.InitNewCreature(_rand, _names);
            creature.Texture = _basicCreatureTexture;
            creature.Origin = new Vector2(_basicCreatureTexture.Width / 2, _basicCreatureTexture.Height / 2);
            creature.Position = new Vector2(Global.WORLD_SIZE / 2, Global.WORLD_SIZE / 2);
            _creatures.Add(creature);

            Creature creature2 = new Creature();
            creature2.InitNewCreature(_rand, _names);
            creature2.Texture = _basicCreatureTexture;
            creature2.Origin = new Vector2(_basicCreatureTexture.Width / 2, _basicCreatureTexture.Height / 2);
            creature2.Position = new Vector2(Global.WORLD_SIZE / 2 - 20, Global.WORLD_SIZE / 2 - 20);
            _creatures.Add(creature2);
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

            elapsedSecondsSinceTick += gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedSecondsSinceTick > TICK_SECONDS)
            {
                elapsedSecondsSinceTick = elapsedSecondsSinceTick - TICK_SECONDS; //Start the next tick with the overage
                tick = true;
            }

            if (tick)
            {
                for (int i = 0; i < _creatures.Count; i++)
                {

                }
            }

            _inputState.Update();
            Global.Camera.HandleInput(_inputState, PlayerIndex.One);

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);

            //Draw Creatures
            for (int i = 0; i < _creatures.Count; i++)
            {
                _spriteBatch.Draw(_creatures[i].Texture, _creatures[i].Position, null, Color.White, 0f, _creatures[i].Origin, 1f, SpriteEffects.None, 1f);
            }

            //Draw Borders
            _spriteBatch.Draw(_blackPixel, new Rectangle((int)_borders.LeftWall.X - BORDER_WIDTH, (int)_borders.LeftWall.Y, BORDER_WIDTH, Global.WORLD_SIZE + BORDER_WIDTH), Color.Black);
            _spriteBatch.Draw(_blackPixel, new Rectangle((int)_borders.RightWall.X, (int)_borders.RightWall.Y - BORDER_WIDTH, BORDER_WIDTH, Global.WORLD_SIZE + BORDER_WIDTH), Color.Black);
            _spriteBatch.Draw(_blackPixel, new Rectangle((int)_borders.TopWall.X - BORDER_WIDTH, (int)_borders.TopWall.Y - BORDER_WIDTH, Global.WORLD_SIZE + BORDER_WIDTH, BORDER_WIDTH), Color.Black);
            _spriteBatch.Draw(_blackPixel, new Rectangle((int)_borders.BottomWall.X - BORDER_WIDTH, (int)_borders.BottomWall.Y, Global.WORLD_SIZE + (BORDER_WIDTH * 2), BORDER_WIDTH), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
