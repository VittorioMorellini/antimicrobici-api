using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Antimicrobici.Core.Classes;
//using AntimicrobiciUtil.Helpers;
//using System.DirectoryServices;
//using System.DirectoryServices.AccountManagement;
//using Antimicrobici.Api.Utils;
using Microsoft.OpenApi.Services;
using Serilog;
using System.Net.Mail;
using System.Reflection.PortableExecutable;

namespace Antimicrobici.Api.Helpers
{
    public class Authentication
    {
        private readonly IHttpContextAccessor context;

        public Authentication(IHttpContextAccessor context)
        {
            this.context = context;
        }
        public String GetWindowsAuthenticated()
        {
            String userID = context.HttpContext?.User.Identity.Name;
            //Log.logInfo("Utente è: " + userID);
            //bool isDebug = System.Configuration.ConfigurationManager.AppSettings[Constants.WebConfigConstants.isDebug].ToString() == "1" ? true : false;
            bool isDebug = true;
            if (string.IsNullOrWhiteSpace(userID) && isDebug)
            {
                // userID = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                userID = String.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName);
            }

            if (!String.IsNullOrWhiteSpace(userID))
            {
                if (false /*!UtentiDAL.IsUserAuthorized(userID)*/)
                {
                    // Devo ritornare Forbidden 403
                    //Log.logError("User ID not authorized: " + userID);
                    throw new Exception("The resourse is Forbidden 403");
                }
                //Log.logInfo("User is Authorized: " + userID);
            }
            else if (String.IsNullOrWhiteSpace(userID))
            {
                System.Diagnostics.Debugger.Break();
                // userID = @"AZOSPFE\igconsulting1";
                //Log.logError("User ID NULL. Not recognized");
                throw new Exception("The resourse is Forbidden 403");
            }
            //Log.logInfo("User ID lo ritorno: " + userID);
            return userID;
        }

        /// <summary>
        /// Uses the directory services to determine the full user name for the given user,
        /// assumes the active identity has access to the domain directory services.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="domain">
        /// The user's domain.
        /// </param>
        /// <returns>
        /// The full user name if found.
        /// </returns>
        public static string GetFullUserName(string userName, string domain)
        {
            string fullName = null;

            DirectoryEntry entry = new DirectoryEntry(/*"LDAP://" + domain*/);
            //using (Directory searcher = new Directory(entry))
            //{
            //    searcher.Filter = string.Format("(&(objectClass=user)(objectCategory=person)(samAccountName={0}))", userName);
            //    SearchResult result = searcher.FindOne();

            //    if (null != result)
            //    {
            //        fullName = result.GetDirectoryEntry().Properties["displayName"].Value.ToString();
            //    }
            //}
            return fullName;
        }

    }
}