using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
//using Microsoft.Office.Interop.Excel;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;

namespace OlympicSort
{
    public partial class Form1 : Form
    {
        private int[] numbers;

        public Form1()
        {
            InitializeComponent();
        }

       /* private void LoadFromExcelButton_Click(object sender, EventArgs e)
        {
            // Code to load data from Excel
            LoadDataFromExcel();
        }

        private void LoadFromGoogleSheetsButton_Click(object sender, EventArgs e)
        {
            // Code to load data from Google Sheets
            LoadDataFromGoogleSheets();
        }

        private void GenerateDataButton_Click(object sender, EventArgs e)
        {
            // Code to generate random data
            GenerateRandomData();
        }

        private void LoadDataFromExcel()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                var workbook = excelApp.Workbooks.Open(openFileDialog.FileName);
                var worksheet = (Worksheet)workbook.Worksheets[1];
                var range = worksheet.UsedRange;

                dataGridView.Rows.Clear();

                for (int i = 1; i <= range.Rows.Count; i++)
                {
                    // Assuming your DataGridView has columns named "Column1", "Column2", ...
                    dataGridView.Rows.Add(((Range)range.Cells[i, 1]).Value, ((Range)range.Cells[i, 2]).Value);
                }

                workbook.Close(false);
                excelApp.Quit();
            }
        }

        private void LoadDataFromGoogleSheets()
        {
            string spreadsheetId = "forolympicsort";
            string range = "Sheet1!A1:B10"; // Range in A1 notation

            // Path to your JSON service account key file downloaded from Google Cloud Console
            string jsonPath = "C:\\Users\anast\\Downloads\\forolympicsort-c34568779cb0.json";

            var service = GetSheetsService(jsonPath);
            var values = ReadDataFromSheet(service, spreadsheetId, range);

            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    Console.WriteLine($"{row[0]}, {row[1]}");
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
        }
        static SheetsService GetSheetsService(string jsonPath)
        {
            var credential = GoogleCredential.FromFile(jsonPath)
                .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Sheets API C# Example",
            });

            return service;
        }

        static IList<IList<object>> ReadDataFromSheet(SheetsService service, string spreadsheetId, string range)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;

            return values;
        }

        private void GenerateRandomData()
        {
            // Generate random data and populate the DataGridView
            Random random = new Random();
            dataGridView.Rows.Clear();

            for (int i = 0; i < 10; i++)
            {
                dataGridView.Rows.Add(random.Next(100), random.Next(100));
            }
        }*/

        private void calculateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] numbersAsString = textBox1.Text.Split(' ');

            double[] arr = new double[numbersAsString.Length];
            bool can = true;

            for (int i = 0; i < numbersAsString.Length; i++)
            {
                if (double.TryParse(numbersAsString[i], out double number))
                {
                    arr[i] = number;
                }
                else
                {
                    MessageBox.Show($"Ошибка преобразования числа: {numbersAsString[i]}");
                    can = false;
                }
            }

