using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace LabKG
{
    class DilationFilter : Filters
    {
        protected int kernelwidth;
        protected int kernelheight;
        protected float[,] kernel;

        public DilationFilter(int _w, int _h, float[,] _kernel)
        {
            kernelwidth = _w;
            kernelheight = _h;
            kernel = _kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernelwidth / 2;
            int radiusY = kernelheight / 2;

            Color max = Color.Black;
            double maxIntensity = Double.MinValue;

            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    if ((kernel[radiusX + k, radiusY + l] != 0) &&
                        (Intensity(sourceImage.GetPixel(idX, idY)) > maxIntensity))
                    //(sourceImage.GetPixel(idX, idY) > max))
                    {
                        max = sourceImage.GetPixel(idX, idY);
                        maxIntensity = Intensity(max);
                    }
                }
            return max;
        }
    };

    class ErosionFilter : Filters
    {
        protected int kernelwidth;
        protected int kernelheight;
        protected float[,] kernel;

        public ErosionFilter(int _w, int _h, float[,] _kernel)
        {
            kernelwidth = _w;
            kernelheight = _h;
            kernel = _kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernelwidth / 2;
            int radiusY = kernelheight / 2;

            Color min = Color.White;
            double minIntensity = Double.MaxValue;

            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    if ((kernel[radiusX + k, radiusY + l] != 0) &&
                        (Intensity(sourceImage.GetPixel(idX, idY)) < minIntensity))
                    {
                        min = sourceImage.GetPixel(idX, idY);
                        minIntensity = Intensity(min);
                    }
                }
            return min;
        }
    };

    class ClosingFilter : Filters
    {
        private DilationFilter dilationFilter;
        private ErosionFilter erosionFilter;

        public ClosingFilter(int _w, int _h, float[,] _kernel)
        {
            dilationFilter = new DilationFilter(_w, _h, _kernel);
            erosionFilter = new ErosionFilter(_w, _h, _kernel);
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            return Color.White;
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap res = dilationFilter.processImage(sourceImage, worker);
            Bitmap finalRes = erosionFilter.processImage(res, worker);

            return finalRes;
        }
    };

    class OpeningFilter : Filters
    {
        private DilationFilter dilationFilter;
        private ErosionFilter erosionFilter;

        public OpeningFilter(int _w, int _h, float[,] _kernel)
        {

            dilationFilter = new DilationFilter(_w, _h, _kernel);
            erosionFilter = new ErosionFilter(_w, _h, _kernel);
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            return Color.White;
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap res = erosionFilter.processImage(sourceImage, worker);
            Bitmap finalRes = dilationFilter.processImage(res, worker);

            return finalRes;
        }
    };

    class TopHatFilter : Filters
    {
       

        private Bitmap openedImage;

        private int _kwidth;
        private int _kheight;
        private float[,] _kmatrix;

        public TopHatFilter(int w, int h, float[,] k)
        {
            _kwidth = w;
            _kheight = h;
            _kmatrix = k;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color color = openedImage.GetPixel(x, y);
            if (color.R >= 250 && color.G >= 250 && color.B >= 250)
            {
                return Color.Black;
            }

            return sourceImage.GetPixel(x, y);
        }


        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker bgWorker)
        {
            OpeningFilter of = new OpeningFilter(_kwidth, _kheight, _kmatrix);
            openedImage = of.processImage(sourceImage, bgWorker);

            return base.processImage(sourceImage, bgWorker);
        }

    }
}
