using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }

        int cellsWidth, cellsHeight, cellSize,
            windowWidth, windowHeight, midX, midY,
            foodPosX, foodPosY, nextX, nextY, score, scoreBest;
        bool IsGameOver;
        List<Cell>Snake = new List<Cell>();
        List<Cell>Wall = new List<Cell>();
        Direction direction = new Direction();
        Timer moveTimer = new Timer();
        Font font = new Font(
           new FontFamily("Consolas"),
           16,
           FontStyle.Regular,
           GraphicsUnit.Pixel);
        Random rand = new Random();
        
        private void SetCellSize(int size)
        {
            cellSize = size;
        }

        private void SetCellsWidthHeight(int width, int height)
        {
            cellsWidth = width;
            cellsHeight = height;
            midX = (int)(cellsWidth / 2);
            midY = (int)(cellsHeight / 2);
            windowWidth = cellsWidth * cellSize - 14;
            windowHeight = cellsHeight * cellSize - 4;
        }

        private void SetWindowSize()
        {
            this.Width = windowWidth;
            this.Height = windowHeight;
        }

        private void StartGame()
        {
            IsGameOver = false;
            score = 0;
            moveTimer.Enabled = true;
            moveTimer.Interval = 85;

            Wall.Clear();
            Snake.Clear();
            Cell head = new Cell(midX, midY);
            Snake.Add(head);
            direction = Direction.Right;
            
            GenerateFood();
        }

        private void MoveStep()
        {
            nextX = Snake[0].posX;
            nextY = Snake[0].posY;
            switch (direction)
            {
                case Direction.Up:
                    Snake[0].posY--;
                    break;
                case Direction.Down:
                    Snake[0].posY++;
                    break;
                case Direction.Left:
                    Snake[0].posX--;
                    break;
                case Direction.Right:
                    Snake[0].posX++;
                    break;
                default:
                    break;
            }
            if (Snake.Count > 0)
            {
                for (int i = 1; i < Snake.Count; i++)
                {
                    int t;

                    t = Snake[i].posX;
                    Snake[i].posX = nextX;
                    nextX = t;

                    t = Snake[i].posY;
                    Snake[i].posY = nextY;
                    nextY = t;
                }
            }
            
        }

        void IncreaseSpeed()
        {
            if (moveTimer.Interval > 4)
                moveTimer.Interval -= 3;
        }

        private void GameOver()
        {
            IsGameOver = true;
            moveTimer.Enabled = false;
            if (score > scoreBest)
                scoreBest = score;
        }

        

        private void GenerateFood()
        {
            int posX, posY;
            bool flag;
            flag = true;
            posX = 0;
            posY = 0;
            while (flag)
            {
                flag = false;
                posX = rand.Next(1, cellsWidth - 1);
                posY = rand.Next(1, cellsHeight - 1);
                for (int i = 0; i < Wall.Count; i++)
                    if ((posX == Wall[i].posX) && (posY == Wall[i].posY))
                        flag = true;
                for (int i = 0; i < Snake.Count; i++)
                    if ((posX == Snake[i].posX) && (posY == Snake[i].posY))
                        flag = true;
            }
            foodPosX = posX;
            foodPosY = posY;
        }

        private void GenerateWall()
        {
            if (rand.Next(1, 5) == 1)
            {
                Cell wall = new Cell(rand.Next(2, cellsWidth - 2), rand.Next(2, cellsHeight - 2));
                Wall.Add(wall);
            }
        }

        private void Eat()
        {
            Cell snakePart = new Cell(nextX, nextY);
            Snake.Add(snakePart);
            score++;
            GenerateFood();
            if (score % 5 == 0)
                IncreaseSpeed();
            //GenerateWall();
        }

        

        private void CheckForCollision()
        {
            if ((Snake[0].posX < 0) || (Snake[0].posX > cellsWidth - 1) ||
                (Snake[0].posY < 0) || (Snake[0].posY > cellsHeight - 2))
                GameOver();
            if ((Snake[0].posX == foodPosX) && (Snake[0].posY == foodPosY))
                Eat();
            if (Snake.Count > 1)
                for (int i = 1; i < Snake.Count; i++)
                    if ((Snake[0].posX == Snake[i].posX) && (Snake[0].posY == Snake[i].posY))
                        GameOver();
            for (int i = 0; i < Wall.Count; i++)
                if ((Snake[0].posX == Wall[i].posX) && (Snake[0].posY == Wall[i].posY))
                    GameOver();
        }
                    

        public Form1()
        {
            InitializeComponent();

            SetCellSize(25);
            SetCellsWidthHeight(32, 20);
            SetWindowSize();
            
            moveTimer.Interval = 85;
            moveTimer.Tick += moveTimer_Tick;

            DoubleBuffered = true;
            StartGame();
            Invalidate();
        }

        private void moveTimer_Tick(object sender, EventArgs e)
        {
            MoveStep();
            CheckForCollision();
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush brushBlue = new SolidBrush(Color.CornflowerBlue);
            SolidBrush brushGreen = new SolidBrush(Color.Green);
            SolidBrush brushBlack = new SolidBrush(Color.Black);
            Graphics g = e.Graphics;

            if (!IsGameOver)
            {
                g.FillRectangle(brushGreen, foodPosX * (cellSize - 1), foodPosY * (cellSize - 1), cellSize, cellSize);

                for (int i = 0; i < Wall.Count; i++)
                    g.FillRectangle(brushBlack, Wall[i].posX * (cellSize - 1), Wall[i].posY * (cellSize - 1), cellSize, cellSize);

                for (int i = 0; i < Snake.Count; i++)
                    g.FillRectangle(brushBlue, Snake[i].posX * (cellSize - 1), Snake[i].posY * (cellSize - 1), cellSize, cellSize);

                g.DrawString("Score: " + score.ToString(), font, brushBlack, cellSize * (cellsWidth - 5), 10);
                g.DrawString("Best: " + scoreBest.ToString(), font, brushBlack, cellSize * (cellsWidth - 5), 25);
            }
            else
            {
                g.DrawString("Game Over", font, brushBlack, cellSize * (midX - 2), cellSize * (midY - 2));
                g.DrawString("Your score: " + score.ToString(), font, brushBlack, cellSize * (midX - 3), cellSize * (midY - 2) + 15);
                g.DrawString("[Enter]: New Game", font, brushBlack, cellSize * (midX - 3) - 10, cellSize * (midY - 2) + 30);
            }
                
            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsGameOver)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        if (direction != Direction.Right)
                            direction = Direction.Left;
                        break;
                    case Keys.Right:
                        if (direction != Direction.Left)
                            direction = Direction.Right;
                        break;
                    case Keys.Up:
                        if (direction != Direction.Down)
                            direction = Direction.Up;
                        break;
                    case Keys.Down:
                        if (direction != Direction.Up)
                            direction = Direction.Down;
                        break;
                    default:
                        break;
                }
            }
            else
                if (e.KeyCode == Keys.Enter)
                    StartGame();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Width = windowWidth;
            this.Height = windowHeight;
        }

        
    }
}
