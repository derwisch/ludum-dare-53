using DeliveryGame.Core;
using DeliveryGame.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

using static DeliveryGame.Core.ContentLibrary.Keys;

namespace DeliveryGame.UI
{
    public class UserInterface : IRenderable
    {
        private static Rectangle PauseScreenArea => new()
        {
            X = 0,
            Y = 0,
            Width = Camera.Instance.ViewportWidth,
            Height = Camera.Instance.ViewportHeight
        };

        private static readonly Color pauseScreenColor = new(Color.Black, 127);

        private static readonly Lazy<Texture2D> textureActiveBorder = new(() => ContentLibrary.Textures[TextureActiveBorder]);

        private static readonly Lazy<Texture2D> textureBuildables = new(() => ContentLibrary.Textures[TextureBuildablesUI]);

        private static readonly Lazy<Texture2D> textureButtonDown = new(() => ContentLibrary.Textures[TextureButtonDown]);

        private static readonly Lazy<Texture2D> textureButtonHover = new(() => ContentLibrary.Textures[TextureButtonHover]);

        private static readonly Lazy<Texture2D> textureButtonUp = new(() => ContentLibrary.Textures[TextureButtonUp]);

        private static readonly Lazy<Texture2D> textureMenu = new(() => ContentLibrary.Textures[TextureMenuUI]);

        private static readonly Lazy<Texture2D> textureWhitePixel = new(() => ContentLibrary.Textures[TextureWhitePixel]);

        private static readonly Lazy<Texture2D> textureSplash = new(() => ContentLibrary.Textures[TextureSplash]);

        private static readonly Lazy<Texture2D> textureButtonMenuDown = new(() => ContentLibrary.Textures[TextureMenuButtonDown]);

        private static readonly Lazy<Texture2D> textureButtonMenuHover = new(() => ContentLibrary.Textures[TextureMenuButtonHover]);

        private static readonly Lazy<Texture2D> textureButtonMenuUp = new(() => ContentLibrary.Textures[TextureMenuButtonUp]);

        private readonly List<Button> buttonsBuildables = new();

        private readonly List<Button> buttonsMenu = new();

        private Button buttonConveyor;

        private Button buttonDivider;

        private Button buttonMerger;

        private Button buttonRecipes;

        private Button buttonRequests;

        private Button buttonSelect;

        private Button buttonQuit;

        private bool isBuildablesFolded = false;

        private bool isMenuFolded = false;

        private Window recipesWindow;

        private Window requestsWindow;

        private Window selectionWindow;

        public UserInterface()
        {
            RenderPool.Instance.RegisterRenderable(this);
            InputState.Instance.KeyPressed += InputKeyPressed;
            InputState.Instance.LeftClick += MouseLeftClick;
            InputState.Instance.RightClick += MouseRightClick;
        }

        public bool IsMouseOnUI => UIAreas.Concat(new[] { recipesWindow.IsVisible ? recipesWindow.WindowArea : Rectangle.Empty,
                                                          requestsWindow.IsVisible ? requestsWindow.WindowArea : Rectangle.Empty,
                                                          selectionWindow.IsVisible ? selectionWindow.WindowArea : Rectangle.Empty })
                                          .Any(x => x.Contains(InputState.Instance.MouseState.Position));

        public int ZIndex => Constants.LayerUI;

        private Rectangle BuildablesArea => new()
        {
            X = 0,
            Y = Camera.Instance.ViewportHeight - (isBuildablesFolded ? 18 : textureBuildables.Value.Height),
            Width = textureBuildables.Value.Width,
            Height = textureBuildables.Value.Height
        };

        private Rectangle BuildablesFoldArea => new()
        {
            X = 163,
            Y = Camera.Instance.ViewportHeight - (isBuildablesFolded ? 18 : textureBuildables.Value.Height),
            Width = 18,
            Height = 18
        };

        private Rectangle MenuArea => new()
        {
            X = Camera.Instance.ViewportWidth - textureMenu.Value.Width,
            Y = Camera.Instance.ViewportHeight - (isMenuFolded ? 18 : textureMenu.Value.Height),
            Width = textureMenu.Value.Width,
            Height = textureMenu.Value.Height
        };

