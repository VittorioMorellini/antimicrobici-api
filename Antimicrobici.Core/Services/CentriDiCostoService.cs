using Antimicrobici.Core.Models;
using Antimicrobici.Core.Utils;
using Antimicrobici.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Antimicrobici.Core.Services
{
    public interface ICentriDiCostoService
    {
        List<CentroDiCosto> GetCentriDiCosto(string userID);
        List<CentroDiCosto> SearchCentriDiCosto(string valore, string userID);
    }

    public class CentriDiCostoService : ICentriDiCostoService
    {
        private readonly IDataHelperService dataHelperService;
        public CentriDiCostoService(IDataHelperService dataHelperService)
        {
            this.dataHelperService = dataHelperService;
        }

        public List<CentroDiCosto> GetCentriDiCosto(string userID)
        {
            #region Declaration
            List<CentroDiCosto> lstResult = new List<CentroDiCosto>();
            string s = String.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();
            #endregion

            using (SqlEngine db = new SqlEngine(dataHelperService.GetConnectionString()))
            {
                s = @" SELECT DISTINCT CodCdc, Cdc, Azienda
                    FROM MatScadutoStruttura
                    ";

                #region FILTRI 
                // TODO DATASET after
                //s += String.Format(" WHERE IDCdc IN ({0}) ", DataSetDAL.GetDataSetCdc(userID));
                #endregion
                s += " ORDER BY 2";

                DataTable dt = db.Query(s, parameters.ToArray());
                #region CREATE LIST
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        CentroDiCosto item = new CentroDiCosto();
                        item.CodCdc = General.DBToString(row["CodCdc"]);
                        item.Cdc = General.DBToString(row["Cdc"]);
                        item.Azienda = General.DBToString(row["Azienda"]);
                        lstResult.Add(item);
                    }
                }
                #endregion
            }
            return lstResult;
        }


        public List<CentroDiCosto> SearchCentriDiCosto(string valore, string userID)
        {
            #region Declaration
            List<CentroDiCosto> lstResult = new List<CentroDiCosto>();
            string s = String.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();
            #endregion

            using (SqlEngine db = new SqlEngine(dataHelperService.GetConnectionString()))
            {
                s = @" SELECT DISTINCT CodCdc, Cdc, Azienda
                    FROM MatScadutoStruttura
                        WHERE (CodCdc LIKE @nome
                        OR Cdc LIKE @nome ) ";

                s += " ORDER BY 2";

                parameters.Add(new SqlParameter("nome", "%" + valore + "%"));
                DataTable dt = db.Query(s, parameters.ToArray());
                #region CREATE LIST
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        CentroDiCosto item = new CentroDiCosto();
                        item.CodCdc = General.DBToString(row["CodCdc"]);
                        item.Cdc = General.DBToString(row["Cdc"]);
                        item.Azienda = General.DBToString(row["Azienda"]);
                        lstResult.Add(item);
                    }
                }
                #endregion
            }
            return lstResult;
        }

    }
}
