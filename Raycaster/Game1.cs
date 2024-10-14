using System;
using System.Collections.Generic;
using caster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using minimap;
using player;

namespace Raycaster;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    //set-up variables
    private const int screenWidth = 720;
    private const int screenHeight = 405;
    private const int framerate = 60;
    private Texture2D blankTexture;

    //block variables
    private const int blockSize = 28;
    private List<Rectangle> blocks = new List<Rectangle>();
    private int maxLength;

    //player variables
    private const int playerSize = blockSize / 4;
    private Player player;

    //raycaster variables
    private const float fov = 60;
    private const float rays = 180;
    private Caster caster;
    private int rayWidth = Convert.ToInt32(screenWidth / rays); //not used in the caster class, only in the DrawRays method, but there is no point redefining it each time the method is called
    private List<int[]> rayLengths = new List<int[]>();
    
    //minimap variables
    private const int miniMapSize = 90;
    private MiniMap miniMap;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        //setting screen dimensions
        _graphics.PreferredBackBufferWidth = screenWidth;
        _graphics.PreferredBackBufferHeight = screenHeight;
        _graphics.ApplyChanges();

        //setting framerate
        TargetElapsedTime = TimeSpan.FromMilliseconds(1000 / framerate);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        //0 = space
        //1 = block
        //9 = player starting position
        int[][] grid = [[1,1,1,1,1,1,1,1,1,1],
                        [1,9,0,0,0,0,0,0,0,1],
                        [1,0,1,1,0,0,1,1,0,1],
                        [1,0,1,0,0,0,0,1,0,1],
                        [1,0,0,0,0,0,0,0,0,1],
                        [1,0,0,0,0,0,0,0,0,1],
                        [1,0,1,0,0,0,0,1,0,1],
                        [1,0,1,1,0,0,1,1,0,1],
                        [1,0,0,0,0,0,0,0,0,1],
                        [1,1,1,1,1,1,1,1,1,1]];

        
        maxLength = blockSize * Convert.ToInt32(Math.Sqrt(((grid.Length * blockSize) ^ 2) + ((grid[0].Length * blockSize) ^ 2)));

        //setting player x and y to be later passed into the player constructor
        var playerX = 0;
        var playerY = 0;

        //generating rectangles from tile map
        for (int y = 0; y < grid.Length; y++)
        {
            for (int x = 0; x < grid[0].Length; x++)
            {
                if (grid[y][x] == 1)
                {
                    blocks.Add(new Rectangle(x * blockSize, y * blockSize, blockSize, blockSize));
                }
                else if (grid[y][x] == 9)
                {
                    playerX = (x * blockSize) + (blockSize / 2) - (playerSize / 2);
                    playerY = (y * blockSize) + (blockSize / 2) - (playerSize / 2);
                }
            }
        }

        //creating player object
        player = new Player(playerSize, playerX, playerY);

        //creating ray caster object
        caster = new Caster(fov, rays, maxLength);

        //creating minimap object
        miniMap = new MiniMap(miniMapSize, blockSize, grid.Length);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        blankTexture = new Texture2D(GraphicsDevice, 1, 1);
        blankTexture.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        //player inputs
        var keystate = Keyboard.GetState();
        //4-directional movement
        //forward
        if (keystate.IsKeyDown(Keys.W))
        {
            player.Move('f', blocks);
        }
        //backward
        if (keystate.IsKeyDown(Keys.S))
        {
            player.Move('b', blocks);
        }
        //strafe left
        if (keystate.IsKeyDown(Keys.A))
        {
            player.Move('l', blocks);
        }
        //strafe right
        if (keystate.IsKeyDown(Keys.D))
        {
            player.Move('r', blocks);
        }

        //rotation
        //rotate left
        if (keystate.IsKeyDown(Keys.Left))
        {
            player.Rotate('l');
        }
        //rotate right
        if (keystate.IsKeyDown(Keys.Right))
        {
            player.Rotate('r');
        }

        rayLengths = caster.CastRays(player, blocks);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        DrawRays();
        miniMap.DrawMap(_spriteBatch, blankTexture, player, fov, rays, rayLengths, blocks);
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    //draw rays method
    private void DrawRays()
    {
        for (int i = 0; i < rayLengths.Count; i++)
        {
            //creating ray colour
            var hue = Convert.ToInt32(255 * (i / rays));
            var colour = new Color(255 - hue, 0, hue);

            //ray dimensions
            var rayLength = maxLength - rayLengths[i][0]; //isnt used for drawing the ray, only for finding the rays height
            var rayHeight = Convert.ToInt32(screenHeight * (rayLength / (float)maxLength)) - 110; //the decrement at the end is to make the rays shorter (personal preference)
            var rayX = i * rayWidth;
            var rayY = (screenHeight / 2) - (rayHeight / 2);
            Rectangle backRay = new Rectangle(rayX, rayY, rayWidth, rayHeight); //the coloured ray that will be covered by a shorter black ray
            _spriteBatch.Draw(blankTexture, new Vector2(rayX, rayY), backRay, colour, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);

            //drawing a second shorter ray to cover the first (only if it is not a corner)
            if (rayLengths[i][1] == 0)
            {
                rayHeight -= 12;
                rayY = (screenHeight / 2) - (rayHeight / 2);
                Rectangle frontRay = new Rectangle(rayX, rayY, rayWidth, rayHeight);
                _spriteBatch.Draw(blankTexture, new Vector2(rayX, rayY), frontRay, Color.Black, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            }
        }
    }
}