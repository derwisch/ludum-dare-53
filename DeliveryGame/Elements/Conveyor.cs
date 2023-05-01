using DeliveryGame.Core;
using DeliveryGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryGame.Elements
{
    internal class Conveyor : StaticElement
    {
        private static readonly Rectangle arrowSource = new(0, 0, 9, 9);

        private readonly Lazy<Dictionary<string, Texture2D>> textures = new(() => new()
        {
            { "invalid", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureChecker) },
            { "dir", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureDirectionArrow) },

            { "we", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureConveyorHorizontal) },
            { "ew", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureConveyorHorizontal) },

            { "ns", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureConveyorVertical) },
            { "sn", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureConveyorVertical) },

            { "ws", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureConveyorBottomLeft) },
            { "sw", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureConveyorBottomLeft) },

            { "es", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureConveyorBottomRight) },
            { "se", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureConveyorBottomRight) },

            { "wn", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureConveyorTopLeft) },
            { "nw", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureConveyorTopLeft) },

            { "en", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureConveyorTopRight) },
            { "ne", ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureConveyorTopRight) }
        });

        private double cooldown = 0;
        public Conveyor(Tile parent, Side inputSide, Side outputSide) : base(parent)
        {
            InputSide = inputSide;
            OutputSide = outputSide;
            WareHandler = new WareHandler(1, 1, new[] { inputSide }, new[] { outputSide }, parent);
            InputState.Instance.KeyPressed += InputKeyPressed;
            DisplayName = "Conveyor Belt";
            IsRemoveable = true;
        }

        public override string Info
        {
            get
            {
                string result = "Currently transporting:";

                if (WareHandler.Storage.FirstOrDefault() is Ware storageWare)
                {
                    var name = UI.UserInterface.GetWareDisplayName(storageWare.Type);
                    result += $"\n {name}";
                }
                if (WareHandler.Output.FirstOrDefault() is Ware outputWare)
                {
                    var name = UI.UserInterface.GetWareDisplayName(outputWare.Type);
                    result += $"\n {name}";
                }

                result += "\n";
                result += "\n[Q] to rotate input";
                result += "\n[E] to rotate output";
                result += "\n[R] to rotate";
                result += "\n[F] to show/hide directions";

                return result;
            }
        }

        public Side InputSide { get; set; }
        public Side OutputSide { get; set; }
        public override int ZIndex => Constants.LayerBuildables;
        private int DirectionArrowHeight => textures.Value["dir"].Height;

        private int DirectionArrowWidth => textures.Value["dir"].Width;

        public override void Render(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
        {
            var conveyorTexture = textures.Value[GenerateTextureKey()];

            spriteBatch.Draw(conveyorTexture, StaticElementArea, Color.White);

            if (GameState.Current.DirectionArrowsVisible)
            {
                var arrowTexture = textures.Value["dir"];
                var arrowOrigin = new Vector2(DirectionArrowWidth / 2f, DirectionArrowHeight / 2f);

                var inputRotation = GetSideRotation(InputSide, false);
                var inputArrowRect = GetSideRectangle(InputSide);

                spriteBatch.Draw(arrowTexture, inputArrowRect, arrowSource, Color.Green, inputRotation, arrowOrigin, SpriteEffects.None, 0);

                var outputRotation = GetSideRotation(OutputSide, true);
                var outputArrowRect = GetSideRectangle(OutputSide);

                spriteBatch.Draw(arrowTexture, outputArrowRect, arrowSource, Color.Red, outputRotation, arrowOrigin, SpriteEffects.None, 0);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (cooldown <= 0)
            {
                UpdateSlots();
                cooldown = Constants.ConveyorCooldown;
            }
            else
            {
                cooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            UpdateWarePositions();
        }

        internal override void CleanUp()
        {
            InputState.Instance.KeyPressed -= InputKeyPressed;
            WareHandler.CleanUp();
        }

        private static float GetSideRotation(Side side, bool isOutput)
        {
            var deg = side switch
            {
                Side.Top => 90,
                Side.Right => 180,
                Side.Bottom => 270,
                Side.Left => 0,
                _ => 0
            };
            deg += isOutput ? 180 : 0;
            deg %= 360;
            return MathHelper.ToRadians(deg);
        }

        private string GenerateTextureKey()
        {
            var inSide = InputSide switch
            {
                Side.Top => "n",
                Side.Right => "e",
                Side.Bottom => "s",
                Side.Left => "w",
                _ => "w"
            };
            var outSide = OutputSide switch
            {
                Side.Top => "n",
                Side.Right => "e",
                Side.Bottom => "s",
                Side.Left => "w",
                _ => "e"
            };

            if (inSide == outSide)
            {
                return "invalid";
            }

            return inSide + outSide;
        }

        private Rectangle GetSideRectangle(Side side)
        {
            float offsetX = side switch
            {
                Side.Top or Side.Bottom => (Constants.TileWidth / 2),
                Side.Right => Constants.TileWidth - (DirectionArrowWidth / 2),
                Side.Left => (DirectionArrowWidth / 2),
                _ => 0
            };

            float offsetY = side switch
            {
                Side.Top => (DirectionArrowHeight / 2),
                Side.Bottom => Constants.TileHeight - (DirectionArrowHeight / 2),
                Side.Right or Side.Left => (Constants.TileHeight / 2),
                _ => 0
            };

            float width = DirectionArrowWidth * Camera.Instance.ZoomFactor;
            float height = DirectionArrowHeight * Camera.Instance.ZoomFactor;

            float x = ((tileX * Constants.TileWidth) + offsetX + Camera.Instance.OffsetX) * Camera.Instance.ZoomFactor + (Camera.Instance.ViewportWidth / 2);
            float y = ((tileY * Constants.TileHeight) + offsetY + Camera.Instance.OffsetY) * Camera.Instance.ZoomFactor + (Camera.Instance.ViewportHeight / 2);

            return new()
            {
                Location = new Point((int)Math.Floor(x), (int)Math.Floor(y)),
                Size = new Point((int)Math.Ceiling(width), (int)Math.Ceiling(height))
            };
        }

        private void InputKeyPressed(Keys key)
        {
            if (GameState.Current.HoveredTile != parent)
                return;

            if (key == Keys.Q)
            {
                InputSide = (Side)(((int)InputSide + 1) % 4);
                if (GenerateTextureKey() == "invalid")
                {
                    InputSide = (Side)(((int)InputSide + 1) % 4);
                }
                WareHandler.UpdateInputSides(new[] { InputSide });
            }
            else if (key == Keys.E)
            {
                OutputSide = (Side)(((int)OutputSide + 1) % 4);
                if (GenerateTextureKey() == "invalid")
                {
                    OutputSide = (Side)(((int)OutputSide + 1) % 4);
                }
                WareHandler.UpdateOutputSides(new[] { OutputSide });
            }
            else if (key == Keys.R)
            {
                OutputSide = (Side)(((int)OutputSide + 3) % 4);
                InputSide = (Side)(((int)InputSide + 3) % 4);
                WareHandler.UpdateInputSides(new[] { InputSide });
                WareHandler.UpdateOutputSides(new[] { OutputSide });
            }
        }
        private void UpdateSlots()
        {
            WareHandler.UpdateSlots();
            UpdateWarePositions();
        }

        private void UpdateWarePositions()
        {
            var slotOutput = WareHandler.Output.FirstOrDefault();
            var slotStorage = WareHandler.Storage.FirstOrDefault();

            if (slotOutput != null)
            {
                slotOutput.IsVisible = true;
                float offsetX = OutputSide switch
                {
                    Side.Top or Side.Bottom => (Constants.TileWidth / 2),
                    Side.Right => Constants.TileWidth - (DirectionArrowWidth / 2),
                    Side.Left => (DirectionArrowWidth / 2),
                    _ => 0
                };

                float offsetY = OutputSide switch
                {
                    Side.Top => (DirectionArrowHeight / 2),
                    Side.Bottom => Constants.TileHeight - (DirectionArrowHeight / 2),
                    Side.Right or Side.Left => (Constants.TileHeight / 2),
                    _ => 0
                };

                slotOutput.X = (tileX * Constants.TileWidth) + offsetX;
                slotOutput.Y = (tileY * Constants.TileHeight) + offsetY;
            }

            if (slotStorage != null)
            {
                slotStorage.IsVisible = true;
                slotStorage.X = tileX * Constants.TileWidth + (0.5f * Constants.TileWidth);
                slotStorage.Y = tileY * Constants.TileHeight + (0.5f * Constants.TileHeight);
            }
        }
    }
}