using DeliveryGame.Core;
using DeliveryGame.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryGame.UI
{
    internal class UserInterface : IRenderable
    {
        private static readonly Rectangle pauseScreenArea = new()
        {
            X = 0,
            Y = 0,
            Width = Camera.Instance.ViewportWidth,
            Height = Camera.Instance.ViewportHeight
        };

        private static readonly Color pauseScreenColor = new(Color.Black, 127);

        private static readonly Lazy<Texture2D> textureActiveBorder = new(() => ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureActiveBorder));

        private static readonly Lazy<Texture2D> textureBuildables = new(() => ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureBuildablesUI));

        private static readonly Lazy<Texture2D> textureButtonDown = new(() => ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureButtonDown));

        private static readonly Lazy<Texture2D> textureButtonHover = new(() => ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureButtonHover));

        private static readonly Lazy<Texture2D> textureButtonUp = new(() => ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureButtonUp));

        private static readonly Lazy<Texture2D> textureMenu = new(() => ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureMenuUI));

        private static readonly Lazy<Texture2D> textureWhitePixel = new(() => ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureWhitePixel));

        private readonly List<Button> buttonsBuildables = new();

        private readonly List<Button> buttonsMenu = new();

        private Button buttonConveyor;

        private Button buttonDivider;

        private Button buttonMerger;

        private Button buttonRecipes;

        private Button buttonRequests;

        private Button buttonSelect;

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
                spriteBatch.Draw(textureWhitePixel.Value, pauseScreenArea, pauseScreenColor);
            }
        }

        internal void Update()
        {
            var mouseState = InputState.Instance.MouseState;

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
            }

            if (requestsWindow.IsVisible && GameState.Current.CurrentQuest != null)
            {
                var quest = GameState.Current.CurrentQuest;

                requestsWindow.Text = "Requested Wares:";

                foreach (var (count, type) in quest.RequestedWares)
                {
                    var deliveredAmount = quest.DeliveredWares[type];
                    var name = GetWareDisplayName(type);
                    requestsWindow.Text += $"\n({deliveredAmount}/{count}) {name}";
                }
            }

            if (selectionWindow.IsVisible)
            {
                var building = GameState.Current.SelectedBuilding;

                selectionWindow.Title = building.DisplayName;
                selectionWindow.Text = building.Info;
            }
        }

        private static void DrawButtons(SpriteBatch spriteBatch, List<Button> buttons)
        {
            foreach (var button in buttons)
            {
                var buttonBaseTexture = GetButtonTexture(button.State);
                var buttonIconTexture = ContentLibrary.Instance.GetTexture(button.IconKey);
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

            recipesWindow = new(ContentLibrary.Instance.GetTexture(ContentLibrary.Keys.TextureRecipeWindow))
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
                        switch (GameState.Current.SelectedBuildable)
                        {
                            case BuildableType.Conveyor:
                                if (tile.Type == TileType.Grass)
                                {
                                    tile.SetBuilding(new Conveyor(tile, Side.Top, Side.Bottom));
                                }

                                break;

                            case BuildableType.Divider:
                                if (tile.Type == TileType.Grass)
                                {
                                    tile.SetBuilding(new Divider(tile));
                                }

                                break;

                            case BuildableType.Merger:
                                if (tile.Type == TileType.Grass)
                                {
                                    tile.SetBuilding(new Merger(tile));
                                }

                                break;

                            case BuildableType.Extractor:
                                if (tile.Type != TileType.Grass)
                                {
                                    tile.SetBuilding(new Extractor(tile));
                                }

                                break;
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
            int buildablesX = BuildablesArea.X;
            int buildablesY = BuildablesArea.Y;

            buttonConveyor.X = buildablesX + 9;
            buttonConveyor.Y = buildablesY + 28;

            buttonDivider.X = buildablesX + Constants.ButtonWidth + 8 + 9;
            buttonDivider.Y = buildablesY + 28;

            buttonMerger.X = buildablesX + (2 * (Constants.ButtonWidth + 8)) + 9;
            buttonMerger.Y = buildablesY + 28;

            int menuX = MenuArea.X;
            int menuY = MenuArea.Y;

            buttonRecipes.X = menuX + 9;
            buttonRecipes.Y = menuY + 28;

            buttonRequests.X = menuX + Constants.ButtonWidth + 8 + 9;
            buttonRequests.Y = menuY + 28;

            buttonSelect.X = menuX + (2 * (Constants.ButtonWidth + 8)) + 9;
            buttonSelect.Y = menuY + 28;
        }
    }
}