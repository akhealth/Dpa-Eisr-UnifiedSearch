using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using SearchApi.Models;
using SearchApi.Repositories;
using Microsoft.Extensions.Logging;

namespace SearchApi.Controllers
{
    /// <summary>
    /// Provides data for the 'Person and Case Details' view. This includes the subjects personal information
    /// like id numbers, as well as information of cases and applications they are involved with.
    /// </summary>
    [Route("person")]
    public class PersonController : BaseController
    {
        private IAriesRepository _ariesRepository { get; set; }
        private IEisRepository _eisRepository { get; set; }
        private IMciRepository _mciRepository { get; set; }

        public PersonController(
            IAriesRepository ariesRepository,
            IEisRepository eisRepository,
            IMciRepository mciRepository,
            ILogger<PersonController> logger) : base(logger)
        {
            _ariesRepository = ariesRepository;
            _eisRepository = eisRepository;
            _mciRepository = mciRepository;
        }

        /// <summary>
        /// Retrieve the ARIES cases associated with a certain client.
        /// </summary>
        /// <param name="clientId">
        /// The ARIES ID of the client in question.
        /// </param>
        /// <response code="200">
        /// { success = true, data = [ location, caseNumber, clientId, programs, primaryIndividual ] }
        /// </response>
        /// <response code="400">
        /// { success = false, data = "Invalid client ID" }
        /// </response>
        [HttpGet("{clientId}/cases/aries")]
        public async Task<IActionResult> GetAriesCases(string clientId)
        {
            if (clientId == "" || clientId.Length != 10 || long.Parse(clientId) <= 0)
            {
                return BadRequest("Invalid client ID");
            }

            var cases = await _ariesRepository.GetFormattedAriesCases(clientId);

            if (cases == null)
            {
                return Ok();
            }
            return Ok(cases);
        }

        /// <summary>
        /// Retrieve the EIS cases associated with a client.
        /// </summary>
        /// <param name="clientId">
        /// The EIS ID of the client.
        /// </param>
        /// <response code="200">
        /// { success = true, data = [ location, caseNumber, clientId, programs, primaryIndividual ] }
        /// </response>
        /// <response code="400">
        /// { success = false, data = "Invalid client ID" }
        /// </response>
        [HttpGet("{clientId}/cases/eis")]
        public async Task<IActionResult> GetEisCases(string clientId)
        {
            if (clientId == "" || clientId.Length != 10 || long.Parse(clientId) <= 0)
            {
                return BadRequest("Invalid client ID");
            }

            var cases = await _eisRepository.GetFormattedEisCases(clientId);

            if (cases == null)
            {
                return Ok();
            }
            return Ok(cases);
        }

