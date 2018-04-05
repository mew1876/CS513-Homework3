using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;
using System.IO;

namespace CS513_Homework3
{
    class BingFetcher
    {
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            //Bitmap image = GetImageInBB(41.78, -87.7, 41.783, -87.705);
            Bitmap image = GetImageInBB(41, -87, 41.0001, -87.0001);
            image.Save("test.png", ImageFormat.Png);
        }

        static Bitmap GetImageInBB(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            Quilt quilt = new Quilt(latitude1, longitude1, latitude2, longitude2, 15);
            RecursivelyGetImageinBB(latitude1, longitude1, latitude2, longitude2, quilt, "", 0);
            return quilt.GetImage();
        }

        static void RecursivelyGetImageinBB(double latitude1, double longitude1, double latitude2, double longitude2, Quilt targetQuilt, string quadKey, int levelOfDetail)
        {
            if(quadKey.Length > 0)
            {
                Bitmap tileImage = GetTile("http://h0.ortho.tiles.virtualearth.net/tiles/a" + quadKey + ".jpeg?g=131");
                if (tileImage == null)
                {
                    return;
                }
                targetQuilt.Add(tileImage, quadKey);
                if(levelOfDetail >= targetQuilt.getMaxLevelOfDetail())
                {
                    return;
                }
            }
            levelOfDetail++;

            RecursivelyGetImageinBB(latitude1, longitude1, latitude2, longitude2, targetQuilt, quadKey + "0", levelOfDetail);
            RecursivelyGetImageinBB(latitude1, longitude1, latitude2, longitude2, targetQuilt, quadKey + "1", levelOfDetail);
            RecursivelyGetImageinBB(latitude1, longitude1, latitude2, longitude2, targetQuilt, quadKey + "2", levelOfDetail);
            RecursivelyGetImageinBB(latitude1, longitude1, latitude2, longitude2, targetQuilt, quadKey + "3", levelOfDetail);
        }

        static Bitmap GetTile(string URI)
        {
            Bitmap image = null;
            Task get = getTileAsync(URI).ContinueWith(x => image = x.Result);
            get.Wait();
            return image;
        }

        static async Task<Bitmap> getTileAsync(string URI)
        {
            Bitmap image = null;
            HttpResponseMessage response = await client.GetAsync(URI);
            if(response.IsSuccessStatusCode)
            {
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                MemoryStream stream = new MemoryStream(bytes);
                image = new Bitmap(Image.FromStream(stream));
            }
            return image;
        }
    }
}
