using Antimicrobici.Core.Filters;
using Antimicrobici.Core.Models;
using Antimicrobici.Core.Utils;
using Antimicrobici.SqlServer;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;

namespace Antimicrobici.Core.Services
{
    public interface IMatScadutoService
    {
        List<MatScaduto> GetMatScaduto(MatScadutoFilter filter, string userID, out Int32 total, bool useCount);
        Task<long> InsertToDB(MatScaduto item, string userID);

    }

    public class MatScadutoService : BaseService<MatScaduto, long, AmDbContext>, IMatScadutoService
    {
        private readonly IDataHelperService dataHelperService;
        private readonly ILogger<MatScadutoService> logger;

        public MatScadutoService(IDataHelperService dataHelperService, ILogger<MatScadutoService> logger, AmDbContext ctx = null)
            : base(ctx)
        {
            this.dataHelperService = dataHelperService;
            this.logger = logger;
        }

        public List<MatScaduto> GetMatScaduto(MatScadutoFilter filter, string userID, out Int32 total, bool useCount)
        {
            #region DECLARATION

            total = 0;
            List<MatScaduto> lstResult = new List<MatScaduto>();
            DataTable dt = null;
            string s = String.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();

            #endregion

            using (SqlEngine db = new SqlEngine(dataHelperService.GetConnectionString()))
            {
                s = @" SELECT MatScaduto.Id, 
                      MatScaduto.CodCdc, MatScadutoStruttura.Cdc, 
                      MatScadutoStruttura.Azienda,
                      MatScaduto.ControlDate, MatScaduto.CodMateriale, 
                      MatScadutoCatalogo.DescMateriale, 
                      --MatScadutoCatalogo.CodTipoMateriale, 
                      --MatScadutoCatalogo.DescTipoMateriale, 
                      MatScadutoCatalogo.DescUnitaMisura, 
                      MatScaduto.Quantity, 
                      MatScaduto.ExpireDate, 
                      MatScaduto.Notes, MatScaduto.Lotto,
                      MatScaduto.CompileUser,
                      MatScaduto.QualificationUser,
                      MatScaduto.RetirementDate, 
                      MatScaduto.RetirementNotes
                      FROM MatScaduto
                      LEFT OUTER JOIN MatScadutoCatalogo ON MatScaduto.CodMateriale = MatScadutoCatalogo.CodMateriale
                      LEFT OUTER JOIN MatScadutoStruttura ON MatScaduto.CodCdc = MatScadutoStruttura.CodCdc
                      WHERE 1=1";

                #region FILTER
                if (!string.IsNullOrWhiteSpace(filter.Cdc))
                {
                    s += " AND MatScaduto.CodCdc = '" + filter.Cdc + "'";
                }
                if (!string.IsNullOrWhiteSpace(filter.Materiale))
                {
                    s += " AND MatScaduto.CodMateriale = @materiale";
                    SqlParameter parMateriale = new SqlParameter("materiale", filter.Materiale);
                    parameters.Add(parMateriale);
                }
                //if (!string.IsNullOrWhiteSpace(filter.TipoMateriale))
                //{
                //    s += " AND MatScadutoCatalogo.IDTipoMateriale = @tipo";
                //    SqlParameter parTipo = new SqlParameter("tipo", filter.TipoMateriale);
                //    parameters.Add(parTipo);
                //}
                if (filter.DataDa.HasValue)
                {
                    s += " AND MatScaduto.ControlDate >= @datada";
                    SqlParameter parDataDa = new SqlParameter("datada", filter.DataDa);
                    parameters.Add(parDataDa);
                }
                if (filter.DataA.HasValue)
                {
                    s += " AND MatScaduto.ControlDate <= @dataa";
                    SqlParameter parDataA = new SqlParameter("dataa", filter.DataA);
                    parameters.Add(parDataA);
                }
                if (String.IsNullOrWhiteSpace(filter.Stato))
                {

                }
                else if (filter.Stato == "1")  // Ritirato
                {
                    s += " AND MatScaduto.RetirementDate <= GetDate()";
                }
                else if (filter.Stato == "0")  // NON Ritirato
                {
                    s += " AND MatScaduto.RetirementDate > GetDate()";
                }
                // DataSet
                //s += String.Format(" AND MatScaduto.IDCdc IN ({0}) ", DataSetDAL.GetDataSetCdc(userID));
                #endregion

                #region ORDINAMENTO
                if (!String.IsNullOrWhiteSpace(filter.sortBy) && (filter.sortDir == 1 || filter.sortDir == 2))
                {
                    if (filter.sortBy == "id")
                        s += " ORDER BY MatScaduto.Id";
                    if (filter.sortBy == "dataControllo")
                        s += " ORDER BY MatScaduto.ControlDate";
                    //if (filter.sortBy == "centroDiCosto")
                    //    s += " ORDER BY MatScadutoStrutture.CDC";
                    //if (filter.sortBy == "tipo")
                    //    s += " ORDER BY MatScadutoCatalogo.DesTipoMateriale";
                    //if (filter.sortBy == "materiale")
                    //    s += " ORDER BY MatScadutoCatalogo.DesMateriale";
                    //if (filter.sortBy == "UM")
                    //    s += " ORDER BY MatScadutoCatalogo.DesUnitaMisura";
                    //if (filter.sortBy == "qta")
                    //    s += " ORDER BY MatScaduto.Quantita";
                    //if (filter.sortBy == "scadenzaMateriale")
                    //    s += " ORDER BY MatScaduto.DataScadenzaMateriale";
                    //if (filter.sortBy == "lotto")
                    //    s += " ORDER BY MatScaduto.LottoMateriale";
                    //if (filter.sortBy == "compilatore")
                    //    s += " ORDER BY MatScaduto.UtenteCompilatore";
                    //if (filter.sortBy == "qualificaCompilatore")
                    //    s += " ORDER BY MatScaduto.QualificaCompilatore";
                    //if (filter.sortBy == "note")
                    //    s += " ORDER BY MatScaduto.Note";
                    //if (filter.sortBy == "ritirato")
                    //    s += " ORDER BY MatScaduto.DataRitiro";
                    //if (filter.sortBy == "noteRitiro")
                    //    s += " ORDER BY MatScaduto.NoteRitiro";

                    s += (filter.sortDir == 2 ? " DESC" : string.Empty);
                }
                else
                {
                    // TODO ordinamento di deafult
                    s += " ORDER BY MatScaduto.Id";
                }
                #endregion

                dt = db.Query(s, parameters.ToArray());

                #region CREATE LIST
                total = dt.Rows.Count;
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        MatScaduto item = new MatScaduto();

                        item.Id = General.DBToBigint(row["id"]);
                        item.CodCdc = General.DBToString(row["CodCdc"]);
                        //item.Cd = General.DBToString(row["Cdc"]);
                        //item.Azienda = General.DBToString(row["Azienda"]);
                        item.CodMateriale = General.DBToString(row["CodMateriale"]);
                        item.ControlDate = General.DBToDateTime(row["ControlDate"]);
                        item.RetirementDate = General.DBToNullableDateTime(row["RetirementDate"]);
                        item.ExpireDate = General.DBToDateTime(row["ExpireDate"]);
                        item.Quantity = General.DBToInt(row["Quantity"]);
                        //item.DescMateriale = General.DBToString(row["DescMateriale"]);
                        item.Notes = General.DBToString(row["Notes"]);
                        item.RetirementNotes = General.DBToString(row["RetirementNotes"]);
                        item.Lotto = General.DBToString(row["Lotto"]);
                        //item.Um = General.DBToString(row["DesUnitaMisura"]);
                        //item.IDTipoMateriale = General.DBToString(row["IDTipoMateriale"]);
                        //item.DesTipoMateriale = General.DBToString(row["DesTipoMateriale"]);
                        item.CompileUser = General.DBToString(row["CompileUser"]);
                        item.QualificationUser = General.DBToString(row["QualificationUser"]);

                        lstResult.Add(item);
                    }
                }