        /// <summary>
        /// Retrieves information on a person from the MCI, then retrieves all cases, case information,
        /// and applications that person is a part of in ARIES or EIS.
        /// </summary>
        /// <remarks>
        /// On an invalid input (invalid ssn, EIS, and ARIES ID) or internal error, returns error message.
        ///
        /// On valid input, returns information on a person and all cases and applications for them.
        /// Returns cases sorted in the following order:
        /// 1. Location (ARIES first, then EIS)
        /// 2. Cases that have programs without an Issuance
        /// 3. Cases that have programs with Issuance, ordered by Issuance Date descending
        /// 4. Cases that don't have any programs
        /// </remarks>
        /// <param name="registration">
        /// The EIS, ARIES, or SSN of the person to search for.
        /// </param>
        /// <response code="200">
        /// { success = true, data = [ personalInfo, systemInfo, applications, cases ] }
        /// </response>
        /// <response code="400">
        /// { success = false, data = errorMessage }
        /// </response>
        [HttpGet("{registration}")]
        public async Task<IActionResult> GetPersonDetails(string registration)
        {
            var invalidInput = _mciRepository.ValidateMciInput(null, null, registration);

            if (invalidInput != null)
            {
                return BadRequest(invalidInput);
            }

            var response = await _mciRepository.GetMci(null, null, registration);

            if (response == null)
            {
                return Ok();
            }

            var person = response[0];

            var personalInfo = new
            {
                firstName = person.FirstName,
                name = person.FormattedName,
                ssns = person.Registrations.Registration
                    .Where(r => r.RegistrationName == "SSN")
                    .Select(r => r.RegistrationValue),
                dob = person.DateOfBirth.Date.ToShortDateString()
            };

            var systemInfo = new
            {
                eisClientIds = person.Registrations.Registration
                    .Where(r => r.RegistrationName == "EIS_ID")
                    .Select(r => r.RegistrationValue),
                ariesClientIds = person.Registrations.Registration
                    .Where(r => r.RegistrationName == "ARIES_ID")
                    .Select(r => r.RegistrationValue)
            };

            var concurrentCases = new ConcurrentBag<CaseModel>();
            var cases = new List<CaseModel>();
            var concurrentApps = new ConcurrentBag<ApplicationModel>();
            var applications = new List<ApplicationModel>();

            var caseTasks = new List<Task>();
            var secondaryTasks = new List<Task>();

            // Get ARIES cases
            foreach (var ariesClientId in systemInfo.ariesClientIds)
            {
                caseTasks.Add(_ariesRepository.GetFormattedAriesCases(ariesClientId).ContinueWith(ariesCasesResult =>
                {
                    if (ariesCasesResult.Result != null)
                    {
                        var ariesCases = ariesCasesResult.Result.ToList();
                        ariesCases.ForEach(c => concurrentCases.Add(c));

                        List<CaseModel> allAriesCases = concurrentCases.Where(c => c.Location.Equals("ARIES")).ToList();
                        for (int i = 0; i < allAriesCases.Count(); i++)
                        {
                            var ariesCase = allAriesCases[i];

                            secondaryTasks.Add(_ariesRepository.GetAriesCaseBenefits(ariesCase).ContinueWith(benefit =>
                            {
                                ariesCase = benefit.Result;
                            }));
                        }
                    }
                }));

                secondaryTasks.Add(_ariesRepository.GetAriesApplications(ariesClientId).ContinueWith(apps =>
                {
                    if (apps.Result != null)
                    {
                        var appList = apps.Result.ToList();
                        appList.ForEach(a => concurrentApps.Add(a));
                    }
                }));
            }

            // Get EIS cases
            foreach (var eisClientId in systemInfo.eisClientIds)
            {
                caseTasks.Add(_eisRepository.GetFormattedEisCases(eisClientId).ContinueWith(eisCasesResult =>
                {
                    if (eisCasesResult.Result != null)
                    {
                        var eisCases = eisCasesResult.Result.ToList();
                        eisCases.ForEach(c => concurrentCases.Add(c));

                        List<CaseModel> allEisCases = concurrentCases.Where(c => c.Location.Equals("EIS")).ToList();
                        for (int i = 0; i < allEisCases.Count(); i++)
                        {
                            var eisCase = allEisCases[i];

                            secondaryTasks.Add(_eisRepository.GetEisCaseBenefits(eisCase).ContinueWith(benefit =>
                            {
                                eisCase = benefit.Result;
                            }));
                        }
                    }
                }));
            }

            Task.WaitAll(caseTasks.ToArray());
            Task.WaitAll(secondaryTasks.ToArray());

            applications = concurrentApps.OrderByDescending(a => a.ReceivedDate).ToList();

            // Sort order:
            // 1. Location (ARIES first, then EIS)
            // 2. Cases that have programs without an Issuance
            // 3. Cases that have programs with Issuance, ordered by Issuance Date descending
            // 4. Cases that don't have any programs
            cases = concurrentCases.OrderBy(c => c.Location).ThenByDescending(c => c.Programs.Any() ?
                c.Programs.FirstOrDefault().Issuances.Any() ? 0 : 1
                : 0)
                .ThenByDescending(c => c.Programs.FirstOrDefault()?.Issuances?.FirstOrDefault()?.IssuanceDate).ToList();

            if (personalInfo == null && systemInfo == null && applications == null && cases == null)
            {
                return BadRequest();
            }

            return Ok(new
            {
                personalInfo,
                systemInfo,
                applications,
                cases
            });
        }
    }
}
