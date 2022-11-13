using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Antimicrobici.Core.Utils;
using Antimicrobici.Core.Models;
using Antimicrobici.Core.Services;
using Antimicrobici.SqlServer;
using Antimicrobici.Core.Filters;
using Microsoft.Extensions.Logging;

namespace Antimicrobici.Core.Services
{
    public interface IRichiestaService
    {
        Delivery UpdateQuantita(Richiesta richiesta, string utente);
        List<Richiesta> GetRichieste(RichiestaFilter filter, string userID, out Int32 total, bool useCount);
        List<Richiesta> GetRichieste(RichiestaFilter filter, string userID, out Int32 total);
    }

    public class RichiestaService : BaseService<Delivery, long, AmDbContext>, IRichiestaService
    {
        private readonly IDataHelperService dataHelperService;
        private readonly ILogger<IDataHelperService> logger;
        public RichiestaService(IDataHelperService dataHelperService, ILogger<IDataHelperService> logger, AmDbContext ctx = null) 
            : base(ctx)
        { 
            this.dataHelperService = dataHelperService;
            this.logger = logger;
        }

        #region Richieste e conferma Medico

        public List<Richiesta> GetRichieste(RichiestaFilter filter, string userID, out Int32 total)
        {
            return GetRichieste(filter, userID, out total, true);
        }

