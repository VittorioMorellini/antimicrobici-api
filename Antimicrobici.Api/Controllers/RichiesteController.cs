using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Antimicrobici.Api.Models;
using Antimicrobici.Core;
using Antimicrobici.Core.Filters;
using Antimicrobici.Core.Models;
using Antimicrobici.Core.Services;
using Antimicrobici.Core.Utils;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Antimicrobici.Api.Controllers
{
    // [Authorize]
    [EnableCors()]
    [Route("richieste")]
    public class RichiesteController : ControllerBase
    {
        private readonly IRichiestaService service;
        private readonly ILogger<IRichiestaService> logger;
        public RichiesteController(IRichiestaService service, ILogger<IRichiestaService> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        // GET: api/ListeAttesa
        [HttpGet]
        public Result<RichiestaRecord> Get(Int32? count = 0, string codiceRichiedente = "", string codiceMateriale = "", string medico = "", string motivazione = "",
            string destinatario = "", String dataDa = "", String dataA = "", string stato = "", string statoMedico = "", Boolean isInfettivologo = false,
            Int32? offset = 0, string sortBy = "", Int32? sortDir = 0)
        {
            #region DECLARATION
            List<RichiestaRecord> listeResult = new List<RichiestaRecord>();
            RichiestaFilter filter = new RichiestaFilter();
            Int32 total = 0;
            bool alertKey = false;
            #endregion

            #region SET FILTER
            filter.CodiceRichiedente = codiceRichiedente;
            filter.CodiceMateriale = codiceMateriale;
            filter.Medico = medico;
            filter.Motivazione = motivazione;
            filter.Destinatario = destinatario;
            filter.StatoMedico = statoMedico;
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
            String username = "siamorellini";
            List<Richiesta> liste = service.GetRichieste(filter, username, out total);
            foreach (Richiesta lst in liste)
            {
                RichiestaRecord item = new RichiestaRecord();
                item.NrImpegno = lst.NrImpegno;
                item.Quantity = lst.Quantita;
                item.QtaRichiesta = lst.QtaRichiesta;
                item.CdcRichiedente = new NamedEntity();
                item.CdcRichiedente.Codice = lst.CdcRichiedente.Codice;
                item.CdcRichiedente.Nome = lst.CdcRichiedente.Nome;
                item.TipoImpegno = lst.TipoImpegno;
                item.DataErogazione = lst.DataErogazione;
                item.DataCreazioneImpegno = lst.DataCreazioneImpegno;
                item.CodiceMateriale = lst.CodiceMateriale;
                item.Motivazione = lst.Motivazione;
                item.Stato = lst.Stato;
                item.StatoMedico = lst.StatoMedico;
                item.InserimentoData = lst.InserimentoData;
                item.DescrizioneMateriale = lst.DescrizioneMateriale;
                item.MedicoRichiedente = lst.MedicoRichiedente;
                item.DestinatarioPazienteDettaglio = lst.DestinatarioPazienteDettaglio;
                item.Note = lst.Note;
                item.NoteMedico = lst.NoteMedico;
                item.MedicoUO = lst.MedicoUO;
                item.ConsulenzaInfettivologica = lst.ConsulenzaInfettivologica;
                item.Antibiogramma = lst.Antibiogramma;
                item.Posologia = lst.Posologia;
                item.OffLabel = lst.OffLabel;
                item.Alert = lst.Alert;
                if (item.Alert)
                {
                    item.ClassAlert = "red-icon td-icon";
                    alertKey = true;
                }
                if (item.Stato == 0)
                {
                    // e.Item.ImageIndex = 0; // blu chiaro -> inserimento Ok ma aggiornamento con dati obbligatori mancanti
                    item.ClassIcon = "yellow-icon td-icon";
                }
                else if (item.Stato == 1)
                    item.ClassIcon = "green-icon td-icon";
                if (item.StatoMedico == 0)
                {
                    // e.Item.ImageIndex = 0; // blu chiaro -> inserimento Ok ma aggiornamento con dati obbligatori mancanti
                    item.ClassIconMedico = "yellow-icon td-icon";
                }
                else if (item.StatoMedico == 1)
                    item.ClassIconMedico = "green-icon td-icon";

                listeResult.Add(item);
            }
            return new Result<RichiestaRecord>(total, listeResult, alertKey);
        }

        //[Route("api/richieste/excel")]
        //[HttpGet]
        //public HttpResponseMessage GetExcel(Int32? count = 0, string codiceRichiedente = "", string codiceMateriale = "", string medico = "", string motivazione = "",
        //  string destinatario = "", String dataDa = "", String dataA = "", string stato = "", string statoMedico = "", Boolean isInfettivologo = false, Int32? offset = 0, string sortBy = "", Int32? sortDir = 0)
        //{
        //    List<RichiestaRecord> listeResult = new List<RichiestaRecord>();
        //    RichiestaFilter filter = new RichiestaFilter();
        //    Int32 total = 0;
        //    int k = 5;

        //    #region SET FILTER
        //    filter.CodiceRichiedente = codiceRichiedente;
        //    filter.CodiceMateriale = codiceMateriale;
        //    filter.Medico = medico;
        //    filter.Motivazione = motivazione;
        //    filter.Destinatario = destinatario;
        //    filter.StatoMedico = statoMedico;
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
        //    String username = "siamorellini";
        //    try
        //    {
        //        List<Richiesta> liste = service.GetRichieste(filter, username, out total, false);
        //        XLWorkbook workbook = new XLWorkbook();
        //        var worksheet = workbook.Worksheets.Add("Richieste");
        //        String targetFile = System.Web.Hosting.HostingEnvironment.MapPath("/App_Data/xls/Richieste.xlsx");
        //        worksheet.Cell(1, 1).Style.Font.Bold = true;
        //        worksheet.Cell(1, 1).Style.Font.FontSize = 18;
        //        worksheet.Cell(1, 1).SetValue("Stampa situazione richieste");
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
        //        if (!String.IsNullOrWhiteSpace(filter.CodiceMateriale))
        //        {
        //            titoloFiltro = "Farmaco: " + MaterialiDAL.GetMateriale(filter.CodiceMateriale).nome;
        //        }
        //        else
        //        {
        //            titoloFiltro = "Farmaco: tutti";
        //        }
        //        if (!String.IsNullOrWhiteSpace(filter.CodiceMateriale))
        //        {
        //            worksheet.Cell(k, 1).SetValue(titoloFiltro);
        //            k++;
        //        }

        //        if (!String.IsNullOrWhiteSpace(filter.Medico))
        //        {
        //            titoloFiltro = "Medico: " + filter.Medico;
        //        }
        //        else
        //        {
        //            titoloFiltro = "Medico: tutti";
        //        }
        //        if (!String.IsNullOrWhiteSpace(filter.Medico))
        //        {
        //            worksheet.Cell(k, 1).SetValue(titoloFiltro);
        //            k++;
        //        }

        //        if (!String.IsNullOrWhiteSpace(filter.StatoMedico))
        //        {
        //            titoloFiltro = "Stato medico: " + filter.StatoMedico;
        //        }
        //        else
        //        {
        //            titoloFiltro = "Stato medico: tutti";
        //        }
        //        if (!String.IsNullOrWhiteSpace(filter.StatoMedico))
        //        {
        //            worksheet.Cell(k, 1).SetValue(titoloFiltro);
        //            k++;
        //        }

        //        if (!String.IsNullOrWhiteSpace(filter.Motivazione))
        //        {
        //            titoloFiltro = "Motivazione: " + filter.Motivazione;
        //        }
        //        else
        //        {
        //            titoloFiltro = "Motivazione: tutte";
        //        }
        //        if (!String.IsNullOrWhiteSpace(filter.Motivazione))
        //        {
        //            worksheet.Cell(k, 1).SetValue(titoloFiltro);
        //            k++;
        //        }

        //        if (!String.IsNullOrWhiteSpace(filter.Destinatario))
        //        {
        //            titoloFiltro = "Paziente: " + filter.Destinatario;
        //        }
        //        else
        //        {
        //            titoloFiltro = "Paziente: tutti";
        //        }
        //        if (!String.IsNullOrWhiteSpace(filter.Destinatario))
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
        //        worksheet.Cell(k, 1).SetValue("Nr.Impegno");
        //        worksheet.Cell(k, 1).Style.Font.Bold = true;

        //        worksheet.Cell(k, 2).SetValue("CDC Richiedente");
        //        worksheet.Cell(k, 2).Style.Font.Bold = true;

        //        worksheet.Cell(k, 3).SetValue("Data impegno");
        //        worksheet.Cell(k, 3).Style.Font.Bold = true;

        //        worksheet.Cell(k, 4).SetValue("Farmaco");
        //        worksheet.Cell(k, 4).Style.Font.Bold = true;

        //        worksheet.Cell(k, 5).SetValue("Paziente");
        //        worksheet.Cell(k, 5).Style.Font.Bold = true;

        //        worksheet.Cell(k, 6).SetValue("Diagnosi");
        //        worksheet.Cell(k, 6).Style.Font.Bold = true;

        //        worksheet.Cell(k, 7).SetValue("Posologia");
        //        worksheet.Cell(k, 7).Style.Font.Bold = true;

        //        worksheet.Cell(k, 8).SetValue("Medico richiedente");
        //        worksheet.Cell(k, 8).Style.Font.Bold = true;

        //        worksheet.Cell(k, 9).SetValue("Consul.Infettivologo");
        //        worksheet.Cell(k, 9).Style.Font.Bold = true;

        //        worksheet.Cell(k, 10).SetValue("Medico U.O.");
        //        worksheet.Cell(k, 10).Style.Font.Bold = true;

        //        worksheet.Cell(k, 11).SetValue("ABC");
        //        worksheet.Cell(k, 11).Style.Font.Bold = true;

        //        worksheet.Cell(k, 12).SetValue("Qta richiesta");
        //        worksheet.Cell(k, 12).Style.Font.Bold = true;

        //        worksheet.Cell(k, 13).SetValue("Qta Consegnata");
        //        worksheet.Cell(k, 13).Style.Font.Bold = true;

        //        worksheet.Cell(k, 14).SetValue("Data inserimento ");
        //        worksheet.Cell(k, 14).Style.Font.Bold = true;

        //        worksheet.Cell(k, 15).SetValue("Off Label");
        //        worksheet.Cell(k, 15).Style.Font.Bold = true;

        //        worksheet.Cell(k, 16).SetValue("Stato Farm");
        //        worksheet.Cell(k, 16).Style.Font.Bold = true;

        //        worksheet.Cell(k, 17).SetValue("Note Farm");
        //        worksheet.Cell(k, 17).Style.Font.Bold = true;

        //        worksheet.Cell(k, 18).SetValue("Stato Infettiv");
        //        worksheet.Cell(k, 18).Style.Font.Bold = true;

        //        worksheet.Cell(k, 19).SetValue("Note Infettiv");
        //        worksheet.Cell(k, 19).Style.Font.Bold = true;
        //        /*
        //        */
        //        k++;
        //        foreach (Richiesta lst in liste)
        //        {
        //            worksheet.Cell(k, 1).SetValue(lst.NrImpegno);
        //            worksheet.Cell(k, 2).SetValue(lst.CdcRichiedente.codice + " " + lst.CdcRichiedente.nome);
        //            worksheet.Cell(k, 2).SetDataType(XLDataType.Text);
        //            worksheet.Cell(k, 3).SetValue(lst.DataCreazioneImpegno);
        //            worksheet.Cell(k, 3).SetDataType(XLDataType.DateTime);

        //            worksheet.Cell(k, 4).SetValue(lst.CodiceMateriale + " " + lst.DescrizioneMateriale);
        //            worksheet.Cell(k, 4).SetDataType(XLDataType.Text);
        //            worksheet.Cell(k, 5).SetValue(lst.DestinatarioPazienteDettaglio);
        //            worksheet.Cell(k, 6).SetValue(lst.Motivazione);

        //            worksheet.Cell(k, 7).SetValue(lst.Posologia);
        //            // worksheet.Cell(k, 8).SetDataType(XLDataType.DateTime);
        //            worksheet.Cell(k, 8).SetValue(lst.MedicoRichiedente);
        //            worksheet.Cell(k, 9).SetValue(lst.ConsulenzaInfettivologica);
        //            worksheet.Cell(k, 10).SetValue(lst.MedicoUO);
        //            worksheet.Cell(k, 11).SetValue(lst.Antibiogramma);
        //            worksheet.Cell(k, 12).SetValue(lst.QtaRichiesta);
        //            worksheet.Cell(k, 12).SetDataType(XLDataType.Number);
        //            worksheet.Cell(k, 13).SetValue(lst.Quantita);
        //            worksheet.Cell(k, 13).SetDataType(XLDataType.Number);
        //            worksheet.Cell(k, 14).SetValue(lst.InserimentoData);
        //            worksheet.Cell(k, 14).SetDataType(XLDataType.DateTime);
        //            worksheet.Cell(k, 15).SetValue(lst.OffLabel);
        //            worksheet.Cell(k, 16).SetValue(lst.Stato == 1 ? "true" : "false");
        //            worksheet.Cell(k, 17).SetValue(lst.Note);
        //            worksheet.Cell(k, 17).SetDataType(XLDataType.Text);
        //            worksheet.Cell(k, 18).SetValue(lst.StatoMedico == 1 ? "true" : "false");
        //            worksheet.Cell(k, 19).SetValue(lst.NoteMedico);
        //            worksheet.Cell(k, 19).SetDataType(XLDataType.Text);
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



        // POST: api/richieste
        [Route("api/richieste")]
        [HttpPost]
        public RichiestaRecord Post([FromBody] RichiestaRecord value)
        {
            // Log.logInfo("Sono nel Post Controller");
            String username = "siamorellini";
            Richiesta item = new Richiesta();
            RichiestaRecord ritorno = new RichiestaRecord();
            try
            {
                if (value == null)
                {
                    // Log.logInfo("Il body è null, value null");
                }

                item.NrImpegno = value.NrImpegno;
                // Log.logInfo("Importato in Numero Impegno");
                item.Quantita = value.Quantity;
                item.Note = value.Note;
                item.Stato = 1;
                item.OffLabel = value.OffLabel;
                Delivery result = service.UpdateQuantita(item, username);
                ritorno.InserimentoData = result.InsertDate;
                ritorno.Quantity = result.Quantity;
                ritorno.Note = result.Notes;
                ritorno.NrImpegno = result.NrImpegno;
            }
            catch (Exception ex)
            {
                logger.LogError("Errore in update post ", ex);
                throw;
            }
            return ritorno;
        }

    }
}
