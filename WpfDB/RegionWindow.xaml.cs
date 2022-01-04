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
    /// Interaction logic for RerionWindow.xaml
    /// </summary>
    public partial class RegionWindow : Window
    {
        public static OracleDataTable regionOrclDT;
        public RegionWindow()
        {
            InitializeComponent();
        }

        //выполенение команды select и заполнение таблицы
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTable();
        }

        //выполнение вставки в таблицу
        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            RowUpdate rowUpdateWindow = new RowUpdate();
            if (!rowUpdateWindow.ShowDialog().Value)
                return;
            string returnStr = rowUpdateWindow.InputResult;

            //создание и выполнеине sql-команды
            string sqlStr = "insert into p_uch (name,data,login)values ('" + returnStr + "',sysdate,'" + Common.userName + "')";
            Common.exeSqlCmd(sqlStr);
            
            //обновление таблицы
            UpdateTable();
        }

        //удаление элемента
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //если ни одна строка не выделена, то выходим из метода
            if (regionDataGrid.SelectedItem == null)
                return;

            //поиск и удаление будут производиться по HID
            decimal hid = -1;
            if (Common.TableContainsHeader("HID", ref regionOrclDT))
                Decimal.TryParse((regionDataGrid.SelectedItem as DataRowView)["HID"].ToString(), out hid);
            //создаем команду на удаление и выполняем ее
            string sqlStr = String.Format("DELETE FROM p_uch WHERE HID={0}", hid);
            Common.exeSqlCmd(sqlStr);

            //обновляем таблицу. используем метод от окна
            UpdateTable();
        }

        //изменение значения элемента
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            //если ни одна строка не выделена, то выходим из метода
            if (regionDataGrid.SelectedItem == null)
                return;

            //получить значение из выбранной строки в столбце с соответствующим заголовком
            string name =
                Common.TableContainsHeader("NAME", ref regionOrclDT) ? 
                Convert.ToString((regionDataGrid.SelectedItem as DataRowView)["NAME"]) :
                null;
            
            //окно изменения названия поля
            RowUpdate rowUpdateWindow = new RowUpdate(name);
            if (!rowUpdateWindow.ShowDialog().Value)
                return;

            string returnStr = rowUpdateWindow.InputResult;

            decimal hid = -1;
            if (Common.TableContainsHeader("HID", ref regionOrclDT))
                Decimal.TryParse((regionDataGrid.SelectedItem as DataRowView)["HID"].ToString(), out hid);
            
            //инициализируем и выполняем sql-команду для обновления
            string updateStr = String.Format(
                "UPDATE p_uch SET name = '{0}',data = sysdate,login = '{1}' WHERE HID = {2}",
                returnStr, Common.userName, hid); 
            Common.exeSqlCmd(updateStr.ToString());

            UpdateTable();
        }

        //обновление таблицы через select
        private void UpdateTable() {
            using (OracleConnection orclConn = new OracleConnection(Common.orclConnStr))
            {
                orclConn.Open();
                regionOrclDT = new OracleDataTable("select * from p_uch", orclConn);
                regionOrclDT.Active = true;
                regionOrclDT.Fill();
                orclConn.Close();
            }
            regionDataGrid.ItemsSource = regionOrclDT.DefaultView;

            //прячем лишние столбцы
            regionDataGrid.Columns[0].Visibility = System.Windows.Visibility.Hidden;
            regionDataGrid.Columns[2].Visibility = System.Windows.Visibility.Hidden;
            regionDataGrid.Columns[3].Visibility = System.Windows.Visibility.Hidden;
            //меняем заголовки оставшихся столбцов
            regionDataGrid.Columns[1].Header = "Название";
        }
    }
}
