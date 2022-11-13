using Antimicrobici.Core.Models;
using Antimicrobici.Core.Utils;
using Antimicrobici.SqlServer;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antimicrobici.Core.Services
{
    public interface IRichiedenteService
    {
        List<Richiedente> GetRichiedenti(string username);
        Richiedente GetRichiedente(string codice);
    }

    public class RichiedenteService : IRichiedenteService
    {
        private readonly ILogger<RichiedenteService> logger;
        private readonly IPrincipalService principalService;
        private readonly IDataHelperService dataHelperService;
        public RichiedenteService(ILogger<RichiedenteService> logger, IPrincipalService principalService, IDataHelperService dataHelperService)
        {
            this.logger = logger;
            this.principalService = principalService;
            this.dataHelperService = dataHelperService;
        }

        public List<Richiedente> GetRichiedenti(string username)
        {
            #region Declaration
            List<Richiedente> lstResult = new List<Richiedente>();
            string s = String.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();
            #endregion

            using (SqlEngine db = new SqlEngine(dataHelperService.GetConnectionString()))
            {
                s = @" SELECT DISTINCT CdcRichiedente, DescrizioneCdcRichiedente 
                    FROM RichiestaImpegno
                    ORDER BY 1";

                DataTable dt = db.Query(s, parameters.ToArray());
                #region CREATE LIST
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Richiedente item = new Richiedente();
                        item.Codice = General.DBToString(row["CdcRichiedente"]);
                        item.Nome = General.DBToString(row["DescrizioneCdcRichiedente"]);
                        lstResult.Add(item);
                    }
                }
                #endregion
            }
            return lstResult;
        }

        public Richiedente GetRichiedente(string codice)
        {
            #region DECLARATION
            Richiedente result = null;
            string s = String.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();
            #endregion

            using (SqlEngine db = new SqlEngine(dataHelperService.GetConnectionString()))
            {
                s = @" SELECT DISTINCT CdcRichiedente, DescrizioneCdcRichiedente 
                    FROM RichiestaImpegno";

                #region FILTRI 
                s += String.Format(" WHERE CdcRichiedente = '{0}' ", codice);
                #endregion

                DataTable dt = db.Query(s, parameters.ToArray());
                #region CREATE LIST
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    result = new Richiedente();
                    result.Codice = General.DBToString(row["CdcRichiedente"]);
                    result.Nome = General.DBToString(row["DescrizioneCdcRichiedente"]);
                }
                #endregion
            }
            return result;
        }

    }
}
