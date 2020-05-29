using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
//using System.ComponentModel;

namespace LabKG
{
    class MatrixFilter : Filters
    {
        protected float[,] kernel = null; //двумерный массив
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1); //Clamp позволяет не выходить за границы изображения
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            return Color.FromArgb(
                Clamp((int)resultR, 0, 255),
                Clamp((int)resultG, 0, 255),
                Clamp((int)resultB, 0, 255)
                );
        }
    };

    class GaussianFilter : MatrixFilter
    {
        public void createGaussiaKernel(int radius, float sigma)
        {
            int size = 2 * radius + 1; //определяем размер ядра
            kernel = new float[size, size]; //создаем ядро фильтра
            float norm = 0; //коэффициент нормировки ядра
            for (int i = -radius; i <= radius; i++) //рассчитываем ядро линейного фильтра
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            for (int i = 0; i < size; i++) //нормируем ядро
                for (int j = 0; j < size; j++)
                    kernel[i, j] /= norm;
        }

        public GaussianFilter()
        {
            createGaussiaKernel(3, 2);
        }
    };

    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
        }
    };

    class SobelFilter : MatrixFilter
    {

        protected float[,] kernelX;
        protected float[,] kernelY;

        public SobelFilter()
        {
            kernelX = new float[,] { { -1, 0, 1 }, { -2, 0, 1 }, { -1, 0, 1 } };
            kernelY = new float[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            kernel = kernelX;
            Color valueX = base.calculateNewPixelColor(sourceImage, x, y);
            kernel = kernelY;
            Color valueY = base.calculateNewPixelColor(sourceImage, x, y);

            return Color.FromArgb(
                    Clamp((int)Math.Sqrt(Math.Pow(valueX.R, 2) + Math.Pow(valueY.R, 2)), 0, 255),
                    Clamp((int)Math.Sqrt(Math.Pow(valueX.G, 2) + Math.Pow(valueY.G, 2)), 0, 255),
                    Clamp((int)Math.Sqrt(Math.Pow(valueX.B, 2) + Math.Pow(valueY.B, 2)), 0, 255)
                    );
        }
    };



    class SharpnessFilter : MatrixFilter
    {
        public SharpnessFilter()
        {
            kernel = new float[,] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
        }
    };

    class Sharpness_2_Filter : MatrixFilter
    {
        public Sharpness_2_Filter()
        {
            kernel = new float[,] { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } };
        }
    };

    class SharrFilter : MatrixFilter
    {

        protected float[,] resultX;
        protected float[,] resultY;

        public SharrFilter()
        {
            resultX = new float[,] { { 3, 0, -3 }, { 10, 0, -10 }, { 3, 0, -3 } };
            resultY = new float[,] { { 3, 10, 3 }, { 0, 0, 0 }, { -3, -10, -3 } };
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            kernel = resultX;
            Color valueX = base.calculateNewPixelColor(sourceImage, x, y);
            kernel = resultY;
            Color valueY = base.calculateNewPixelColor(sourceImage, x, y);

            return Color.FromArgb(
                    Clamp((int)Math.Sqrt(Math.Pow(valueX.R, 2) + Math.Pow(valueY.R, 2)), 0, 255),
                    Clamp((int)Math.Sqrt(Math.Pow(valueX.G, 2) + Math.Pow(valueY.G, 2)), 0, 255),
                    Clamp((int)Math.Sqrt(Math.Pow(valueX.B, 2) + Math.Pow(valueY.B, 2)), 0, 255)
                    );
        }
    };

    class PruittFilter : MatrixFilter
    {

        protected float[,] resultX;
        protected float[,] resultY;

        public PruittFilter()
        {
            resultX = new float[,] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
            resultY = new float[,] { { -1, -1, -1 }, { 0, 0, 0 }, { 1, 1, 1 } };
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            kernel = resultX;
            Color valueX = base.calculateNewPixelColor(sourceImage, x, y);
            kernel = resultY;
            Color valueY = base.calculateNewPixelColor(sourceImage, x, y);

            return Color.FromArgb(
                    Clamp((int)Math.Sqrt(Math.Pow(valueX.R, 2) + Math.Pow(valueY.R, 2)), 0, 255),
                    Clamp((int)Math.Sqrt(Math.Pow(valueX.G, 2) + Math.Pow(valueY.G, 2)), 0, 255),
                    Clamp((int)Math.Sqrt(Math.Pow(valueX.B, 2) + Math.Pow(valueY.B, 2)), 0, 255)
                    );
        }
    }
}

   