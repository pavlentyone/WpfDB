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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //основная таблица
        public static OracleDataTable mainOrclDT;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //инициализация дат для фильтра
            startDatePicker.SelectedDate = new DateTime(DateTime.Now.Year ,DateTime.Now.Month, 1);
            endDatePicker.SelectedDate = DateTime.Now.AddDays(30);

            //добавляем обработчики после инициализации дат
            startDatePicker.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(startDatePicker_SelectedDateChanged);
            endDatePicker.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(endDatePicker_SelectedDateChanged);

            //обновляем таблицу
            UpdateTable();
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            RowMTUpdate rowMTUpdate = new RowMTUpdate();
            rowMTUpdate.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            if (!rowMTUpdate.ShowDialog().Value)
                return;

            string sqlStr = String.Format("insert into p_main (data, km_pk, uch, otstup, prohod, prim) values (TO_DATE('{0}','DD.MM.YYYY'),'{1}',{2},{3},{4},'{5}')",
                rowMTUpdate.date.Day.ToString() + "." + rowMTUpdate.date.Month + "." + rowMTUpdate.date.Year, rowMTUpdate.km_pk.ToString(), rowMTUpdate.region.ToString(), rowMTUpdate.passage.ToString(), rowMTUpdate.canal.ToString(), rowMTUpdate.example.ToString());
            Common.exeSqlCmd(sqlStr);

            //обновление таблицы
            UpdateTable();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainDataGrid.SelectedItem == null)
                return;

            //поиск и удаление будут производиться по HID
            decimal hid = -1;
            if (Common.TableContainsHeader("HID", ref mainOrclDT))
                Decimal.TryParse((mainDataGrid.SelectedItem as DataRowView)["HID"].ToString(), out hid);

            string sqlStr = "DELETE FROM p_main WHERE HID=" + hid.ToString(); 
            Common.exeSqlCmd(sqlStr);

            //обновляем таблицу. используем метод от окна
            UpdateTable();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {

            if (mainDataGrid.SelectedItem == null)
                return;

            //получаем данные из выбранной строки
            object temp;
            //если колонка с таким именем есть в таблице, 
            //то получить значение из ячейки выбранной строки по соответствующему столбцу
            temp =
                Common.TableContainsHeader("DATA", ref mainOrclDT) ? 
                (mainDataGrid.SelectedItem as DataRowView)["DATA"] : 
                System.DBNull.Value;
            //если ячейка равна Null, то записать текущую дату. иначе записать заначение из ячейки
            DateTime dateTime = 
                temp.Equals(System.DBNull.Value) ? 
                DateTime.Now : 
                Convert.ToDateTime(temp);
            temp =
                Common.TableContainsHeader("KM_PK", ref mainOrclDT) ? 
                (mainDataGrid.SelectedItem as DataRowView)["KM_PK"] :
                System.DBNull.Value;
            string km_pk = 
                temp.Equals(System.DBNull.Value) ? 
                null : 
                Convert.ToString(temp);
            temp = Common.TableContainsHeader("UCH_HID", ref mainOrclDT) ?
                (mainDataGrid.SelectedItem as DataRowView)["UCH_HID"] :
                System.DBNull.Value;
            decimal uch = 
                temp.Equals(System.DBNull.Value) ? 
                -1 : 
                Convert.ToDecimal(temp);
            temp = Common.TableContainsHeader("OTSTUP_HID", ref mainOrclDT) ? 
                (mainDataGrid.SelectedItem as DataRowView)["OTSTUP_HID"] :
                System.DBNull.Value;
            decimal otstup = 
                temp.Equals(System.DBNull.Value) ? 
                -1 : 
                Convert.ToDecimal(temp);
            temp = Common.TableContainsHeader("PROHOD_HID", ref mainOrclDT) ? 
                (mainDataGrid.SelectedItem as DataRowView)["PROHOD_HID"] :
                System.DBNull.Value;
            decimal prohod = 
                temp.Equals(System.DBNull.Value) ? 
                -1 : 
                Convert.ToDecimal(temp);
            temp = Common.TableContainsHeader("PRIM", ref mainOrclDT) ?
                (mainDataGrid.SelectedItem as DataRowView)["PRIM"] :
                System.DBNull.Value;
            string prim = 
                temp.Equals(System.DBNull.Value) ? 
                "" : 
                Convert.ToString(temp);

            RowMTUpdate rowMTUpdate = new RowMTUpdate(dateTime, km_pk, uch, otstup, prohod, prim);
            rowMTUpdate.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            if (!rowMTUpdate.ShowDialog().Value)
                return;

            //поиск и удаление будут производиться по HID
            decimal hid = -1;
            if(Common.TableContainsHeader("HID", ref mainOrclDT))
                Decimal.TryParse((mainDataGrid.SelectedItem as DataRowView)["HID"].ToString(), out hid);
            //выполняем sql команду
            string sqlStr = String.Format(
                "update p_main set data=TO_DATE('{0}','DD.MM.YYYY'), km_pk='{1}', uch={2}, otstup={3}, prohod={4}, prim = '{5}' where hid={6}",
                 rowMTUpdate.date.Day + "." + rowMTUpdate.date.Month + "." + rowMTUpdate.date.Year, rowMTUpdate.km_pk,
                rowMTUpdate.region, rowMTUpdate.passage, rowMTUpdate.canal, rowMTUpdate.example, hid);
            Common.exeSqlCmd(sqlStr);

            //обновление таблицы
            UpdateTable();
        }

        private void RegionButton_Click(object sender, RoutedEventArgs e)
        {
            RegionWindow w = new RegionWindow();
            w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            w.ShowDialog();
        }

        private void PassageTypeButton_Click(object sender, RoutedEventArgs e)
        {
            PassageWindow w = new PassageWindow();
            w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            w.ShowDialog();
        }

        private void startDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //обновление таблицы
            UpdateTable();
        }

        private void endDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //обновление таблицы
            UpdateTable();
        }

        private void UpdateTable() {
            string sqlStr = String.Format(
            @"select a.hid,a.data, a.km_pk,b.HID AS UCH_HID, b.NAME AS UCH,b2.HID AS OTSTUP_HID, b2.NAME AS OTSTUP,b3.HID AS PROHOD_HID, b3.name AS PROHOD,a.PRIM
            from p_main a
            left join p_uch b on a.uch=b.hid
            left join p_otstup b2 on a.otstup=b2.hid
            left join p_prohod b3 on a.prohod=b3.hid
            where a.data >= TO_DATE('{0}','DD.MM.YYYY') and a.data <= TO_DATE('{1}','DD.MM.YYYY')",
            startDatePicker.SelectedDate.Value.Day.ToString() + "." +
            startDatePicker.SelectedDate.Value.Month.ToString() + "." +
            startDatePicker.SelectedDate.Value.Year.ToString(),
            endDatePicker.SelectedDate.Value.Day.ToString() + "." +
            endDatePicker.SelectedDate.Value.Month.ToString() + "." +
            endDatePicker.SelectedDate.Value.Year.ToString());

            using (OracleConnection orclConn = new OracleConnection(Common.orclConnStr))
            {
                orclConn.Open();
                mainOrclDT = new OracleDataTable(sqlStr, orclConn);
                mainOrclDT.Active = true;
                mainOrclDT.Fill();
                orclConn.Close();
            }
            mainDataGrid.ItemsSource = mainOrclDT.DefaultView;

            mainDataGrid.Columns[1].ClipboardContentBinding.StringFormat = "dd.MM.yyyy";

            mainDataGrid.Columns[0].Visibility = System.Windows.Visibility.Hidden;
            mainDataGrid.Columns[3].Visibility = System.Windows.Visibility.Hidden;
            mainDataGrid.Columns[5].Visibility = System.Windows.Visibility.Hidden;
            mainDataGrid.Columns[7].Visibility = System.Windows.Visibility.Hidden;

            mainDataGrid.Columns[1].Header = "Дата";
            mainDataGrid.Columns[2].Header = "Км-пк";
            mainDataGrid.Columns[4].Header = "Участок";
            mainDataGrid.Columns[6].Header = "Отступ";
            mainDataGrid.Columns[8].Header = "Проход";
            mainDataGrid.Columns[9].Header = "Примечание";

        }

        private void km_pkButton_Click(object sender, RoutedEventArgs e)
        {
//            string searchStr = (new Regex("_")).Replace(km_pkSearchMaskedTextBox.Text, "");
//            string sqlStr = String.Format(
//            @"select a.hid,a.data, a.km_pk,b.HID AS UCH_HID, b.NAME AS UCH,b2.HID AS OTSTUP_HID, b2.NAME AS OTSTUP,b3.HID AS PROHOD_HID, b3.name AS PROHOD,a.PRIM
//            from p_main a
//            left join p_uch b on a.uch=b.hid
//            left join p_otstup b2 on a.otstup=b2.hid
//            left join p_prohod b3 on a.prohod=b3.hid
//            where a.data >= TO_DATE('{0}','DD.MM.YYYY') and a.data <= TO_DATE('{1}','DD.MM.YYYY') and KM_PK='{2}'", 
//            startDatePicker.SelectedDate.Value.Day + "." + startDatePicker.SelectedDate.Value.Month + "." + startDatePicker.SelectedDate.Value.Year,
//            endDatePicker.SelectedDate.Value.Day + "." + endDatePicker.SelectedDate.Value.Month + "." + endDatePicker.SelectedDate.Value.Year,
//            searchStr);

//            using (OracleConnection orclConn = new OracleConnection(orclCSB))
//            {
//                orclConn.Open();
//                mainOrclDT = new OracleDataTable(sqlStr, orclConn);
//                mainOrclDT.Active = true;
//                mainOrclDT.Fill();
//                orclConn.Close();
//            }
//            //mainOrclDT.DefaultView.RowFilter
//            mainDataGrid.ItemsSource = mainOrclDT.DefaultView;//.RowFilter = "%" + searchStr + "%";
            Regex underscoreRegex = new Regex("_");
            mainOrclDT.DefaultView.RowFilter = string.Format("KM_PK LIKE '%{0}%'", underscoreRegex.Replace(km_pkSearchMaskedTextBox.Text, ""));
        }

        private void kmButton_Click(object sender, RoutedEventArgs e)
        {

            //OracleDataTable tempTable = mainOrclDT;
            //for (int i = 0; i < tempTable.Rows.Count; i++)
            //{
            //    string tempStr;
            //    tempStr = Convert.ToString(tempTable.Rows[i]["KM_PK"]);
            //    //избавляемся от всего после тире и от тире тоже
            //    tempStr = tempStr.IndexOf('-') > -1 ? tempStr.Substring(0, tempStr.Length - tempStr.IndexOf('-')) : tempStr;
            //    //избавляемся от нижних подчеркиваний
            //    tempStr = (new Regex("_")).Replace(tempStr, "");

            //    if (tempStr != (new Regex("_")).Replace(kmSearchMaskedTextBox.Text, ""))
            //        tempTable.Rows.RemoveAt(i);
            //}
            //mainDataGrid.ItemsSource = tempTable.DefaultView;
            Regex underscoreRegex = new Regex("_");
            mainOrclDT.DefaultView.RowFilter = string.Format("KM_PK LIKE '%{0}-%'", underscoreRegex.Replace(kmSearchMaskedTextBox.Text, ""));
            //mainDataGrid.ItemsSource = mainOrclDT.DefaultView;
        }
    }
}
