using DeliveryGame.Core;
using DeliveryGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

using static DeliveryGame.Core.ContentLibrary.Keys;

namespace DeliveryGame.Elements
{
    public class Conveyor : StaticElement
    {
        private static readonly Rectangle arrowSource = new(0, 0, 9, 9);

        private readonly Lazy<Texture2D> arrowTexture = new(() => ContentLibrary.Textures[TextureDirectionArrow]);
        private readonly Lazy<Dictionary<string, AnimatedTexture>> textures = new(() => new()
        {
            { "n", ContentLibrary.Animations[TextureConveyorVerticalTop] },
            { "s", ContentLibrary.Animations[TextureConveyorVerticalBottom] },
            { "w", ContentLibrary.Animations[TextureConveyorHorizontalLeft] },
            { "e", ContentLibrary.Animations[TextureConveyorHorizontalRight] },

            { "we", ContentLibrary.Animations[TextureConveyorConnectorHorizontal] },
            { "ew", ContentLibrary.Animations[TextureConveyorConnectorHorizontal] },

            { "ns", ContentLibrary.Animations[TextureConveyorConnectorVertical] },
            { "sn", ContentLibrary.Animations[TextureConveyorConnectorVertical] },

            { "ws", ContentLibrary.Animations[TextureConveyorConnectorBottomLeft] },
            { "sw", ContentLibrary.Animations[TextureConveyorConnectorBottomLeft] },

            { "es", ContentLibrary.Animations[TextureConveyorConnectorBottomRight] },
            { "se", ContentLibrary.Animations[TextureConveyorConnectorBottomRight] },

            { "wn", ContentLibrary.Animations[TextureConveyorConnectorTopLeft] },
            { "nw", ContentLibrary.Animations[TextureConveyorConnectorTopLeft] },

            { "en", ContentLibrary.Animations[TextureConveyorConnectorTopRight] },
            { "ne", ContentLibrary.Animations[TextureConveyorConnectorTopRight] }
        });

