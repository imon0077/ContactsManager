using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        public PersonsService()
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();
        }

        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByCountryID(personResponse.CountryID)?.CountryName;
            return personResponse;
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            //check if personAddRequest is null
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest));

            ////check if person name is null
            //if (string.IsNullOrEmpty(personAddRequest.PersonName))
            //    throw new ArgumentException("Person name cant be blank");

            //Model Validation
            ValidationHelper.ModelValidation(personAddRequest);

            //convert PersonAddRequest into Person type
            Person person = personAddRequest.ToPerson();

            //Generate PersonID
            person.PersonID = Guid.NewGuid();

            //Add person to person list
            _persons.Add(person);

            //Convert the person object into PersonRespinse type
            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            throw new NotImplementedException();
        }

        public PersonResponse? GetPersonByPersonID(Guid? PersonID)
        {
            throw new NotImplementedException();
        }
    }
}
