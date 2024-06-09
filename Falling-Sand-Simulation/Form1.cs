using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Falling_Sand_Simulation
{
    public partial class Form1 : Form
    {
        int WIDTH = 550, HEIGHT = 570;
        int cell_width = 10, cell_height = 10;
        int[,] grid;
        Color[,] color_grid;
        bool drag_drawing;

        Graphics g;
        Random r;

        List<Color> colors;
        int color_index = 0;
        int color_step = 100;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            grid = new int[WIDTH / cell_width, HEIGHT / cell_height];
            color_grid = new Color[WIDTH / cell_width, HEIGHT / cell_height];
            g = CreateGraphics();
            r = new Random();
            colors = GetGradients(color_step);
            DrawGrid();
        }


        public List<Color> GetGradients(int steps)
        {
            List<Color> gradient = new List<Color>();

            // Define base colors for a wide gradient range
            List<Color> baseColors = new List<Color>
        {
            Color.Red,
            Color.Orange,
            Color.Yellow,
            Color.Green,
            Color.Cyan,
            Color.Blue,
            Color.Indigo,
            Color.Violet
        };

            // Ensure we have at least 2 steps per base color segment
            if (steps < baseColors.Count - 1)
            {
                throw new ArgumentException("Number of steps must be at least " + (baseColors.Count - 1));
            }

            // Calculate the number of steps per color segment
            int stepsPerSegment = steps / (baseColors.Count - 1);

            // Generate the gradient
            for (int i = 0; i < baseColors.Count - 1; i++)
            {
                Color start = baseColors[i];
                Color end = baseColors[i + 1];
                for (int j = 0; j < stepsPerSegment; j++)
                {
                    float ratio = (float)j / stepsPerSegment;
                    int r = (int)(start.R * (1 - ratio) + end.R * ratio);
                    int g = (int)(start.G * (1 - ratio) + end.G * ratio);
                    int b = (int)(start.B * (1 - ratio) + end.B * ratio);
                    gradient.Add(Color.FromArgb(r, g, b));
                }
            }

            // Add the last color to complete the gradient
            gradient.Add(baseColors[baseColors.Count - 1]);

            return gradient;
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

        private void Form1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int x = e.X / cell_width;
            int y = e.Y / cell_height;
            grid[x, y] = 1;
            color_grid[x, y] = colors[color_index];
        }

        void DrawCells()
        {
            for (int i = 0; i < WIDTH / cell_width; i++)
            {
                for (int j = 0; j < HEIGHT / cell_width; j++)
                {
                    SolidBrush myBrush = new SolidBrush(Color.Black);
                    if (grid[i, j] == 1)
                        myBrush.Color = color_grid[i,j];
                    Rectangle r = new Rectangle(i * cell_width, j * cell_height, cell_width, cell_height);

                    g.FillRectangle(myBrush, r);
                }
            }
        }

        void update()
        {
            DrawCells();
            
            int[,] new_grid = new int[WIDTH / cell_width, HEIGHT / cell_height];
            Color[,] new_color_grid = new Color[WIDTH / cell_width, HEIGHT / cell_height];
            for (int i = 0; i< WIDTH / cell_width; i++)
            {
                for(int j = 0; j < HEIGHT / cell_width; j++)
                {
                    int floor_index = HEIGHT / cell_height - 2;
                    int left_wall_index = 0;
                    int right_wall_index = (WIDTH / cell_width) - 1;

                    int current = grid[i, j];
                    if (current == 0)
                        continue;
                    int above, below, down_right, down_left;
                    if (j == floor_index)
                    {
                        below = -1;
                    }
                    else
                    {
                        below = grid[i, j + 1];
                    }
                    if(j == 0)
                    {
                        above = -1;
                    }
                    else
                    {
                        above = grid[i, j - 1];
                    }

                    if(i == left_wall_index)
                    {
                        down_left = -1;
                    }
                    else
                    {
                        down_left = grid[i-1, j + 1];
                    }
                    if(i == right_wall_index)
                    {
                        down_right = -1;
                    }
                    else
                    {
                        down_right= grid[i+1, j + 1];
                    }

                    if (j == floor_index)
                    {
                        if (new_grid[i, j] != grid[i, j])
                        {
                            new_grid[i, j] = 1;
                            new_color_grid[i, j] = color_grid[i, j];
                        }else 
                        { 
                            new_grid[i, j] = current;
                            new_color_grid[i, j] = color_grid[i, j];
                        }
                        break;
                    }



                    if (below == 1)
                    {
                        int side = r.Next(0, 2);
                        if (side == 0 && down_left == 0)
                        {
                            new_grid[i - 1, j + 1] = 1;
                            new_color_grid[i - 1, j + 1] = color_grid[i, j];
                        }
                        else if (side == 1 && down_right == 0)
                        {
                            new_grid[i + 1, j + 1] = 1;
                            new_color_grid[i + 1, j + 1] = color_grid[i, j];
                        }
                        else
                        {
                            new_grid[i, j] = 1;
                            new_color_grid[i, j] = color_grid[i, j];
                        }
                            
                    }
                    else
                    {
                        new_grid[i, j] = 0;
                        new_grid[i, j + 1] = 1;
                        new_color_grid[i, j] = Color.Black;
                        new_color_grid[i, j + 1] = color_grid[i, j];
                    }
                    
                    
                }
            }
            grid = new_grid;
            color_grid = new_color_grid;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(color_index == colors.Count-1)
                color_index = 0;
            color_index++;

            update();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            grid = new int[WIDTH / cell_width, HEIGHT / cell_height];
            color_grid = new Color[WIDTH / cell_width, HEIGHT / cell_height];
        }

        private void Form1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(!drag_drawing)
                return;

            int x = e.X / cell_width;
            int y = e.Y / cell_height;

            if(x == 0 || x == (WIDTH / cell_width) - 1)
            {
                grid[x, y] = 1;
                color_grid[x, y] = colors[color_index];
                return;
            }
            if(y == 0 || y == (HEIGHT / cell_height) - 1)
            {
                grid[x, y] = 1;
                color_grid[x, y] = colors[color_index];
                return;
            }

            for (int x_cell = x-1; x_cell <= x+1; x_cell++)
            {
                for(int y_cell = y-1; y_cell <= y+1; y_cell++)
                {
                    grid[x_cell, y_cell] = 1;
                    color_grid[x_cell, y_cell] = colors[color_index];
                }
            }
            
        }

        private void dragCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            int value = dragCheckBox.Checked ? 1 : 0;
            drag_drawing = value == 1;
        }
    }
}
