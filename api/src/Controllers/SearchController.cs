using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SearchApi.Repositories;
using Microsoft.Extensions.Logging;

namespace SearchApi.Controllers
{
    /// <summary>
    /// SearchController provides searching functionality for the main results page.
    /// </summary>
    [Route("search")]
    public class SearchController : BaseController
    {
        private IMciRepository _mciRepository { get; set; }

        public SearchController(IMciRepository mciRepository, ILogger<SearchController> logger) : base(logger)
        {
            _mciRepository = mciRepository;
        }

        /// <summary>
        /// Connect to the ESB mci endpoint, query by first name, last name, or registration.
        /// </summary>
        /// <remarks>
        /// The result returned from the ESB endpoint: /mci/person/search/
        ///
        /// On invalid parameters, returns a json result listing errors.
        /// Text fields support Exact, SoundEx, and Synonyms.
        /// </remarks>
        /// <param name="firstName">
        /// The first name of the individual.
        /// </param>
        /// <param name="lastName">
        /// The last name of the individual.
        /// </param>
        /// <param name="registration">
        /// The SSN, EIS Client ID, or ARIES Client ID of the individual.
        /// </param>
        /// <response code="200">
        /// { success = true, data = [ virtualId, matchPercentage, title, firstName, middleName, lastName,
        /// suffix, dateOfBirth, gender, registrations, names, formattedDateOfBirth ] }
        /// </response>
        /// <response code="400">
        /// { success = true, data = errorMessage }
        /// </response>
        [HttpGet("getmci")]
        public async Task<IActionResult> GetMci(string firstName, string lastName, string registration)
        {
            var invalidInput = _mciRepository.ValidateMciInput(firstName, lastName, registration);

            if (invalidInput != null)
            {
                return BadRequest(invalidInput);
            }

            var result = await _mciRepository.GetMci(firstName, lastName, registration);

            if (result == null)
            {
                return Ok();
            }

            return Ok(result);
        }
    }
}
