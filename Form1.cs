using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;

namespace DownloadOSMTiles
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public struct Tile
        {
            public int x;
            public int x_size;
            public int y;
            public int y_size;
            public int zoom;
            public string name;
        }
        private void button1_Click(object sender, EventArgs e)
        {
             

            Tile t2 = new Tile
            {
                x = 17,
                x_size = 3,
                y = 23,
                y_size = 3,
                name = "new york",
                zoom = 6
            };

            LatLongToPixelXYOSM(40.90039f, -74.18632f, 8, out int pixelX, out int pixelY, out int tilex , out int tiley);
            Tile t3 = new Tile
            {
                x = tilex,
                x_size = 3,
                y = tiley,
                y_size = 3,
                name = "new york",
                zoom = 8
            };

             
            DownloadTiles(t3);

        }

        void PixelXYToLatLongOSM(int pixelX, int pixelY, int zoomLevel, out float latitude, out float longitude)
        {
            int mapSize = (int)Math.Pow(2, zoomLevel) * 256;
            int tileX = (int)Math.Truncate((decimal)(pixelX / 256));
            int tileY = (int)Math.Truncate((decimal)pixelY / 256);

            float n = (float)Math.PI - ((2.0f * (float)Math.PI * (ClipByRange(pixelY, mapSize - 1) / 256)) / (float)Math.Pow(2.0, zoomLevel));

            longitude = ((ClipByRange(pixelX, mapSize - 1) / 256) / (float)Math.Pow(2.0, zoomLevel) * 360.0f) - 180.0f;
            latitude = (180.0f / (float)Math.PI * (float)Math.Atan(Math.Sinh(n)));
        }

        float ClipByRange(float n, float range)
        {
            return n % range;
        }

        float Clip(float n, float minValue, float maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }

        void LatLongToPixelXYOSM(float latitude, float longitude, int zoomLevel, 
                                 out int pixelX, 
                                 out int pixelY,
                                 out int tilex,
                                 out int tiley)
        {
            float MinLatitude = -85.05112878f;
            float MaxLatitude = 85.05112878f;
            float MinLongitude = -180;
            float MaxLongitude = 180;
            float mapSize = (float)Math.Pow(2, zoomLevel) * 256;

            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            longitude = Clip(longitude, MinLongitude, MaxLongitude);

            float X = (float)((longitude + 180.0f) / 360.0f * (float)(1 << zoomLevel));
            float Y = (float)((1.0 - Math.Log(Math.Tan(latitude * (Math.PI / 180.0)) + 1.0 / Math.Cos(latitude * (Math.PI / 180.0))) / Math.PI) / 2.0 * (1 << zoomLevel));

            tilex = (int)(Math.Truncate(X));
            tiley = (int)(Math.Truncate(Y));
            pixelX = (int)ClipByRange((tilex * 256) + ((X - tilex) * 256), mapSize - 1);
            pixelY = (int)ClipByRange((tiley * 256) + ((Y - tiley) * 256), mapSize - 1);
        }


        private readonly string[] _serverEndpoints = { "a", "b", "c" };

        public void DownloadTiles(List<Tile> tiles)
        {
            foreach (Tile t in tiles)
            {
                DownloadTiles(t);
            }
        }

        public async void DownloadTiles(Tile tile)
        {
            Directory.CreateDirectory("c:\\OSM_Tiles");
            HttpClient client = new HttpClient();
            var random = new Random();
            for (int x = tile.x; x < (tile.x + tile.x_size); x++)
            {
                for (int y = tile.y; y < (tile.y + tile.y_size); y++)
                {
                    try
                    {
                        var url = $"http://{_serverEndpoints[random.Next(0, 2)]}.tile.openstreetmap.org/{tile.zoom}/{x}/{y}.png";
                        var data = await client.GetByteArrayAsync(url);
                        File.WriteAllBytes("c:\\OSM_Tiles\\" + tile.name + "_" + x + "_" + y + "_" + tile.zoom + ".png", data);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);                            
                    }
                }
            }
            MessageBox.Show("Finished");
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Tile> tiles = new List<Tile>();

            for (int zoom = 4; zoom < 15; zoom++)
            {
                LatLongToPixelXYOSM(40.90039f, -74.18632f, zoom, out int pixelX, out int pixelY, out int tilex, out int tiley);
                Tile t3 = new Tile
                {
                    x = tilex,
                    x_size = 3,
                    y = tiley,
                    y_size = 3,
                    name = "new york",
                    zoom = zoom
                };
                tiles.Add(t3);
            }

            DownloadTiles(tiles);
        }
    }
}
