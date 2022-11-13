using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;


namespace IG.SqlServerDBHelper
{

	public partial class SqlEngine : IDisposable
	{
		#region Constructors


		private SqlEngine()
		{
			dbConnection = new SqlConnection();
		}

		/// <summary>
		/// distruttore
		/// </summary>
		~SqlEngine()
		{
			Dispose(false);
		}


		/// <summary>
		/// costruttore
		/// </summary>
		/// <param name="connectionString"></param>
		public SqlEngine(String connectionString)
			: this()
		{
			dbConnection.ConnectionString = connectionString;
		}

		#endregion

		#region Methods

		/// <summary>
		/// apre la connessione al db
		/// </summary>
		public void Open()
		{
			try
			{
				if (dbConnection.State == ConnectionState.Closed)
					dbConnection.Open();
			}
			catch
			{
			}
		}

		/// <summary>
		/// chiude la connessione al db
		/// </summary>
		public void Close()
		{
			try
			{
				dbConnection.Close();
			}
			catch
			{
			}
		}

		/// <summary>
		/// apre una transazione sulla connessione dichiarata in precedenza
		/// </summary>
		/// <returns>la transazione che e' stata aperta</returns>
		public SqlTransaction BeginTransaction()
		{
			if (dbConnection.State == ConnectionState.Closed)
			{
				dbConnection.Open();
			}

			return dbConnection.BeginTransaction(IsolationLevel.ReadCommitted);
		}

		/// <summary>
		/// chiude la transazione passata facendo eventualmente un commit 
		/// </summary>
		/// <param name="transaction">transazione da chiudere</param>
		/// <param name="doCommit">true = commit, false = rollback</param>
		public void EndTransaction(SqlTransaction transaction, Boolean doCommit)
		{
			if (transaction != null)
			{
				if (doCommit)
				{
					transaction.Commit();
				}
				else
				{
					transaction.Rollback();
				}

				transaction.Dispose();
			}

			dbConnection.Close();
		}

		/// <summary>
		/// non query che esegue in transazione l'array di comandi passati
		/// </summary>
		/// <param name="commands"></param>
		/// <param name="commandsParameters"></param>
		public void NonQueryInTransaction(String[] commands, List<SqlParameter[]> commandsParameters)
		{
			NonQueryInTransaction(commands, commandsParameters, IsolationLevel.ReadCommitted);
		}

