using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using Devart.Data;
using Devart.Data.Oracle;
using System.Text.RegularExpressions;

namespace WpfDB
{
    /// <summary>
    /// Interaction logic for RowUpdate.xaml
    /// </summary>
    public partial class RowUpdate : Window
    {
        public string InputResult { get; set; }

        public RowUpdate()
        {
            InitializeComponent();
        }
        public RowUpdate(string defaultResult)
        {
            InitializeComponent();

            this.InputResult = defaultResult;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBox.Text = InputResult;
            textBox.Focus();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            InputResult = textBox.Text;
            this.DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
    }
}