        public List<Richiesta> GetRichieste(RichiestaFilter filter, string userID, out Int32 total, bool useCount)
        {
            #region Declaration
            // List<ListaAttesa> results = null;
            total = 0;
            List<Richiesta> lstResult = new List<Richiesta>();
            DataTable dt = null;
            string s = String.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();
            Dictionary<String, DateTime> keysFound = new Dictionary<string, DateTime>();
            Dictionary<String, Int32> keysCountInWeek = new Dictionary<string, Int32>();
            #endregion

            using (SqlEngine db = new SqlEngine(dataHelperService.GetConnectionString()))
            {
                s = QueryForRichieste();

                #region FILTRI 
                if (!String.IsNullOrWhiteSpace(filter.CodiceRichiedente))
                {
                    s += " AND CdcRichiedente = @richiedente";
                    parameters.Add(new SqlParameter("richiedente", filter.CodiceRichiedente));
                }
                if (!String.IsNullOrWhiteSpace(filter.CodiceMateriale))
                {
                    s += " AND CodiceMateriale = @materiale";
                    parameters.Add(new SqlParameter("materiale", filter.CodiceMateriale));
                }
                if (!String.IsNullOrWhiteSpace(filter.Medico))
                {
                    s += " AND MedicoRichiedente LIKE @medico";
                    parameters.Add(new SqlParameter("medico", filter.Medico + "%"));
                }
                if (!String.IsNullOrWhiteSpace(filter.Destinatario))
                {
                    s += " AND DestinatarioPazienteDettaglio LIKE @destinatario";
                    parameters.Add(new SqlParameter("destinatario", filter.Destinatario + "%"));
                }
                if (!String.IsNullOrWhiteSpace(filter.Motivazione))
                {
                    s += " AND Motivazione LIKE @motivazione";
                    parameters.Add(new SqlParameter("motivazione", filter.Motivazione + "%"));
                }
                if (!String.IsNullOrWhiteSpace(filter.Stato))
                {
                    s += " AND ISNULL(Stato,0) = @stato";
                    parameters.Add(new SqlParameter("stato", Convert.ToByte(filter.Stato)));
                }
                if (!String.IsNullOrWhiteSpace(filter.StatoMedico))
                {
                    s += " AND ISNULL(StatoMedico,0) = @statoMedico";
                    parameters.Add(new SqlParameter("statoMedico", Convert.ToByte(filter.StatoMedico)));
                }

                if (filter.DataDa != null)
                {
                    s += " AND DataCreazioneImpegno >= @dataDa";
                    parameters.Add(new SqlParameter("dataDa", filter.DataDa));
                }
                if (filter.DataA != null)
                {
                    s += " AND DataCreazioneImpegno <= @dataA";
                    parameters.Add(new SqlParameter("dataA", filter.DataA));
                }
                //TODO DATASET
                //s += String.Format(" AND IDReparto IN ({0}) ", DataSetDAL.GetDataSetReparti(userID));
                //s += String.Format(" AND IDOspedale IN ({0}) ", DataSetDAL.GetDataSetOspedali(userID));
                #endregion

                #region ORDINAMENTO
                if (!String.IsNullOrWhiteSpace(filter.sortBy) && filter.sortDir == 1)
                {
                    if (filter.sortBy == "nrImpegno")
                        s += " ORDER BY R.nrImpegno";
                    if (filter.sortBy == "cdc_nome")
                        s += " ORDER BY R.CdcRichiedente";
                    if (filter.sortBy == "dataCreazioneImpegno")
                        s += " ORDER BY R.dataCreazioneImpegno";
                    if (filter.sortBy == "materiale")
                        s += " ORDER BY R.DescrizioneMateriale";
                    if (filter.sortBy == "paziente")
                        s += " ORDER BY R.DestinatarioPazienteDettaglio";
                    if (filter.sortBy == "quantita")
                        s += " ORDER BY ISNULL(C.Quantita, R.QtaConsegnata)";
                }
                else if (!String.IsNullOrWhiteSpace(filter.sortBy) && filter.sortDir == 2)
                {
                    if (filter.sortBy == "nrImpegno")
                        s += " ORDER BY R.nrImpegno DESC";
                    if (filter.sortBy == "cdc_nome")
                        s += " ORDER BY R.CdcRichiedente DESC";
                    if (filter.sortBy == "dataCreazioneImpegno")
                        s += " ORDER BY R.dataCreazioneImpegno DESC";
                    if (filter.sortBy == "materiale")
                        s += " ORDER BY R.DescrizioneMateriale DESC";
                    if (filter.sortBy == "paziente")
                        s += " ORDER BY R.DestinatarioPazienteDettaglio DESC";
                    if (filter.sortBy == "quantita")
                        s += " ORDER BY ISNULL(C.Quantita, R.QtaConsegnata) DESC";
                }
                else
                {
                    // TODO ordinamento di deafult
                    s += " ORDER BY R.DataCreazioneImpegno";
                }
                #endregion

                dt = db.Query(s, parameters.ToArray());

                #region CREATE LIST
                total = dt.Rows.Count;
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        String key = String.Empty;
                        if (!String.IsNullOrWhiteSpace(General.DBToString(row["DestinatarioPazienteDettaglio"])))
                            key = General.DBToString(row["CodiceMateriale"]).ToLower() + General.DBToString(row["DestinatarioPazienteDettaglio"]).ToLower();

                        Richiesta item = new Richiesta();
                        item.NrImpegno = General.DBToString(row["NrImpegno"]);
                        if (row["Quantita"] != DBNull.Value)
                        {
                            if (row["Quantita"] != DBNull.Value)
                                item.Quantita = General.DBToInt(row["Quantita"]);
                            else
                                item.Quantita = null;
                        }
                        else
                            item.Quantita = General.DBToInt(row["QtaConsegnata"]);

                        item.CdcRichiedente = new NamedEntity();
                        item.CdcRichiedente.Codice = General.DBToString(row["CdcRichiedente"]);
                        item.CdcRichiedente.Nome = General.DBToString(row["DescrizioneCdcRichiedente"]);
                        item.TipoImpegno = General.DBToString(row["TipoImpegno"]);
                        item.QtaRichiesta = General.DBToInt(row["QtaRichiesta"]);
                        item.Stato = General.DBToInt(row["Status"]);
                        item.CodiceMateriale = General.DBToString(row["CodiceMateriale"]);
                        item.DataErogazione = General.DBToDateTime(row["DataErogazione"]);
                        item.DataCreazioneImpegno = General.DBToDateTime(row["DataCreazioneImpegno"]);
                        item.Motivazione = General.DBToString(row["Motivazione"]);
                        item.InserimentoData = General.DBToNullableDateTime(row["InsertDate"]);
                        item.DescrizioneMateriale = General.DBToString(row["DescrizioneMateriale"]);
                        item.MedicoRichiedente = General.DBToString(row["MedicoRichiedente"]);
                        item.DestinatarioPazienteDettaglio = General.DBToString(row["DestinatarioPazienteDettaglio"]);
                        item.Note = General.DBToString(row["Notes"]);
                        item.MedicoUO = General.DBToString(row["MedicoUO"]);
                        item.Antibiogramma = General.DBToString(row["Antibiogramma"]);
                        item.ConsulenzaInfettivologica = General.DBToString(row["ConsulenzaInfettivologica"]);
                        item.NoteMedico = General.DBToString(row["MedicalNotes"]);
                        item.StatoMedico = General.DBToInt(row["MedicalStatus"]);
                        item.ModificaDataMedico = General.DBToNullableDateTime(row["MedicalUpdateDate"]);
                        item.ModificaUtenteMedico = General.DBToString(row["MedicalUpdateUser"]);
                        item.Posologia = General.DBToString(row["Posologia"]);
                        item.OffLabel = General.DBToInt(row["OffLabel"]) == 1;
                        lstResult.Add(item);

                        if (!String.IsNullOrWhiteSpace(key))
                        {
                            if (!keysFound.ContainsKey(key))
                            {
                                keysFound.Add(key, General.DBToDateTime(row["DataCreazioneImpegno"]));
                                keysCountInWeek.Add(key, 1);
                            }
                            else
                            {
                                // Devo salvare la minima data
                                DateTime valore = keysFound[key];
                                double days = (valore - General.DBToDateTime(row["DataCreazioneImpegno"])).TotalDays;
                                if (Math.Abs(days) <= 7)
                                {
                                    if (General.DBToDateTime(row["DataCreazioneImpegno"]) < valore)
                                        keysFound[key] = General.DBToDateTime(row["DataCreazioneImpegno"]);
                                    keysCountInWeek[key]++;
                                    Int32 numero = keysCountInWeek[key];
                                    if (numero > 2)
                                        item.Alert = true;
                                }
                            }
                        }
                    }
                }

