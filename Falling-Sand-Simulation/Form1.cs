using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Falling_Sand_Simulation
{
    public partial class Form1 : Form
    {
        int WIDTH = 560, HEIGHT = 560;
        int cell_width = 10, cell_height = 10;
        int[,] grid;
        Graphics g;
        public Form1()
        {
            InitializeComponent();
        }

        void DrawLine(Graphics g, Pen pen, int x0, int y0, int x1, int y1)
        {
            g.DrawLine(pen, x0, y0, x1, y1);
        }

        Pen GetPen(int r, int g, int b)
        {
            Color color = Color.FromArgb(r, g, b);
            return new Pen(color);
        }

        void DrawGrid()
        {
            Pen pen = GetPen(255, 255, 255);
            for (int i = 0; i < WIDTH / cell_width; i++)
                DrawLine(g, pen, i * cell_width, 0, i * cell_width, HEIGHT);

            for (int j = 0; j < HEIGHT / cell_height; j++)
                DrawLine(g, pen, 0, j * cell_height, WIDTH, j * cell_height);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / cell_width;
            int y = e.Y / cell_height;
            grid[x, y] = 1;
        }

        void DrawCells()
        {
            for (int i = 0; i < WIDTH / cell_width; i++)
            {
                for (int j = 0; j < HEIGHT / cell_width; j++)
                {
                    SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                    if (grid[i, j] == 1)
                        myBrush.Color = Color.White;
                    Rectangle r = new Rectangle(i * cell_width, j * cell_height, cell_width, cell_height);

                    g.FillRectangle(myBrush, r);
                }
            }
        }

        void update()
        {
            int[,] new_grid = new int[WIDTH / cell_width, HEIGHT / cell_height];

            for (int i = 0; i< WIDTH / cell_width; i++)
            {
                for(int j = 0; j < HEIGHT / cell_width; j++)
                {
                    int floor_index = HEIGHT / cell_height - 2;

                    if (j == floor_index)
                    {
                        if(grid[i, j] == 0 && grid[i, j-1] == 1)
                            new_grid[i, j] = grid[i, j-1];
                        else
                            new_grid[i,j] = grid[i, j];
                        break;
                    }
                        

                    if (grid[i, j] == 1 && grid[i, j + 1] == 1)
                    {
                        new_grid[i, j] = 1;
                    }


                    if (grid[i,j] == 1 && grid[i, j+1] == 0)
                    {
                        new_grid[i, j] = 0;
                        new_grid[i, j + 1] = 1;
                        continue;
                    }
                    
                }
            }

            DrawCells();
            grid = new_grid;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            update();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X / cell_width;
            int y = e.Y / cell_height;
            grid[x, y] = 1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            grid = new int[WIDTH / cell_width, HEIGHT / cell_height];
            g = CreateGraphics();
            DrawGrid();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
