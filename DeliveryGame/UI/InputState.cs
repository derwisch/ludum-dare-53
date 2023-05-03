using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace DeliveryGame.UI
{
    public class InputState
    {
        private static readonly Keys[] relevantKeys = new[]
        {
            // Rotating stuff
            Keys.Q,
            Keys.E,
            Keys.R,

            // UI
            Keys.F,
            Keys.Escape
        };

        public static InputState Instance => instance.Value;
        private static readonly Lazy<InputState> instance = new(() => new());

        private readonly Dictionary<Keys, KeyState> keyboardButtons = new();
        private ButtonState leftMouseState = ButtonState.Released;
        private ButtonState rightMouseState = ButtonState.Released;

        private InputState()
        {
            foreach (var key in relevantKeys)
            {
                keyboardButtons[key] = KeyState.Up;
            }
        }

        public event Action<Keys> KeyPressed;

        public event Action LeftClick;

        public event Action RightClick;

        public KeyboardState KeyboardState { get; private set; }

        public MouseState MouseState { get; private set; }

        public void Update()
        {
            MouseState = Mouse.GetState();
            KeyboardState = Keyboard.GetState();

            HandleRelevantButtons();
            HandleRelevantKeys();
        }
        private void HandleRelevantButtons()
        {
            if (leftMouseState == ButtonState.Pressed && MouseState.LeftButton == ButtonState.Released)
            {
                LeftClick?.Invoke();
            }
            leftMouseState = MouseState.LeftButton;

            if (rightMouseState == ButtonState.Pressed && MouseState.RightButton == ButtonState.Released)
            {
                RightClick?.Invoke();
            }
            rightMouseState = MouseState.RightButton;
        }

        private void HandleRelevantKeys()
        {
            foreach (var key in relevantKeys)
            {
                var newState = KeyboardState[key];

                if (keyboardButtons[key] == KeyState.Down && newState == KeyState.Up)
                {
                    KeyPressed?.Invoke(key);
                }
                keyboardButtons[key] = newState;
            }
        }
    }
}