		/// <summary>
		/// non query che esegue in transazione con l'isolation level dichiarato l'array di comandi passati
		/// </summary>
		/// <param name="commands"></param>
		/// <param name="commandsParameters"></param>
		/// <param name="isolationLevel"></param>
		public void NonQueryInTransaction(String[] commands, List<SqlParameter[]> commandsParameters, IsolationLevel isolationLevel)
		{
			SqlTransaction dbTransaction = null;

			//11122008 mi serve per sapere _esattamente_ che comando e' saltato
			Int32 iCommand = 0;

			try
			{
				if (dbConnection.State == ConnectionState.Closed)
				{
					dbConnection.Open();
				}

				dbTransaction = dbConnection.BeginTransaction(isolationLevel);

				for (iCommand = 0; iCommand < commands.Length; iCommand++)
				{
					using (SqlCommand dbCommand = new SqlCommand(commands[iCommand], dbConnection))
					{
						SqlParameter[] commandParameters = commandsParameters[iCommand];

						foreach (SqlParameter commandParameter in commandParameters)
						{
							SqlParameter parameter = new SqlParameter(commandParameter.ParameterName, commandParameter.Value);

							parameter.Direction = commandParameter.Direction;
							parameter.SqlDbType = commandParameter.SqlDbType;

							dbCommand.Parameters.Add(parameter);

						}

						dbCommand.Transaction = dbTransaction;
						dbCommand.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException sqlExc)
			{
				//troviamo il comando che e' esploso: commands[icommand]


				StringBuilder sCommandsParList = new StringBuilder();

				foreach (SqlParameter oraPar in commandsParameters[iCommand])
				{
					sCommandsParList.AppendFormat("{0} = {1}\n", oraPar.ParameterName, oraPar.Value);

				}

				dbTransaction.Rollback();
				dbTransaction.Dispose();
				dbConnection.Close();

				throw new Exception(String.Format("CommandText:\n{0}\nParams:\n{1}\n",
																commands[iCommand],
																sCommandsParList.ToString()),
																sqlExc);

			}
			catch
			{
				dbTransaction.Rollback();
				dbTransaction.Dispose();
				dbConnection.Close();

				throw;
			}

			dbTransaction.Commit();

			dbTransaction.Dispose();
			dbConnection.Close();
		}

		/// <summary>
		/// non query che esegue in transazione l'array di comandi utilizzando la lista di array parameters passati
		/// </summary>
		/// <param name="commands"></param>
		/// <param name="commandsParameters"></param>
		public void NonQueryInTransaction(String[] commands, params SqlParameter[] commandsParameters)
		{

			SqlTransaction dbTransaction = null;
			SqlCommand dbCommand = null;

			try
			{
				if (dbConnection.State == ConnectionState.Closed)
				{
					dbConnection.Open();
				}

				dbTransaction = dbConnection.BeginTransaction(IsolationLevel.ReadCommitted);

				foreach (String command in commands)
				{


					using (dbCommand = new SqlCommand(command, dbConnection))
					{

						foreach (SqlParameter commandParameter in commandsParameters)
						{
							SqlParameter parameter = new SqlParameter(commandParameter.ParameterName, commandParameter.Value);

							parameter.Direction = commandParameter.Direction;
							parameter.SqlDbType = commandParameter.SqlDbType;

							dbCommand.Parameters.Add(parameter);
						}

						dbCommand.Transaction = dbTransaction;
						dbCommand.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException sqlExc)
			{
				String sCommands = (dbCommand != null ? dbCommand.CommandText : String.Empty);

				dbTransaction.Rollback();
				dbTransaction.Dispose();
				dbConnection.Close();

				//foreach (String OraComText in commands)
				//{
				//    sCommands += String.Format("{0}\n", OraComText);
				//}
				//queryException = new UNException(sqlExc);
				throw new Exception(String.Format("CommandText:\n{0}\nParams:\n{1}\n",
																sCommands, commandsParameters.ToString()),
																sqlExc);
			}
			catch
			{
				dbTransaction.Rollback();
				dbTransaction.Dispose();
				dbConnection.Close();

				throw;
			}

			dbTransaction.Commit();
			dbTransaction.Dispose();
			dbConnection.Close();

		}

		/// <summary>
		/// non query che ritorna le righe interessate
		/// </summary>
		/// <param name="commandText"></param>
		/// <param name="commandParameters"></param>
		/// <returns></returns>
		public Int32 NonQueryRowsAffected(String commandText, params SqlParameter[] commandParameters)
		{
			Int32 rowsAffected = 0;

			NonQuery(out rowsAffected, commandText, commandParameters);

			return rowsAffected;
		}

		/// <summary>
		/// non query che ritorna le righe interessate all'interno della transazione in uso
		/// </summary>
		/// <param name="commandText"></param>
		/// <param name="sqlTrans">Transazione aperta esternamente</param>
		/// <param name="commandParameters"></param>
		/// <returns></returns>
		public Int32 NonQueryRowsAffectedInTransaction(String commandText, SqlTransaction sqlTrans, params SqlParameter[] commandParameters)
		{
			Int32 rowsAffected = 0;

			NonQueryInTransaction(out rowsAffected, commandText, sqlTrans, commandParameters);

			return rowsAffected;
		}

		/// <summary>
		/// non query 
		/// </summary>
		/// <param name="commandText"></param>
		/// <param name="commandParameters"></param>
		/// <returns></returns>
		public String NonQuery(String commandText, params SqlParameter[] commandParameters)
		{
			Int32 rowsAffected = 0;

			return NonQuery(out rowsAffected, commandText, commandParameters);
		}

		//27082008
		public void ExecuteCommand(SqlCommand command, params SqlParameter[] commandParameters)
		{

			int rowsAffected;


			rowsAffected = 0;

			try
			{
				if (dbConnection.State == ConnectionState.Closed)
				{
					dbConnection.Open();
				}

				command.Connection = dbConnection;

				foreach (SqlParameter commandParameter in commandParameters)
				{
					command.Parameters.Add(commandParameter);
				}

				rowsAffected = command.ExecuteNonQuery();

			}
			catch (SqlException sqlExc)
			{
				String sParams = String.Empty;
				foreach (SqlParameter OraPar in commandParameters)
				{
					sParams += String.Format("{0}={1}\n", OraPar.ParameterName, OraPar.Value);
				}
				throw new Exception(String.Format("CommandText:\n{0}\nParams:\n{1}\n",
																command.CommandText, sParams),
																sqlExc);
			}
			catch
			{
				throw;
			}
			finally
			{
				dbConnection.Close();
			}

			return;
		}

		//public String NonQuery(String commandText, params SqlParameter[] commandParameters)
		public String NonQuery(out Int32 rowsAffected, String commandText, params SqlParameter[] commandParameters)
		{
			String rowId = String.Empty;

			//[patch 20080702 afancinelli
			rowsAffected = 0;
			//]

			try
			{
				if (dbConnection.State == ConnectionState.Closed)
				{
					dbConnection.Open();
				}

				using (SqlCommand dbCommand = new SqlCommand(commandText, dbConnection))
				{
					foreach (SqlParameter commandParameter in commandParameters)
					{
						SqlParameter parameter = new SqlParameter(commandParameter.ParameterName, commandParameter.Value);

						parameter.Direction = commandParameter.Direction;
						parameter.SqlDbType = commandParameter.SqlDbType;

						dbCommand.Parameters.Add(parameter);
					}

					rowsAffected = dbCommand.ExecuteNonQuery();
				}

			}
			catch (SqlException sqlExc)
			{
				String sParams = String.Empty;
				foreach (SqlParameter OraPar in commandParameters)
				{
					sParams += String.Format("{0}={1}\n", OraPar.ParameterName, OraPar.Value);
				}
				throw new Exception(String.Format("CommandText:\n{0}\nParams:\n{1}\n",
																commandText, sParams),
																sqlExc);

			}
			catch
			{
				throw;
			}
			finally
			{
				dbConnection.Close();
			}

			return rowId.ToString();
		}

		/// <summary>
		/// Sfrutta la transaction passata come parametro senza aprirne una nuova ad ogni chiamata
		/// La connessione deve essere chiusa successivamente in fase di commit/rollback
		/// </summary>
		/// <param name="rowsAffected"></param>
		/// <param name="commandText"></param>
		/// <param name="sqlTrans">Transazione aperta esternamente</param>
		/// <param name="commandParameters"></param>
		public void NonQueryInTransaction(out Int32 rowsAffected, String commandText, SqlTransaction sqlTrans, params SqlParameter[] commandParameters)
		{
			try
			{
				using (SqlCommand dbCommand = new SqlCommand(commandText, sqlTrans.Connection))
				{
					foreach (SqlParameter commandParameter in commandParameters)
					{
						SqlParameter parameter = new SqlParameter(commandParameter.ParameterName, commandParameter.Value);

						parameter.Direction = commandParameter.Direction;
						parameter.SqlDbType = commandParameter.SqlDbType;

						dbCommand.Parameters.Add(parameter);
					}

					dbCommand.Transaction = sqlTrans;
					rowsAffected = dbCommand.ExecuteNonQuery();
				}
			}
			catch (SqlException sqlExc)
			{
				String sParams = String.Empty;
				foreach (SqlParameter OraPar in commandParameters)
				{
					sParams += String.Format("{0}={1}\n", OraPar.ParameterName, OraPar.Value);
				}
				throw new Exception(String.Format("CommandText:\n{0}\nParams:\n{1}\n",
																commandText, sParams),
																sqlExc);

			}
			catch
			{
				throw;
			}
			finally
			{

			}
		}

		/// <summary>
		/// executes a non query inside a given transaction
		/// </summary>
		/// <param name="commands"></param>
		/// <param name="commandsParameters"></param>
		/// <param name="transaction"></param>
		public void NonQuery(String[] commands, List<SqlParameter[]> commandsParameters, SqlTransaction transaction)
		{
			try
			{
				for (Int32 iCommand = 0; iCommand < commands.Length; iCommand++)
				{
					Int32 dummyOut;
					this.NonQueryInTransaction(out dummyOut, commands[iCommand], transaction, commandsParameters.ToArray()[iCommand]);
				}
			}
			catch (SqlException sqlExc)
			{
				String sCommands = String.Empty;

				foreach (String OraComText in commands)
				{
					sCommands += String.Format("{0}\n", OraComText);
				}

				//queryException = new UNException(sqlExc);
				throw new Exception(String.Format("CommandText:\n{0}\nParams:\n{1}\n",
																sCommands, commandsParameters.ToArray().ToString()),
																sqlExc);
			}
			catch
			{
				throw;
			}
		}


		public Object ScalarQuery(String commandText, params SqlParameter[] commandParameters)
		{
			Object scalarResult = null;

			try
			{
				if (dbConnection.State == ConnectionState.Closed)
				{
					dbConnection.Open();
				}

				using (SqlCommand dbCommand = new SqlCommand(commandText, dbConnection))
				{

					if (commandParameters != null)
					{
						foreach (SqlParameter commandParameter in commandParameters)
						{
							SqlParameter parameter = new SqlParameter(commandParameter.ParameterName, commandParameter.Value);

							parameter.Direction = commandParameter.Direction;
							parameter.SqlDbType = commandParameter.SqlDbType;

							dbCommand.Parameters.Add(parameter);
						}
					}

					scalarResult = dbCommand.ExecuteScalar();
				}
			}
			catch (SqlException sqlExc)
			{
				String sParams = String.Empty;
				foreach (SqlParameter OraPar in commandParameters)
				{
					sParams += String.Format("{0}={1} \n", OraPar.ParameterName, OraPar.Value);
				}
				//queryException = new UNException(sqlExc);
				throw new Exception(String.Format("CommandText:\n{0} \nParams:\n{1} \n",
																commandText, sParams),
																sqlExc);
			}
			catch
			{
				throw;
			}
			finally
			{
				dbConnection.Close();
			}

			return scalarResult;
		}


		public Object ScalarQuery(SqlTransaction transaction, String commandText, params SqlParameter[] commandParameters)
		{
			Object scalarResult = null;

			try
			{

				using (SqlCommand dbCommand = new SqlCommand(commandText, dbConnection))
				{

					if (commandParameters != null)
					{
						foreach (SqlParameter commandParameter in commandParameters)
						{
							SqlParameter parameter = new SqlParameter(commandParameter.ParameterName, commandParameter.Value);

							parameter.Direction = commandParameter.Direction;
							parameter.SqlDbType = commandParameter.SqlDbType;

							dbCommand.Parameters.Add(parameter);
						}
					}

					dbCommand.Transaction = dbCommand.Transaction = transaction;

					scalarResult = dbCommand.ExecuteScalar();
				}
			}
			catch (SqlException sqlExc)
			{
				String sParams = String.Empty;
				foreach (SqlParameter OraPar in commandParameters)
				{
					sParams += String.Format("{0}={1} \n", OraPar.ParameterName, OraPar.Value);
				}
				//queryException = new UNException(sqlExc);
				throw new Exception(String.Format("CommandText:\n{0} \nParams:\n{1} \n",
																commandText, sParams),
																sqlExc);
			}
			catch
			{
				throw;
			}

			return scalarResult;
		}


		/// <summary>
		/// attenzione -> il parametro useinferredschema va a modificare le clonne aggiungendo dynamic_
		/// bello perche' tutta sta storia del dynamic non la si capisce proprio.
		/// correggo per gestire -1 o 0 nel paging... che mi indica di prendere tutto
		/// </summary>
		/// <param name="commandText"></param>
		/// <param name="pageStartIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="useInferredSchema"></param>
		/// <param name="commandParameters"></param>
		/// <returns></returns>
		public DataTable Query(String commandText, Int32 pageStartIndex, Int32 pageSize, Boolean useInferredSchema, params SqlParameter[] commandParameters)
		{
			DataTable dtResults = new DataTable();
			Int32 pageEndIndex = 0; //pageStartIndex + pageSize, 
			Int32 currentIndex = 0;
			Boolean hasMoreRows = true;

			try
			{


				if (dbConnection.State == ConnectionState.Closed)
				{
					dbConnection.Open();
				}

				using (SqlCommand dbCommand = new SqlCommand(commandText, dbConnection))
				{

					foreach (SqlParameter commandParameter in commandParameters)
					{
						SqlParameter parameter = new SqlParameter(commandParameter.ParameterName, commandParameter.Value);

						parameter.Direction = commandParameter.Direction;
						parameter.SqlDbType = commandParameter.SqlDbType;

						dbCommand.Parameters.Add(parameter);
					}

					using (SqlDataReader reader = dbCommand.ExecuteReader())
					{


						//27012010 - problema pagesize.. se voglio usare inferred schema e 
						if (pageStartIndex <= 0)
						{
							pageStartIndex = 0;
						}

						if (pageSize <= 0)
						{
							//full
							pageSize = 100000; //centomila pagine saranno sufficienti?
						}

						//fixe page end
						pageEndIndex = pageStartIndex + pageSize;


						try
						{
							DataTable dtResultsSchema = reader.GetSchemaTable();

							foreach (DataRow rowColumnData in dtResultsSchema.Rows)
							{
								String columnDataType = rowColumnData["datatype"].ToString();
								String columnName = String.Empty;

								if (useInferredSchema)
								{
									columnName = String.Format("dynamic_{0}", rowColumnData["columnname"].ToString());
								}
								else
								{
									columnName = rowColumnData["columnname"].ToString();
								}

								DataColumn columnToAdd = new DataColumn(columnName, Type.GetType(columnDataType));

								dtResults.Columns.Add(columnToAdd);
							}

							while ((hasMoreRows) && (currentIndex < pageStartIndex))
							{
								hasMoreRows = reader.Read();
								currentIndex++;
							}

							while ((hasMoreRows) && (currentIndex >= pageStartIndex) && (currentIndex < pageEndIndex))
							{
								hasMoreRows = reader.Read();

								if (hasMoreRows)
								{
									currentIndex++;

									Object[] rowValues = new Object[reader.FieldCount];

									reader.GetValues(rowValues);
									dtResults.LoadDataRow(rowValues, false);
								}
							}

						}
						finally
						{
							if (reader != null) reader.Close();
						}
					}
				}
			}
			catch (SqlException sqlExc)
			{
				String sParams = String.Empty;
				foreach (SqlParameter OraPar in commandParameters)
				{
					sParams += String.Format("{0}={1}\n", OraPar.ParameterName, OraPar.Value);
				}
				throw new Exception(String.Format("CommandText:\n{0}\nParams:\n{1}\n",
																								commandText, sParams),
																								sqlExc);
			}
			catch
			{
				throw;
			}
			finally
			{
				dbConnection.Close();
			}

			return dtResults;
		}


		/// <summary>
		/// prova
		/// </summary>
		/// <param name="commandText"></param>
		/// <returns></returns>
		public DataTable Query(String commandText)
		{
			//DataSet dsResults = new DataSet("results");
			DataTable dtResults = new DataTable();

			try
			{
				if (dbConnection.State == ConnectionState.Closed)
				{
					dbConnection.Open();
				}

				using (SqlCommand dbCommand = new SqlCommand(commandText, dbConnection))
				{
					using (SqlDataAdapter dbAdapter = new SqlDataAdapter(dbCommand))
					{
						//SqlDataAdapter dbAdapter = new SqlDataAdapter(commandText, dbConnection);
						dtResults.Clear();
						dbAdapter.AcceptChangesDuringFill = false;
						dbAdapter.FillLoadOption = LoadOption.Upsert;
						dbAdapter.AcceptChangesDuringUpdate = false;
						//dsResults.Clear();
						//dbAdapter.Fill(dsResults);
						dtResults.BeginLoadData();
						dbAdapter.Fill(dtResults);
						dtResults.EndLoadData();
					}
				}
			}
			catch (SqlException sqlExc)
			{
				//queryException = new UNException(sqlExc);
				String sParams = String.Empty;
				throw new Exception(String.Format("CommandText: \n{0}  \nParams: \n{1}\n",
																commandText, sParams),
																sqlExc);
			}
			catch
			{
				throw;
			}
			finally
			{
				dbConnection.Close();
			}

			return dtResults;
		}

		public DataTable Query(String commandText, params SqlParameter[] commandParameters)
		{
			return Query(commandText, 0, commandParameters);
		}

		/// <summary>
		/// vmorell strabaco 
		/// </summary>
		/// <param name="commandText"></param>
		/// <param name="commandParameters"></param>
		/// <returns></returns>
		public DataTable Query(String commandText, Int32 commandTimeout, params SqlParameter[] commandParameters)
		{
			DataTable dtResults = new DataTable();
			try
			{
				if (dbConnection.State == ConnectionState.Closed)
				{
					dbConnection.Open();
				}

				using (SqlCommand dbCommand = new SqlCommand(commandText, dbConnection))
				{
					//HACK
					if (commandTimeout != 0)
					{
						dbCommand.CommandTimeout = commandTimeout;
					}
					//25062008
					//in questo momento non si puo' far una query cui non vengano passati parametri..
					//non mi garba, se voglio una distinct da una tabelladi lookup senza where non posso.
					//non va bene.

					//check solo su null commandparameters. se ci sono nel command dei named pars (es. :parametro)
					//e l'array commandparameters e' vuoto, beh alzera' l'errore corrispondente...

					if (commandParameters != null)
					{
						//cicla... 
						foreach (SqlParameter commandParameter in commandParameters)
						{
							SqlParameter parameter = new SqlParameter(commandParameter.ParameterName, commandParameter.Value);

							parameter.Direction = commandParameter.Direction;
							parameter.SqlDbType = commandParameter.SqlDbType;

							dbCommand.Parameters.Add(parameter);
						}
					}
					using (SqlDataAdapter dbAdapter = new SqlDataAdapter(dbCommand))
					{
						dtResults.BeginLoadData();
						dbAdapter.Fill(dtResults);
						dtResults.EndLoadData();
					}
				}
			}
			catch (SqlException sqlExc)
			{
				//queryException = new UNException(sqlExc);
				String sParams = String.Empty;
				if (commandParameters != null)
				{
					foreach (SqlParameter OraPar in commandParameters)
					{
						sParams += String.Format(" {0}={1} \n", OraPar.ParameterName, OraPar.Value);
					}
				}

				throw new Exception(String.Format("CommandText: \n{0}  \nParams: \n{1}\n",
																commandText, sParams),
																sqlExc);
			}
			catch
			{
				throw;
			}
			finally
			{
				dbConnection.Close();
			}

			return dtResults;
		}

		//19092008 - INSERIMENTO TABELLA
		//h bisogno di eseguire query e non query dentro una transazione
		//non posso fare una unica procedura di inserimento richieste perche' sarebbe il caos.
		//quindi comincio a prevedere alcuni comandi e query (sol quelle che mi servono) da eseguire in transaction 
		//chissa' che no vengano utili dopo...


		/// <summary>
		/// executes a command in transaction...
		/// </summary>
		/// <param name="oracleTrans"></param>
		/// <param name="command"></param>
		/// <param name="commandParameters"></param>
		public void ExecuteCommand(SqlTransaction oracleTrans, SqlCommand command, params SqlParameter[] commandParameters)
		{
			String rowId = String.Empty;
			int rowsAffected;


			rowsAffected = 0;

			try
			{

				command.Connection = oracleTrans.Connection;
				command.Transaction = oracleTrans;

				foreach (SqlParameter commandParameter in commandParameters)
				{
					command.Parameters.Add(commandParameter);
				}

				rowsAffected = command.ExecuteNonQuery();

			}
			catch (SqlException sqlExc)
			{
				String sParams = String.Empty;
				foreach (SqlParameter OraPar in commandParameters)
				{
					sParams += String.Format("{0}={1}\n", OraPar.ParameterName, OraPar.Value);
				}
				throw new Exception(String.Format("CommandText:\n{0}\nParams:\n{1}\n",
																command.CommandText, sParams),
																sqlExc);
			}
			catch
			{
				throw;
			}

			return;
		}


		public DataTable Query(SqlTransaction trans, String commandText, params SqlParameter[] commandParameters)
		{
			DataTable dtResults = new DataTable();

			try
			{


				using (SqlCommand dbCommand = new SqlCommand(commandText, trans.Connection, trans))
				{

					//25062008
					//in questo momento non si puo' far una query cui non vengano passati parametri..
					//non mi garba, se voglio una distinct da una tabelladi lookup senza where non posso.
					//non va bene.
					//check solo su null commandparameters. se ci sono nel command dei named pars (es. :parametro)
					//e l'array commandparameters e' vuoto, beh alzera' l'errore corrispondente...

					if (commandParameters != null)
					{
						//cicla... 
						foreach (SqlParameter commandParameter in commandParameters)
						{
							SqlParameter parameter = new SqlParameter(commandParameter.ParameterName, commandParameter.Value);

							parameter.Direction = commandParameter.Direction;
							parameter.SqlDbType = commandParameter.SqlDbType;

							dbCommand.Parameters.Add(parameter);
						}

					}

					using (SqlDataAdapter dbAdapter = new SqlDataAdapter(dbCommand))
					{
						dtResults.BeginLoadData();
						dbAdapter.Fill(dtResults);
						dtResults.EndLoadData();
					}
				}
			}
			catch (SqlException sqlExc)
			{
				//queryException = new UNException(sqlExc);
				String sParams = String.Empty;
				if (commandParameters != null)
				{
					foreach (SqlParameter OraPar in commandParameters)
					{
						sParams += String.Format(" {0}={1} \n", OraPar.ParameterName, OraPar.Value);
					}
				}

				throw new Exception(String.Format("CommandText: \n{0}  \nParams: \n{1}\n",
																commandText, sParams),
																sqlExc);
			}
			catch
			{
				throw;
			}

			return dtResults;
		}


		public void NonQuery(SqlTransaction transaction, String[] commands, List<SqlParameter[]> commandsParameters)
		{
			NonQuery(commands, commandsParameters, transaction);
		}


		public String NonQuery(SqlTransaction transaction, String commandText, params SqlParameter[] commandParameters)
		{
			Int32 rowsAffected = 0;

			return NonQuery(transaction, out rowsAffected, commandText, commandParameters);
		}

		public String NonQuery(SqlTransaction transaction, out Int32 rowsAffected, String commandText, params SqlParameter[] commandParameters)
		{
			String rowId = String.Empty;
			rowsAffected = 0;

			try
			{
				using (SqlCommand dbCommand = new SqlCommand(commandText, transaction.Connection, transaction))
				{
					foreach (SqlParameter commandParameter in commandParameters)
					{
						SqlParameter parameter = new SqlParameter(commandParameter.ParameterName, commandParameter.Value);
						parameter.Direction = commandParameter.Direction;
						parameter.SqlDbType = commandParameter.SqlDbType;

						dbCommand.Parameters.Add(parameter);
					}

					rowsAffected = dbCommand.ExecuteNonQuery();
				}
			}
			catch (SqlException sqlExc)
			{
				String sParams = String.Empty;
				foreach (SqlParameter OraPar in commandParameters)
				{
					sParams += String.Format("{0}={1}\n", OraPar.ParameterName, OraPar.Value);
				}
				throw new Exception(String.Format("CommandText:\n{0}\nParams:\n{1}\n",
																commandText, sParams),
																sqlExc);
			}
			catch
			{
				throw;
			}

			return rowId.ToString();
		}

		#endregion

		#region Properties

		public ConnectionState ConnectionState
		{
			get { return dbConnection.State; }
		}

		//public SqlConnection Connection
		//{
		//    get { return dbConnection; }
		//}


		#endregion

		#region Fields

		private SqlConnection dbConnection;

		#endregion

		#region IDisposable Membri di

		// Track whether Dispose has been called.
		private bool disposed = false;

		/// <summary>
		/// dispose esplicito
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if (!this.disposed)
			{
				// If disposing equals true, dispose all managed 
				// and unmanaged resources.
				if (disposing)
				{
					// Dispose managed resources.
					if (!(this.dbConnection == null))
					{
						//se la conn e' ancora aperta... beh...
						switch (this.dbConnection.State)
						{
							case ConnectionState.Connecting:
							case ConnectionState.Open:
							case ConnectionState.Executing:
							case ConnectionState.Fetching:
							case ConnectionState.Broken:
								try
								{
									this.dbConnection.Close();
								}
								catch { }
								break;
							case ConnectionState.Closed:
								break;
						}

						this.dbConnection.Dispose();
					}

				}

			}
			disposed = true;
		}


		/// <summary>
		/// dispose esplicito
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
