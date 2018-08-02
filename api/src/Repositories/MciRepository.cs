using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SearchApi.Clients;
using SearchApi.Models;

namespace SearchApi.Repositories
{
    public interface IMciRepository
    {
        Task<List<SearchResponsePerson>> GetMci(string firstName, string lastName, string registration);
        List<string> ValidateMciInput(string firstName, string lastName, string registration);
    }

    public class MciRepository : BaseRepository, IMciRepository
    {
        // These are placeholder values for the actual constraints,
        // will be replaced when available
        public static int FIRSTNAME_MAX_LENGTH = 32;
        public static int LASTNAME_MAX_LENGTH = 32;
        public static int REGISTRATION_MAX_LENGTH = 10;
        public static int CLIENTID_MAX_LENGTH = 10;
        public static string NAME_INVALID_CHAR_REGEX = @"[^A-Za-z\-\.\'\, ]";
        public static string CLIENTID_INVALID_CHAR_REGEX = @"[^0-9a-zA-Z]";
        /*
            Validate that 'SSN' must be 9 digit numeric.
            Validate that all digits are not the same.
            *SSN = AAA-BB-CCCC*
            > AAA cannot be '000' or '666'
            > AAA cannot be between 900-999
            > BB cannot be '00'
            > CCCC cannot be '0000'
        */
        public static string VALID_SSN_FORMAT = @"^(?!666|000|9\d{2})\d{3}(-?)(?!00)\d{2}\1(?!0{4})\d{4}$";
        // Aries Client IDs must start with 2 and be 10 digits long
        public static string VALID_ARIES_CLIENT_ID_FORMAT = @"2[0-9]{9}";
        // EIS Client IDs must start with 06 and be 10 digits long
        public static string VALID_EIS_CLIENT_ID_FORMAT = @"06[0-9]{8}";

        public MciRepository(IEsbClient esbClient) : base(esbClient) { }

        /// <summary>
        /// Retrieve the aries application information associated with a certain case.
        /// </summary>
        public async Task<List<SearchResponsePerson>> GetMci(string firstName, string lastName, string registration)
        {
            var response = await _esbClient.PostAsync<MciSearchResponse>("mci/person/search/", new
            {
                FirstName = firstName,
                LastName = lastName,
                Registration = registration
            });

            if (response == null || response.SearchResponsePerson == null)
            {
                return null;
            }

            return response.SearchResponsePerson.Where(p => p.Registrations.Registration
                .Exists(r => r.RegistrationName == "EIS_ID" || r.RegistrationName == "ARIES_ID")).ToList();
        }

        /// <summary>
        /// Validate input to the MCI endpoint
        /// </summary>
        /// <returns>
        /// On invalid input, returns json result with error messages.
        /// On valid input, returns null.
        /// </returns>
        public List<string> ValidateMciInput(string firstName, string lastName, string registration)
        {
            List<string> invalidMessages = new List<string>();

            // Test fields for exceeding length
            if (!string.IsNullOrEmpty(firstName) && firstName.Length > FIRSTNAME_MAX_LENGTH)
            {
                invalidMessages.Add(string.Concat("First Name must be less than ", (FIRSTNAME_MAX_LENGTH + 1), " characters long."));
            }
            if (!string.IsNullOrEmpty(lastName) && lastName.Length > LASTNAME_MAX_LENGTH)
            {
                invalidMessages.Add(string.Concat("Last Name must be less than ", LASTNAME_MAX_LENGTH + 1, " characters long."));
            }
            if (!string.IsNullOrEmpty(registration) && registration.Length > REGISTRATION_MAX_LENGTH)
            {
                invalidMessages.Add(string.Concat("Registration must be less than ", REGISTRATION_MAX_LENGTH + 1, " digits long."));
            }

            // Test name fields for invalid characters
            if (!string.IsNullOrEmpty(firstName) && Regex.IsMatch(firstName, NAME_INVALID_CHAR_REGEX))
            {
                invalidMessages.Add("First Name contains invalid character(s).");
            }
            if (!string.IsNullOrEmpty(lastName) && Regex.IsMatch(lastName, NAME_INVALID_CHAR_REGEX))
            {
                invalidMessages.Add("Last Name contains invalid character(s).");
            }

            // Test for presence of search criteria
            if (string.IsNullOrEmpty(registration) && string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(firstName))
            {
                invalidMessages.Add("Some search criteria must be specified.");
            }

            if (invalidMessages.Count > 0)
            {
                return invalidMessages;
            }

            return null;
        }
    }
}