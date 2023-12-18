using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtility
{
    public class ExcelHelper
    {
        /// <summary>
        /// Get connection string by file path(IMEX=1将所有读入数据看作字符，其他值（0、2）请查阅相关帮助文档；)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="flagHDR">HDR表示要把第一行作为数据还是作为列名，作为数据用HDR=no，作为列名用HDR=yes；</param>
        /// <returns></returns>
        public static string GetConnectionStringByFilePath(string filePath, string flagHDR = "no")
        {
            string connectionString = string.Empty;
            FileInfo file = new FileInfo(filePath);
            if (!file.Exists) { throw new Exception("File not exists"); }
            string extension = file.Extension;
            switch (extension)
            {
                case ".xls":
                    connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR={0};IMEX=1;'";
                    break;
                case ".xlsx":
                    connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 12.0;HDR={0};IMEX=1;'";
                    break;
                default:
                    connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR={0};IMEX=1;'";
                    break;
            }
            connectionString = string.Format(connectionString, flagHDR);
            return connectionString;
        }

        public static OleDbDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(connectionString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        
        public static DataTable ExecuteDataTable(string connectionString, CommandType cmdType, string cmdText, params OleDbParameter[] commandParameters)
        {
            DataTable dt = new DataTable();
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(connectionString);

            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OleDbDataAdapter dbAdapter = new OleDbDataAdapter();
                dbAdapter.SelectCommand = cmd;
                dbAdapter.Fill(dt);
            }
            catch(Exception e)
            {
                Debug.Write(e.Message);
                conn.Close();
                throw;
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        private static void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, CommandType cmdType, string cmdText, OleDbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (OleDbParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
    }
}
