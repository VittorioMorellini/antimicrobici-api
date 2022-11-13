using Antimicrobici.Core.Config;
using Antimicrobici.SqlServer;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Antimicrobici.Core.Services
{
    public interface IDataHelperService
    {
        string GetConnectionString();
        SqlEngine GetEngine();
        DataTable Query(string commandText, params SqlParameter[] commandParameters);
        string GetTipoEpisodioDescrizione(string idTipo);
    }

    public class DataHelperService : IDataHelperService
    {

        #region HELPER FOR THE CONNECTION

        public const string DB_TYPE = "DbType";
        private readonly ConfigOptions options;
        public DataHelperService(IOptions<ConfigOptions> options)
        {
            this.options = options.Value;
        }

        public string GetConnectionString()
        {
            return options.Antimicrobici;
        }

        /// <summary>
        /// Author: vmorell 20150512
        ///		Returns the Engine Sql for connections and query
        /// </summary>
        /// <returns></returns>
        public SqlEngine GetEngine()
        {
            return new SqlEngine(options.Antimicrobici);
        }


        #endregion

        #region FUNCTION used Via SQL

        /// <summary>
        /// author: vmorell 
        ///		Execute Query via Sql if I need and Get a DataTable
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataTable Query(string commandText, params SqlParameter[] commandParameters)
        {
            DataTable dtResults = new DataTable();
            SqlConnection dbConnection = new SqlConnection();
            try
            {
                if (dbConnection.State == ConnectionState.Closed)
                {
                    dbConnection.ConnectionString = GetConnectionString();
                    dbConnection.Open();
                }

                using (SqlCommand dbCommand = new SqlCommand(commandText, dbConnection))
                {
                    dbCommand.CommandTimeout = 180;
                    //in questo momento non si puo' far una query cui non vengano passati parametri..
                    //non mi garba, se voglio una distinct da una tabelladi lookup senza where non posso.
                    //non va bene.
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
                string sParams = string.Empty;
                if (commandParameters != null)
                {
                    foreach (SqlParameter OraPar in commandParameters)
                    {
                        sParams += string.Format(" {0}={1} \n", OraPar.ParameterName, OraPar.Value);
                    }
                }
                throw new Exception(string.Format("CommandText: \n{0}  \nParams: \n{1}\n", commandText, sParams), sqlExc);
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

        #endregion


        #region Helper fro Project

        public string GetTipoEpisodioDescrizione(string idTipo)
        {
            switch (idTipo)
            {
                case "O":
                    return "Ordinario";

                case "H":
                    return "Day Hospital";

                case "I":
                    return "Day Hospital";

                case "S":
                    return "Day Service";

                default:
                    return string.Empty;
            }
        }

        #endregion
    }
}
