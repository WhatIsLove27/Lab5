using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Lab4
{
    public partial class Form1 : Form
    {
        private double currentValue = 0;
        private double storedValue = 0;
        private string currentOperation = "";
        private bool isNewNumber = true;
        private bool operationPending = false;
        private List<string> operationHistory = new List<string>();

        public Form1()
        {
            InitializeComponent();
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            txtDisplay.Text = currentValue.ToString();
        }

        private void UpdateHistoryDisplay()
        {
            txtHistory.Text = string.Join(Environment.NewLine, operationHistory);
            txtHistory.SelectionStart = txtHistory.Text.Length;
            txtHistory.ScrollToCaret();
        }

        private void AddToHistory(string entry)
        {
            operationHistory.Add(entry);
            UpdateHistoryDisplay();
        }

        private void btnNumber_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string number = button.Text;

            if (isNewNumber)
            {
                currentValue = number == "." ? 0 : double.Parse(number);
                isNewNumber = false;
            }
            else
            {
                if (number == "." && txtDisplay.Text.Contains("."))
                    return;

                currentValue = double.Parse(txtDisplay.Text + number);
            }

            UpdateDisplay();
        }

        private void btnOperation_Click(object sender, EventArgs e)
        {
            if (operationPending)
                CalculateResult();

            Button button = (Button)sender;
            currentOperation = button.Text;
            storedValue = currentValue;
            isNewNumber = true;
            operationPending = true;
        }

        private void btnEquals_Click(object sender, EventArgs e)
        {
            CalculateResult();
            currentOperation = "";
            operationPending = false;
        }

        private void CalculateResult()
        {
            try
            {
                string historyEntry = $"{storedValue} {currentOperation} {currentValue} = ";

                switch (currentOperation)
                {
                    case "+":
                        currentValue = storedValue + currentValue;
                        break;
                    case "-":
                        currentValue = storedValue - currentValue;
                        break;
                    case "*":
                        currentValue = storedValue * currentValue;
                        break;
                    case "/":
                        if (currentValue == 0)
                            throw new DivideByZeroException();
                        currentValue = storedValue / currentValue;
                        break;
                    case "%":
                        currentValue = storedValue * currentValue / 100;
                        break;
                }

                historyEntry += currentValue;
                AddToHistory(historyEntry);
                UpdateDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка вычисления",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearAll();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (button.Text == "C")
                ClearAll();
            else if (button.Text == "CE")
                ClearEntry();
        }

        private void ClearAll()
        {
            currentValue = 0;
            storedValue = 0;
            currentOperation = "";
            isNewNumber = true;
            operationPending = false;
            UpdateDisplay();
        }

        private void ClearEntry()
        {
            currentValue = 0;
            isNewNumber = true;
            UpdateDisplay();
        }

        private void btnSpecial_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            try
            {
                string historyEntry = "";

                switch (button.Text)
                {
                    case "1/x":
                        if (currentValue == 0)
                            throw new DivideByZeroException();
                        historyEntry = $"1/({currentValue}) = ";
                        currentValue = 1 / currentValue;
                        break;
                    case "x²":
                        historyEntry = $"({currentValue})² = ";
                        currentValue = Math.Pow(currentValue, 2);
                        break;
                    case "√x":
                        if (currentValue < 0)
                            throw new ArgumentException("Корень из отрицательного числа");
                        historyEntry = $"√({currentValue}) = ";
                        currentValue = Math.Sqrt(currentValue);
                        break;
                    case "+/-":
                        currentValue = -currentValue;
                        UpdateDisplay();
                        return;
                }

                historyEntry += currentValue;
                AddToHistory(historyEntry);
                UpdateDisplay();
                isNewNumber = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка вычисления",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearAll();
            }
        }

        private void btnSaveHistory_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt",
                Title = "Сохранить журнал операций"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllLines(saveFileDialog.FileName, operationHistory);
                    MessageBox.Show("Журнал успешно сохранен", "Сохранение",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLoadHistory_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt",
                Title = "Загрузить журнал операций"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    operationHistory = new List<string>(File.ReadAllLines(openFileDialog.FileName));
                    UpdateHistoryDisplay();
                    MessageBox.Show("Журнал успешно загружен", "Загрузка",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            operationHistory.Clear();
            UpdateHistoryDisplay();
        }
    }
}