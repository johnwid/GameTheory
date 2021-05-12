using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMethod
{
    class Matrix
    {
        public Matrix() // створення матриці 3х3
        {
            int[,] M = new int[3, 3];

            // заповнення випадковими величинами
            Random rand = new Random();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    M[i, j] = rand.Next(0, 100);
            
            /*таблиця без протиріч, має рішення
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

            CheckMatrix(M);
        }
        public void CheckMatrix(int [,] M) // демонстрація матриці
        {
            Console.WriteLine("Матрица:");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write(String.Format("{0,-3}", M[i, j]));
                }
                Console.WriteLine();
            }

            if (MaxiMin(M) == MiniMax(M))
            {
                Console.WriteLine("\nВ задаче имеется седловая точка. Смешанная стратегия не работает. Демонстрация работы проведется на примере решаемой платежной матрицы:");
                M[0, 0] = 4;
                M[0, 1] = 2;
                M[0, 2] = 2;
                M[1, 0] = 2;
                M[1, 1] = 5;
                M[1, 2] = 0;
                M[2, 0] = 0;
                M[2, 1] = 2;
                M[2, 2] = 5;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Console.Write(String.Format("{0,-3}", M[i, j]));
                    }
                    Console.WriteLine();
                }
            }
            SimplexMethod(M);
        }
        public void SimplexMethod(int[,] M)
        {
            int[,] table = {{1, 0, 0, 0},{1, 0, 0, 0},{1, 0, 0, 0},{0, -1, -1, -1}}; // створення каркасу симплексної таблиці
            Console.WriteLine("\nСистема ограничений:\n");
            Console.WriteLine("f = x1 + x2 + x3 -> max,");
            for (int i = 0; i < 3; i++)
            {
                Console.Write("{ ");
                for (int j = 0; j < 3; j++)
                {
                    if(j != 2)
                    {
                        Console.Write(String.Format("({0})x{1} + ", M[i, j], j + 1));
                    }
                    else
                        Console.Write(String.Format("({0})x{1}", M[i, j], j + 1));
                }
                Console.WriteLine(" <= 1");
            }
            Console.WriteLine("{ xj >= 0 (j = 1, 2, 3)");
            
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    table[i,j+1] = M[i, j];

            Console.WriteLine("\nРЕШЕНИЕ ЗАДАЧИ СИМПЛЕКСНЫМ МЕТОДОМ");

            Console.WriteLine("\nСимплекс таблица:\n");
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Console.Write(String.Format("{0,3}", table[i, j]));
                }
                Console.WriteLine();
            }

            Simplex S = new Simplex(table);
            double[] res = S.Calculate();

            Console.WriteLine("\nРезультаты:\n");
            Console.WriteLine("X[1] = {0:0.000}", res[0]);
            Console.WriteLine("X[2] = {0:0.000}", res[1]);
            Console.WriteLine("X[3] = {0:0.000}", res[2]);

            if (!CheckResults(res))
            { // перевірка на наявність рішень задачі (якщо корені != 0)
                Console.WriteLine("\nСистема ограничений противоречива, поэтому задача не имеет решений. Демонстрация работы проведется на примере решаемой платежной матрицы:\n");
                M[0, 0] = 4;
                M[0, 1] = 2;
                M[0, 2] = 2;
                M[1, 0] = 2;
                M[1, 1] = 5;
                M[1, 2] = 0;
                M[2, 0] = 0;
                M[2, 1] = 2;
                M[2, 2] = 5;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Console.Write(String.Format("{0,-3}", M[i, j]));
                    }
                    Console.WriteLine();
                }
                SimplexMethod(M);
            }
            else
                Calculate(res, M);
        }
        public bool CheckResults(double[] res) // перевірка на наявність рішень задачі (якщо всі корені != 0)
        {
            if (res[0] != 0 && res[1] != 0 && res[2] != 0)
                return true;
            else
                return false;

        }
        public void Calculate(double[] res, int[,] M)
        {
            Console.WriteLine("\nПолучаем решение прямой задачи:");
            Console.WriteLine("x* = ({0:0.000}; {1:0.000}; {2:0.000})", res[0], res[1], res[2]);
            Console.WriteLine("\nНаходим линейную форму оптимальных планов как сумму найденных координат:");
            Console.WriteLine("f(x*) = {0:0.000}", res[0] + res[1] + res[2]);

            // транспонування матриці для вирішення двоїстої задачі
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

            int[,] table2 = { { 1, 0, 0, 0 }, { 1, 0, 0, 0 }, { 1, 0, 0, 0 }, { 0, -1, -1, -1 } };
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    table2[i, j + 1] = M[i, j];


            Simplex S2 = new Simplex(table2);
            double[] res2 = S2.Calculate();

            Console.WriteLine("\nРезультаты (двойственная задача):\n");
            Console.WriteLine("Y[1] = {0:0.000}", res2[0]);
            Console.WriteLine("Y[2] = {0:0.000}", res2[1]);
            Console.WriteLine("Y[3] = {0:0.000}", res2[2]);

            Console.WriteLine("\nПолучаем решение двойственной задачи:");
            Console.WriteLine("y* = ({0:0.000}; {1:0.000}; {2:0.000})", res2[0], res2[1], res2[2]);
            Console.WriteLine("\nНаходим линейную форму оптимальных планов как сумму найденных координат:");
            Console.WriteLine("g(y*) = {0:0.000}", res2[0] + res2[1] + res2[2]);

            double price = 1 / (res2[0] + res2[1] + res2[2]);
            Console.WriteLine("\nНаходим цену игры:");
            Console.WriteLine("V = 1/f(x*) = 1/g(y*) = {0:0.000}", price);
            
            Console.WriteLine("\nНаходим оптимальную смешанную стратегию первого игрока:");
            double[] p = { res2[0] * price, res2[1] * price, res2[2] * price };
            Console.WriteLine("p* = V * y* = ({0:0.000}; {1:0.000}; {2:0.000})", p[0], p[1], p[2]);

            Console.WriteLine("\nНаходим оптимальную смешанную стратегию второго игрока:");
            double[] q = { res[0] * price, res[1] * price, res[2] * price };
            Console.WriteLine("q* = V * x* = ({0:0.000}; {1:0.000}; {2:0.000})", q[0], q[1], q[2]);
        }
        public double MaxiMin(int[,] M) // знаходження мінімумів та Максиміну
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
        public double MiniMax(int[,] M) // знаходження максимумів та Мінімаксу
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
    public class Simplex
    {
        //source - симплекс таблица без базисных переменных
        double[,] table; //симплекс таблица
        double[] result = new double[3];
        int m, n;

        List<int> basis; //список базисных переменных

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

        //result - в этот массив будут записаны полученные значения X
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
            //заносим в result найденные значения X
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
            {
                if (table[m - 1, j] < 0)
                {
                    flag = false;
                    break;
                }
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
