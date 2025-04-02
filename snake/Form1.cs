using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace snake
{
    public partial class Form1 : Form
    {
        // Основные игровые переменные
        private List<Point> snake; // Список сегментов змейки (координаты)
        private Point food;        // Позиция еды
        private Direction currentDirection; // Текущее направление движения
        private int cellSize = 15; // Размер одной клетки игрового поля
        private Random random = new Random(); // Генератор случайных чисел
        private int score = 0;     // Счет игрока
        private Timer gameTimer;   // Таймер для обновления игры
       

        public Form1()
        {
            InitializeComponent();
            InitializeGame(); // Инициализация игровых параметров

            this.DoubleBuffered = true;
            this.Paint += Form1_Paint;

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Цвета для клеток
            Color color1 = Color.FromArgb(30, 30, 30); // Темно-серый
            Color color2 = Color.FromArgb(20, 20, 20); // Почти черный

            // Рисуем шахматный фон
            for (int x = 0; x < ClientSize.Width; x += cellSize)
            {
                for (int y = 0; y < ClientSize.Height; y += cellSize)
                {
                    var brush = ((x / cellSize + y / cellSize) % 2 == 0)
                        ? new SolidBrush(color1)
                    : new SolidBrush(color2);

                    e.Graphics.FillRectangle(brush, x, y, cellSize, cellSize);
                    brush.Dispose();

                }

            }

        }



        // Инициализация игрового состояния
        private void InitializeGame()
        {
            // Настройка окна игры
            this.ClientSize = new Size(600, 450); // Размер игрового поля
            this.DoubleBuffered = true; // Включаем двойную буферизацию для устранения мерцания
            this.BackColor = Color.Empty; // Цвет фона
            this.Paint += MainForm_Paint; // Подписываемся на событие отрисовки
            this.KeyDown += MainForm_KeyDown; // Подписываемся на событие нажатия клавиш

            // Инициализация змейки (3 сегмента)
            snake = new List<Point>
        {
            new Point(10, 10), // Голова
            new Point(9, 10),  // Первый сегмент тела
            new Point(8, 10)   // Второй сегмент тела
        };

            currentDirection = Direction.Right; // Начальное направление
            SpawnFood(); // Генерация первой еды

            // Настройка игрового таймера
            gameTimer = new Timer();
            gameTimer.Interval = 100; // Интервал обновления (100 мс)
            gameTimer.Tick += GameTimer_Tick; // Подписываемся на событие таймера
            gameTimer.Start(); // Запускаем таймер
        }

        // Генерация новой еды
        private void SpawnFood()
        {
            do
            {
                // Генерация случайной позиции в пределах игрового поля
                food = new Point(
                    random.Next(0, ClientSize.Width / cellSize),  // X координата
                    random.Next(0, ClientSize.Height / cellSize)); // Y координата
            }
            while (snake.Contains(food)); // Проверка, чтобы еда не появилась внутри змейки
        }

        // Обработчик тика таймера (основной игровой цикл)
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            MoveSnake();    // Движение змейки
            CheckCollisions(); // Проверка столкновений
            this.Invalidate(); // Перерисовка формы
        }

        // Логика движения змейки
        private void MoveSnake()
        {
            // Получаем текущую позицию головы
            Point newHead = snake[0];

            // Вычисляем новую позицию головы в зависимости от направления
            switch (currentDirection)
            {
                case Direction.Up: newHead.Y--; break;
                case Direction.Down: newHead.Y++; break;
                case Direction.Left: newHead.X--; break;
                case Direction.Right: newHead.X++; break;
            }

            // Добавляем новую голову
            snake.Insert(0, newHead);

            // Проверка съедания еды
            if (newHead == food)
            {
                score++; // Увеличиваем счет
                SpawnFood(); // Генерируем новую еду
            }
            else
            {
                // Удаляем хвост, если еда не съедена
                snake.RemoveAt(snake.Count - 1);
            }
        }

        // Проверка столкновений
        private void CheckCollisions()
        {
            Point head = snake[0];

            // Проверка столкновения с границами экрана
            if (head.X < 0 || head.X >= ClientSize.Width / cellSize ||
                head.Y < 0 || head.Y >= ClientSize.Height / cellSize)
            {
                GameOver();
            }

            // Проверка столкновения с собственным телом
            for (int i = 1; i < snake.Count; i++)
            {
                if (head == snake[i])
                {
                    GameOver();
                    break;
                }
            }
        }

        // Обработка окончания игры
        private void GameOver()
        {
            gameTimer.Stop(); // Останавливаем таймер
            MessageBox.Show($"Game Over! Score: {score}");
            InitializeGame(); // Перезапуск игры
        }

        // Обработка нажатий клавиш
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Меняем направление с проверкой на противоположное
            switch (e.KeyCode)
            {
                case Keys.W when currentDirection != Direction.Down:
                    currentDirection = Direction.Up; break;
                case Keys.S when currentDirection != Direction.Up:
                    currentDirection = Direction.Down; break;
                case Keys.A when currentDirection != Direction.Right:
                    currentDirection = Direction.Left; break;
                case Keys.D when currentDirection != Direction.Left:
                    currentDirection = Direction.Right; break;
            }
        }

        // Отрисовка игровых объектов
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            // Отрисовка змейки
            using (var brush = new SolidBrush(Color.LimeGreen))
            {
                foreach (var segment in snake)
                {
                    // Перевод координат в пиксели
                    e.Graphics.FillRectangle(brush,
                        segment.X * cellSize,
                        segment.Y * cellSize,
                        cellSize,
                        cellSize);
                }
            }

            // Отрисовка еды
            e.Graphics.FillEllipse(Brushes.Red,
                food.X * cellSize,
                food.Y * cellSize,
                cellSize,
                cellSize);

            // Отрисовка счета
            e.Graphics.DrawString($"Score: {score}",
                new Font("Arial", 16),
                Brushes.Bisque,
                new Point(10, 10));
        }

        // Пустой обработчик загрузки формы (может использоваться для дополнительной инициализации)
        private void Form1_Load(object sender, EventArgs e) { }

        // Перечисление возможных направлений движения
        enum Direction { Up, Down, Left, Right }

        
    }

}



