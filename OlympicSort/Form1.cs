using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;

namespace OlympicSort
{
    public partial class Form1 : Form
    {

        private List<double> arr = new List<double>();
        public Form1()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            dataGridView1.ColumnCount = 1;
            dataGridView1.Columns[0].Name = "Число";
        }

        private void GenerateRandomData()
        {
            // Generate random data and populate the DataGridView
            Random random = new Random();
            dataGridView1.Rows.Clear();

            for (int i = 0; i < 10; i++)
            {
                dataGridView1.Rows.Add(random.Next(100), random.Next(100));
            }
        }
        Dictionary<string, TimeSpan> sortingMethodsTimes = new Dictionary<string, TimeSpan>();
        private void calculateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool can = true;

            arr.Clear();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null && double.TryParse(row.Cells[0].Value.ToString(), out double number))
                {
                    arr.Add(number);
                }
            }

            if (checkBox1.Checked)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                string method = "Bubble Sort";
                BubbleSort(arr);
                stopwatch.Stop();
                //MessageBox.Show($"Время выполнения пузырьковой: {stopwatch.Elapsed}");
                sortingMethodsTimes.Add(method, stopwatch.Elapsed);
            }
            else if (checkBox5.Checked)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                string method = "Insertion Sort";
                InsertionSort(arr);
                stopwatch.Stop();
                //MessageBox.Show($"Время выполнения вставками: {stopwatch.Elapsed}");
                sortingMethodsTimes.Add(method, stopwatch.Elapsed);
            }
            else if (checkBox3.Checked)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                string method = "Shaker Sort";
                ShakerSort(arr);
                stopwatch.Stop();
                //MessageBox.Show($"Время выполнения шейкерной: {stopwatch.Elapsed}");
                sortingMethodsTimes.Add(method, stopwatch.Elapsed);
            }
            else if (checkBox2.Checked)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                string method = "Quick Sort";
                QuickSort(arr, 0, arr.Count - 1);
                stopwatch.Stop();
                //MessageBox.Show($"Время выполнения быстрой: {stopwatch.Elapsed}");
                sortingMethodsTimes.Add(method, stopwatch.Elapsed);
            }
            else if (checkBox4.Checked)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Restart();
                string method = "BOGO Sort";
                BogoSort(arr);
                stopwatch.Stop();
                var time = stopwatch.Elapsed;
                //MessageBox.Show($"Время выполнения BOGO: {stopwatch.Elapsed}");
                sortingMethodsTimes.Add(method, stopwatch.Elapsed);
            }
            if (sortingMethodsTimes.Count > 0)
            {
                var fastestMethod = sortingMethodsTimes.OrderBy(x => x.Value).First();
                MessageBox.Show($"Самый быстрый метод сортировки: {fastestMethod.Key}\n" +
                                $"Время выполнения: {fastestMethod.Value}");
            } 
            textBox2.Clear();
            for (int i = 0; i < arr.Count; i++)
            {
                if (can)
                {
                    textBox2.Text += arr[i].ToString() + " ";
                } 
            }
        }
        private string FindFastestSortingMethod(List<double> list)
        {
            Dictionary<string, TimeSpan> sortingMethodsTimes = new Dictionary<string, TimeSpan>();

            List<double> copyList = new List<double>(list);
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            BubbleSort(copyList);
            stopwatch.Stop();
            sortingMethodsTimes.Add("Bubble Sort", stopwatch.Elapsed);

            //copyList = new List<double>(list);

            stopwatch.Restart();
            QuickSort(copyList, 0, copyList.Count - 1);
            stopwatch.Stop();
            sortingMethodsTimes.Add("Quick Sort", stopwatch.Elapsed);
            

            stopwatch.Restart();
            ShakerSort(copyList);
            stopwatch.Stop();
            sortingMethodsTimes.Add("Shaker Sort", stopwatch.Elapsed);

            stopwatch.Restart();
            BogoSort(copyList);
            stopwatch.Stop();
            sortingMethodsTimes.Add("BOGO Sort", stopwatch.Elapsed);

            stopwatch.Restart();
            InsertionSort(copyList);
            stopwatch.Stop();
            sortingMethodsTimes.Add("Insertion Sort", stopwatch.Elapsed);

            string fastestMethod = sortingMethodsTimes.OrderBy(x => x.Value).First().Key;

            return fastestMethod;
        }

        private void UpdateChart(List<double> list)
        {
            chart1.Series.Clear();
            chart1.Series.Add("Numbers");

            foreach (var number in list)
            {
                chart1.Series["Numbers"].Points.AddY(number);
            }

            // Обновите график
            chart1.Invalidate();
        }
        private void BubbleSort(List<double> list)
        {
            
            int n = list.Count;
            double temp;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (list[j] > list[j + 1])
                    {
                        // Обменять элементы и запомнить их индексы
                        temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                        UpdateChart(list);

                        // Приостановка выполнения для визуализации процесса сортировки
                        //Thread.Sleep(100);
                    }
                }
            }
        }


        private void InsertionSort(List<double> list)
        {            
            double n = list.Count;
            for (int i = 1; i < n; i++)
            {
                double k = list[i];
                int j = i - 1;

                while (j >= 0 && list[j] > k)
                {
                    list[j + 1] = list[j];
                    list[j] = k;
                    j--;
                    UpdateChart(list);

                    // Приостановка выполнения для визуализации процесса сортировки
                    //Thread.Sleep(100);
                }
            }
        }

        private void QuickSort(List<double> list, int left, int right)
        {
            List<(double Index1, double Index2)> swapSteps = new List<(double Index1, double Index2)>();
            if (left < right)
            {
                int pivot = Partition(list, left, right);

                QuickSort(list, left, pivot - 1);
                QuickSort(list, pivot + 1, right);
                UpdateChart(list);

                // Приостановка выполнения для визуализации процесса сортировки
               //Thread.Sleep(100);
            }
        }
        static int Partition(List<double> list, int left, int right)
        {
            double pivot = list[right];
            int i = (left - 1);

            for (int j = left; j < right; j++)
            {
                if (list[j] <= pivot)
                {
                    i++;
                    double temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                }
            }
            double temp1 = list[i + 1];
            list[i + 1] = list[right];
            list[right] = temp1;

            return i + 1;
        }

        private void ShakerSort(List<double> list)
        {            
            bool swapped;
            do
            {
                swapped = false;
                for (int i = 0; i <= list.Count - 2; i++)
                {
                    if (arr[i] > arr[i + 1])
                    {
                        Swap(list, i, i + 1);
                        swapped = true;
                    }
                }

                if (!swapped)
                {
                    break;
                }

                swapped = false;

                for (int i = list.Count - 2; i >= 0; i--)
                {
                    if (arr[i] > arr[i + 1])
                    {
                        Swap(list, i, i + 1);
                        swapped = true;
                    }
                }
                UpdateChart(list);

                // Приостановка выполнения для визуализации процесса сортировки
                //Thread.Sleep(100);
            } while (swapped);
        }
        static void Swap(List<double> list, int i, int j)
        {
            double temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        private void BogoSort(List<double> list)
        {
            List<(double Index1, double Index2)> swapSteps = new List<(double Index1, double Index2)>();
            Random random = new Random();

            while (!IsSorted(arr))
            {
                Shuffle(arr, random);
                UpdateChart(list);

                // Приостановка выполнения для визуализации процесса сортировки
                //Thread.Sleep(100);
            }
        }
        static void Shuffle(List<double> list, Random random)
        {
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                int randomIndex = i + random.Next(n - i);
                double temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        static bool IsSorted(List<double> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i] < list[i - 1])
                {
                    return false;
                }
            }
            return true;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox2.Clear();            
        }

        private void menuStrip3_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void reverseToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            bool can = true;
            arr.Clear();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null && double.TryParse(row.Cells[0].Value.ToString(), out double number))
                {
                    arr.Add(number);
                }
            }
            if (checkBox1.Checked)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                BubbleSort(arr);
                arr.Reverse();
                stopwatch.Stop();
                MessageBox.Show($"Время выполнения пузырьковой: {stopwatch.Elapsed}");
            }
            else if (checkBox5.Checked)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();InsertionSort(arr);
                arr.Reverse();
                stopwatch.Stop();
                MessageBox.Show($"Время выполнения вставками: {stopwatch.Elapsed}");
            }
            else if (checkBox3.Checked)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                ShakerSort(arr);
                arr.Reverse();
                stopwatch.Stop();
                MessageBox.Show($"Время выполнения шейкерной: {stopwatch.Elapsed}");
            }
            else if (checkBox2.Checked)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                QuickSort(arr, 0, arr.Count - 1);
                arr.Reverse();
                stopwatch.Stop();
                MessageBox.Show($"Время выполнения быстрой: {stopwatch.Elapsed}");
            }
            else if (checkBox4.Checked)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                BogoSort(arr);
                arr.Reverse();
                stopwatch.Stop();
                MessageBox.Show($"Время выполнения BOGO: {stopwatch.Elapsed}");
            }
            textBox2.Clear();
            for (int i = 0; i < arr.Count; i++)
            {
                if (can)
                {
                    textBox2.Text += arr[i].ToString() + " ";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GenerateRandomData();
        }
    }
}