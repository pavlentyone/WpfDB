using System;
using Devart.Data;
using Devart.Data.Oracle;
using System.Windows;
using System.Data;
using System.Data.SqlClient;

namespace WpfDB
{
    static class Common
    {
        public static string orclConnStr = "User Id=tor;Password=tor7ujn;Server=10.3.0.38;Direct=True;Service Name=bivc.brest.rw;";
        

        public static string userName = "ivan";

        public static bool TableContainsHeader(string header, ref OracleDataTable orclDT)
        {
            foreach (DataColumn col in orclDT.Columns)
                if (col.ColumnName == header)
                    return true;
            return false;
        }

        //выполняем команды для удаления, вставки и изменения данных в таблице через строку command. команда select выполняется отдельно.
        public static int exeSqlCmd(string command){
            try
            {
                using (OracleConnection orclConn = new OracleConnection(Common.orclConnStr))
                {
                    OracleCommand orclSQLCommand = new OracleCommand(command, orclConn);
                    orclConn.Open();
                    orclSQLCommand.ExecuteNonQuery();
                    orclConn.Close();
                }
            }
            catch (Exception exp) {
                MessageBox.Show(exp.Message);
                return exp.HResult;
            }
            return 0;
        }
    }

}

