using Antimicrobici.Core.Models;
using Antimicrobici.Core.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antimicrobici.Core.Services
{
    public interface IUserProfileService
    {
        UserProfile GetProfile();
        //bool PrincipalnameExists(string principalname);
    }
    
    public class UserProfileService : IUserProfileService
    {
        private readonly ILogger<UserProfileService> logger;
        private readonly IPrincipalService principalService;
        public UserProfileService(ILogger<UserProfileService> logger, IPrincipalService principalService)
        {
            this.logger = logger;
            this.principalService = principalService;
        }
        
        public UserProfile GetProfile()
        {
            #region DECLARATION
            UserProfile result = new UserProfile();
            String userId = String.Empty;
            #endregion

            // Authentication
            try
            {
                userId = "siamorellini"; //Authentication.GetWindowsAuthenticated();
                result.id = userId;
                //int pos = userId.IndexOf('\\');
                String[] domainUser = new string[] {"Sixtema"};
                logger.LogInformation("Domani recuperato: " + domainUser);
                try
                {
                    result.descrizione = principalService.GetFullName(userId);
                }
                catch (Exception ex)
                {

                }
                result.landingPage = principalService.GetLandingPage(userId);
                //pos = userID.IndexOf('\\');
                //if (pos > -1)
                //    result.domain = userID.Substring(0, pos);
            }
            catch (Exception ex)
            {
                logger.LogError("Errore nel recupero della descrizione dell'utente", ex);
                throw;
            }

            return result;
        }

    }
}
