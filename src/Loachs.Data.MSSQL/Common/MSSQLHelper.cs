﻿ 
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using Loachs.Common; 

namespace Loachs.Data.MSSQL
{
    /// <summary> 
    /// 数据基类
    /// 打开方式需要改进
    /// </summary>
    public class MSSQLHelper
    {
        public static string ConnectionString = ConfigHelper.DbConnection;

       /// <summary>
       /// 查询次数统计
       /// </summary>
       private static int _querycount = 0;
    
        
        /// <summary>
       /// 查询次数统计
       /// </summary>
       public static int QueryCount
       {
           get { return _querycount; }
           set { _querycount = value; }
       }

   

        #region MakeCommand
        ///// <summary>
        ///// 创建Command命令
        ///// </summary>
        ///// <param name="conn">数据连接</param>
        ///// <param name="cmdType">命令类型</param>
        ///// <param name="cmdText">SQL语句</param>
        ///// <returns>SqlCommand</returns>
        //private static SqlCommand MakeCommand(SqlConnection conn, CommandType cmdType, string cmdText)
        //{
        //    if (conn.State != ConnectionState.Open)
        //    {
        //        conn.Open();
        //    }
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = conn;
        //    cmd.CommandText = cmdText;
        //    cmd.CommandType = cmdType;
        //    return cmd;
        //}

