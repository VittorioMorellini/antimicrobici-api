using Antimicrobici.Core.Services;
using Antimicrobici.Core.Utils;
using Antimicrobici.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antimicrobici.Core.Models
{
    public interface ITipoMaterialeService
    {
        List<NamedEntity> GetTipiMateriale(string userID);
    }

    public class TipoMaterialeService : ITipoMaterialeService
    {
        private readonly IDataHelperService dataHelperService;
        public TipoMaterialeService(IDataHelperService dataHelperService)
        {
            this.dataHelperService = dataHelperService;
        }

        public List<NamedEntity> GetTipiMateriale(string userID)
        {
            #region Declaration
            List<NamedEntity> lstResult = new List<NamedEntity>();
            string s = String.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();
            #endregion

            using (SqlEngine db = new SqlEngine(dataHelperService.GetConnectionString()))
            {
                s = @" SELECT DISTINCT CodTipoMateriale, DescTipoMateriale
                    FROM MatScadutoCatalogo
                    ORDER BY 2";

                #region FILTRI 
                // TODO DATASET after
                // s += String.Format(" AND IDReparto IN ({0}) ", DataSetDAL.GetDataSetReparti(userID));
                // s += String.Format(" AND IDOspedale IN ({0}) ", DataSetDAL.GetDataSetOspedali(userID));
                #endregion

                DataTable dt = db.Query(s, parameters.ToArray());
                #region CREATE LIST
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        NamedEntity item = new NamedEntity();
                        item.Codice = General.DBToString(row["CodTipoMateriale"]);
                        item.Nome = General.DBToString(row["DescTipoMateriale"]);
                        lstResult.Add(item);
                    }
                }
                #endregion
            }
            return lstResult;
        }

    }
}
