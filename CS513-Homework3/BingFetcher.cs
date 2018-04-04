using System.Threading.Tasks;
using System.Drawing;
using System.Net.Http;
using System.IO;

namespace CS513_Homework3
{
    class BingFetcher
    {
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            Image image = getTile("http://h0.ortho.tiles.virtualearth.net/tiles/a023131022213211200.jpeg?g=131");
            image.Save("tileImage.jpg");
        }

        static Image getTile(string URI)
        {
            Image image = null;
            Task get = getTileAsync(URI).ContinueWith(x => image = x.Result);
            get.Wait();
            return image;
        }

        static async Task<Image> getTileAsync(string URI)
        {
            Image image = null;
            HttpResponseMessage response = await client.GetAsync(URI);
            if(response.IsSuccessStatusCode)
            {
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                MemoryStream stream = new MemoryStream(bytes);
                image = Image.FromStream(stream);
            }
            return image;
        }
    }
}
