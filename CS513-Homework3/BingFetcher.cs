﻿using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;
using System.IO;
using Microsoft.MapPoint;
using System;
using System.Collections.Generic;

// TODO: check for minimum and maximum Bing longitude and latitude values and set to them if outside of them.

namespace CS513_Homework3
{
    class BingFetcher
    {
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            double minLatitudeInput  = 0;
            double minLongitudeInput = 0;
            double maxLatitudeInput  = 0;
            double maxLongitudeInput = 0;
            int    maxLevelOfDetail  = 1; // between 1 and 23
            string imagePath         = "";
            Quilt quilt;

            // get user input
            Console.WriteLine("Enter q for any input to quit.");
            while(true)
            {
                minLatitudeInput = double.Parse(GetUserInput("Enter the minimum latitude value of the desired bounding box: ", "double"), System.Globalization.CultureInfo.InvariantCulture);
                minLongitudeInput = double.Parse(GetUserInput("Enter the minimum longitude value of the desired bounding box: ", "double"), System.Globalization.CultureInfo.InvariantCulture);
                maxLatitudeInput = double.Parse(GetUserInput("Enter the maximum latitude value of the desired bounding box: ", "double"), System.Globalization.CultureInfo.InvariantCulture);
                maxLongitudeInput = double.Parse(GetUserInput("Enter the maximum longitude value of the desired bounding box: ", "double"), System.Globalization.CultureInfo.InvariantCulture);
                maxLevelOfDetail = int.Parse(GetUserInput("Enter the maximum level of detail (1-23): ", "int"));

                // correct values before sending them to get bounding box
                if (maxLevelOfDetail > 21)
                {
                    maxLevelOfDetail = 21;
                }
                else if (maxLevelOfDetail < 1)
                {
                    maxLevelOfDetail = 1;
                }
                TileSystem.Clip(minLatitudeInput, TileSystem.MinLatitude, TileSystem.MaxLatitude);
                TileSystem.Clip(minLongitudeInput, TileSystem.MinLongitude, TileSystem.MaxLongitude);

                try
                {
                    quilt = new Quilt(minLatitudeInput, minLongitudeInput, maxLatitudeInput, maxLongitudeInput, maxLevelOfDetail);
                    break; 
                }
                catch (Exception)
                {
                    Console.WriteLine("Bounding Box invalid (too large or zero).");
                }
            }
            imagePath = GetUserInput("Enter the absolute path to the directory you would like the image saved in: ", "directory");
            Console.WriteLine("Fetching images... (this may take some time)");
            // Bitmap image = GetImageInBBAsync(41.78, -87.7, 41.783, -87.705);
            // my absolute path = C:\Users\Julianna\Documents\Documents\Academic\CS 513 Windows\CS 513 Repository HW 3\CS513-Homework3
            Bitmap image = GetImageInBBAsync(quilt, minLatitudeInput, minLongitudeInput, maxLatitudeInput, maxLongitudeInput, maxLevelOfDetail);
            image.Save(imagePath+"/test.png", ImageFormat.Png); 
        }

        static string GetUserInput(string prompt, string type)
        {
            string inputString = "";
            bool validInput = false;
            while (validInput == false)
            {
                Console.WriteLine(prompt);
                inputString = Console.ReadLine().Trim();
                if (inputString == "q")
                {
                    Console.WriteLine("Quitting");
                    Environment.Exit(0);
                }
                if (type == "double")
                {
                    validInput = double.TryParse(inputString, out double inputFloat);
                }
                else if(type == "int")
                {
                    validInput = int.TryParse(inputString, out int inputInt);
                }
                else if (type == "directory")
                {
                    validInput = Directory.Exists(inputString);
                }
                else
                {
                    throw new ArgumentException("Invalid target type given");
                }
                if(validInput == false)
                {
                    Console.WriteLine("Invalid input. Try again.");
                }
            }
            return inputString;
        }

        static Bitmap GetImageInBB(double latitude1, double longitude1, double latitude2, double longitude2, int maxLevelofDetail)
        {
            Quilt quilt = new Quilt(latitude1, longitude1, latitude2, longitude2, maxLevelofDetail);
            RecursivelyGetImageinBB(latitude1, longitude1, latitude2, longitude2, quilt, "");
            return quilt.GetImage();
        }