                total = lstResult.Count;
                #endregion

            }
            if (useCount == true)
                return lstResult.Skip<Richiesta>(filter.Offset).Take(filter.Count).ToList();
            else
                return lstResult.Skip<Richiesta>(filter.Offset).ToList();
        }

        public Delivery UpdateQuantita(Richiesta richiesta, string utente)
        {
            Delivery result = null;
            string s = String.Empty;
            DateTime dataIns = DateTime.Now;
            try
            {
                Delivery consegna = ctx.Delivery.Where(x => x.NrImpegno == richiesta.NrImpegno).FirstOrDefault();
                if (consegna == null)
                {
                    Delivery consegnaNew = new Delivery();
                    consegnaNew.NrImpegno = richiesta.NrImpegno;
                    consegnaNew.Quantity = richiesta.Quantita != null ? (int)richiesta.Quantita : 0;
                    consegnaNew.Status = (byte)richiesta.Stato;
                    consegnaNew.Notes = richiesta.Note;
                    consegnaNew.OffLabel = richiesta.OffLabel ? (byte)1 : (byte)0;
                    consegnaNew.InsertDate = dataIns;
                    consegnaNew.UpdateDate = dataIns;
                    consegnaNew.InsertUser = utente;
                    consegnaNew.UpdateUser = utente;
                    result = consegnaNew;
                    ctx.Delivery.Add(consegnaNew);
                    ctx.SaveChanges();
                }
                else
                {
                    consegna.Quantity = richiesta.Quantita != null ? (int)richiesta.Quantita : 0;
                    consegna.Notes = richiesta.Note;
                    consegna.UpdateDate = dataIns;
                    consegna.UpdateUser = utente;
                    consegna.Status = (byte)richiesta.Stato;
                    consegna.OffLabel = richiesta.OffLabel ? (byte)1 : (byte)0;
                    result = consegna;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Errore in salvataggio consegne", ex);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Conferma medico
        /// </summary>
        /// <param name="richiesta"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public Delivery ConfermaMedico(Richiesta richiesta, string user)
        {
            Delivery result = null;
            string s = String.Empty;
            DateTime dataIns = DateTime.Now;
            try
            {
                var consegna = ctx.Delivery.Where(x => x.NrImpegno == richiesta.NrImpegno).FirstOrDefault();
                if (consegna == null)
                {
                    throw new Exception("Operazione non possibile per il medico");
                    //Consegne consegnaNew = new Consegne();
                    //consegnaNew.NrImpegno = richiesta.NrImpegno;
                    //consegnaNew.StatoMedico = (byte)richiesta.StatoMedico;
                    //consegnaNew.NoteMedico = richiesta.NoteMedico;
                    //consegnaNew.InserimentoDataMedico = dataIns;
                    //consegnaNew.ModificaDataMedico = dataIns;
                    //consegnaNew.InserimentoUtenteMedico = utente;
                    //consegnaNew.ModificaUtenteMedico = utente;
                    //result = consegnaNew;
                    //db.Consegnes.Add(consegnaNew);
                    //db.SaveChanges();
                }
                else
                {
                    // consegna.Quantita = richiesta.Quantita;
                    consegna.MedicalNotes = richiesta.NoteMedico;
                    consegna.MedicalUpdateDate = dataIns;
                    consegna.MedicalUpdateUser = user;
                    consegna.MedicalStatus = (byte)richiesta.StatoMedico;
                    result = consegna;
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Errore in conferma medico", ex);
                throw;
            }
            return result;
        }
        #endregion

        #region PRIVATE
        private String QueryForRichieste()
        {
            string s = @" SELECT 
                    R.NrImpegno, R.Quantita, R.QtaRichiesta, R.Posologia,
                    R.DataErogazione, R.Motivazione, R.CodiceMateriale,
                    R.CdcRichiedente, R.DescrizioneCdcRichiedente, R.TipoImpegno,
                    R.DataCreazioneImpegno, R.DescrizioneMateriale, 
                    R.MedicoRichiedente, R.DestinatarioPazienteDettaglio,
                    R.MedicoUO, R.Antibiogramma, 
                    R.ConsulenzaInfettivologica,
                    C.Quantity, 
					C.Status, 
					C.InsertDate, 
					C.Notes, 
                    C.InsertUser, 
					C.MedicalNotes, 
					C.OffLabel,
                    C.MedicalStatus, 
					C.MedicalUpdateUser, 
					C.MedicalUpdateDate
                    FROM RichiestaImpegno R
                    LEFT OUTER JOIN Delivery C ON R.NrImpegno = C.NrImpegno
                    WHERE 1=1 ";
            return s;
        }

        #endregion
    }
}
