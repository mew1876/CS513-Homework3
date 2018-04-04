using System;
using System.Drawing;

/// <summary>
/// Bitmap class that has methods which consist of "stitching"/placing images next to each other to make one big image
/// </summary>
public class Quilt
{
    private Bitmap quilt;
    private Graphics graphics;

	public Quilt(double minLatitude, double minLongitude, double maxLatitude, double maxLongitude, int levelOfDetail = 23)
	{
		//Create an empty image of appropriate size - see TileSystem
	}

    public void add(Bitmap image)
    {
        //Draw image over the correct part of the quilt - see Graphics
    }

    public Bitmap getImage()
    {
        return quilt;
    }

    private Bitmap GlueHorizontally(Bitmap leftImage, Bitmap rightImage)
    {
        //To be removed - use as reference
        Bitmap finalImage = new Bitmap(leftImage.Width + rightImage.Width, Math.Max(leftImage.Height, rightImage.Height));
        using (Graphics g = Graphics.FromImage(finalImage))
        {
            g.DrawImage(leftImage, 0, 0);
            g.DrawImage(rightImage, leftImage.Width, 0);
        }
        return finalImage;
    }
}