        static Bitmap GetImageInBBAsync(Quilt quilt, double latitude1, double longitude1, double latitude2, double longitude2, int maxLevelofDetail)
        {
            Task drawQuilt = RecursivelyGetImageinBBAsync(latitude1, longitude1, latitude2, longitude2, quilt, "");
            drawQuilt.Wait();
            return quilt.GetImage();
        }

        static void RecursivelyGetImageinBB(double latitude1, double longitude1, double latitude2, double longitude2, Quilt targetQuilt, string quadKey)
        {
            int levelOfDetail = quadKey.Length;
            if (levelOfDetail > 0)
            {
                //Console.WriteLine("Downloading image " + quadKey);
                Bitmap tileImage = GetTile("http://h0.ortho.tiles.virtualearth.net/tiles/a" + quadKey + ".jpeg?g=131");
                if (tileImage == null)
                {
                    return;
                }
                targetQuilt.Add(tileImage, quadKey);
                if (levelOfDetail >= targetQuilt.getMaxLevelOfDetail())
                {
                    return;
                }
            }
            levelOfDetail++;

            foreach (char d in "0123")
            {
                if (DoesQuadTouchBB(latitude1, longitude1, latitude2, longitude2, quadKey + d))
                {
                    RecursivelyGetImageinBB(latitude1, longitude1, latitude2, longitude2, targetQuilt, quadKey + d);
                }
            }
        }

        static async Task RecursivelyGetImageinBBAsync(double latitude1, double longitude1, double latitude2, double longitude2, Quilt targetQuilt, string quadKey)
        {
            int levelOfDetail = quadKey.Length;
            if (levelOfDetail > 0)
            {
                //Console.WriteLine("\tDownloading image " + quadKey);
                Bitmap tileImage = GetTile("http://h0.ortho.tiles.virtualearth.net/tiles/a" + quadKey + ".jpeg?g=131");
                if (tileImage == null)
                {
                    //Console.WriteLine("\tImage for " + quadKey + " was null");
                    return;
                }
                targetQuilt.Add(tileImage, quadKey);
                //Console.WriteLine("\tDrew image " + quadKey);
                if (levelOfDetail >= targetQuilt.getMaxLevelOfDetail())
                {
                    return;
                }
            }

            List<Task> children = new List<Task>();
            foreach (char d in "0123")
            {
                if (DoesQuadTouchBB(latitude1, longitude1, latitude2, longitude2, quadKey + d))
                {
                    //Console.WriteLine("Created child for " + quadKey + d);
                    children.Add(RecursivelyGetImageinBBAsync(latitude1, longitude1, latitude2, longitude2, targetQuilt, quadKey + d));
                }
            }
            await Task.WhenAll(children.ToArray());
            //Console.WriteLine("Task " + quadKey + " finished");
        }

        static Bitmap GetTile(string URI)
        {
            Bitmap image = null;
            Task<Bitmap> get = GetTileAsync(URI).ContinueWith(x => image = x.Result);
            get.Wait();
            return image;
        }

        static async Task<Bitmap> GetTileAsync(string URI)
        {
            Bitmap image = null;
            HttpResponseMessage response = await client.GetAsync(URI);
            
            if(!response.Headers.Contains("X-VE-Tile-Info"))
            {
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                MemoryStream stream = new MemoryStream(bytes);
                image = new Bitmap(Image.FromStream(stream));
            }
            return image;
        }

        static bool DoesQuadTouchBB(double latitude1, double longitude1, double latitude2, double longitude2, string quadKey)
        {
            double top1 = Math.Max(latitude1, latitude2), bottom1 = Math.Min(latitude1, latitude2), left1 = Math.Min(longitude1, longitude2), right1 = Math.Max(longitude1, longitude2);

            TileSystem.QuadKeyToTileXY(quadKey, out int tileX, out int tileY, out int levelOfDetail);
            TileSystem.TileXYToPixelXY(tileX, tileY, out int pixelX, out int pixelY);
            TileSystem.PixelXYToLatLong(pixelX, pixelY, levelOfDetail, out latitude1, out longitude1);
            TileSystem.PixelXYToLatLong(pixelX + 256, pixelY + 256, levelOfDetail, out latitude2, out longitude2);

            double top2 = Math.Max(latitude1, latitude2), bottom2 = Math.Min(latitude1, latitude2), left2 = Math.Min(longitude1, longitude2), right2 = Math.Max(longitude1, longitude2);
            return left1 < right2 && left2 < right1 && top1 > bottom2 && top2 > bottom1;
        }
    }
}
