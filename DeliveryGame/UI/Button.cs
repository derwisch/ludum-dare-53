using DeliveryGame.Core;
using Microsoft.Xna.Framework;
using System;

using static DeliveryGame.Core.ContentLibrary.Keys;

namespace DeliveryGame.UI
{
    public class Button
    {
        public Button()
        {
            InputState.Instance.LeftClick += MouseLeftClick;
        }

        public Rectangle ButtonArea => new()
        {
            X = X,
            Y = Y,
            Width = Width,
            Height = Height
        };

        public bool IsClickable { get; set; }
        public int Width { get; set; } = Constants.ButtonWidth;
        public int Height { get; set; } = Constants.ButtonHeight;
        public string IconKey { get; set; }
        public int Index { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; init; }
        public Action OnClick { get; set; }
        public UIButtonState State { get; set; } = UIButtonState.Up;
        public int X { get; set; }
        public int Y { get; set; }

        private void MouseLeftClick()
        {
            if (IsClickable && ButtonArea.Contains(InputState.Instance.MouseState.Position))
            {
                OnClick?.Invoke();
                ContentLibrary.SoundEffects[SoundEffectClick].Play(0.5f, 0, 0);
            }
        }
    }
}