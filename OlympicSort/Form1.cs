using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text;

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
            Random random = new Random();
            dataGridView1.Rows.Clear();

            for (int i = 0; i < 10; i++)
            {
                dataGridView1.Rows.Add(random.Next(100), random.Next(100));
            }
        }
        
        private void calculateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SortNumbers(SortOrder.Ascending);
        }
        private void QuickSortWrapper(List<double> list, SortOrder sortOrder)
        {
            List<double> copy = new List<double>(list);
            QuickSort(copy, 0, copy.Count - 1, sortOrder);
        }
        private void SortNumbers(SortOrder sortOrder)
        { 
            if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked &&
                !checkBox4.Checked && !checkBox5.Checked)
            {
                MessageBox.Show("Не выбран ни один из методов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<double> dataGridViewNumbers = new List<double>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null && double.TryParse(row.Cells[0].Value.ToString(), out double number))
                {
                    dataGridViewNumbers.Add(number);
                }
            }
            if (checkBox1.Checked)
            {
                BubbleSort(dataGridViewNumbers, sortOrder);
            }
            if (checkBox5.Checked)
            {
                InsertionSort(dataGridViewNumbers, sortOrder);
            }
            if (checkBox3.Checked)
            {
                ShakerSort(dataGridViewNumbers, sortOrder);
            }
            if (checkBox2.Checked)
            {
                QuickSort(dataGridViewNumbers, 0, dataGridViewNumbers.Count - 1, sortOrder);
            }
            if (checkBox4.Checked)
            {
                BogoSort(dataGridViewNumbers, sortOrder);
            }

            
            StringBuilder resultBuilder = new StringBuilder();

            if (checkBox1.Checked)
            {
                resultBuilder.AppendLine($"время выполнения пузырьковой: {SortAndMeasureTime(BubbleSort, sortOrder)} нс");
            }
            if (checkBox5.Checked)
            {
                resultBuilder.AppendLine($"время выполнения вставок: {SortAndMeasureTime(InsertionSort, sortOrder)} нс");
            }
            if (checkBox3.Checked)
            {
                resultBuilder.AppendLine($"время выполнения шейкерной: {SortAndMeasureTime(ShakerSort, sortOrder)} нс");
            }
            if (checkBox2.Checked)
            {
                resultBuilder.AppendLine($"время выполнения быстрой: {SortAndMeasureTime(QuickSortWrapper, sortOrder)} нс");
            }
            if (checkBox4.Checked)
            {
                resultBuilder.AppendLine($"время выполнения BOGO: {SortAndMeasureTime(BogoSort, sortOrder)} нс");
            }
            MessageBox.Show(resultBuilder.ToString(), "Затраченное время", MessageBoxButtons.OK, MessageBoxIcon.Information);
            textBox2.Clear();
            for (int i = 0; i < dataGridViewNumbers.Count; i++)
            {
                textBox2.Text += dataGridViewNumbers[i].ToString() + " ";
            }
        }
        private double SortAndMeasureTime(Action<List<double>, SortOrder> sortingMethod, SortOrder sortOrder)
        {
            List<double> numbersToSort = new List<double>(arr);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            sortingMethod(numbersToSort, sortOrder);
            stopwatch.Stop();
            return (double)stopwatch.ElapsedTicks * 1000000000 / Stopwatch.Frequency;
        }
        
        
        private void UpdateChart(List<double> list)
        {
            chart1.Series.Clear();
            chart1.Series.Add("Numbers");

            foreach (var number in list)
            {
                chart1.Series["Numbers"].Points.AddY(number);
            }

            chart1.Invalidate();
        }
        private void BubbleSort(List<double> list, SortOrder sortOrder)
        {
            
            int n = list.Count;
            double temp;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if ((sortOrder == SortOrder.Ascending && list[j] > list[j + 1]) ||
                        (sortOrder == SortOrder.Descending && list[j] < list[j + 1]))
                    {
                        temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                        UpdateChart(list);
                    }
                }
            }
        }


        private void InsertionSort(List<double> list, SortOrder sortOrder)
        {            
            double n = list.Count;
            for (int i = 1; i < n; i++)
            {
                double k = list[i];
                int j = i - 1;

                while ((j >= 0 && sortOrder == SortOrder.Ascending  && list[j] > k) ||
                       (j >= 0 && sortOrder == SortOrder.Descending && list[j] > k))
                {
                    list[j + 1] = list[j];
                    list[j] = k;
                    j--;
                    UpdateChart(list);
                }
            }
        }

        private void QuickSort(List<double> list, int left, int right, SortOrder sortOrder)
        {            
            if (left < right)
            {
                int pivot = Partition(list, left, right, sortOrder);

                QuickSort(list, left, pivot - 1, sortOrder);
                QuickSort(list, pivot + 1, right, sortOrder);
                UpdateChart(list);
            }
        }
        static int Partition(List<double> list, int left, int right, SortOrder sortOrder)
        {
            double pivot = list[right];
            int i = (left - 1);

            for (int j = left; j < right; j++)
            {
                if ((sortOrder == SortOrder.Ascending && list[j] <= pivot) ||
                    (sortOrder == SortOrder.Descending && list[j] <= pivot))
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

        private void ShakerSort(List<double> list, SortOrder sortOrder)
        {
            int left = 0;
            int right = list.Count - 1;
            bool swapped = true;
            while (left < right && swapped)
            {
                swapped = false;
                for (int i = left; i < right; ++i)
                {
                    if ((sortOrder == SortOrder.Ascending && list[i] > list[i + 1]) ||
                        (sortOrder == SortOrder.Descending && list[i] > list[i + 1]))
                    {
                        Swap(list, i, i + 1);
                        swapped = true;
                    }
                }
                --right;
                for (int i = right; i > left; --i)
                {
                    if ((sortOrder == SortOrder.Ascending && list[i] < list[i - 1]) ||
                        (sortOrder == SortOrder.Ascending && list[i] < list[i - 1]))
                    {
                        Swap(list, i, i - 1);
                        swapped = true;
                    }
                }
                ++left;
                UpdateChart(list);
            }
        }
        static void Swap(List<double> list, int i, int j)
        {
            double temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        private void BogoSort(List<double> list, SortOrder sortOrder)
        {            
            Random random = new Random();

            while (!IsSorted(arr, sortOrder))
            {
                Shuffle(arr, random);
                UpdateChart(list);
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

        static bool IsSorted(List<double> list, SortOrder sortOrder)
        {
            for (int i = 1; i < list.Count; i++)
            {
                if ((sortOrder == SortOrder.Ascending && list[i] < list[i - 1]) ||
                    (sortOrder == SortOrder.Descending && list[i] < list[i - 1]))
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
            arr.Clear();
            SortNumbers(SortOrder.Descending);
        }


        private void button1_Click(object sender, EventArgs e)
        {
           GenerateRandomData();
        }
    }
}