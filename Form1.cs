using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using System.Drawing.Drawing2D;

namespace Pythagorean_tree
{
    public partial class Form1 : Form
    {
        bool movable;
        float mouse_x;
        float mouse_y;
        float mouse_x_offset;
        float  mouse_y_offset;
        Graphics gr;
        Bitmap image;
        float root_x;
        float root_y;
        int depth;
        int length;
        float alpha;
        public Form1()
        {
            InitializeComponent();
            Init();
            
        }

        public void Init()
        {
            image = new Bitmap(Panel_paint.Width, Panel_paint.Height); // создаем растровое изображение
            gr = Graphics.FromImage(image);
            gr.Clear(Color.White);
            Panel_paint.Image = image;
            gr.SmoothingMode = SmoothingMode.AntiAlias;
        }

        // Рисуем дерево.
        private void Panel_Paint(object sender, PaintEventArgs e)
        {
          
            gr.Clear(Panel_paint.BackColor);

            try
            {
                depth = (int)nudDepth.Value;
                length = (int)nudLength.Value;
                if (auto.Checked)
                {
                    depth = length / 10 + 4;
                }
                alpha = (float)((double)nudAlpha.Value * Math.PI / 180.0);
                root_x = Panel_paint.ClientSize.Width / 2 + (float)numericUpDown1.Value * 10 + mouse_x_offset;
                root_y = Panel_paint.ClientSize.Height * 0.95f + (float)numericUpDown2.Value * 10 + mouse_y_offset;
                VectorF v_base = new VectorF(length, 0);
                PointF ll_corner = new PointF(root_x, root_y) - v_base / 2;

                Brush brush = null;
                Brush brush1 = null;
                if (chkFill.Checked)
                {
                    brush = Brushes.Green;
                    brush1 = Brushes.Brown;
                }
               

                DrawBranch(gr, Pens.Black, brush,brush1,
                    depth, ll_corner, v_base, alpha);
            }
            catch
            {

            }
        }

        // Перерисовка.
        private void parameter_ValueChanged(object sender, EventArgs e)
        {
            //Panel_paint.Refresh();

        }
        //private void Panel_Resize(object sender, EventArgs e)
        //{
        //    Panel_paint.Refresh();
        //}
        private void nud_KeyUp(object sender, KeyEventArgs e)
        {
            //Panel_paint.Refresh();
        }

        // Рекурсивная функция отрисовки ветвей.
        private void DrawBranch(Graphics grf, Pen pen, Brush brush, Brush brush1,
            int depth, PointF ll_corner, VectorF v_base, float alpha)
        {
            // находим координаты точек квадрата.
            VectorF v_height = v_base.PerpendicularCCW();
            PointF[] points =
            {
                ll_corner,
                ll_corner + v_base,
                ll_corner + v_base + v_height,
                ll_corner + v_height,
            };

            // Рисуем квадрат.
            if (brush1 != null) { 
                gr.FillPolygon(brush1, points);
                if (v_base.Length<15) gr.FillPolygon(brush, points);
            }
            gr.DrawPolygon(pen, points);
            Panel_paint.Image=image;
            
            // если глубина не 0 рисуем ветви.
            if (depth > 0)
            {
                // ***********
                // Левая ветвь
                // ***********
                // новыя длина стороны.
                double w1 = v_base.Length * Math.Cos(alpha);

                //  координаты нового базового вектора.
                float wb1 = (float)(w1 * Math.Cos(alpha));
                float wh1 = (float)(w1 * Math.Sin(alpha));
                VectorF v_base1 = v_base.Scale(wb1) + v_height.Scale(wh1);

                // нижняя левая координата.
                PointF ll_corner1 = ll_corner + v_height;

                // отрисовка.
                DrawBranch(gr, pen, brush,brush1, depth - 1, ll_corner1, v_base1, alpha);

                // ************
                // Правая ветвь
                // ************
                // новая длина.
                double beta = Math.PI / 2.0 - alpha;
                double w2 = v_base.Length * Math.Sin(alpha);

                // новый базовый вектор.
                float wb2 = (float)(w2 * Math.Cos(beta));
                float wh2 = (float)(w2 * Math.Sin(beta));
                VectorF v_base2 = v_base.Scale(wb2) - v_height.Scale(wh2);

                // нижняя левая координата.
                PointF ll_corner2 = ll_corner1 + v_base1;

                // отрисовка.
                DrawBranch(gr, pen, brush,brush1, depth - 1, ll_corner2, v_base2, alpha);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Сохранить изображение как...";
            saveDialog.OverwritePrompt = true; // предупреждение, если пользователь указывает имя уже существующего файла
            saveDialog.CheckPathExists = true; // предупреждение, если пользователь указывает несуществующий путь
            saveDialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
            saveDialog.ShowHelp = true; // кнопка "Справка" в диалоговом окне

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image.Save(saveDialog.FileName);
                }
                catch
                {
                    MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //начинаем двигать картинку, нажимая кнопку
        private void pic_down(object sender, MouseEventArgs e)
        {
            movable = true;
            mouse_x = Cursor.Position.X;
            mouse_y = Cursor.Position.Y;
        }
        //двигаем картинки при нажатой кнопке мыши
        private void pic_move(object sender, MouseEventArgs e)
        {
            if (movable)
            {
                mouse_x_offset = Cursor.Position.X - mouse_x;
                mouse_y_offset = Cursor.Position.Y - mouse_y;
                //mouse_x = Cursor.Position.X;
                //mouse_y = Cursor.Position.Y;
                //Panel_paint.Invalidate();

            }
        }

        // при отжатии кнопки мыши перестаем перемещать картинку
        private void pic_up(object sender, MouseEventArgs e)
        {
            movable = false;

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //Panel_paint.Refresh();
        }

        

        //перерисовыываем изображение
        //private void repaint_img(object sender, PaintEventArgs e)
        //{
        //    e.Graphics.Clear(Color.White);
        //    e.Graphics.DrawImageUnscaled(image,(int)root_x, (int)root_y);
        //}

       
       
    }
}
