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
    public interface ICatalogoService
    {
        List<NamedEntity> GetMateriali(string userID);
        List<MatScadutoCatalogo> GetMaterialiScadutiCatalogo();
        List<NamedEntity> SearchMateriali(string nomeMat, string userID);
        List<MatScadutoCatalogo> GetMaterialiScadutiCatalogoByNome(string nomeMateriale);
    }

    public class CatalogoService : BaseService<MatScadutoCatalogo, long, AmDbContext>, ICatalogoService
    {
        private readonly IDataHelperService dataHelperService;
        public CatalogoService(IDataHelperService dataHelperService, AmDbContext ctx = null)
            : base(ctx)
        {
            this.dataHelperService = dataHelperService; 
        }

        public List<NamedEntity> GetMateriali(string userID)
        {
            #region Declaration
            List<NamedEntity> lstResult = new List<NamedEntity>();
            string s = String.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();
            #endregion

            using (SqlEngine db = new SqlEngine(dataHelperService.GetConnectionString()))
            {
                s = @" SELECT CodMateriale, DescMateriale
                        FROM MatScadutoCatalogo 
                        WHERE 1=1";

                #region FILTRI 
                //TODO DATASET
                // s += String.Format(" AND Azienda IN ({0}) ", DataSetDAL.GetDataSetAzienda(userID));
                #endregion

                DataTable dt = db.Query(s, parameters.ToArray());

                #region CREATE LIST
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        NamedEntity item = new NamedEntity();
                        item.Codice = General.DBToString(row["CodMateriale"]);
                        item.Nome = General.DBToString(row["DescMateriale"]);
                        lstResult.Add(item);
                    }
                }
                #endregion

            }
            return lstResult;
        }

        public List<MatScadutoCatalogo> GetMaterialiScadutiCatalogo()
        {            
            return ctx.MatScadutoCatalogo.ToList();
        }

        public List<NamedEntity> SearchMateriali(string nomeMat, string userID)
        {
            #region Declaration
            List<NamedEntity> result = new List<NamedEntity>();
            string s = String.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();
            #endregion

            using (SqlEngine db = new SqlEngine(dataHelperService.GetConnectionString()))
            {
                s = @" SELECT DISTINCT CodMateriale, DescMateriale
                    FROM MatScadutoCatalogo 
                    WHERE 1=1
                    AND ( DescMateriale LIKE @nome
                    OR CodMateriale LIKE @nome )";

                #region FILTRI 
                parameters.Add(new SqlParameter("nome", "%" + nomeMat + "%"));
                // s += String.Format(" AND Azienda IN ({0}) ", DataSetDAL.GetDataSetAzienda(userID));
                #endregion

                DataTable dt = db.Query(s, parameters.ToArray());

                #region CREATE LIST
                foreach (DataRow row in dt.Rows)
                {
                    NamedEntity item = new NamedEntity();
                    item.Codice = General.DBToString(row["CodMateriale"]);
                    item.Nome = General.DBToString(row["DescMateriale"]);
                    result.Add(item);
                }
                #endregion
            }
            return result;
        }

        public List<MatScadutoCatalogo> GetMaterialiScadutiCatalogoByNome(string nomeMateriale)
        {
            return ctx.MatScadutoCatalogo.Where(x => x.CodMateriale.Contains(nomeMateriale) || x.DescMateriale.Contains(nomeMateriale)).ToList();
        }

    }
}
