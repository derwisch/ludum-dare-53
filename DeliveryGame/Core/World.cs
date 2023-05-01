
using DeliveryGame.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace DeliveryGame.Core
{
    internal class World
    {
        private readonly Tile[,] tiles;

        public World(int mapWidth, int mapHeight)
        {
            tiles = new Tile[mapWidth, mapHeight];
        }
        public IEnumerable<StaticElement> StaticElements => tiles.OfType<Tile>().Where(x => x.Building != null).Select(x => x.Building);
        public IEnumerable<Tile> Tiles => tiles.OfType<Tile>();

        public Tile this[(int x, int y) location] => this[location.x, location.y];

        public Tile this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0 || x >= Constants.MapWidth || y >= Constants.MapHeight)
                {
                    return null;
                }
                else
                {
                    return tiles[x, y];
                }
            }
        }
        public void Load(Texture2D map)
        {
            Color[] pixels = new Color[map.Width * map.Height];
            map.GetData(pixels);

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var color = pixels[y * map.Width + x];

                    color.Deconstruct(out byte r, out byte g, out byte b);

                    switch ($"{r},{g},{b}")
                    {
                        case "0,255,0":
                            tiles[x, y].Type = TileType.Grass;
                            break;
                        case "64,64,64":
                            tiles[x, y].Type = TileType.DepositCoal;
                            break;
                        case "127,0,0":
                            tiles[x, y].Type = TileType.DepositIron;
                            break;
                        case "255,127,0":
                            tiles[x, y].Type = TileType.DepositCopper;
                            break;
                        case "255,255,255":
                            tiles[x, y].Type = TileType.DepositSilicon;
                            break;
                        case "0,0,0":
                            tiles[x, y].Type = TileType.DepositOil;
                            break;
                        case "255,0,0":
                            tiles[x, y].Type = TileType.Grass;
                            tiles[x, y].SetBuilding(new Smeltery(tiles[x, y]));
                            break;
                        case "255,0,64":
                            tiles[x, y].Type = TileType.Grass;
                            Quest.RewardSmelteryTiles.Add(tiles[x, y]);
                            break;
                        case "0,0,255":
                            tiles[x, y].Type = TileType.Grass;
                            tiles[x, y].SetBuilding(new Assembler(tiles[x, y]));
                            break;
                        case "64,0,255":
                            tiles[x, y].Type = TileType.Grass;
                            Quest.RewardAssemblerTiles.Add(tiles[x, y]);
                            break;
                        case "255,0,255":
                            tiles[x, y].Type = TileType.Grass;
                            tiles[x, y].SetBuilding(new Hub(tiles[x, y]));
                            break;
                    }
                }
            }

            var coalTile = Tiles.Where(x => x.Type == TileType.DepositCoal && x.Building == null).First();
            coalTile.SetBuilding(new Extractor(coalTile));

            var ironTile = Tiles.Where(x => x.Type == TileType.DepositIron && x.Building == null).First();
            ironTile.SetBuilding(new Extractor(ironTile));
        }

        public void SetTile(Tile tile)
        {
            tiles[tile.X, tile.Y] = tile;
            RenderPool.Instance.RegisterRenderable(tile);
        }

        public void Update(GameTime gameTime)
        {
            GameState.Current.HoveredTile = null;
            foreach (var tile in tiles.OfType<Tile>())
            {
                tile.Update(gameTime);
            }
        }
    }
}
