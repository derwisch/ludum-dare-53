using DeliveryGame.Core;
using Microsoft.Xna.Framework;
using System;

namespace DeliveryGame.UI
{
    internal class Button
    {
        public Button()
        {
            InputState.Instance.LeftClick += MouseLeftClick;
        }

        public Rectangle ButtonArea => new()
        {
            X = X,
            Y = Y,
            Width = Constants.ButtonWidth,
            Height = Constants.ButtonHeight
        };

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
            if (ButtonArea.Contains(InputState.Instance.MouseState.Position))
            {
                OnClick?.Invoke();
            }
        }
    }
}