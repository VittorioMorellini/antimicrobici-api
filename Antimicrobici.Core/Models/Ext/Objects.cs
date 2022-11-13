using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antimicrobici.Core.Models
{
    public class Role
    {
        public const string ADMIN = "ADMIN";    //Amministratore
        public const string MANAGER = "MANAGER";    //Capogruppo della Company
        public const string SALESMAN = "SALESMAN";  //VENDITORE  o Commerciale
        public const string OPERATOR = "OPERATOR";  //Centralino
        public const string CUSTOMER = "CUSTOMER";  //Cliente che vede i suoi ordini
        public const string LAWYER = "LAWYER";      //aVVOCATO CHE GESTISCE IL POST VENDITA
        //public const string LOGISTICA = "LOGISTICA";
    }

    public class CustomerTypes
    {
        public const string CORPORATE = "0";
        public const string PERSON = "1";
    }
    
    public class NotificationTypes
    {
        public const string MAIL = "M";
        public const string APP_MESSAGE = "A";
    }

    public class NotificationCategory
    {
        public const string SELF_CUSTOMER_MAIL = "SELF_CUSTOMER_MAIL"; // mail di ingaggio cliente per notificare una attività di validazione manuale della pratica DoCheck da parte di un operatore
    }

    public class Environments
    {
        public const string PRODUCTION = "PRODUCTION";
        public const string STAGING = "STAGING";
    }
    public class ResourceVisibility
    {
        public const string Undefined = "-1";
        public const string Personal = "0";
        public const string Agency = "1";
        public const string Company = "2";
    }

    public class BoradcastingType
    {
        public const string TvNazionale = "TVN";
        public const string TvLocale = "TVL";
        public const string Telemarketing = "TMK";
        public const string Giornale = "GRN";
        public const string WEB = "WEB";
    }

    public class PersonDetailModes
    {
        public const string STANDARD = "STANDARD";
        public const string EXTENDED = "EXTENDED";
    }

    public class InvestmentType
    {
        public const string FIXED = "FIXED";
        public const string ROYALTY = "ROYALTY";
    }
}