        private Rectangle MenuFoldArea => new()
        {
            X = Camera.Instance.ViewportWidth - 18,
            Y = Camera.Instance.ViewportHeight - (isMenuFolded ? 18 : textureMenu.Value.Height),
            Width = 18,
            Height = 18
        };

        private IEnumerable<Rectangle> UIAreas => new[] { BuildablesArea, MenuArea };

        public static string GetTileDisplayName(TileType? type)
        {
            if (type == null)
            {
                return "";
            }

            return type switch
            {
                TileType.Grass => "",
                TileType.DepositCoal => "Coal Deposit",
                TileType.DepositIron => "Iron Ore Deposit",
                TileType.DepositCopper => "Copper Ore Deposit",
                TileType.DepositSilicon => "Silicon Deposit",
                TileType.DepositOil => "Oil Deposit",
                _ => ""
            };
        }

        public static string GetWareDisplayName(WareType? type)
        {
            if (type == null)
            {
                return "";
            }

            return type switch
            {
                WareType.Coal => "Coal",
                WareType.IronOre => "Iron Ore",
                WareType.IronBar => "Iron Bar",
                WareType.CopperOre => "Copper Ore",
                WareType.CopperBar => "Copper Bar",
                WareType.Silicon => "Silicon",
                WareType.Oil => "Oil",
                WareType.Plastic => "Plastic",
                WareType.Gear => "Gear",
                WareType.CopperCoil => "Copper Coil",
                WareType.Circuit => "Circuit",
                WareType.Motor => "Motor",
                WareType.Computer => "Computer",
                WareType.Robot => "Robot",
                _ => ""
            };
        }

        public void Render(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(textureBuildables.Value, BuildablesArea, Color.White);
            spriteBatch.Draw(textureMenu.Value, MenuArea, Color.White);

            if (!isBuildablesFolded)
            {
                DrawButtons(spriteBatch, buttonsBuildables);
            }
            if (!isMenuFolded)
            {
                DrawButtons(spriteBatch, buttonsMenu);
            }

            spriteBatch.DrawString(ContentLibrary.Instance.Font, GameState.Current.HoveredTile?.Building?.DisplayName ?? "", new Vector2(5, 5), Color.White);
            spriteBatch.DrawString(ContentLibrary.Instance.Font, GetTileDisplayName(GameState.Current.HoveredTile?.Type), new Vector2(5, 20), Color.White);

            if (GameState.Current.IsGamePaused)
            {
                DrawPauseMenu(spriteBatch, gameTime);
            }
        }

        private void DrawPauseMenu(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Rectangle splashRect = new()
            {
                X = (Camera.Instance.ViewportWidth / 2) - (textureSplash.Value.Width / 2),
                Y = (Camera.Instance.ViewportHeight / 2) - (textureSplash.Value.Height / 2),
                Width = textureSplash.Value.Width,
                Height = textureSplash.Value.Height
            };

            spriteBatch.Draw(textureWhitePixel.Value, PauseScreenArea, pauseScreenColor);
            spriteBatch.Draw(textureSplash.Value, splashRect, Color.White);

            var font = ContentLibrary.Instance.TitleFont;
            var buttonTexture = buttonQuit.State switch
            {
                UIButtonState.Up => textureButtonMenuUp.Value,
                UIButtonState.Down => textureButtonMenuDown.Value,
                UIButtonState.Hover => textureButtonMenuHover.Value,
                _ => textureButtonUp.Value,
            };
            var buttonRect = new Rectangle()
            {
                X = (Camera.Instance.ViewportWidth / 2) - (buttonTexture.Width / 2),
                Y = Camera.Instance.ViewportHeight - (buttonTexture.Height * 2),
                Width = buttonTexture.Width,
                Height = buttonTexture.Height
            };
            buttonRect.Width = buttonTexture.Width;
            buttonRect.Height = buttonTexture.Height;

            var vec = font.MeasureString("Quit");
            var textPos = new Vector2()
            {
                X = buttonRect.Center.X - (vec.X / 2),
                Y = buttonRect.Center.Y - (vec.Y / 2)
            };

            spriteBatch.Draw(buttonTexture, buttonRect, Color.White);
            spriteBatch.DrawString(font, "Quit", textPos, Color.Black);
        }

