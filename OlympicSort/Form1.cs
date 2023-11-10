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

            for (int i = 0; i < 11; i++)
            {
                double number;
                int dec = random.Next(1, 3);
                number = Math.Round(random.NextDouble() * 201 - 100, dec);
                dataGridView1.Rows.Add(number);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            GenerateRandomData();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double cellValue;
            int rCnt;
            int cCnt;

            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Excel (*.XLSX)|*.XLSX";
            opf.ShowDialog();
            System.Data.DataTable tb = new System.Data.DataTable();
            string filename = opf.FileName;

            Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook ExcelWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet ExcelWorkSheet;
            Microsoft.Office.Interop.Excel.Range ExcelRange;

            ExcelWorkBook = ExcelApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false,
                false, 0, true, 1, 0);
            ExcelWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)ExcelWorkBook.Worksheets.get_Item(1);

            ExcelRange = ExcelWorkSheet.UsedRange;
            dataGridView1.RowCount = 7;
            dataGridView1.ColumnCount = 1;
            for (rCnt = 1; rCnt <= ExcelRange.Rows.Count; rCnt++)
            {
                dataGridView1.Rows.Add(1);
                for (cCnt = 1; cCnt <= 1; cCnt++)
                {
                    object cellValueObject = ExcelRange.Cells[rCnt, cCnt].Value2;

                    if (cellValueObject != null && cellValueObject != DBNull.Value)
                    {
                        cellValue = (double)(ExcelRange.Cells[rCnt, cCnt] as Microsoft.Office.Interop.Excel.Range).Value2;
                        dataGridView1.Rows[rCnt - 1].Cells[cCnt - 1].Value = cellValue;
                    }
                    else
                    {
                        dataGridView1.Rows[rCnt - 1].Cells[cCnt - 1].Value = 0; 
                    }
                }

            }
            ExcelWorkBook.Close(true, null, null);
            ExcelApp.Quit();

            releaseObject(ExcelWorkSheet);
            releaseObject(ExcelWorkBook);
            releaseObject(ExcelApp);
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
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

        public struct SortStats
        {
            public double Time { get; set; }
            public int Iterations { get; set; }
        }
        private void SortNumbers(SortOrder sortOrder)
        {  // выбрана ли хотя бы однасортировка?
            if (!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked &&
                !checkBox4.Checked && !checkBox5.Checked)
            {
                MessageBox.Show("Отсутствуют данные для сортировки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            Dictionary<string, SortStats> sortStats = new Dictionary<string, SortStats>();

            if (checkBox1.Checked)
            {
                sortStats["Пузырьковая"] = MeasureSortingStats(() => BubbleSort(dataGridViewNumbers, sortOrder));                               
            }
            if (checkBox5.Checked)
            {
                sortStats["Вставками"] = MeasureSortingStats(() => InsertionSort(dataGridViewNumbers, sortOrder));
            }
            if (checkBox3.Checked)
            {
                sortStats["Шейкерная"] = MeasureSortingStats(() => ShakerSort(dataGridViewNumbers, sortOrder));
            }
            if (checkBox2.Checked)
            {
                sortStats["Быстрая"] = MeasureSortingStats(() => QuickSortWrapper(dataGridViewNumbers, sortOrder));
            }
            if (checkBox4.Checked)
            {
                sortStats["BOGO"] = MeasureSortingStats(() => BogoSort(dataGridViewNumbers, sortOrder));
            }
            ShowSortingStats(sortStats);
            textBox2.Clear();
            for (int i = 0; i < dataGridViewNumbers.Count; i++)
            {
                textBox2.Text += dataGridViewNumbers[i].ToString() + " ";
            }
        }

        private SortStats MeasureSortingStats(Action sortingAction)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            sortingAction();
            stopwatch.Stop();
            double time = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency * 1000000000;
            return new SortStats { Time = time, Iterations = count };
        }

        private void ShowSortingStats(Dictionary<string, SortStats> sortStats)
        {
            StringBuilder resultBuilder = new StringBuilder();
            foreach (var kvp in sortStats)
            {
                resultBuilder.AppendLine($"{kvp.Key}: Время выполнения - {kvp.Value.Time} нс, Количество итераций - {kvp.Value.Iterations}");
            }
            MessageBox.Show(resultBuilder.ToString(), "Результаты сортировки", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        int count;
        private void BubbleSort(List<double> list, SortOrder sortOrder)
        {
            
            int n = list.Count;
            double temp;
            for (int i = 0; i < n - 1; i++)
            {
                //count++;
                for (int j = 0; j < n - i - 1; j++)
                {
                    ++count;
                    if ((sortOrder == SortOrder.Ascending && list[j] > list[j + 1]) ||
                        (sortOrder == SortOrder.Descending && list[j] < list[j + 1]))
                    {
                        temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;

                        UpdateChart(list);

                    }
                }
                count++;
            }
            //MessageBox.Show($"Iterations: {count}");
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
                    count++;
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
            }
            UpdateChart(list);
            count++;
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
                count++;
            }
            //MessageBox.Show($"Количество итераций для шейкерной сортировки: {count}", "Итерации", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            while (!IsSorted(list, sortOrder))
            {
                Shuffle(list, random);
            }
            UpdateChart(list);
            count++;
        }
        static void Shuffle(List<double> list, Random random)//, Random random)
        {
            int n = list.Count;
            //Random rand = new Random();
            while (n > 1)
            {
                --n;
                int randomIndex = random.Next(n + 1);
                double temp = list[randomIndex];
                list[randomIndex] = list[n];
                list[n] = temp;
            }
        }

        static bool IsSorted(List<double> list, SortOrder sortOrder)
        {
            for (int i = 1; i < list.Count; i++)
            {
                if ((sortOrder == SortOrder.Ascending && list[i - 1] > list[i ]) ||
                    (sortOrder == SortOrder.Descending && list[i - 1] < list[i]))
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
    }
}