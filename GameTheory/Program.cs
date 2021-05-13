using System;
using System.Collections.Generic;
using System.Linq;

namespace GameMethod
{
    class Matrix // класс для создания и работы с платежной матрицей
    {
        public Matrix() // создание платежной матрицы 3х3
        {
            int[,] M = new int[3, 3];

            // заполнение случайными числами
            Random rand = new Random();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    M[i, j] = rand.Next(0, 100);

            /* платежная матрица без противоречий
            M[0, 0] = 4;
            M[0, 1] = 2;
            M[0, 2] = 2;
            M[1, 0] = 2;
            M[1, 1] = 5;
            M[1, 2] = 0;
            M[2, 0] = 0;
            M[2, 1] = 2;
            M[2, 2] = 5;
            */

            CheckSaddlePoint(M);
        }
        public void CheckSaddlePoint(int [,] M) // проверка седловой точки
        {
            Console.WriteLine("Матрица:");
            Show(M);

            if (MaxiMin(M) == MiniMax(M))
            {
                Console.WriteLine("\nВ задаче имеется седловая точка. Смешанная стратегия не работает. Демонстрация работы проведется на примере решаемой платежной матрицы:");
                M = SetDefaultMatrix(M);
                Show(M);
            }
            else
                Console.WriteLine("\nВ задаче нет седловой точки.");
            CheckSimplexMethod(M);
        }
        public void Show(int[,] M) // демонстрация матрицы
        {
            for (int i = 0; i < M.GetLength(0); i++)
            {
                for (int j = 0; j < M.GetLength(1); j++)
                {
                    Console.Write(String.Format("{0,3}", M[i, j]));
                }
                Console.WriteLine();
            }
        }
        public int[,] SetDefaultMatrix(int[,] M) // заполнение матрицы базовыми значениями
        {
            M[0, 0] = 4;
            M[0, 1] = 2;
            M[0, 2] = 2;
            M[1, 0] = 2;
            M[1, 1] = 5;
            M[1, 2] = 0;
            M[2, 0] = 0;
            M[2, 1] = 2;
            M[2, 2] = 5;
            return M;
        }
        public void ShowSystemOfEquations(int[,] M) // демонстрация системы ограничений
        {
            Console.WriteLine("\nСистема ограничений:\n");
            Console.WriteLine("f = x1 + x2 + x3 -> max,");
            for (int i = 0; i < 3; i++)
            {
                Console.Write("{ ");
                for (int j = 0; j < 3; j++)
                {
                    if (j != 2)
                    {
                        Console.Write(String.Format("({0})x{1} + ", M[i, j], j + 1));
                    }
                    else
                        Console.Write(String.Format("({0})x{1}", M[i, j], j + 1));
                }
                Console.WriteLine(" <= 1");
            }
            Console.WriteLine("{ xj >= 0 (j = 1, 2, 3)");
        } 
        public int[,] SetSimplexTable(int[,] M) // инициализация симплекс таблицы
        {
            int[,] table = { { 1, 0, 0, 0 }, { 1, 0, 0, 0 }, { 1, 0, 0, 0 }, { 0, -1, -1, -1 } };
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    table[i, j + 1] = M[i, j];

            return table;
        }
        public int[,] TableTranspose(int[,] M) // транспонирование матрицы
        {
            int tmp;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    tmp = M[i, j];
                    M[i, j] = M[j, i];
                    M[j, i] = tmp;
                }
            }
            return M;
        }
        public void CheckSimplexMethod(int[,] M) // проверка возможности решения симплексным методом
        {
            Console.WriteLine("\nРЕШЕНИЕ ЗАДАЧИ СИМПЛЕКСНЫМ МЕТОДОМ");
            ShowSystemOfEquations(M);
            Console.WriteLine("\nСимплекс таблица:\n");
            Show(SetSimplexTable(M));

            Simplex S = new Simplex(SetSimplexTable(M));
            double[] res_x = S.Calculate();

            Console.WriteLine("\nРезультаты:\n");
            Console.WriteLine("X[1] = {0:0.000}", res_x[0]);
            Console.WriteLine("X[2] = {0:0.000}", res_x[1]);
            Console.WriteLine("X[3] = {0:0.000}", res_x[2]);

            if (!CheckResults(res_x)) // перевірка на наявність рішень задачі (якщо корені != 0)
            { 
                Console.WriteLine("\nСистема ограничений противоречива, поэтому задача не имеет решений. Демонстрация работы проведется на примере решаемой платежной матрицы:\n");
                SetDefaultMatrix(M);
                Show(M);
                CheckSimplexMethod(M);
            }
            else
                Calculate(res_x, M);
        }
        public bool CheckResults(double[] res) // проверка найденых корней
        {
            if (res[0] != 0 && res[1] != 0 && res[2] != 0)
                return true;
            else
                return false;

        }
        public void Calculate(double[] res_x, int[,] M) // решение задачи
        {
            Console.WriteLine("\nПолучаем решение прямой задачи:");
            Console.WriteLine("x* = ({0:0.000}; {1:0.000}; {2:0.000})", res_x[0], res_x[1], res_x[2]);
            Console.WriteLine("\nНаходим линейную форму оптимальных планов как сумму найденных координат:");
            Console.WriteLine("f(x*) = {0:0.000}", res_x[0] + res_x[1] + res_x[2]);

            TableTranspose(M);
            
            Simplex S2 = new Simplex(SetSimplexTable(M));
            double[] res_y = S2.Calculate();

            Console.WriteLine("\nРезультаты (двойственная задача):\n");
            Console.WriteLine("Y[1] = {0:0.000}", res_y[0]);
            Console.WriteLine("Y[2] = {0:0.000}", res_y[1]);
            Console.WriteLine("Y[3] = {0:0.000}", res_y[2]);

            Console.WriteLine("\nПолучаем решение двойственной задачи:");
            Console.WriteLine("y* = ({0:0.000}; {1:0.000}; {2:0.000})", res_y[0], res_y[1], res_y[2]);
            Console.WriteLine("\nНаходим линейную форму оптимальных планов как сумму найденных координат:");
            Console.WriteLine("g(y*) = {0:0.000}", res_y[0] + res_y[1] + res_y[2]);

            double price = 1 / (res_y[0] + res_y[1] + res_y[2]);
            Console.WriteLine("\nНаходим цену игры:");
            Console.WriteLine("V = 1/f(x*) = 1/g(y*) = {0:0.000}", price);
            
            Console.WriteLine("\nНаходим оптимальную смешанную стратегию первого игрока:");
            double[] p = { res_y[0] * price, res_y[1] * price, res_y[2] * price };
            Console.WriteLine("p* = V * y* = ({0:0.000}; {1:0.000}; {2:0.000})", p[0], p[1], p[2]);

            Console.WriteLine("\nНаходим оптимальную смешанную стратегию второго игрока:");
            double[] q = { res_x[0] * price, res_x[1] * price, res_x[2] * price };
            Console.WriteLine("q* = V * x* = ({0:0.000}; {1:0.000}; {2:0.000})", q[0], q[1], q[2]);
        }
        public double MaxiMin(int[,] M) // нахождение минимумов и максимина
        {
            int[] Min = new int[3];
            for (int i = 0; i < 3; i++)
            {
                Min[i] = M[i, 0];
                for (int j = 0; j < 3; j++)
                    if (Min[i] > M[i, j])
                        Min[i] = M[i, j];
            }
            Console.Write("\nМинимумы: ");
            for (int i = 0; i < 3; i++)
                Console.Write(String.Format("{0}; ", Min[i]));
            Console.WriteLine("\nМаксимин = {0}.", Min.Max());
            return Min.Max();
        }
        public double MiniMax(int[,] M) // нахождение максимумов и минимакса
        {
            int[] Max = new int[3];
            for (int i = 0; i < 3; i++)
            {
                Max[i] = M[0, i];
                for (int j = 0; j < 3; j++)
                    if (Max[i] < M[j, i])
                        Max[i] = M[j, i];
            }
            Console.Write("\nМаксимумы: ");
            for (int i = 0; i < 3; i++)
                Console.Write(String.Format("{0}; ", Max[i]));
            Console.WriteLine("\nМинимакс = {0}.", Max.Min());
            return Max.Min();
        }
    };
    public class Simplex // класс для решения задачи симплексным методом
    {
        // source - симплекс таблица без базисных переменных
        double[,] table; // симплекс таблица
        double[] result = new double[3];
        int m, n;
        List<int> basis; // список базисных переменных

        public Simplex(int[,] source)
        {
            m = source.GetLength(0);
            n = source.GetLength(1);
            table = new double[m, n + m - 1];
            basis = new List<int>();

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    if (j < n)
                        table[i, j] = source[i, j];
                    else
                        table[i, j] = 0;
                }
                //выставляем коэффициент 1 перед базисной переменной в строке
                if ((n + i) < table.GetLength(1))
                {
                    table[i, n + i] = 1;
                    basis.Add(n + i);
                }
            }
            n = table.GetLength(1);
        }
        public double[] Calculate()
        {
            int mainCol, mainRow; //ведущие столбец и строка
            while (!IsItEnd())
            {
                mainCol = findMainCol();
                mainRow = findMainRow(mainCol);
                basis[mainRow] = mainCol;
                double[,] new_table = new double[m, n];

                for (int j = 0; j < n; j++)
                    new_table[mainRow, j] = table[mainRow, j] / table[mainRow, mainCol];

                for (int i = 0; i < m; i++)
                {
                    if (i == mainRow)
                        continue;

                    for (int j = 0; j < n; j++)
                        new_table[i, j] = table[i, j] - table[i, mainCol] * new_table[mainRow, j];
                }
                table = new_table;
            }
            // result - найденные значения (корни)
            for (int i = 0; i < result.Length; i++)
            {
                int k = basis.IndexOf(i + 1);
                if (k != -1)
                    result[i] = table[k, 0];
                else
                    result[i] = 0;
            }
            return result;
        }
        private bool IsItEnd()
        {
            bool flag = true;
            for (int j = 1; j < n; j++)
                if (table[m - 1, j] < 0)
                {
                    flag = false;
                    break;
                }
            return flag;
        }
        private int findMainCol()
        {
            int mainCol = 1;
            for (int j = 2; j < n; j++)
                if (table[m - 1, j] < table[m - 1, mainCol])
                    mainCol = j;
            return mainCol;
        }
        private int findMainRow(int mainCol)
        {
            int mainRow = 0;
            for (int i = 0; i < m - 1; i++)
                if (table[i, mainCol] > 0)
                {
                    mainRow = i;
                    break;
                }
            for (int i = mainRow + 1; i < m - 1; i++)
                if ((table[i, mainCol] > 0) && ((table[i, 0] / table[i, mainCol]) < (table[mainRow, 0] / table[mainRow, mainCol])))
                    mainRow = i;
            return mainRow;
        }
    };
    class Program
    {
        static void Main(string[] args)
        {
            Matrix G = new Matrix();
            Console.ReadKey();
        }
    }
}