                #endregion
            }
            if (useCount == true)
                return lstResult.Skip<MatScaduto>(filter.Offset).Take(filter.Count).ToList();
            else
                return lstResult.Skip<MatScaduto>(filter.Offset).ToList();
        }


        //public DataTable GetDataForReport(Int32 ID, string userID)
        //{
        //    #region Declaration
        //    DataTable dt = null;
        //    string s = String.Empty;
        //    List<SqlParameter> parameters = new List<SqlParameter>();
        //    #endregion

        //    using (SqlEngine db = new SqlEngine(DataHelpers.GetConnectionString()))
        //    {
        //        s = String.Format(@" SELECT MatScaduto.ID,
        //                    MatScaduto.IDCdc,
        //                    MatScaduto.DataModifica, MatScaduto.Quantita,
        //                    MatScaduto.LottoMateriale,
        //                    MatScadutoStrutture.Cdc, MatScadutoStrutture.Azienda
        //                    FROM MatScaduto
        //                    LEFT OUTER JOIN MatScadutoCatalogo ON MatScaduto.IDMateriale = MatScadutoCatalogo.IDMateriale
        //                    LEFT OUTER JOIN MatScadutoStrutture ON MatScaduto.IDCDC = MatScadutoStrutture.IDCDC
        //                    WHERE MatScaduto.ID = {0}", ID);


        //        dt = db.Query(s);

        //    }

        //    return dt;

        //}


        //public bool DeleteFromDB(int id, string userID)
        //{
        //    #region DECLARATION
        //    bool result = false;
        //    string s = String.Empty;
        //    List<SqlParameter> parameters = new List<SqlParameter>();

        //    #endregion

        //    using (SqlEngine db = new SqlEngine(DataHelpers.GetConnectionString()))
        //    {
        //        s = @" DELETE FROM MatScaduto
        //       WHERE ID = " + id;

        //        try
        //        {
        //            #region FILTRI 
        //            // s += String.Format(" AND IDOspedale IN ({0}) ", DataSetDAL.GetDataSetOspedali(userID));
        //            #endregion
        //            db.NonQuery(s);
        //            result = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.logError("Error IN Delete Materiale scaduto", ex);
        //            result = false;
        //        }
        //    }
        //    return result;

        //}

        /// <summary>
        /// Author: vmorell 20200815, insert new entity in DB
        /// </summary>
        /// <param name="item"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<long> InsertToDB(MatScaduto item, string userID)
        {
            long result = 0;
            var newEntity = ctx.MatScaduto.Add(item);
            await ctx.SaveChangesAsync();
            result = newEntity.Entity.Id;

            return result;
        }

        ///// <summary>
        ///// AUTHOR: VMORELL 20200816
        /////   Devo salvare il materiale scaduto 
        ///// </summary>
        ///// <param name="item"></param>
        ///// <param name="userID"></param>
        ///// <returns></returns>
        //public MatScaduto UpdateToDB(MatScaduto item, string userID)
        //{
        //    MatScaduto result = null;
        //    using (AntimicrobiciEntities db = new AntimicrobiciEntitiesExt())
        //    {
        //        MatScaduto entity = db.MatScadutos.Find(item.ID);
        //        entity.NoteRitiro = item.NoteRitiro;
        //        entity.DataRitiro = item.DataRitiro;
        //        entity.DataModifica = item.DataModifica;
        //        entity.UtenteModifica = item.UtenteModifica;
        //        entity.UtenteCompilatore = item.UtenteCompilatore;
        //        entity.QualificaCompilatore = UtentiDAL.GetQualificaCompilatore(userID);
        //        db.SaveChanges();
        //        result = entity;
        //    }

        //    return result;
        //}

    }
}