        /// <summary>
        /// 创建Command命令
        /// </summary>
        /// <param name="conn">数据连接</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="prams">参数级</param>
        /// <returns></returns>
        private static SqlCommand MakeCommand(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] prams)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            if (prams != null)
            {
                foreach (SqlParameter p in prams)
                {
                    cmd.Parameters.Add(p);
                }
            }
            return cmd;
        }
        #endregion

        #region MakeParam
        /// <summary>
        /// 生成输入参数
        /// </summary>
        /// <param name="ParamName">Name of param.</param>
        /// <param name="DbType">Param type.</param>
        /// <param name="Size">Param size.</param>
        /// <param name="Value">Param value.</param>
        /// <returns>New parameter.</returns>
        public static SqlParameter MakeInParam(string ParamName, SqlDbType DbType, int Size, object Value)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }

        /// <summary>
        /// 生成输出参数
        /// </summary>
        /// <param name="ParamName">Name of param.</param>
        /// <param name="DbType">Param type.</param>
        /// <param name="Size">Param size.</param>
        /// <returns>New parameter.</returns>
        public static SqlParameter MakeOutParam(string ParamName, SqlDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }

        /// <summary>
        /// 生成返回参数,我添加
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="DbType"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static SqlParameter MakeReturnParam(string ParamName, SqlDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.ReturnValue, null);
        }

        /// <summary>
        /// 生成各种参数
        /// </summary>
        /// <param name="ParamName">Name of param.</param>
        /// <param name="DbType">Param type.</param>
        /// <param name="Size">Param size.</param>
        /// <param name="Direction">Parm direction.</param>
        /// <param name="Value">Param value.</param>
        /// <returns>New parameter.</returns>
        private static SqlParameter MakeParam(string ParamName, SqlDbType DbType, Int32 Size, ParameterDirection Direction, object Value)
        {
            SqlParameter param;

            if (Size > 0)
                param = new SqlParameter(ParamName, DbType, Size);
            else
                param = new SqlParameter(ParamName, DbType);

            param.Direction = Direction;
            if (Direction == ParameterDirection.Input && Value != null)
            {
                param.Value = Value;
            }
            return param;
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 返回查询结果的第一行的第一列,忽略其他列或行
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>object</returns>
        public static object ExecuteScalar(string cmdText)
        {
            return ExecuteScalar(CommandType.Text, cmdText);
        }

        /// <summary>
        /// 返回查询结果的第一行的第一列,忽略其他列或行
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>object</returns>
        public static object ExecuteScalar(CommandType cmdType, string cmdText)
        {
 
            return ExecuteScalar(cmdType, cmdText, null);
        }

        /// <summary>
        /// 返回查询结果的第一行的第一列,忽略其他列或行
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="prams">参数组</param>
        /// <returns>object</returns>
        public static object ExecuteScalar(CommandType cmdType, string cmdText, SqlParameter[] prams)
        {
            _querycount++;
            System.Web.HttpContext.Current.Application["total"] = Convert.ToInt32(System.Web.HttpContext.Current.Application["total"]) + 1;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = MakeCommand(conn, cmdType, cmdText, prams);
                object obj = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return obj;
            }
        }

        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// 返回受影响的行数,常用于Update和Delete 语句
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>int</returns>
        public static int ExecuteNonQuery(string cmdText)
        {
            return ExecuteNonQuery(CommandType.Text, cmdText);
        }

        /// <summary>
        /// 返回受影响的行数,常用于Update和Delete 语句
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>int</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText)
        {
            return ExecuteNonQuery(cmdType, cmdText, null);
        }

        /// <summary>
        /// 返回受影响的行数,常用于Insert,Update和Delete 语句
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="prams">参数组</param>
        /// <returns>int</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, SqlParameter[] prams)
        {
            _querycount++;
            System.Web.HttpContext.Current.Application["total"] = Convert.ToInt32(System.Web.HttpContext.Current.Application["total"]) + 1;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = MakeCommand(conn, cmdType, cmdText, prams);
                int i = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return i;
            }
        }

        #endregion

        #region ExecuteReader
        /// <summary>
        /// 返回 SqlDataReader
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string cmdText)
        {
            return ExecuteReader(CommandType.Text, cmdText);
        }

        [Obsolete]
        /// <summary>
        /// 返回 SqlDataReader
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader  ExecuteReader(CommandType cmdType, string cmdText)
        {
            return ExecuteReader(cmdType, cmdText, null);
        }

        /// <summary>
        /// 返回 SqlDataReader
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="prams"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string cmdText, SqlParameter[] prams)
        {
            return ExecuteReader(CommandType.Text, cmdText, prams);
        }

        /// <summary>
        /// 返回 SqlDataReader
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="prams">参数组</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(CommandType cmdType, string cmdText, SqlParameter[] prams)
        {
          
            _querycount++;

            System.Web.HttpContext.Current.Application["total"] = Convert.ToInt32(System.Web.HttpContext.Current.Application["total"]) + 1;

            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = MakeCommand(conn, cmdType, cmdText, prams);
            SqlDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return read;
        }

        #endregion

        #region ExecuteDataSet
        /// <summary>
        /// 返回 DataSet
        /// </summary>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>DataSet</returns>
        public static DataSet ExecuteDataSet(string cmdText)
        {
            return ExecuteDataSet(CommandType.Text, cmdText, null);
        }
        /// <summary>
        /// 返回 DataSet
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <returns>DataSet</returns>
        public static DataSet ExecuteDataSet(CommandType cmdType, string cmdText)
        {
            //using (SqlConnection conn = new SqlConnection(ConnectionString))
            //{
            //    SqlCommand cmd = MakeCommand(conn, cmdType, cmdText);
            //    SqlDataAdapter apt = new SqlDataAdapter(cmd);
            //    DataSet ds = new DataSet();
            //    apt.Fill(ds);
            //    cmd.Parameters.Clear();
            //    return ds;
            //}
            return ExecuteDataSet(cmdType, cmdText, null);
        }

        /// <summary>
        /// 返回 DataSet
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">SQL语句或存储过程名</param>
        /// <param name="prams">参数组</param>
        /// <returns>DataSet</returns>
        public static DataSet ExecuteDataSet(CommandType cmdType, string cmdText, SqlParameter[] prams)
        {
            _querycount++;
            System.Web.HttpContext.Current.Application["total"] = Convert.ToInt32(System.Web.HttpContext.Current.Application["total"]) + 1;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = MakeCommand(conn, cmdType, cmdText, prams);
                SqlDataAdapter apt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                apt.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }

        #endregion

        /// <summary>
        /// 获取分页Sql
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="colName"></param>
        /// <param name="colList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="orderBy"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string GetPageSql(string tableName, string colName, string colList, int pageSize, int pageIndex, int orderBy, string condition)
        {
            string temp = string.Empty;
            string sql = string.Empty;
            if (string.IsNullOrEmpty(condition))
            {
                condition = " 1=1 ";
            }

            //降序
            if (orderBy == 1)
            {
                temp = "select top {0} {1} from {2} where {5} and {3} <(select min(pk) from ( select top {4} {3} as pk from {2} where {5} order by {3} desc) t) order by {3} desc";
                sql = string.Format(temp, pageSize, colList, tableName, colName, pageSize * (pageIndex - 1), condition);
            }
            //降序
            if (orderBy == 0)
            {
                temp = "select top {0} {1} from {2} where {5} and {3} >(select max(pk) from ( select top {4} {3} as pk from {2} where {5} order by {3} asc) t) order by {3} asc";
                sql = string.Format(temp, pageSize, colList, tableName, colName, pageSize * (pageIndex - 1), condition);
            }
            //第一页
            if (pageIndex == 1)
            {
                temp = "select top {0} {1} from {2} where {3} order by {4} {5}";
                sql = string.Format(temp, pageSize, colList, tableName, condition, colName, orderBy == 1 ? "desc" : "asc");
            }

            return sql;
        }
    }
}