        public void Update()
        {
            if (!buttonsBuildables.Any())
            {
                InitializeUI();
            }
            else
            {
                UpdateButtonLocations();
                foreach (var button in buttonsMenu.Concat(buttonsBuildables))
                {
                    var area = button.ButtonArea;

                    UpdateButton(button, area);
                }
                var quitButtonRect = new Rectangle()
                {
                    X = (Camera.Instance.ViewportWidth / 2) - (textureButtonMenuUp.Value.Width / 2),
                    Y = Camera.Instance.ViewportHeight - (textureButtonMenuUp.Value.Height * 2),
                    Width = textureButtonMenuUp.Value.Width,
                    Height = textureButtonMenuUp.Value.Height
                };
                UpdateButton(buttonQuit, quitButtonRect);
            }

            if (requestsWindow.IsVisible && GameState.Current.CurrentQuest != null)
            {
                var quest = GameState.Current.CurrentQuest;

                if (quest.RequestedWares.Any())
                {
                    requestsWindow.Text = "Requested Wares:";
                    foreach (var (count, type) in quest.RequestedWares)
                    {
                        var deliveredAmount = quest.DeliveredWares[type];
                        var name = GetWareDisplayName(type);
                        requestsWindow.Text += $"\n({deliveredAmount}/{count}) {name}";
                    }
                }
                else
                {
                    requestsWindow.Text = "Congratulations!";
                    requestsWindow.Text += "\nYou delivered all the requests";
                }

            }

            if (selectionWindow.IsVisible)
            {
                var building = GameState.Current.SelectedBuilding;

                selectionWindow.Title = building.DisplayName;
                selectionWindow.Text = building.Info;
            }
        }

        private static void UpdateButton(Button button, Rectangle area)
        {
            var mouseState = InputState.Instance.MouseState;

            var over = area.Contains(mouseState.Position.X, mouseState.Position.Y);
            var down = mouseState.LeftButton == ButtonState.Pressed;

            if (over && down)
            {
                button.State = UIButtonState.Down;
            }
            else if (over)
            {
                button.State = UIButtonState.Hover;
            }
            else
            {
                button.State = UIButtonState.Up;
            }
        }

        private static void DrawButtons(SpriteBatch spriteBatch, List<Button> buttons)
        {
            foreach (var button in buttons)
            {
                var buttonBaseTexture = GetButtonTexture(button.State);
                var buttonIconTexture = ContentLibrary.Textures[button.IconKey];
                var buttonRect = button.ButtonArea;

                spriteBatch.Draw(buttonBaseTexture, buttonRect, Color.White);
                spriteBatch.Draw(buttonIconTexture, buttonRect, Color.White);

                if (button.IsActive)
                {
                    spriteBatch.Draw(textureActiveBorder.Value, buttonRect, Color.White);
                }
            }
        }

        private static Texture2D GetButtonTexture(UIButtonState state)
        {
            return state switch
            {
                UIButtonState.Up => textureButtonUp.Value,
                UIButtonState.Down => textureButtonDown.Value,
                UIButtonState.Hover => textureButtonHover.Value,
                _ => textureButtonUp.Value,
            };
        }

