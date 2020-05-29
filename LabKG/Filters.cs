using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace LabKG
{
    abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);

        public int Clamp(int value, int min, int max) //приведение к допустимому диапазону значения компоненты цвета
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        protected double Intensity(Color color)
        {
            return 0.36 * color.R + 0.53 * color.G + 0.11 * color.B;
        }

        public virtual Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100)); //приведение типов
                if (worker.CancellationPending) //сигнал о текущем прогрессе
                    return null;
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j)); //каждому пикселю изображения присваивает значение функции
                }
            }
            return resultImage;
        }

    };

    class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);
            return resultColor;
        }
    };

    class GrayScaleFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int Intensity = Clamp((int)(0.36f * sourceColor.R + 0.53f * sourceColor.G + 0.11f * sourceColor.B), 0, 255);
            Color resultColor = Color.FromArgb(Intensity, Intensity, Intensity);
            return resultColor;
        }
    };

    class SepiaFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int Intensity = Clamp((int)(0.36f * sourceColor.R + 0.53f * sourceColor.G + 0.11f * sourceColor.B), 0, 255);
            float k = 10;
            int R = Clamp((int)(Intensity + 4 * k), 0, 255);
            int G = Clamp((int)(Intensity + 2 * k), 0, 255);
            int B = Clamp((int)(Intensity - 1 * k), 0, 255);
            Color resultColor = Color.FromArgb(R, G, B);
            return resultColor;
        }
    };

    class IncreaseBrightnessFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int k = 30;
            int R = Clamp(k + sourceColor.R, 0, 255);
            int G = Clamp(k + sourceColor.G, 0, 255);
            int B = Clamp(k + sourceColor.B, 0, 255);
            Color resultColor = Color.FromArgb(R, G, B);
            return resultColor;
        }
    };

    class TransferenceFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            if (x + 200 >= sourceImage.Width)
            {
                return Color.FromArgb(0, 0, 0);
            }
            else
            {
                return sourceImage.GetPixel(x + 150, y);
            }
        }
    };

    class TurnFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int centreX = sourceImage.Width / 2;
            int centreY = sourceImage.Height / 2;
            int turnX = (int)((x - centreX) * Math.Cos(Math.PI / 6) - (y - centreY) * Math.Sin(Math.PI / 6)) + centreX;
            int turnY = (int)((x - centreX) * Math.Sin(Math.PI / 6) + (y - centreY) * Math.Cos(Math.PI / 6)) + centreY;

            if (turnX >= sourceImage.Width || turnX < 0 || turnY >= sourceImage.Height || turnY < 0)
            {
                return Color.Black;
            }

            return sourceImage.GetPixel(turnX, turnY);
        }
    };

    /*
    class WavesFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int resultX = (int)(x + 20 * Math.Sin(2 * Math.PI * y / 60));
            return sourceImage.GetPixel(resultX, y);
        }
    };
    */

    class WaveFilter1 : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int idX = Clamp(x + (int)(20 * Math.Sin(2 * Math.PI * (y) / 60)), 0, sourceImage.Width - 1);
            int idY = Clamp(y, 0, sourceImage.Height - 1);
            Color sourceColor = sourceImage.GetPixel(idX, idY);

            return sourceColor;
        }
    }
    /*
     последний вариант
    class WaveFilter2 : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int idX = Clamp(x, 0, sourceImage.Width - 1);
            int idY = Clamp(y + (int)(20 * Math.Sin(2 * Math.PI * (y) / 128)), 0, sourceImage.Height - 1);
            Color sourceColor = sourceImage.GetPixel(idX, idY);

            return sourceColor;
        }
    };
    */
    /*
    class WaveFilter2 : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int idX = Clamp(x + (int)(20 * Math.Sin(2 * Math.PI * (x) / 30)), 0, sourceImage.Width - 1);
            int idY = Clamp(y, 0, sourceImage.Height - 1);
            Color sourceColor = sourceImage.GetPixel(idX, idY);

            return sourceColor;
        }
    };
    */
    class GreyWorldFilter : Filters
    {
        public double avgR;
        public double avgG;
        public double avgB;
        public double avgAll;

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color color = sourceImage.GetPixel(x, y);
            return Color.FromArgb(
                Clamp((int)(color.R * avgAll / avgR), 0, 255),
                Clamp((int)(color.G * avgAll / avgG), 0, 255),
                Clamp((int)(color.B * avgAll / avgB), 0, 255)
            );
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker bgWorker)
        {
            double sumR = 0;
            double sumG = 0;
            double sumB = 0;
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Color curColor = sourceImage.GetPixel(i, j);
                    sumR += curColor.R;
                    sumG += curColor.G;
                    sumB += curColor.G;
                }
            }

            avgR = sumR / (sourceImage.Width * sourceImage.Height);
            avgG = sumG / (sourceImage.Width * sourceImage.Height);
            avgB = sumB / (sourceImage.Width * sourceImage.Height);

            avgAll = (avgR + avgB + avgG) / 3;

            return base.processImage(sourceImage, bgWorker);
        }
    };

    class DopFilter : Filters
    {
        int _x, _y;

        public DopFilter(int a, int b)
        {
            _x = a;
            _y = b;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int centreX = _x;
            int centreY = _y;
            int turnX = (int)((x - centreX) * Math.Cos(Math.PI / 6) - (y - centreY) * Math.Sin(Math.PI / 6)) + centreX;
            int turnY = (int)((x - centreX) * Math.Sin(Math.PI / 6) + (y - centreY) * Math.Cos(Math.PI / 6)) + centreY;

            if (turnX >= sourceImage.Width || turnX < 0 || turnY >= sourceImage.Height || turnY < 0)
            {
                return Color.Black;
            }

            return sourceImage.GetPixel(turnX, turnY);
        }
    };
}
