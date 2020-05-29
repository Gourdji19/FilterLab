using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabKG
{
    public partial class Form1 : Form
    {
        Bitmap image;
        private int oldWidth;
        private int oldHeight;
        public bool pressed = false;
        public Color referencecolor;
        int a = 0, b = 0;

        private readonly Stack<Bitmap> oldImage = new Stack<Bitmap>();
        private readonly Stack<Bitmap> newImage = new Stack<Bitmap>();

        private int MathMorphWidth = 3;
        private int MathMorphkHeight = 3;
        private float[,] MathMorphKernel = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };

        public Form1()
        {
            InitializeComponent();
            oldWidth = 0;
            oldHeight = 0;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog(); //создаем диалог для открытия файла
            dialog.Filter = "Image files | *.png; *.jpg; *.bmp | All Files (*.*) | */*"; //фильтр для открытия файлов только типа изображение

            if (dialog.ShowDialog() == DialogResult.OK) //проверка действия выбора файлы пользователем
            {
                image = new Bitmap(dialog.FileName);
                pictureBox1.Image = image; //добавление изображения в pictureBox
                //Саша:  pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Refresh(); //обновление pictureBox
            }

        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Title = "Сохранить изображение как";
                dialog.OverwritePrompt = true; //возможность перезаписать при совпадении имён
                dialog.CheckPathExists = true; //предупреждение, если пути к файлу не существует
                dialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";

                dialog.ShowDialog();
                if (dialog.FileName != "")
                {
                    System.IO.FileStream fs = (System.IO.FileStream)dialog.OpenFile();
                    switch (dialog.FilterIndex)
                    {
                        case 1:
                            {
                                image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                                break;
                            }
                        case 2:
                            {
                                image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                                break;
                            }
                        case 3:
                            {
                                image.Save(fs, System.Drawing.Imaging.ImageFormat.Gif);
                                break;
                            }
                    }
                    fs.Close();
                }
            }
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvertFilter filter = new InvertFilter();
            /*
             * Bitmap resultImage = filter.processImage(image);
            pictureBox1.Image = resultImage;
            pictureBox1.Refresh();
             */
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
                image = newImage;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage; //изменение цвета полосы
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                oldImage.Push(new Bitmap(pictureBox1.Image));
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void размытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void размытиеГауссаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void чернобелыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сепияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SepiaFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void увеличитьЯркостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new IncreaseBrightnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void матричныеToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void увеличитьРезкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }


        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (oldImage.Count > 0)
            {
                newImage.Push(image);
                image = oldImage.Pop();
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }

        private void впередToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (newImage.Count > 0)
            {
                oldImage.Push(image);
                image = newImage.Pop();
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }

        private void фильтрСобеляToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Filters filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void вариант2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sharpness_2_Filter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void вариант1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SharpnessFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void операторЩарраToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SharrFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void операторПрюиттаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new PruittFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void переносToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new TransferenceFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void поворотToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new TurnFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void расширениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new DilationFilter(MathMorphWidth, MathMorphkHeight, MathMorphKernel);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сужениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ErosionFilter(MathMorphWidth, MathMorphkHeight, MathMorphKernel);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void открытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new OpeningFilter(MathMorphWidth, MathMorphkHeight, MathMorphKernel);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void закрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ClosingFilter(MathMorphWidth, MathMorphkHeight, MathMorphKernel);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void верхШляпыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new TopHatFilter(MathMorphWidth, MathMorphkHeight, MathMorphKernel);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void медианныйФильтрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MedianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void эффектВолнToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new WaveFilter1();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void операторПрюиттаToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Filters filter = new PruittFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void операторToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SharrFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void серыйМирToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GreyWorldFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void допЗаданиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new DopFilter(a, b);
            backgroundWorker1.RunWorkerAsync(filter);

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {

            a = e.X - ((pictureBox1.Width - pictureBox1.Image.Width) / 2);
            b = e.Y - ((pictureBox1.Height - pictureBox1.Image.Height) / 2);

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

    };



}
