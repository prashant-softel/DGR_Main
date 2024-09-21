using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data;
//using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using DGRAPIs.Helper;

namespace DGRAPIs.Helper
{
    public class MYSQLDBHelper
    {
        public MYSQLDBHelper(string constr)
        {

            ConnStr = constr;
        }

        private string ConnStr { get; set; }

        internal SqlConnection TheConnection => new SqlConnection(ConnStr);


        internal async Task<List<T>> GetData<T>(string qry) where T : class, new()
        {
            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getQryCommand(qry, conn))
                    {
                        await conn.OpenAsync();

                        //SqlCommand  cmd1 = new SqlCommand ("set net_write_timeout=99999; set net_read_timeout=99999", conn); // Setting tiimeout on mysqlServer
                        //cmd1.ExecuteNonQuery();
                        cmd.CommandTimeout = 99999;
                        cmd.CommandType = CommandType.Text;



                        using (DbDataReader dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable();
                            ds.Tables.Add(dt);
                            dt.DataSet.EnforceConstraints = false;
                            dt.Load(dataReader);
                            cmd.Parameters.Clear();
                            return dt.MapTo<T>();
                        }
                    }
                }
            }
            catch (Exception sqlex)
            {
                throw sqlex;
                throw new Exception("GetData =" + Environment.NewLine, sqlex);
            }
            finally
            {


            }
        }

        //Use this method to get DataTable
        internal async Task<dynamic> FetchData(string qry)
        {
            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getQryCommand(qry, conn))
                    {
                        await conn.OpenAsync();

                        //SqlCommand  cmd1 = new SqlCommand ("set net_write_timeout=99999; set net_read_timeout=99999", conn); // Setting tiimeout on mysqlServer
                        //cmd1.ExecuteNonQuery();
                        cmd.CommandTimeout = 99999;
                        cmd.CommandType = CommandType.Text;



                        using (DbDataReader dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable();
                            ds.Tables.Add(dt);
                            dt.DataSet.EnforceConstraints = false;
                            dt.Load(dataReader);
                            cmd.Parameters.Clear();
                            return dt;
                        }
                    }
                }
            }
            catch (Exception sqlex)
            {
                throw sqlex;
                throw new Exception("GetData =" + Environment.NewLine, sqlex);
            }
            finally
            {


            }
        }
        internal async Task<int> CheckGetData(string qry)
        {
            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getQryCommand(qry, conn))
                    {
                        await conn.OpenAsync();

                        //SqlCommand  cmd1 = new SqlCommand ("set net_write_timeout=99999; set net_read_timeout=99999", conn); // Setting tiimeout on mysqlServer
                        //cmd1.ExecuteNonQuery();
                        cmd.CommandTimeout = 99999;
                        cmd.CommandType = CommandType.Text;



                        using (DbDataReader dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable();
                            ds.Tables.Add(dt);
                            dt.DataSet.EnforceConstraints = false;
                            dt.Load(dataReader);
                            cmd.Parameters.Clear();
                            return dt.Rows.Count;
                        }
                    }
                }
            }
            catch (Exception sqlex)
            {
                throw sqlex;
                throw new Exception("GetData =" + Environment.NewLine, sqlex);
            }
            finally
            {


            }
        }

        internal async Task<int> ExecuteNonQuery(string SpName, SqlParameter[] sqlpara = null, bool returnvalue = false)
        {
            // MySqlParameter retpara = null;

            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getCommand(SpName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await conn.OpenAsync();
                        if (sqlpara != null)
                        {
                            foreach (SqlParameter para in sqlpara)
                            {
                                if (para.Value == null)
                                {
                                    para.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(para);
                            }
                        }

                        int i = await cmd.ExecuteNonQueryAsync();

                        return i;

                    }
                }
            }
            catch (Exception sqlex)
            {
                throw new Exception("ExecuteNonQuery SPname=" + SpName + Environment.NewLine + sqlex.Message, sqlex);
            }
            finally
            {

                // retpara = null;

            }
        }
        internal async Task<int> ExecuteNonQry<T>(string qry)
        {
            // MySqlParameter retpara = null;

            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getQryCommand(qry, conn))
                    {
                        cmd.CommandTimeout = 99999;
                        cmd.CommandType = CommandType.Text;
                        await conn.OpenAsync();


                        int i = await cmd.ExecuteNonQueryAsync();

                        return i;

                    }
                }
            }
            catch (Exception sqlex)
            {
                throw new Exception("ExecuteNonQuery SPname=" + qry + Environment.NewLine + sqlex.Message, sqlex);
            }
            finally
            {

                // retpara = null;

            }
        }
        internal async Task<List<T>> ExecuteToDataTable<T>(string SpName, SqlParameter[] sqlpara = null) where T : class, new()
        {
            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getCommand(SpName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await conn.OpenAsync();

                        if (sqlpara != null)
                        {
                            foreach (SqlParameter para in sqlpara)
                            {
                                if (para.Value == null)
                                {
                                    para.Value = DBNull.Value;
                                }

                                cmd.Parameters.Add(para);
                            }
                        }

                        using (DbDataReader dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            DataSet ds = new DataSet();
                            DataTable dt = new DataTable();
                            ds.Tables.Add(dt);
                            dt.DataSet.EnforceConstraints = false;
                            dt.Load(dataReader);
                            cmd.Parameters.Clear();
                            return dt.MapTo<T>();
                        }
                    }
                }
            }
            catch (Exception sqlex)
            {

                throw new Exception("ExecuteToDataTable SpName=" + SpName + Environment.NewLine, sqlex);
            }
            finally
            {


            }
        }
        internal async Task<int> ErrorLog(string qry)
        {
            // MySqlParameter retpara = null;

            try
            {
                using (SqlConnection conn = TheConnection)
                {
                    using (SqlCommand cmd = getQryCommand(qry, conn))
                    {
                        cmd.CommandTimeout = 99999;
                        cmd.CommandType = CommandType.Text;
                        await conn.OpenAsync();


                        int i = await cmd.ExecuteNonQueryAsync();

                        return i;

                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw new Exception("ExecuteNonQuery SPname=" + qry + Environment.NewLine + sqlex.Message, sqlex);
            }
            finally
            {

                // retpara = null;

            }
        }
        internal SqlCommand getCommand(string command, SqlConnection conn)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandTimeout = conn.ConnectionTimeout;
            cmd.CommandText = command;
            return cmd;
        }
        internal SqlCommand getQryCommand(string qry, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand(qry);   //check if this line is required? see next line
            cmd = conn.CreateCommand();
            cmd.CommandTimeout = conn.ConnectionTimeout;
            cmd.CommandText = qry;
            return cmd;
        }
        internal void Dispose()
        {
            if (TheConnection != null)
            {
                TheConnection.Dispose();
            }
        }
        private int GetConnectionTimeOut()
        {
            return 600;
        }
    }
}
