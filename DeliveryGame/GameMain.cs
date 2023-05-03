using DeliveryGame.Core;
using DeliveryGame.Elements;
using DeliveryGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

using static DeliveryGame.Core.ContentLibrary.Keys;

namespace DeliveryGame;

public class GameMain : Game
{
#pragma warning disable IDE0052
    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
#pragma warning restore IDE0052

    private World world;
    private UserInterface userInterface;

    public GameMain()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += (sender, e) =>
        {
            Camera.Instance.Update(GraphicsDevice);
        };
    }

    protected override void Initialize()
    {
        Camera.Instance.Initialize(GraphicsDevice);

        world = new World(Constants.MapWidth, Constants.MapHeight);
        userInterface = new UserInterface();

        GameState.Current.Initialize(this);
        GameState.Current.World = world;

        for (int y = 0; y < Constants.MapHeight; y++)
        {
            for (int x = 0; x < Constants.MapWidth; x++)
            {
                Tile tile = new(x, y);

                tile.Type = TileType.Grass;

                world.SetTile(tile);
            }
        }

        GameState.Current.CurrentQuest = Quest.QuestQueue.Dequeue();

        base.Initialize();

        Window.Title = "Conveyor Delivery Quest";
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        ContentLibrary.Instance.Load(Content, GraphicsDevice);
        world.Load(ContentLibrary.Textures[TextureMap]);
    }

    protected override void Update(GameTime gameTime)
    {
        if (!IsActive)
            return;

        InputState.Instance.Update();

        var keyboardState = InputState.Instance.KeyboardState;
        var mouseState = InputState.Instance.MouseState;

        userInterface.Update();
        if (!GameState.Current.IsGamePaused)
        {
            world.Update(gameTime);
            ParticleSystem.UpdateParticleSystems(gameTime);
            HandleScrolling(mouseState, keyboardState);
            HandleZoom(mouseState);
        }

        GameState.Current.InvokeGlobalUpdate(gameTime);

        WareHandler.ResetHandledWares();

        base.Update(gameTime);
    }

    private static void HandleZoom(MouseState mouseState)
    {
        if (mouseState.ScrollWheelValue == 0)
        {
            Camera.Instance.ZoomFactor = 1;
        }
        else if (mouseState.ScrollWheelValue > 0)
        {
            var factor = mouseState.ScrollWheelValue / 120f;

            Camera.Instance.ZoomFactor = (float)Math.Pow(1.05f, factor);
        }
        else if (mouseState.ScrollWheelValue < 0)
        {
            var factor = -mouseState.ScrollWheelValue / 120f;

            Camera.Instance.ZoomFactor = (float)Math.Pow(0.95f, factor);
        }
    }

    private void HandleScrolling(MouseState mouseState, KeyboardState keyboardState)
    {
        Rectangle leftScrollArea = new()
        {
            X = 0,
            Y = 0,
            Width = Constants.ScrollBorderWidth,
            Height = Camera.Instance.ViewportHeight
        };

        Rectangle rightScrollArea = new()
        {
            X = Camera.Instance.ViewportWidth - Constants.ScrollBorderWidth,
            Y = 0,
            Width = Constants.ScrollBorderWidth,
            Height = Camera.Instance.ViewportHeight
        };

        Rectangle downScrollArea = new()
        {
            X = 0,
            Y = Camera.Instance.ViewportHeight - Constants.ScrollBorderWidth,
            Width = Camera.Instance.ViewportWidth,
            Height = Constants.ScrollBorderWidth
        };

        Rectangle upScrollArea = new()
        {
            X = 0,
            Y = 0,
            Width = Camera.Instance.ViewportWidth,
            Height = Constants.ScrollBorderWidth
        };

        var speed = (Constants.ScrollSpeed / Camera.Instance.ZoomFactor);

        if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A) || (!userInterface.IsMouseOnUI && leftScrollArea.Contains(mouseState.Position)))
        {
            Camera.Instance.OffsetX += speed;
        }
        else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D) || (!userInterface.IsMouseOnUI && rightScrollArea.Contains(mouseState.Position)))
        {
            Camera.Instance.OffsetX -= speed;
        }

        if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W) || (!userInterface.IsMouseOnUI && upScrollArea.Contains(mouseState.Position)))
        {
            Camera.Instance.OffsetY += speed;
        }
        else if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S) || (!userInterface.IsMouseOnUI && downScrollArea.Contains(mouseState.Position)))
        {
            Camera.Instance.OffsetY -= speed;
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp ,rasterizerState: RasterizerState.CullCounterClockwise);

        foreach (var renderable in RenderPool.Instance.OrderBy(x => x.ZIndex))
        {
            renderable.Render(GraphicsDevice, spriteBatch, gameTime);
        }
        
        spriteBatch.End();

        base.Draw(gameTime);
    }
}
