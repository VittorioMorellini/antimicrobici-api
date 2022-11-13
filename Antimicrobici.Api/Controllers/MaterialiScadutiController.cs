using Antimicrobici.Api.Models;
using Antimicrobici.Core.Filters;
using Antimicrobici.Core.Models;
using Antimicrobici.Core.Services;
using Antimicrobici.Core.Utils;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Antimicrobici.Api.Controllers
{
    [Route("materialiscaduti")]
    [ApiController]
    [EnableCors]
    public class MaterialiScadutiController : ControllerBase
    {
        // GET: api/MaterialiScaduti
        private readonly IMatScadutoService service;
        private readonly ILogger<IMatScadutoService> logger;
        public MaterialiScadutiController(IMatScadutoService service, ILogger<IMatScadutoService> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        [HttpGet]
        public Result<MatScaduto> Get(Int32? count = 0, string codiceRichiedente = "", string cdc = "", string materiale = "", string tipo = "",
          String dataDa = "", String dataA = "", string stato = "", Int32? offset = 0, string sortBy = "", Int32? sortDir = 0)
        {
            #region DECLARATION
            List<MatScaduto> listeResult = new List<MatScaduto>();
            MatScadutoFilter filter = new MatScadutoFilter();
            Int32 total = 0;
            bool alertKey = false;
            #endregion

            #region SET FILTER
            filter.CodiceRichiedente = codiceRichiedente;
            filter.Materiale = materiale;
            filter.TipoMateriale = tipo;
            filter.Cdc = cdc;
            filter.Stato = stato;
            if (!String.IsNullOrWhiteSpace(dataDa))
            {
                DateTime data;
                string format = "yyyy-MM-dd";
                DateTime.TryParseExact(dataDa, format, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out data);
                filter.DataDa = data;
            }
            if (!String.IsNullOrWhiteSpace(dataA))
            {
                DateTime data;
                string format = "yyyy-MM-dd";
                DateTime.TryParseExact(dataA, format, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out data);
                filter.DataA = data;
            }
            filter.Count = count.HasValue && count > 0 ? count.Value : 10;
            filter.Offset = offset.HasValue ? offset.Value : 0;
            filter.sortDir = sortDir.HasValue ? sortDir.Value : 0;
            filter.sortBy = sortBy;
            #endregion

            // Authentication
            String userID = "siamorellini";
            List<MatScaduto> liste = service.GetMatScaduto(filter, userID, out total, true);
            
            foreach (MatScaduto item in liste)
            {
                //MatScaduto item = new MatScaduto();
                //item.id = lst.ID;
                //item.dataControllo = lst.DataControllo;
                //item.cdc = new CentroDiCosto();
                //item.cdc.idCdc = lst.IDCdc;
                //item.cdc.cdc = lst.Cdc;
                //item.cdc.azienda = lst.Azienda;
                //item.idMateriale = lst.IDMateriale;
                //item.desMateriale = lst.DesMateriale;
                //item.dataScadenzaMateriale = lst.DataScadenzaMateriale;
                if (!String.IsNullOrWhiteSpace(item.Notes))
                {
                    item.Notes = "SI";
                    item.TooltipNotes = item.Notes;
                }
                else
                {
                    item.Notes = "NO";
                    item.TooltipNotes = String.Empty;
                }
                //item.quantita = lst.Quantita;
                //item.lotto = lst.Lotto;
                //item.um = lst.Um;
                item.Type = new NamedEntity();
                //item.Type.codice = item.IdTipoMateriale;
                //item.Type.nome = item.DescTipoMateriale;
                
                //item.compilatore = lst.Compilatore;
                //item.qualificaCompilatore = lst.QualificaCompilatore;
                //item.dataRitiro = lst.DataRitiro;
                //item.noteRitiro = lst.NoteRitiro;
                //if (item.dataRitiro.HasValue && item.dataRitiro.Value <= DateTime.Today)
                //{
                //    item.classIcon = "green-icon td-icon";
                //}
                //else
                //{
                //    item.classIcon = "no-color td-icon";
                //}

                /*
                item.qtaRichiesta = lst.QtaRichiesta;
                item.cdcRichiedente = new NamedEntity();
                item.cdcRichiedente.codice = lst.CdcRichiedente.codice;
                item.cdcRichiedente.nome = lst.CdcRichiedente.nome;
                item.tipoImpegno = lst.TipoImpegno;
                item.dataErogazione = lst.DataErogazione;
                item.dataCreazioneImpegno = lst.DataCreazioneImpegno;
                item.codiceMateriale = lst.CodiceMateriale;
                item.motivazione = lst.Motivazione;
                item.stato = lst.Stato;
                item.statoMedico = lst.StatoMedico;
                item.inserimentoData = lst.InserimentoData;
                item.descrizioneMateriale = lst.DescrizioneMateriale;
                item.medicoRichiedente = lst.MedicoRichiedente;
                item.destinatarioPazienteDettaglio = lst.DestinatarioPazienteDettaglio;
                item.note = lst.Note;
                item.noteMedico = lst.NoteMedico;
                item.medicoUO = lst.MedicoUO;
                item.consulenzaInfettivologica = lst.ConsulenzaInfettivologica;
                item.antibiogramma = lst.Antibiogramma;
                item.posologia = lst.Posologia;
                item.offLabel = lst.OffLabel;
                item.alert = lst.Alert;
                if (item.alert)
                {
                  item.classAlert = "red-icon td-icon";
                  alertKey = true;
                }
                if (item.stato == 0)
                {
                  // e.Item.ImageIndex = 0; // blu chiaro -> inserimento Ok ma aggiornamento con dati obbligatori mancanti
                  item.classIcon = "yellow-icon td-icon";
                }
                else if (item.stato == 1)
                  item.classIcon = "green-icon td-icon";
                if (item.statoMedico == 0)
                {
                  // e.Item.ImageIndex = 0; // blu chiaro -> inserimento Ok ma aggiornamento con dati obbligatori mancanti
                  item.classIconMedico = "yellow-icon td-icon";
                }
                else if (item.statoMedico == 1)
                  item.classIconMedico = "green-icon td-icon";
                */
                listeResult.Add(item);
            }
            return new Result<MatScaduto>(total, listeResult, alertKey);
        }

        [HttpPost]
        public async Task<MatScadutoRecord> Save([FromBody] MatScadutoRecord item)
        {
            long idNew = 0;
            String userID = "siamorellini";

            //To Uncomment
            MatScaduto materiale = new MatScaduto();
            materiale.Cdc = item.Cdc;
            materiale.CodCdc = item.CodCdc;
            materiale.Materiale = item.Materiale;
            materiale.CodMateriale = item.CodMateriale;
            materiale.Lotto = item.Lotto;
            materiale.Notes = item.Notes;
            //DateTime d = new DateTime();
            //DateTime.TryParseExact(, "yyyy-MM-dd", null, DateTimeStyles.None, out d);
            materiale.ControlDate = item.ControlDate;
            // DateTime.TryParseExact(, "yyyy-MM-dd", null, DateTimeStyles.None, out d);
            materiale.ExpireDate = item.ExpireDate;
            materiale.Quantity = item.Quantity;
            materiale.InsertDate = DateTime.Now;
            materiale.InsertUser = userID;
            materiale.UpdateDate = DateTime.Now;
            materiale.UpdateUser = userID;

            if (materiale != null)
                idNew = await service.InsertToDB(materiale, userID);
            else
                idNew = 0;

            item.Id = idNew;
            return item;
        }


        // DELETE: api/MaterialiScaduti/5
        //[Route("api/materialiscaduti/{id}")]
        //[HttpDelete]
        //public bool Delete(int id)
        //{
        //    bool result = false;
        //    String userID = Authentication.GetWindowsAuthenticated();

        //    result = MatScadutoDAL.DeleteFromDB(id, userID);

        //    return result;
        //}

        //[Route("api/materialiscaduti/excel")]
        //[HttpGet]
        //public HttpResponseMessage GetExcel(Int32? count = 0, string codiceRichiedente = "", string cdc = "", string materiale = "", string tipo = "",
        //  String dataDa = "", String dataA = "", string stato = "", Int32? offset = 0, string sortBy = "", Int32? sortDir = 0)
        //{
        //    MatScadutoFilter filter = new MatScadutoFilter();
        //    Int32 total = 0;
        //    int k = 5;

        //    #region SET FILTER
        //    filter.CodiceRichiedente = codiceRichiedente;
        //    filter.Materiale = materiale;
        //    filter.Cdc = cdc;
        //    filter.TipoMateriale = tipo;
        //    filter.Stato = stato;
        //    if (!String.IsNullOrWhiteSpace(dataDa))
        //    {
        //        DateTime data;
        //        // dataDa = dataDa.Substring(0, dataDa.IndexOf('(') - 1);
        //        string format = "yyyy-MM-dd";
        //        // string format = "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(Ora legale centrale )'";
        //        // data = dataDa;
        //        DateTime.TryParseExact(dataDa, format, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out data);
        //        filter.DataDa = data;
        //    }
        //    if (!String.IsNullOrWhiteSpace(dataA))
        //    {
        //        DateTime data;
        //        // dataDa = dataDa.Substring(0, dataDa.IndexOf('(') - 1);
        //        string format = "yyyy-MM-dd";
        //        // string format = "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(Ora legale centrale )'";
        //        // data = dataDa;
        //        DateTime.TryParseExact(dataA, format, CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out data);
        //        filter.DataA = data;
        //    }
        //    filter.Count = 0;
        //    filter.Offset = 0;
        //    filter.sortDir = sortDir.HasValue ? sortDir.Value : 0;
        //    filter.sortBy = sortBy;
        //    #endregion

        //    // Authentication
        //    String userID = Authentication.GetWindowsAuthenticated();
        //    try
        //    {
        //        List<MatScaduto> liste = MatScadutoDAL.GetMaterialiScaduti(filter, userID, out total, false);
        //        XLWorkbook workbook = new XLWorkbook();
        //        var worksheet = workbook.Worksheets.Add("Materiali scaduti");
        //        String targetFile = System.Web.Hosting.HostingEnvironment.MapPath("/App_Data/xls/Materiali.xlsx");
        //        worksheet.Cell(1, 1).Style.Font.Bold = true;
        //        worksheet.Cell(1, 1).Style.Font.FontSize = 18;
        //        worksheet.Cell(1, 1).SetValue("Stampa materiali scaduti");
        //        String dataGenerazione = "Data generazione file: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        //        worksheet.Cell(2, 1).SetValue(dataGenerazione);

        //        worksheet.Cell(4, 1).SetValue("FILTRI");
        //        worksheet.Cell(4, 1).Style.Font.Bold = true;
        //        String titoloFiltro = string.Empty;
        //        if (filter.DataDa.HasValue)
        //        {
        //            titoloFiltro = "Data da" + ": " + filter.DataDa.Value;
        //            worksheet.Cell(k, 1).SetValue(titoloFiltro);
        //            k++;
        //        }
        //        if (filter.DataA.HasValue)
        //        {
        //            titoloFiltro = "Data a" + ": " + filter.DataA.Value;
        //            worksheet.Cell(k, 1).SetValue(titoloFiltro);
        //            k++;
        //        }
        //        if (!String.IsNullOrWhiteSpace(filter.CodiceRichiedente))
        //        {
        //            titoloFiltro = "Richiedente" + ": " + RichiedentiDAL.GetRichiedente(codiceRichiedente).nome;
        //        }
        //        else
        //        {
        //            titoloFiltro = "Richiedenti: tutti";
        //        }
        //        if (!String.IsNullOrWhiteSpace(filter.CodiceRichiedente))
        //        {
        //            worksheet.Cell(k, 1).SetValue(titoloFiltro);
        //            k++;
        //        }
        //        if (!String.IsNullOrWhiteSpace(filter.Materiale))
        //        {
        //            titoloFiltro = "Materiale: " + MaterialiDAL.GetMateriale(filter.Materiale).nome;
        //        }
        //        else
        //        {
        //            titoloFiltro = "Materiale: tutti";
        //        }
        //        if (!String.IsNullOrWhiteSpace(filter.Materiale))
        //        {
        //            worksheet.Cell(k, 1).SetValue(titoloFiltro);
        //            k++;
        //        }

        //        if (!String.IsNullOrWhiteSpace(filter.TipoMateriale))
        //        {
        //            titoloFiltro = "Tipo: " + filter.TipoMateriale;
        //        }
        //        else
        //        {
        //            titoloFiltro = "Tipo: tutti";
        //        }
        //        if (!String.IsNullOrWhiteSpace(filter.TipoMateriale))
        //        {
        //            worksheet.Cell(k, 1).SetValue(titoloFiltro);
        //            k++;
        //        }

        //        if (!String.IsNullOrWhiteSpace(filter.Cdc))
        //        {
        //            titoloFiltro = "Cdc: " + filter.Cdc;
        //        }
        //        else
        //        {
        //            titoloFiltro = "Cdc: tutte";
        //        }
        //        if (!String.IsNullOrWhiteSpace(filter.Cdc))
        //        {
        //            worksheet.Cell(k, 1).SetValue(titoloFiltro);
        //            k++;
        //        }

        //        if (!String.IsNullOrWhiteSpace(filter.Stato))
        //        {
        //            titoloFiltro = "Stato: " + filter.Stato;
        //        }
        //        else
        //        {
        //            titoloFiltro = "Stato: tutti";
        //        }
        //        if (!String.IsNullOrWhiteSpace(filter.Stato))
        //        {
        //            worksheet.Cell(k, 1).SetValue(titoloFiltro);
        //            k++;
        //        }
        //        k++;

        //        // Caption
        //        worksheet.Cell(k, 1).SetValue("ID");
        //        worksheet.Cell(k, 1).Style.Font.Bold = true;

        //        worksheet.Cell(k, 2).SetValue("Data controllo");
        //        worksheet.Cell(k, 2).Style.Font.Bold = true;

        //        worksheet.Cell(k, 3).SetValue("Centro di costo");
        //        worksheet.Cell(k, 3).Style.Font.Bold = true;

        //        worksheet.Cell(k, 4).SetValue("Tipo");
        //        worksheet.Cell(k, 4).Style.Font.Bold = true;

        //        worksheet.Cell(k, 5).SetValue("Materiale");
        //        worksheet.Cell(k, 5).Style.Font.Bold = true;

        //        worksheet.Cell(k, 6).SetValue("UM");
        //        worksheet.Cell(k, 6).Style.Font.Bold = true;

        //        worksheet.Cell(k, 7).SetValue("Quantità");
        //        worksheet.Cell(k, 7).Style.Font.Bold = true;

        //        worksheet.Cell(k, 8).SetValue("Data scadenza materiale");
        //        worksheet.Cell(k, 8).Style.Font.Bold = true;

        //        worksheet.Cell(k, 9).SetValue("Lotto");
        //        worksheet.Cell(k, 9).Style.Font.Bold = true;

        //        worksheet.Cell(k, 10).SetValue("Compilatore");
        //        worksheet.Cell(k, 10).Style.Font.Bold = true;

        //        worksheet.Cell(k, 11).SetValue("Qualifica");
        //        worksheet.Cell(k, 11).Style.Font.Bold = true;

        //        worksheet.Cell(k, 12).SetValue("Note");
        //        worksheet.Cell(k, 12).Style.Font.Bold = true;

        //        /*
        //        */
        //        k++;
        //        foreach (MatScaduto lst in liste)
        //        {
        //            worksheet.Cell(k, 1).SetValue(lst.ID);
        //            worksheet.Cell(k, 2).SetValue(lst.DataControllo);
        //            worksheet.Cell(k, 2).SetDataType(XLDataType.DateTime);
        //            worksheet.Cell(k, 3).SetValue(lst.IDCdc + " " + lst.Cdc);
        //            worksheet.Cell(k, 3).SetDataType(XLDataType.Text);

        //            worksheet.Cell(k, 4).SetValue(lst.IDTipoMateriale + " " + lst.DesTipoMateriale);
        //            worksheet.Cell(k, 4).SetDataType(XLDataType.Text);
        //            worksheet.Cell(k, 5).SetValue(lst.IDTipoMateriale + " " + lst.DesMateriale);
        //            worksheet.Cell(k, 6).SetValue(lst.Um);

        //            worksheet.Cell(k, 7).SetValue(lst.Quantita);
        //            worksheet.Cell(k, 7).SetDataType(XLDataType.Number);
        //            worksheet.Cell(k, 8).SetValue(lst.DataScadenzaMateriale);
        //            worksheet.Cell(k, 8).SetDataType(XLDataType.DateTime);
        //            worksheet.Cell(k, 9).SetValue(lst.Lotto);
        //            worksheet.Cell(k, 10).SetValue(lst.Compilatore);
        //            worksheet.Cell(k, 11).SetValue(lst.QualificaCompilatore);
        //            worksheet.Cell(k, 12).SetValue(lst.Note);
        //            k++;
        //        }

        //        workbook.SaveAs(targetFile);
        //        var byteArray = File.ReadAllBytes(targetFile);
        //        MemoryStream streamOut = new MemoryStream(byteArray);
        //        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
        //        response.Content = new StreamContent(streamOut);
        //        // application/vnd.ms-excel
        //        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        //        // response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        //        response.Content.Headers.Add("content-disposition", "attachment; filename=\"Richieste.xlsx\"");
        //        // response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
        //        //{
        //        //  FileName = "Elaborazione.xlsx"
        //        //};
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.logError("Errore in creazione file Excel", ex);
        //        throw ex;
        //    }
        //}

    }
}
