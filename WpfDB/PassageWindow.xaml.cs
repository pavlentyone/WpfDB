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
    /// Interaction logic for PassageWindow.xaml
    /// </summary>
    public partial class PassageWindow : Window
    {
        public static OracleDataTable passageOrclDT;
        public PassageWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTable();
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            //окно изменения названия поля
            RowUpdate rowUpdateWindow = new RowUpdate();
            rowUpdateWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            if (!rowUpdateWindow.ShowDialog().Value)
                return;
            string returnStr = rowUpdateWindow.InputResult;

            //создание и выполнеине sql-команды
            string sqlStr = String.Format("insert into p_otstup (name,deleted)values ('{0}',0)", returnStr);
            Common.exeSqlCmd(sqlStr);

            //обновление таблицы
            UpdateTable();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //если ни одна строка не выделена, то выходим из метода
            if (passageDataGrid.SelectedItem == null)
                return;

            //поиск и удаление будут производиться по HID
            decimal hid = -1;
            if (Common.TableContainsHeader("HID", ref passageOrclDT))
                Decimal.TryParse((passageDataGrid.SelectedItem as DataRowView)["HID"].ToString(), out hid);

            //создаем команду на удаление и выполняем ее
            string sqlStr = "UPDATE p_otstup SET deleted=1 WHERE HID = " + hid.ToString();
            Common.exeSqlCmd(sqlStr);

            //обновляем таблицу. используем метод от окна
            UpdateTable();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            //если ни одна строка не выделена, то выходим из метода
            if (passageDataGrid.SelectedItem == null)
                return;

            //окно изменения названия поля
            string name =
                Common.TableContainsHeader("NAME", ref passageOrclDT) ? 
                Convert.ToString((passageDataGrid.SelectedItem as DataRowView)["NAME"]) :
                null;
            RowUpdate rowUpdateWindow = new RowUpdate(name);
            rowUpdateWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            if (!rowUpdateWindow.ShowDialog().Value)
                return;
            string returnStr = rowUpdateWindow.InputResult;

            //находим HID выделенного элемента
            decimal hid = -1;
            if (Common.TableContainsHeader("HID", ref passageOrclDT))
                Decimal.TryParse((passageDataGrid.SelectedItem as DataRowView)["HID"].ToString(), out hid);

            string sqlStr = "UPDATE p_otstup SET name = '" + returnStr + "',deleted=0 WHERE HID = " + hid.ToString();
            Common.exeSqlCmd(sqlStr.ToString());

            UpdateTable();
        }

        private void UpdateTable() {
            using (OracleConnection orclConn = new OracleConnection(Common.orclConnStr))
            {
                orclConn.Open();
                passageOrclDT = new OracleDataTable("select * from p_otstup where deleted = 0", orclConn);
                passageOrclDT.Active = true;
                passageOrclDT.Fill();
                orclConn.Close();
            }
            passageDataGrid.ItemsSource = passageOrclDT.DefaultView;

            passageDataGrid.Columns[0].Visibility = System.Windows.Visibility.Hidden;
            passageDataGrid.Columns[2].Visibility = System.Windows.Visibility.Hidden;
            passageDataGrid.Columns[1].Header = "Название";
        }
    }
}
