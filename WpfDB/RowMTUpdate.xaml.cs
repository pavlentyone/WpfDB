using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data;
using Devart.Data;
using Devart.Data.Oracle;
using System.Text.RegularExpressions;

namespace WpfDB
{
    /// <summary>
    /// Interaction logic for RowMTUpdate.xaml
    /// </summary>
    /// 
    
    
    public partial class RowMTUpdate : Window
    {
        //таблицы участков, отступлений, проходов
        public static OracleDataTable regionOrclDB;
        public static OracleDataTable passageOrclDB;
        public static OracleDataTable canalOrclDB;

        //поля, которые представляют собой поля ввода. они нужны из-за того, что они public, 
        //а значит после окончания работы текущего окна можно взять эти значения в окне, вызвавшее текущее окно
        public DateTime date { set; get; }
        public string km_pk { set; get; }
        public decimal region { set; get; }
        public decimal passage { set; get; }
        public decimal canal { set; get; }
        public string example { set; get; }

        //списки hid участка, отступа и прохода
        //начальные инициализации выборанных элементов в ComboBox-ах производится с их помощь
        public List<decimal> regionHidList;
        public List<decimal> passageHidList;
        public List<decimal> canalHidList;

        public RowMTUpdate() {
            InitializeComponent();

            //по умолчанию дата должна инициализироваться сегодняшним числом
            date = DateTime.Now;
        }
        public RowMTUpdate(DateTime date, string km_pk, decimal region, decimal passage, decimal canal, string example)
        {
            InitializeComponent();

            //инициализация полей, который сперва будут играть роль начальной инициализации полей ввода на окне
            this.date = date;
            this.km_pk = km_pk;
            this.region = region;
            this.passage = passage;
            this.canal = canal;
            this.example = example;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //подгрузка таблиц
            using (OracleConnection orclConn = new OracleConnection(Common.orclConnStr))
            {
                orclConn.Open();
                //подгружаем таблицу участка
                regionOrclDB = new OracleDataTable("select * from p_uch", orclConn);
                regionOrclDB.Active = true;
                regionOrclDB.Fill();
                //подгружаем таблицу отступа
                passageOrclDB = new OracleDataTable("select * from p_otstup where deleted = 0", orclConn);
                passageOrclDB.Active = true;
                passageOrclDB.Fill();
                //подгружаем таблицу проходов
                canalOrclDB = new OracleDataTable("select * from p_prohod", orclConn);
                canalOrclDB.Active = true;
                canalOrclDB.Fill();
                orclConn.Close();
            }

            //инициализация списков HID и ComboBox-ов
            regionHidList = new List<decimal>();
            for (int i = 0; i < regionOrclDB.Rows.Count; i++)
            {   
                regionHidList.Add(Convert.ToDecimal( regionOrclDB.Rows[i][0]));
                regionComboBox.Items.Add(regionOrclDB.Rows[i][1].ToString());
            }
            passageHidList = new List<decimal>();
            for (int i = 0; i < passageOrclDB.Rows.Count; i++)
            {
                passageHidList.Add(Convert.ToDecimal(passageOrclDB.Rows[i][0]));
                passageComboBox.Items.Add(passageOrclDB.Rows[i][1].ToString());
            }
            canalHidList = new List<decimal>();
            for (int i = 0; i < canalOrclDB.Rows.Count; i++)
            {
                canalHidList.Add(Convert.ToDecimal(canalOrclDB.Rows[i][0]));
                canalComboBox.Items.Add(canalOrclDB.Rows[i][1].ToString());
            }

            //начальная инициализация элементов ввода
            datePicker.SelectedDate = date;
            km_pkMaskedTextBox.Text = km_pk;
            regionComboBox.SelectedIndex = 
                regionHidList.IndexOf(region) > -1 ? 
                regionHidList.IndexOf(region) : 
                -1;
            passageComboBox.SelectedIndex = 
                passageHidList.IndexOf(passage) > -1 ? 
                passageHidList.IndexOf(passage) : 
                -1;
            canalComboBox.SelectedIndex = 
                canalHidList.IndexOf(canal) > -1 ? 
                canalHidList.IndexOf(canal) : 
                -1;
            exampleTextBox.Text = example;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            //регулярка, которая находит все нижние подчеркивания
            Regex underscoreRegex = new Regex("_");

            //если дата имеет значение, то инициализируем им, иначе инициализируем сегодняшним числом
            date =datePicker.SelectedDate.HasValue ? 
                new DateTime(datePicker.SelectedDate.Value.Year, 
                    datePicker.SelectedDate.Value.Month, 
                    datePicker.SelectedDate.Value.Day) : 
                DateTime.Now;
            //удаляем из км-пк все нижние подчеркивания и записываем получившуюся строку
            km_pk = underscoreRegex.Replace(km_pkMaskedTextBox.Text, "");
            //если есть выбранный участок, то находим его HID и инициализируем соответствующее свойство им
            region =
                regionComboBox.SelectedItem != null && Common.TableContainsHeader("HID", ref regionOrclDB) ? 
                Convert.ToDecimal(regionOrclDB.Rows[regionComboBox.SelectedIndex]["HID"]) :
                -1;
            //если есть выбранное отступление, то находим его HID и инициализируем соответствующее свойство им
            passage =
                passageComboBox.SelectedItem != null && Common.TableContainsHeader("HID", ref passageOrclDB) ? 
                Convert.ToDecimal(passageOrclDB.Rows[passageComboBox.SelectedIndex]["HID"]) :
                -1;
            //если есть выбранный проход, то находим его HID и инициализируем соответствующее свойство им
            canal =
                canalComboBox.SelectedItem != null && Common.TableContainsHeader("HID", ref canalOrclDB) ? 
                Convert.ToDecimal(canalOrclDB.Rows[canalComboBox.SelectedIndex]["HID"]) : 
                -1;
            //просто текст
            example = exampleTextBox.Text;

            //указываем положительный результат работы окна
            this.DialogResult = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            //указываем негативный результат работы окна
            this.DialogResult = false;
            Close();
        }
    }
}