        private void InitializeUI()
        {
            buttonConveyor = new()
            {
                Name = "btn-conveyor",
                Index = 0,
                IconKey = ContentLibrary.Keys.TextureIconConveyor
            };
            buttonDivider = new()
            {
                Name = "btn-divider",
                Index = 1,
                IconKey = ContentLibrary.Keys.TextureIconDivider
            };
            buttonMerger = new()
            {
                Name = "btn-merger",
                Index = 2,
                IconKey = ContentLibrary.Keys.TextureIconMerger
            };

            buttonRecipes = new()
            {
                Name = "btn-recipes",
                Index = 3,
                IconKey = ContentLibrary.Keys.TextureIconRecipes
            };
            buttonRequests = new()
            {
                Name = "btn-requests",
                Index = 4,
                IconKey = ContentLibrary.Keys.TextureIconRequests
            };
            buttonSelect = new()
            {
                IsActive = true,
                Name = "btn-select",
                Index = 5,
                IconKey = ContentLibrary.Keys.TextureIconSelect
            };

            buttonQuit = new()
            {
                Name = "btn-quit",
                Index = 6,
                Width = textureButtonMenuUp.Value.Width,
                Height = textureButtonMenuUp.Value.Height
            };

            buttonConveyor.OnClick = () =>
            {
                GameState.Current.SelectedBuildable = BuildableType.Conveyor;
                buttonConveyor.IsActive = true;
                buttonDivider.IsActive = false;
                buttonMerger.IsActive = false;
                buttonSelect.IsActive = false;
            };
            buttonDivider.OnClick = () =>
            {
                GameState.Current.SelectedBuildable = BuildableType.Divider;
                buttonConveyor.IsActive = false;
                buttonDivider.IsActive = true;
                buttonMerger.IsActive = false;
                buttonSelect.IsActive = false;
            };
            buttonMerger.OnClick = () =>
            {
                GameState.Current.SelectedBuildable = BuildableType.Merger;
                buttonConveyor.IsActive = false;
                buttonDivider.IsActive = false;
                buttonMerger.IsActive = true;
                buttonSelect.IsActive = false;
            };

            buttonQuit.OnClick = () =>
            {
                if (GameState.Current.IsGamePaused)
                {
                    GameState.Current.Quit();
                }
            };

            recipesWindow = new(ContentLibrary.Textures[ContentLibrary.Keys.TextureRecipeWindow])
            {
                Title = "Recipes",
                Text = "Smelting\nPlant\n\n\n\n\n\n\nAssembly\nPlant"
            };
            recipesWindow.Closed += () =>
            {
                buttonRecipes.IsActive = false;
            };

            requestsWindow = new()
            {
                Title = "Requests"
            };
            requestsWindow.Closed += () =>
            {
                buttonRequests.IsActive = false;
            };

            selectionWindow = new()
            {
                Title = "Selection"
            };

            buttonRecipes.OnClick = () =>
            {
                buttonRecipes.IsActive = !buttonRecipes.IsActive;
                if (buttonRecipes.IsActive)
                {
                    recipesWindow.Show();
                    requestsWindow.Hide();
                    buttonRequests.IsActive = false;
                }
                else
                {
                    recipesWindow.Hide();
                }
            };
            buttonRequests.OnClick = () =>
            {
                buttonRequests.IsActive = !buttonRequests.IsActive;
                if (buttonRequests.IsActive)
                {
                    requestsWindow.Show();
                    recipesWindow.Hide();
                    buttonRecipes.IsActive = false;
                }
                else
                {
                    requestsWindow.Hide();
                }
            };
            buttonSelect.OnClick = () =>
            {
                GameState.Current.SelectedBuildable = null;
                buttonConveyor.IsActive = false;
                buttonDivider.IsActive = false;
                buttonMerger.IsActive = false;
                buttonSelect.IsActive = true;
            };

            buttonsBuildables.Add(buttonConveyor);
            buttonsBuildables.Add(buttonDivider);
            buttonsBuildables.Add(buttonMerger);
            buttonsMenu.Add(buttonRecipes);
            buttonsMenu.Add(buttonRequests);
            buttonsMenu.Add(buttonSelect);
        }

        private void InputKeyPressed(Keys key)
        {
            if (key == Keys.F)
            {
                GameState.Current.DirectionArrowsVisible = !GameState.Current.DirectionArrowsVisible;
            }
            if (key == Keys.Escape)
            {
                GameState.Current.IsGamePaused = !GameState.Current.IsGamePaused;
            }
        }

