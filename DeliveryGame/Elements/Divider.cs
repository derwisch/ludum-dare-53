using DeliveryGame.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryGame.Elements
{
    internal class Divider : StaticElement
    {
        private double cooldown = 0;
        private int rotation = 0;
        public Divider(Tile parent) : base(parent)
        {
            WareHandler = new WareHandler(1, 3, new[] { InputSide }, GetOutputSides(), parent);
            UI.InputState.Instance.KeyPressed += InputKeyPressed;
            DisplayName = "Divider";
            IsRemoveable = true;
        }

        public override string Info
        {
            get
            {
                string result = "Currently transporting:";

                int i = 0;
                foreach (var ware in WareHandler.Storage.Cast<Ware>().Where(x => x != null))
                {
                    var name = UI.UserInterface.GetWareDisplayName(ware.Type);
                    result += $"\nInput {++i}: {name}";
                }
                i = 0;
                foreach (var ware in WareHandler.Output.Cast<Ware>().Where(x => x != null))
                {
                    var name = UI.UserInterface.GetWareDisplayName(ware.Type);
                    result += $"\nOutput {++i}: {name}";
                }

                result += "\n";
                result += "\n[Q] to rotate left";
                result += "\n[E] to rotate right";

                return result;
            }
        }

        public Side InputSide => rotation switch
        {
            0 => Side.Bottom,
            90 => Side.Left,
            180 => Side.Top,
            270 => Side.Right,
            _ => Side.Bottom
        };

        public IEnumerable<Side> OutputSides => GetOutputSides();
        public override int ZIndex => Constants.LayerBuildables;

        protected override Lazy<Texture2D> TextureProducer { get; set; } = new(() => ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureDivider));
        public override void Render(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
        {
            var texture = TextureProducer.Value;
            var textureOrigin = texture.Bounds.Center.ToVector2();
            var rotation = MathHelper.ToRadians(this.rotation);
            var textureBaseRect = StaticElementArea;
            var textureRect = new Rectangle()
            {
                X = textureBaseRect.X + (int)(textureOrigin.X * Camera.Instance.ZoomFactor),
                Y = textureBaseRect.Y + (int)(textureOrigin.Y * Camera.Instance.ZoomFactor),
                Width = textureBaseRect.Width,
                Height = textureBaseRect.Height
            };

            spriteBatch.Draw(TextureProducer.Value, textureRect, null, Color.White, rotation, textureOrigin, SpriteEffects.None, 0);
        }

        public override void Update(GameTime gameTime)
        {
            if (cooldown <= 0)
            {
                WareHandler.UpdateSlots();
                cooldown = Constants.ConveyorCooldown;
            }
            else
            {
                cooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            foreach (var ware in WareHandler.Output.Where(x => x != null))
            {
                ware.IsVisible = false;
            }
            foreach (var ware in WareHandler.Storage.Where(x => x != null))
            {
                ware.IsVisible = false;
            }
        }

        internal override void CleanUp()
        {
            UI.InputState.Instance.KeyPressed -= InputKeyPressed;
            WareHandler.CleanUp();
        }

        private Side[] GetOutputSides() => rotation switch
        {
            0 => new[] { Side.Left, Side.Top, Side.Right },
            90 => new[] { Side.Top, Side.Right, Side.Bottom },
            180 => new[] { Side.Left, Side.Right, Side.Bottom },
            270 => new[] { Side.Left, Side.Top, Side.Bottom },
            _ => Array.Empty<Side>(),
        };

        private void InputKeyPressed(Keys key)
        {
            if (GameState.Current.HoveredTile != parent)
                return;

            if (key == Keys.Q)
            {
                rotation += 90;
                rotation %= 360;
            }
            else if (key == Keys.E)
            {
                rotation -= 90;
                rotation += 360;
                rotation %= 360;
            }
            WareHandler.UpdateInputSides(new[] { InputSide });
            WareHandler.UpdateOutputSides(GetOutputSides());
        }
    }
}