            if (checkBox1.Checked)
            {
                BubbleSort(arr);                
            }
            else if (checkBox5.Checked)
            {
                InsertionSort(arr);
                
            }
            else if (checkBox3.Checked)
            {
                ShakerSort(arr);
                
            }
            else if (checkBox2.Checked)
            {
                QuickSort(arr, 0, arr.Length - 1);
                
            }
            else if (checkBox4.Checked)
            {
                BogoSort(arr);
            }
            textBox2.Clear();
            for (int i = 0; i < arr.Length; i++)
            {
                if (can)
                {
                    textBox2.Text += arr[i].ToString() + " ";
                }
            }
        }
        static void BubbleSort(double[] arr, bool ascending = true, Label timeLabel = null)
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int n = arr.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    bool swapRequired = ascending ? arr[j] > arr[j + 1] : arr[j] < arr[j + 1];
                    if (swapRequired)
                    {
                        double temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
            }

            stopwatch.Stop();
            var answer1 = stopwatch.Elapsed;
            //MessageBox.Show($"время выполнения пузырьковой: {answer1}");
        }


        static void InsertionSort(double[] arr, bool ascending = true, Label timeLabel = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            int n = arr.Length;
            for (int i = 1; i < n; ++i)
            {
                double key = arr[i];
                int j = i - 1;

                // Move elements of arr[0..i-1], that are greater (or less) than key,
                // to one position ahead of their current position
                while (j >= 0 && ((ascending && arr[j] > key) || (!ascending && arr[j] < key)))
                {
                    arr[j + 1] = arr[j];
                    j = j - 1;
                }
                arr[j + 1] = key;
            }

            stopwatch.Stop();
            var answer2 = stopwatch.Elapsed;
            MessageBox.Show($"время выполнения вставками: {answer2}");
            //timeExecution[1] = stopwatch.Elapsed;
        }

        static void QuickSort(double[] arr, int left, int right)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (left < right)
            {
                int pivot = Partition(arr, left, right);

                QuickSort(arr, left, pivot - 1);
                QuickSort(arr, pivot + 1, right);
            }

            stopwatch.Stop();
            var answer3 = stopwatch.Elapsed;
            MessageBox.Show($"время выполнения быстрой: {answer3}");
           
        }
        static int Partition(double[] arr, int left, int right)
        {
            double pivot = arr[right];
            int i = (left - 1);

            for (int j = left; j < right; j++)
            {
                if (arr[j] <= pivot)
                {
                    i++;
                    double temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }
            }
            double temp1 = arr[i + 1];
            arr[i + 1] = arr[right];
            arr[right] = temp1;

            return i + 1;
        }

        private void ShakerSort(double[] arr)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            bool swapped;
            do
            {
                swapped = false;
                for (int i = 0; i <= arr.Length - 2; i++)
                {
                    if (arr[i] > arr[i + 1])
                    {
                        Swap(arr, i, i + 1);
                        swapped = true;
                    }
                }

                if (!swapped)
                {
                    break;
                }

                swapped = false;

                for (int i = arr.Length - 2; i >= 0; i--)
                {
                    if (arr[i] > arr[i + 1])
                    {
                        Swap(arr, i, i + 1);
                        swapped = true;
                    }
                }
            } while (swapped);
            stopwatch.Stop();
            var answer4 = stopwatch.Elapsed;
            MessageBox.Show($"время выполнения шейкерной: {answer4}");
        }
        static void Swap(double[] arr, int i, int j)
        {
            double temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }

        private void BogoSort(double[] arr)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Random random = new Random();

            while (!IsSorted(arr))
            {
                Shuffle(arr, random);
            }
            stopwatch.Stop();
            var answer5 = stopwatch.Elapsed;
            MessageBox.Show($"время выполнения бого: {answer5}");
        }
        static void Shuffle(double[] arr, Random random)
        {
            int n = arr.Length;
            for (int i = 0; i < n; i++)
            {
                int randomIndex = i + random.Next(n - i);
                double temp = arr[i];
                arr[i] = arr[randomIndex];
                arr[randomIndex] = temp;
            }
        }

        static bool IsSorted(double[] arr)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] < arr[i - 1])
                {
                    return false;
                }
            }
            return true;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }

        private void menuStrip3_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

       /* private void drawMarking()
        {
            buffered = BufferedGraphicsManager.Current.Allocate(pictureBox1.CreateGraphics(), pictureBox1.DisplayRectangle);
            Pen pen = new Pen(Color.DarkGreen);

            for (int i = 0; i < pictureBox1.Height; i += 15)
                buffered.Graphics.DrawLine(pen, 0, pictureBox1.Height - i, pictureBox1.Width, pictureBox1.Height - i);
            for (int i = 0; i < pictureBox1.Width; i += 15)
                buffered.Graphics.DrawLine(pen, i, 0, i, pictureBox1.Width);
        }
        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            drawMarking();
            buffered.Render();
        }*/
    }
}