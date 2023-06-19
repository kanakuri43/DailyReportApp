using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Windows.Data;
using System.Windows;
using Microsoft.Data.SqlClient;

public class Database
{
    private SqlConnection _connection { set; get; }

    public string SQL { set; get; }
    public string ConnectionString { set; get; }


    public Database()
    {
        string currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        XElement xml = XElement.Load(currentDirectory + "config.xml");
        ConnectionString = xml.Element("ConnectString").Value;

        OpenConnection();
    }

    protected Boolean OpenConnection()
    {

        try
        {
            _connection = new SqlConnection(ConnectionString);
            _connection.Open();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "通知");
            return false;
        }
    }

    public string VersionInfo()
    {
        SqlCommand command = new SqlCommand();
        {
            // バージョン情報取得SQLを実行します。
            command.Connection = _connection;
            command.CommandText = "select version()";
            var value = command.ExecuteScalar();
            var versionNo = value.ToString();
            return ($"{versionNo}");
        }

    }

    public DataTable ReadAsDataTable()
    {
        using (SqlCommand command = new SqlCommand(SQL, _connection))
        {
            DataTable dt;
            var addapter = new SqlDataAdapter(command);
            dt = new DataTable();
            addapter.Fill(dt);
            return dt;
        }
    }
    public SqlDataReader ReadAsDataReader()
    {
        using (SqlCommand command = new SqlCommand(SQL, _connection))
        {
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                return reader;
            }
            else
            {
                return null;
            }

        }
    }

    public bool ExecuteProcedure()
    {
        using (SqlCommand command = new SqlCommand(SQL, _connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = command.ExecuteReader();
            return (reader != null);
        }
    }



}
