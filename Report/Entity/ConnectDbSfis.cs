using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class ConnectDbSfis
    {
        public ConnectDbSfis()
        {

        }
        public OracleConnection GetConnection()
        {

            string DBName = "User Id=TE;Password=B05te;data source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=10.224.81.61)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=vnsfc)))";
            OracleConnection myConn = new OracleConnection(DBName);
            return myConn;
        }
        public DataTable reDt(string StrSQL)
        {
            OracleConnection con = GetConnection();
            con.Open();

            OracleDataAdapter da = new OracleDataAdapter(StrSQL, con);
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception)
            {

               
            }
           
            con.Close();
            return (ds.Tables[0]);

        }
    }
}