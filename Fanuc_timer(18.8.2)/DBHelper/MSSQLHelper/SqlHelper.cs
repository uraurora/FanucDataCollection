using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DCS_DAL
{
	/// <summary>
	/// 数据访问类（非静态类）
	/// </summary>
	public class SqlHelper
	{
		/// <summary>
		/// 定义了数据库的连接字符串
		/// </summary>
		private string connectionString;

		public SqlHelper(string connect)
		{
			this.connectionString = connect;
		}

		#region 公用方法
		/// <summary>
		/// 判断是否存在某表的某个字段
		/// </summary>
		/// <param name="tableName">表名称</param>
		/// <param name="columnName">列名称</param>
		/// <returns>是否存在</returns>
		public bool ColumnExists(string tableName, string columnName)
		{
			string sql = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'";
			object res = GetSingle(sql);
			if (res == null)
			{
				return false;
			}
			return Convert.ToInt32(res) > 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="FieldName"></param>
		/// <param name="TableName"></param>
		/// <returns></returns>
		public int GetMaxID(string FieldName, string TableName)
		{
			string strsql = "select max(" + FieldName + ")+1 from " + TableName;
			object obj = GetSingle(strsql);
			if (obj == null)
			{
				return 1;
			}
			else
			{
				return int.Parse(obj.ToString());
			}
		}

		public bool Exists(string strSql)
		{
			object obj = GetSingle(strSql);
			int cmdresult;
			if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
			{
				cmdresult = 0;
			}
			else
			{
				cmdresult = int.Parse(obj.ToString()); //也可能=0
			}
			if (cmdresult == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		/// <summary>
		/// 表是否存在
		/// </summary>
		/// <param name="TableName"></param>
		/// <returns></returns>
		public bool TabExists(string TableName)
		{
			string strsql = "select count(*) from sysobjects where id = object_id(N'[" + TableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
			//string strsql = "SELECT count(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + TableName + "]') AND type in (N'U')";
			object obj = GetSingle(strsql);
			int cmdresult;
			if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
			{
				cmdresult = 0;
			}
			else
			{
				cmdresult = int.Parse(obj.ToString());
			}
			if (cmdresult == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool Exists(string strSql, params SqlParameter[] cmdParms)
		{
			object obj = GetSingle(strSql, cmdParms);
			int cmdresult;
			if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
			{
				cmdresult = 0;
			}
			else
			{
				cmdresult = int.Parse(obj.ToString());
			}
			if (cmdresult == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		#endregion

		#region  执行简单SQL语句

		/// <summary>
		/// 执行SQL语句，返回影响的记录数
		/// </summary>
		/// <param name="sqlString">SQL语句</param>
		/// <returns>影响的记录数</returns>
		public int ExecuteSql(string sqlString)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(sqlString, connection))
				{
					try
					{
						cmd.CommandTimeout = 180;
						connection.Open();
						int rows = cmd.ExecuteNonQuery();
						return rows;
					}
					catch (SqlException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
		}

		public int ExecuteSqlByTime(string sqlString, int times)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(sqlString, connection))
				{
					try
					{
						cmd.CommandTimeout = 180;
						connection.Open();
						cmd.CommandTimeout = times;
						int rows = cmd.ExecuteNonQuery();
						return rows;
					}
					catch (SqlException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
		}

		//=================================================================================================================      
		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="sqlStringList">多条SQL语句</param>		
		public int ExecuteSqlTran(List<String> sqlStringList)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.CommandTimeout = 180;
				cmd.Connection = conn;
				SqlTransaction tx = conn.BeginTransaction();
				cmd.Transaction = tx;
				try
				{
					int count = 0;
					for (int n = 0; n < sqlStringList.Count; n++)
					{
						string strsql = sqlStringList[n];
						if (strsql.Trim().Length > 1)
						{
							cmd.CommandText = strsql;
							count += cmd.ExecuteNonQuery();
						}
					}
					tx.Commit();
					return count;
				}
				catch
				{
					tx.Rollback();
					return 0;
				}
			}
		}

		//================================================================================================================= 

		/// <summary>
		/// 执行带一个存储过程参数的的SQL语句。
		/// </summary>
		/// <param name="sqlString">SQL语句</param>
		/// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
		/// <returns>影响的记录数</returns>
		public int ExecuteSql(string sqlString, string content)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(sqlString, connection);
				cmd.CommandTimeout = 180;
				SqlParameter myParameter = new SqlParameter("@content", SqlDbType.NText);
				myParameter.Value = content;
				cmd.Parameters.Add(myParameter);
				try
				{
					connection.Open();
					int rows = cmd.ExecuteNonQuery();
					return rows;
				}
				catch (SqlException e)
				{
					throw e;
				}
				finally
				{
					cmd.Dispose();
					connection.Close();
				}
			}
		}
		/// <summary>
		/// 执行带一个存储过程参数的的SQL语句。
		/// </summary>
		/// <param name="sqlString">SQL语句</param>
		/// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
		/// <returns>影响的记录数</returns>
		public object ExecuteSqlGet(string sqlString, string content)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(sqlString, connection);
				cmd.CommandTimeout = 180;
				SqlParameter myParameter = new SqlParameter("@content", SqlDbType.NText);
				myParameter.Value = content;
				cmd.Parameters.Add(myParameter);
				try
				{
					connection.Open();
					object obj = cmd.ExecuteScalar();
					if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
					{
						return null;
					}
					else
					{
						return obj;
					}
				}
				catch (SqlException e)
				{
					throw e;
				}
				finally
				{
					cmd.Dispose();
					connection.Close();
				}
			}
		}



		/// <summary>
		/// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
		/// </summary>
		/// <param name="strSql">SQL语句</param>
		/// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
		/// <returns>影响的记录数</returns>
		public int ExecuteSqlInsertImg(string strSql, byte[] fs)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(strSql, connection);
				SqlParameter myParameter = new SqlParameter("@fs", SqlDbType.Image);
				myParameter.Value = fs;
				cmd.Parameters.Add(myParameter);
				try
				{
					connection.Open();
					int rows = cmd.ExecuteNonQuery();
					return rows;
				}
				catch (System.Data.SqlClient.SqlException e)
				{
					throw e;
				}
				finally
				{
					cmd.Dispose();
					connection.Close();
				}
			}
		}

		/// <summary>
		/// 执行一条计算查询结果语句，返回查询结果（object）。
		/// </summary>
		/// <param name="sqlString">计算查询结果语句</param>
		/// <returns>查询结果（object）</returns>
		public object GetSingle(string sqlString)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(sqlString, connection))
				{
					try
					{
						cmd.CommandTimeout = 180;
						connection.Open();
						object obj = cmd.ExecuteScalar();
						if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
						{
							return null;
						}
						else
						{
							return obj;
						}
					}
					catch (SqlException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
		}
		public object GetSingle(string sqlString, int times)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(sqlString, connection))
				{
					try
					{
						connection.Open();
						cmd.CommandTimeout = times;
						object obj = cmd.ExecuteScalar();
						if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
						{
							return null;
						}
						else
						{
							return obj;
						}
					}
					catch (SqlException e)
					{
						connection.Close();
						throw e;
					}
				}
			}
		}



		/// <summary>
		/// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
		/// </summary>
		/// <param name="strSql">查询语句</param>
		/// <returns>SqlDataReader</returns>
		public SqlDataReader ExecuteReader(string strSql)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			SqlCommand cmd = new SqlCommand(strSql, connection);
			try
			{
				cmd.CommandTimeout = 180;
				connection.Open();
				SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				return myReader;
			}
			catch (SqlException e)
			{
				throw e;
			}

		}


		/// <summary>
		/// 执行查询语句，返回DataSet
		/// </summary>
		/// <param name="sqlString">查询语句</param>
		/// <returns>DataSet</returns>
		public DataSet Query(string sqlString)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				DataSet ds = new DataSet();
				try
				{
					connection.Open();
					SqlDataAdapter command = new SqlDataAdapter(sqlString, connection);
					command.Fill(ds, "ds");
				}
				catch (SqlException ex)
				{
					throw new Exception(ex.Message);
				}
				return ds;
			}
		}


		public DataSet Query(string sqlString, int times)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				DataSet ds = new DataSet();
				try
				{
					connection.Open();
					SqlDataAdapter command = new SqlDataAdapter(sqlString, connection);
					command.SelectCommand.CommandTimeout = times;
					command.Fill(ds, "ds");
				}
				catch (SqlException ex)
				{
					throw new Exception(ex.Message);
				}
				return ds;
			}
		}



		#endregion

		#region 执行带参数的SQL语句

		/// <summary>
		/// 执行SQL语句，返回影响的记录数
		/// </summary>
		/// <param name="sqlString">SQL语句</param>
		/// <param name="cmdParms">无参数传入NULL值</param>
		/// <returns>影响的记录数</returns>
		public int ExecuteSql(string sqlString, params SqlParameter[] cmdParms)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					try
					{
						cmd.CommandTimeout = 180;
						PrepareCommand(cmd, connection, null, sqlString, cmdParms);
						int rows = cmd.ExecuteNonQuery();
						cmd.Parameters.Clear();
						return rows;
					}
					catch (SqlException e)
					{
						throw e;
					}
				}
			}
		}


		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="sqlStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
		public int ExecuteSqlTran(Hashtable sqlStringList)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						int val = 0;
						//循环
						foreach (DictionaryEntry myDE in sqlStringList)
						{
							string cmdText = myDE.Key.ToString();
							SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
							PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							val += cmd.ExecuteNonQuery();
							cmd.Parameters.Clear();
						}
						trans.Commit();
						return val;
					}
					catch
					{
						trans.Rollback();
						return 0;
					}
				}
			}
		}

		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="sqlStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
		public void ExecuteSqlTranWithIndentity(Hashtable sqlStringList)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						int indentity = 0;
						//循环
						foreach (DictionaryEntry myDE in sqlStringList)
						{
							string cmdText = myDE.Key.ToString();
							SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
							foreach (SqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.InputOutput)
								{
									q.Value = indentity;
								}
							}
							PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							foreach (SqlParameter q in cmdParms)
							{
								if (q.Direction == ParameterDirection.Output)
								{
									indentity = Convert.ToInt32(q.Value);
								}
							}
							cmd.Parameters.Clear();
						}
						trans.Commit();
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}
		}

		/// <summary>
		/// 执行一条计算查询结果语句，返回查询结果（object）。
		/// </summary>
		/// <param name="sqlString">计算查询结果语句</param>
		/// <param name="cmdParms">无参数传入NULL值</param>
		/// <returns>查询结果（object）</returns>
		public object GetSingle(string sqlString, params SqlParameter[] cmdParms)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					try
					{
						PrepareCommand(cmd, connection, null, sqlString, cmdParms);
						object obj = cmd.ExecuteScalar();
						cmd.Parameters.Clear();
						if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
						{
							return null;
						}
						else
						{
							return obj;
						}
					}
					catch (System.Data.SqlClient.SqlException e)
					{
						throw e;
					}
				}
			}
		}

		/// <summary>
		/// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
		/// </summary>
		/// <param name="sqlString"></param>
		/// <param name="cmdParms">无参数传入NULL值</param>
		/// <returns>SqlDataReader</returns>
		public SqlDataReader ExecuteReader(string sqlString, params SqlParameter[] cmdParms)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			SqlCommand cmd = new SqlCommand();
			try
			{
				PrepareCommand(cmd, connection, null, sqlString, cmdParms);
				SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				cmd.Parameters.Clear();
				return myReader;
			}
			catch (SqlException e)
			{
				throw e;
			}
			//			finally
			//			{
			//				cmd.Dispose();
			//				connection.Close();
			//			}	

		}

		/// <summary>
		/// 执行查询语句，返回DataSet
		/// </summary>
		/// <param name="sqlString">查询语句</param>
		/// <param name="cmdParms"></param>
		/// <returns>DataSet</returns>
		public DataSet Query(string sqlString, params SqlParameter[] cmdParms)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand();
				PrepareCommand(cmd, connection, null, sqlString, cmdParms);
				using (SqlDataAdapter da = new SqlDataAdapter(cmd))
				{
					DataSet ds = new DataSet();
					try
					{
						da.Fill(ds, "ds");
						cmd.Parameters.Clear();
					}
					catch (SqlException ex)
					{
						throw new Exception(ex.Message);
					}
					return ds;
				}
			}
		}


		private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
		{
			if (conn.State != ConnectionState.Open)
				conn.Open();
			cmd.Connection = conn;
			cmd.CommandText = cmdText;
			cmd.CommandTimeout = 180;
			if (trans != null)
				cmd.Transaction = trans;
			cmd.CommandType = CommandType.Text;//cmdType;
			if (cmdParms != null)
			{
				foreach (SqlParameter parameter in cmdParms)
				{
					if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
						(parameter.Value == null))
					{
						parameter.Value = DBNull.Value;
					}
					cmd.Parameters.Add(parameter);
				}
			}
		}

		#endregion

		#region 存储过程操作

		/// <summary>
		/// 执行存储过程，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <returns>SqlDataReader</returns>
		public SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			SqlDataReader returnReader;
			connection.Open();
			SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
			command.CommandType = CommandType.StoredProcedure;
			returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
			return returnReader;

		}


		/// <summary>
		/// 执行存储过程
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <param name="tableName">DataSet结果中的表名</param>
		/// <returns>DataSet</returns>
		public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				DataSet dataSet = new DataSet();
				connection.Open();
				SqlDataAdapter sqlDA = new SqlDataAdapter();
				sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
				sqlDA.Fill(dataSet, tableName);
				connection.Close();
				return dataSet;
			}
		}
		public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				DataSet dataSet = new DataSet();
				connection.Open();
				SqlDataAdapter sqlDA = new SqlDataAdapter();
				sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
				sqlDA.SelectCommand.CommandTimeout = Times;
				sqlDA.Fill(dataSet, tableName);
				connection.Close();
				return dataSet;
			}
		}


		/// <summary>
		/// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
		/// </summary>
		/// <param name="connection">数据库连接</param>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <returns>SqlCommand</returns>
		private SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand command = new SqlCommand(storedProcName, connection);
			command.CommandType = CommandType.StoredProcedure;
			foreach (SqlParameter parameter in parameters)
			{
				if (parameter != null)
				{
					// 检查未分配值的输出参数,将其分配以DBNull.Value.
					if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
						(parameter.Value == null))
					{
						parameter.Value = DBNull.Value;
					}
					command.Parameters.Add(parameter);
				}
			}

			return command;
		}

		/// <summary>
		/// 执行存储过程，返回影响的行数		
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <param name="rowsAffected">影响的行数</param>
		/// <returns></returns>
		public int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				int result;
				connection.Open();
				SqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
				rowsAffected = command.ExecuteNonQuery();
				result = (int)command.Parameters["ReturnValue"].Value;
				//Connection.Close();
				return result;
			}
		}

		/// <summary>
		/// 创建 SqlCommand 对象实例(用来返回一个整数值)	
		/// </summary>
		/// <param name="storedProcName">存储过程名</param>
		/// <param name="parameters">存储过程参数</param>
		/// <returns>SqlCommand 对象实例</returns>
		private SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
			command.Parameters.Add(new SqlParameter("ReturnValue",
				SqlDbType.Int, 4, ParameterDirection.ReturnValue,
				false, 0, 0, string.Empty, DataRowVersion.Default, null));
			return command;
		}
		#endregion

	}
}