        private void MouseLeftClick()
        {
            if (!IsMouseOnUI)
            {
                var tile = GameState.Current.HoveredTile;
                if (tile != null)
                {
                    if (tile.Building == null && GameState.Current.SelectedBuildable != null)
                    {
                        StaticElement building = null;

                        switch (GameState.Current.SelectedBuildable)
                        {
                            case BuildableType.Conveyor:
                                if (tile.Type == TileType.Grass)
                                {
                                    building = new Conveyor(tile, Side.Top, Side.Bottom);
                                }

                                break;

                            case BuildableType.Divider:
                                if (tile.Type == TileType.Grass)
                                {
                                    building = new Divider(tile);
                                }

                                break;

                            case BuildableType.Merger:
                                if (tile.Type == TileType.Grass)
                                {
                                    building = new Merger(tile);
                                }
                                break;
                        }

                        if (building != null)
                        {
                            tile.SetBuilding(building);
                            ContentLibrary.SoundEffects[ContentLibrary.Keys.SoundEffectPlace].Play(0.3f, 0, 0);
                        }
                    }
                    else if (tile.Building != null && GameState.Current.SelectedBuildable == null)
                    {
                        GameState.Current.SelectedBuilding = tile.Building;
                        selectionWindow.Title = tile.Building.DisplayName;
                        selectionWindow.Show();
                        buttonRecipes.IsActive = false;
                        buttonRequests.IsActive = false;
                    }
                }
            }
            else
            {
                if (BuildablesFoldArea.Contains(InputState.Instance.MouseState.Position))
                {
                    isBuildablesFolded = !isBuildablesFolded;
                }
                if (MenuFoldArea.Contains(InputState.Instance.MouseState.Position))
                {
                    isMenuFolded = !isMenuFolded;
                }
            }
        }

        private void MouseRightClick()
        {
            if (!IsMouseOnUI)
            {
                var tile = GameState.Current.HoveredTile;
                if (tile != null && tile.Building != null && tile.Building.IsRemoveable)
                {
                    if (tile.Building == GameState.Current.SelectedBuilding)
                    {
                        selectionWindow.Hide();
                    }
                    tile.ClearBuilding();
                }
            }
        }

        private void UpdateButtonLocations()
        {
            var isPaused = GameState.Current.IsGamePaused;

            int buildablesX = BuildablesArea.X;
            int buildablesY = BuildablesArea.Y;

            buttonConveyor.X = buildablesX + 9;
            buttonConveyor.Y = buildablesY + 28;
            buttonConveyor.IsClickable = !isBuildablesFolded && !isPaused;

            buttonDivider.X = buildablesX + Constants.ButtonWidth + 8 + 9;
            buttonDivider.Y = buildablesY + 28;
            buttonDivider.IsClickable = !isBuildablesFolded && !isPaused;

            buttonMerger.X = buildablesX + (2 * (Constants.ButtonWidth + 8)) + 9;
            buttonMerger.Y = buildablesY + 28;
            buttonMerger.IsClickable = !isBuildablesFolded && !isPaused;

            int menuX = MenuArea.X;
            int menuY = MenuArea.Y;

            buttonRecipes.X = menuX + 9;
            buttonRecipes.Y = menuY + 28;
            buttonRecipes.IsClickable = !isMenuFolded && !isPaused;

            buttonRequests.X = menuX + Constants.ButtonWidth + 8 + 9;
            buttonRequests.Y = menuY + 28;
            buttonRequests.IsClickable = !isMenuFolded && !isPaused;

            buttonSelect.X = menuX + (2 * (Constants.ButtonWidth + 8)) + 9;
            buttonSelect.Y = menuY + 28;
            buttonSelect.IsClickable = !isMenuFolded && !isPaused;

            buttonQuit.X = (Camera.Instance.ViewportWidth / 2) - (textureButtonMenuUp.Value.Width / 2);
            buttonQuit.Y = Camera.Instance.ViewportHeight - (textureButtonMenuUp.Value.Height * 2);
            buttonQuit.IsClickable = isPaused;
        }
    }
}