        private int cooldownIteration = 0;
        private double outputTransportAge = 0;
        private double storageTransportAge = 0;
        public Conveyor(Tile parent, Side inputSide, Side outputSide) : base(parent)
        {
            DisplayName = "Conveyor Belt";
            IsRemoveable = true;
            InputSide = inputSide;
            OutputSide = outputSide;
            InputState.Instance.KeyPressed += InputKeyPressed;
            WareHandler = new WareHandler(1, 1, new[] { inputSide }, new[] { outputSide }, parent);
            WareHandler.OutputSlotChanged += () =>
            {
                outputTransportAge = GameState.Current.ConveyorCooldown;
            };
            WareHandler.StorageSlotChanged += () =>
            {
                storageTransportAge = GameState.Current.ConveyorCooldown;
            };
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
        private int DirectionArrowHeight => arrowTexture.Value.Height;

        private int DirectionArrowWidth => arrowTexture.Value.Width;

        public override void Render(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
        {
            var connectorTexture = textures.Value[GenerateTextureKey()];
            var inputTexture = textures.Value[GetSideKey(InputSide)];
            var outputTexture = textures.Value[GetSideKey(OutputSide)];

            var reverseConnector = InputSide switch
            {
                Side.Top => true,
                Side.Right => OutputSide != Side.Top,
                Side.Bottom => OutputSide == Side.Left,
                Side.Left => false,
                _ => false
            };
            var reverseInput = InputSide switch
            {
                Side.Top => true,
                Side.Right => true,
                Side.Bottom => false,
                Side.Left => false,
                _ => false
            };
            var reverseOutput = OutputSide switch
            {
                Side.Top => false,
                Side.Right => false,
                Side.Bottom => true,
                Side.Left => true,
                _ => false
            };

            spriteBatch.Draw(reverseInput ? inputTexture.Reversed : inputTexture, StaticElementArea, Color.White);
            spriteBatch.Draw(reverseOutput ? outputTexture.Reversed : outputTexture, StaticElementArea, Color.White);
            spriteBatch.Draw(reverseConnector ? connectorTexture.Reversed : connectorTexture, StaticElementArea, Color.White);

            if (GameState.Current.DirectionArrowsVisible)
            {
                var arrowOrigin = new Vector2(DirectionArrowWidth / 2f, DirectionArrowHeight / 2f);

                var inputRotation = GetSideRotation(InputSide, false);
                var inputArrowRect = GetSideRectangle(InputSide);

                spriteBatch.Draw(arrowTexture.Value, inputArrowRect, arrowSource, Color.Green, inputRotation, arrowOrigin, SpriteEffects.None, 0);

                var outputRotation = GetSideRotation(OutputSide, true);
                var outputArrowRect = GetSideRectangle(OutputSide);

                spriteBatch.Draw(arrowTexture.Value, outputArrowRect, arrowSource, Color.Red, outputRotation, arrowOrigin, SpriteEffects.None, 0);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (outputTransportAge > 0)
            {
                outputTransportAge -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            if (storageTransportAge > 0)
            {
                storageTransportAge -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            var currentCooldownIteration = (int)(gameTime.TotalGameTime.TotalMilliseconds / GameState.Current.ConveyorCooldown);
            if (cooldownIteration < currentCooldownIteration)
            {
                cooldownIteration = currentCooldownIteration;
                WareHandler.Update();
            }
            UpdateWarePositions();
        }

        public override void CleanUp()
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

        private static string GetSideKey(Side side) => side switch
        {
            Side.Top => "n",
            Side.Right => "e",
            Side.Bottom => "s",
            Side.Left => "w",
            _ => "w"
        };

        private string GenerateTextureKey()
        {
            var inSide = GetSideKey(InputSide);
            var outSide = GetSideKey(OutputSide);

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

        private void UpdateWarePositions()
        {
            var storageFactor = 1 - (float)(storageTransportAge / GameState.Current.ConveyorCooldown);
            var outputFactor = 1 - (float)(outputTransportAge / GameState.Current.ConveyorCooldown);

            var (centerX, centerY) = (0.5f * Constants.TileWidth, 0.5f * Constants.TileHeight);
            var (inputX, inputY) = sidePoint(InputSide);
            var (outputX, outputY) = sidePoint(OutputSide);

            var storageSlotOffset = (x: MathHelper.Lerp(inputX, centerX, storageFactor), y: MathHelper.Lerp(inputY, centerY, storageFactor));
            var outputSlotOffset = (x: MathHelper.Lerp(centerX, outputX, outputFactor), y: MathHelper.Lerp(centerY, outputY, outputFactor));

            var slotStorage = WareHandler.Storage.FirstOrDefault();
            var slotOutput = WareHandler.Output.FirstOrDefault();

            if (slotStorage != null)
            {
                slotStorage.IsVisible = true;
                var (offsetX, offsetY) = storageSlotOffset;
                slotStorage.X = tileX * Constants.TileWidth + offsetX;
                slotStorage.Y = tileY * Constants.TileHeight + offsetY;
            }

            if (slotOutput != null)
            {
                slotOutput.IsVisible = true;
                var (offsetX, offsetY) = outputSlotOffset;
                slotOutput.X = (tileX * Constants.TileWidth) + offsetX;
                slotOutput.Y = (tileY * Constants.TileHeight) + offsetY;
            }

            static (float x, float y) sidePoint(Side side)
            {
                return
                (
                    side switch { Side.Top or Side.Bottom => (Constants.TileWidth / 2), Side.Right => Constants.TileWidth, Side.Left => 0, _ => 0 },
                    side switch { Side.Top => 0, Side.Bottom => Constants.TileHeight, Side.Right or Side.Left => (Constants.TileHeight / 2), _ => 0 }
                );
            }
        }
    }
}