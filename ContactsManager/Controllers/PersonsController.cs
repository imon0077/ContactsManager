using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ContactsManager.Controllers
{
    public class PersonsController : Controller
    {
        //private fields
        private readonly IPersonsService _personsService;

        public PersonsController(IPersonsService personsService)
        {
            _personsService = personsService;
        }


        [Route("persons/index")]
        [Route("/")]
        public IActionResult Index(string? searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            //Search
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date Of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryID), "Country" },
                { nameof(PersonResponse.Address), "Address" }
            };
            List<PersonResponse> persons = _personsService.GetFilteredPersons(searchBy, searchString);

            List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            return View(sortedPersons);
        }
    }
}
