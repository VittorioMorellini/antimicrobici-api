using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Antimicrobici.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Antimicrobici.Core.Utils;

namespace Antimicrobici.Core.Services
{
    public interface IPrincipalService : IBaseService<Principal, long, AmDbContext>
    {
        IEnumerable<Principal> Search(PrincipalSearchModel model);
        bool PrincipalnameExists(string principalname);
        string GetFullName(string username);
        string GetLandingPage(string id);
    }

    public class PrincipalService : BaseService<Principal, long, AmDbContext>, IPrincipalService
    {
        private readonly ILogger<PrincipalService> logger;
        public PrincipalService(ILogger<PrincipalService> logger, AmDbContext ctx = null)
            : base(ctx)
        {
            this.logger = logger;
        }

        public override Principal Find(long id)
        {   
            return ctx.Principal
                .AsSplitQuery()
                .Where(x => x.Id == id).FirstOrDefault();
        }

        public string GetFullName(string username)
        {
            var principal = ctx.Principal
                .AsSplitQuery()
                .Where(x => x.Username == username).FirstOrDefault();

            return principal?.Name + " " + principal?.Surname; 
        }

        public IEnumerable<Principal> Search(PrincipalSearchModel model)
        {
            var query = ctx.Principal.AsQueryable();

            //if (model.CompanyId.HasValue)
            //    query = query.Where(x => x.CompanyId == model.CompanyId.GetValueOrDefault() /*|| x.CompanyId == null*/);

            if (!string.IsNullOrWhiteSpace(model.Surname))
                query = query.Where(x => x.Surname.StartsWith(model.Surname));
            if (!string.IsNullOrWhiteSpace(model.Name))
                query = query.Where(x => x.Name.StartsWith(model.Name));
            if (!string.IsNullOrWhiteSpace(model.Username))
                query = query.Where(x => x.Username.StartsWith(model.Username));
            if (!string.IsNullOrWhiteSpace(model.Role))
                query = query.Where(x => x.Role == model.Role);

            query = query.ApplyPaging(model);
            //if (model.GetAppointmentNumber.HasValue && model.GetAppointmentNumber.Value == true)
            //{
            //    foreach(var principal in principals)
            //    {
            //        principal.Appointments
            //    }
            //}
            return query.ToList();
        }

        
        public bool PrincipalnameExists(string username)
        {
            return ctx.Principal.Where(x => x.Username == username).Any();
        }

        public String GetLandingPage(string id)
        {
            String result = string.Empty;
            var principal = ctx.Principal.Where(x => x.Username == id).FirstOrDefault();
            result = principal != null && principal.LandingPage != null ? principal.LandingPage : String.Empty;
            return result;
        }
    }

    public class PrincipalSearchModel : BaseSearchModel
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Role { get; set; }
        public bool? Disabled { get; set; }
    }

    internal static class PasswordGenerator
    {
        /// <summary>
        /// Generates a random password based on the rules passed in the parameters
        /// </summary>
        /// <param name="includeLowercase">Bool to say if lowercase are required</param>
        /// <param name="includeUppercase">Bool to say if uppercase are required</param>
        /// <param name="includeNumeric">Bool to say if numerics are required</param>
        /// <param name="includeSpecial">Bool to say if special characters are required</param>
        /// <param name="includeSpaces">Bool to say if spaces are required</param>
        /// <param name="lengthOfPassword">Length of password required. Should be between 8 and 128</param>
        /// <returns></returns>
        public static string GeneratePassword(int lengthOfPassword, bool includeLowercase = true, bool includeUppercase = true, bool includeNumeric = true, bool includeSpecial = true, bool includeSpaces = false)
        {
            const int MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS = 2;
            const string LOWERCASE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";
            const string UPPERCASE_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string NUMERIC_CHARACTERS = "0123456789";
            const string SPECIAL_CHARACTERS = @"!#$%&*@\";
            const string SPACE_CHARACTER = " ";
            const int PASSWORD_LENGTH_MIN = 8;
            const int PASSWORD_LENGTH_MAX = 128;

            if (lengthOfPassword < PASSWORD_LENGTH_MIN || lengthOfPassword > PASSWORD_LENGTH_MAX)
            {
                return "Password length must be between 8 and 128.";
            }

            string characterSet = "";

            if (includeLowercase)
            {
                characterSet += LOWERCASE_CHARACTERS;
            }

            if (includeUppercase)
            {
                characterSet += UPPERCASE_CHARACTERS;
            }

            if (includeNumeric)
            {
                characterSet += NUMERIC_CHARACTERS;
            }

            if (includeSpecial)
            {
                characterSet += SPECIAL_CHARACTERS;
            }

            if (includeSpaces)
            {
                characterSet += SPACE_CHARACTER;
            }

            char[] password = new char[lengthOfPassword];
            int characterSetLength = characterSet.Length;

            System.Random random = new System.Random();
            for (int characterPosition = 0; characterPosition < lengthOfPassword; characterPosition++)
            {
                password[characterPosition] = characterSet[random.Next(characterSetLength - 1)];

                bool moreThanTwoIdenticalInARow =
                    characterPosition > MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS
                    && password[characterPosition] == password[characterPosition - 1]
                    && password[characterPosition - 1] == password[characterPosition - 2];

                if (moreThanTwoIdenticalInARow)
                {
                    characterPosition--;
                }
            }

            return string.Join(null, password);
        }

        /// <summary>
        /// Checks if the password created is valid
        /// </summary>
        /// <param name="includeLowercase">Bool to say if lowercase are required</param>
        /// <param name="includeUppercase">Bool to say if uppercase are required</param>
        /// <param name="includeNumeric">Bool to say if numerics are required</param>
        /// <param name="includeSpecial">Bool to say if special characters are required</param>
        /// <param name="includeSpaces">Bool to say if spaces are required</param>
        /// <param name="password">Generated password</param>
        /// <returns>True or False to say if the password is valid or not</returns>
        public static bool PasswordIsValid(bool includeLowercase, bool includeUppercase, bool includeNumeric, bool includeSpecial, bool includeSpaces, string password)
        {
            const string REGEX_LOWERCASE = @"[a-z]";
            const string REGEX_UPPERCASE = @"[A-Z]";
            const string REGEX_NUMERIC = @"[\d]";
            const string REGEX_SPECIAL = @"([!#$%&*@\\])+";
            const string REGEX_SPACE = @"([ ])+";

            bool lowerCaseIsValid = !includeLowercase || (includeLowercase && Regex.IsMatch(password, REGEX_LOWERCASE));
            bool upperCaseIsValid = !includeUppercase || (includeUppercase && Regex.IsMatch(password, REGEX_UPPERCASE));
            bool numericIsValid = !includeNumeric || (includeNumeric && Regex.IsMatch(password, REGEX_NUMERIC));
            bool symbolsAreValid = !includeSpecial || (includeSpecial && Regex.IsMatch(password, REGEX_SPECIAL));
            bool spacesAreValid = !includeSpaces || (includeSpaces && Regex.IsMatch(password, REGEX_SPACE));

            return lowerCaseIsValid && upperCaseIsValid && numericIsValid && symbolsAreValid && spacesAreValid;
        }
    }
}