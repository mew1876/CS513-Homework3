using System;
using System.Drawing;

/// <summary>
/// Bitmap class that has methods which consist of "stitching"/placing images next to each other to make one big image
/// </summary>
public class GluedImage
{
	public Quilt()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    private Bitmap GlueHorizontally(Bitmap leftImage, Bitmap rightImage)
    {
        Bitmap finalImage = new Bitmap(leftImage.Width + rightImage.Width, Math.Max(leftImage.Height, rightImage.Height));
        using (Graphics g = Graphics.FromImage(finalImage))
        {
            g.DrawImage(leftImage, 0, 0);
            g.DrawImage(rightImage, leftImage.Width, 0);
        }
    }
}
