using Antimicrobici.Core.Models;
using Antimicrobici.Core.Services;
using Antimicrobici.Core.Utils;
using Antimicrobici.SqlServer;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antimicrobici.Core.Services
{
    public interface IMaterialiService
    {
        List<NamedEntity> GetMateriali(string username);
        NamedEntity GetMateriale(string codice);
    }

    public class MaterialiService: IMaterialiService
    {
        private readonly IDataHelperService dataHelperService;
        public MaterialiService(IDataHelperService dataHelperService) 
        { 
            this.dataHelperService = dataHelperService;
        }

        public List<NamedEntity> GetMateriali(string username)
        {
            #region Declaration
            List<NamedEntity> lstResult = new List<NamedEntity>();
            string s = String.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();
            #endregion

            using (SqlEngine db = new SqlEngine(dataHelperService.GetConnectionString()))
            {
                s = @" SELECT DISTINCT CodiceMateriale, DescrizioneMateriale
               FROM RichiestaImpegno
               ORDER BY 1";

                #region FILTRI 
                //TODO DATASET
                //s += String.Format(" AND IDReparto IN ({0}) ", DataSetDAL.GetDataSetReparti(userID));
                //s += String.Format(" AND IDOspedale IN ({0}) ", DataSetDAL.GetDataSetOspedali(userID));
                #endregion

                DataTable dt = db.Query(s, parameters.ToArray());

                #region CREATE LIST
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        NamedEntity item = new NamedEntity();
                        item.Codice = General.DBToString(row["CodiceMateriale"]);
                        item.Nome = General.DBToString(row["DescrizioneMateriale"]);
                        lstResult.Add(item);
                    }
                }

                #endregion

            }
            return lstResult;
        }

        public NamedEntity GetMateriale(string codice)
        {
            #region Declaration
            NamedEntity result = null;
            string s = String.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();
            #endregion

            using (SqlEngine db = new SqlEngine(dataHelperService.GetConnectionString()))
            {
                s = @" SELECT DISTINCT CodiceMateriale, DescrizioneMateriale
               FROM RichiestaImpegno 
               ";

                #region FILTRI 
                s += String.Format(" WHERE CodiceMateriale = '{0}' ", codice);
                #endregion

                DataTable dt = db.Query(s, parameters.ToArray());

                #region CREATE LIST
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    result = new NamedEntity();
                    result.Codice = General.DBToString(row["CodiceMateriale"]);
                    result.Nome = General.DBToString(row["DescrizioneMateriale"]);
                }

                #endregion

            }
            return result;
        }

    }
}
