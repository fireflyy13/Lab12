using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Lab12
{
    public partial class Form1 : Form
    {
        private int[] initialArray;
        private int[] arrayToSort;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public static int[] Chaotic(int length)
        {
            Random random = new Random();
            int[] a = new int[length];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = random.Next(1, 100000);
            }
            return a;
        }

        public static int[] PartiallySorted(int length)
        {
            Random random = new Random();
            int[] a = new int[length];
            a[0] = random.Next(1, 300);
            for (int i = 1; i < a.Length; i++)
            {
                a[i] = a[i - 1] + random.Next(1, 100000);
            }

            int[] b = Chaotic(length / 2);
            for (int i = 0; i < (length / 2); i++)
            {
                a[i] = b[i];
            }
            return a;
        }

        public static int[] Duplicates(int length)
        {
            Random random = new Random();
            int[] a = new int[length];
            for (int i = 1; i < a.Length; i++)
            {
                a[i] = random.Next(1, 100000);
                if (a[i] % 2 == 0)
                {
                    a[i] = a[i - 1];
                }
            }
            return a;
        }

        public void BubbleSort(int[] a)
        {
            int temp;
            bool swapped;
            for (int i = 0; i < a.Length - 1; i++)
            {
                swapped = false;
                for (int j = 0; j < a.Length - i - 1; j++)
                {
                    if (a[j] > a[j + 1])
                    {
                        temp = a[j];
                        a[j] = a[j + 1];
                        a[j + 1] = temp;
                        swapped = true;
                    }
                }
                if (!swapped)
                {
                    break;
                }
            }
        }

        public static void Swap(int[] a, int i, int j)
        {
            int temp = a[i];
            a[i] = a[j];
            a[j] = temp;
        }

        public static void SortByLSD(int[] a, int radix)
        {
            int n = a.Length;
            int min = a[0];
            int max = a[0];
            
            for (int i = 1; i < n; i++)
            {
                if (a[i] < min)
                {
                    min = a[i];
                }
                else if (a[i] > max)
                {
                    max = a[i];
                }
            }

            int exp = 1;
            while ((max - min) / exp >= 1)
            {
                CountByDigit(a, radix, exp, min);
                exp *= radix;
            }
        }

        public static void CountByDigit(int[] a, int radix, int exp, int min)
        {
            int n = a.Length;
            int bucketIndex;
            int[] buckets = new int[radix];
            int[] result = new int[n];

            for (int i = 0; i < radix; i++)
            {
                buckets[i] = 0;
            }

            for (int i = 0; i < n; i++)
            {
                bucketIndex = ((a[i] - min) / exp) % radix;
                buckets[bucketIndex]++;
            }

            for (int i = 1; i < radix; i++)
            {
                buckets[i] += buckets[i - 1];
            }

            for (int i = n - 1; i >= 0; i--)
            {
                bucketIndex = ((a[i] - min) / exp) % radix;
                result[--buckets[bucketIndex]] = a[i];
            }

            for (int i = 0; i < n; i++)
            {
                a[i] = result[i];
            }
        }


        public static int Partition(int[] a, int low, int high)
        {
            int pivot = a[high];
            int i = low - 1;
            for (int j = low; j <= high - 1; j++)
            {
                if (a[j] < pivot)
                {
                    i++;
                    Swap(a, i, j);
                }
            }
            Swap(a, i + 1, high);
            return i + 1;
        }

        public void SortByMSD(int[] a, int low, int high, int digit)
        {
            if (high <= low || digit < 0)
            {
                return;
            }

            int[] count = new int[12];
            int[] temp = new int[high - low + 1];

            for (int i = low; i <= high; i++)
            {
                int character = (int)(a[i] / Math.Pow(10, digit - 1)) % 10;
                count[character + 2]++;
            }

            for (int j = 0; j < 11; j++)
            {
                count[j + 1] += count[j];
            }

            for (int i = low; i <= high; i++)
            {
                int character = (int)(a[i] / Math.Pow(10, digit - 1)) % 10;
                temp[count[character + 1]++] = a[i];
            }

            for (int i = low; i <= high; i++)
            {
                a[i] = temp[i - low];
            }

            for (int j = 0; j < 10; j++)
            {
                SortByMSD(a, low + count[j], low + count[j + 1] - 1, digit - 1);
            }
        }

        public void RadixSortMSD(int[] a)
        {
            int max = a[0];
            for (int i = 1; i < a.Length; i++)
            {
                if (a[i] > max)
                {
                    max = a[i];
                }
            }

            int digit = (int)Math.Floor(Math.Log10(Math.Abs(max))) + 1;
            SortByMSD(a, 0, a.Length - 1, digit);
        }

       
        public void Chaotic_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            int length;
            if (!int.TryParse(textBox2.Text.Trim(), out length))
            {
                MessageBox.Show("Будь ласка, введіть коректну довжину масиву!", "Помилка", MessageBoxButtons.OK);
                return;
            }

            initialArray = Chaotic(length);
            arrayToSort = (int[])initialArray.Clone();
            PrintArray(textBox1, initialArray, "Початковий масив:");
        }

        public void PartiallySorted_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            int length;
            if (!int.TryParse(textBox2.Text.Trim(), out length))
            {
                MessageBox.Show("Будь ласка, введіть коректну довжину масиву!", "Помилка", MessageBoxButtons.OK);
                return;
            }

            initialArray = PartiallySorted(length);
            arrayToSort = (int[])initialArray.Clone();
            PrintArray(textBox1, initialArray, "Початковий масив:");
        }

        public void Duplicates_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            int length;
            if (!int.TryParse(textBox2.Text.Trim(), out length))
            {
                MessageBox.Show("Будь ласка, введіть коректну довжину масиву!", "Помилка", MessageBoxButtons.OK);
                return;
            }

            initialArray = Duplicates(length);
            arrayToSort = (int[])initialArray.Clone();
            PrintArray(textBox1, initialArray, "Початковий масив:");
        }

        private void MSD_Click(object sender, EventArgs e)
        {
            if (initialArray == null)
            {
                MessageBox.Show("Спочатку згенеруйте масив!", "Помилка", MessageBoxButtons.OK);
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timespan;

            arrayToSort = (int[])initialArray.Clone();
            textBox1.Clear();
            PrintArray(textBox1, initialArray, "Початковий масив:");
            textBox1.AppendText(Environment.NewLine);

            stopwatch.Start();
            RadixSortMSD(arrayToSort);
            stopwatch.Stop();

            PrintArray(textBox1, arrayToSort, "Відсортований масив (MSD RadixSort):");
            timespan = stopwatch.Elapsed;
            label3.Text = $"Час виконання MSD RadixSort в мс: " + (double)timespan.Ticks / 10000;
        }

        public void LSD_Click(object sender, EventArgs e)
        {
            if (initialArray == null)
            {
                MessageBox.Show("Спочатку згенеруйте масив!", "Помилка", MessageBoxButtons.OK);
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timespan;

            arrayToSort = (int[])initialArray.Clone();
            textBox1.Clear();
            PrintArray(textBox1, initialArray, "Початковий масив:");
            textBox1.AppendText(Environment.NewLine);

            stopwatch.Start();
            SortByLSD(arrayToSort, 256);
            stopwatch.Stop();

            PrintArray(textBox1, arrayToSort, "Відсортований масив (LSD Radix Sort):");
            timespan = stopwatch.Elapsed;
            label2.Text = $"Час виконання LSD Radix Sort в мс: " + (double)timespan.Ticks / 10000;
        }

       

        static void PrintArray(System.Windows.Forms.TextBox textBox1, int[] array, string label)
        {
            textBox1.AppendText(label + " " + string.Join(" ", array) + Environment.NewLine);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}


