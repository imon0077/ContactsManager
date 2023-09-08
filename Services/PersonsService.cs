using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        public PersonsService(bool initialize = true)
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();
            if (initialize)
            {
                _persons.AddRange(new List<Person>() { 
                    new Person() {PersonID = Guid.Parse("CEA7610A-8D2D-4867-B2E6-D863BD41C5B3"), PersonName = "Imon Islam", Address = "Ctg", Email = "imon@email.com", CountryID = Guid.Parse("6AF61BFD-D839-400C-91DC-F4DC231F420E"), Gender = "Male", DateOfBirth = DateTime.Parse("1990-01-05"), ReceiveNewsLetters = true},

                    new Person() {PersonID = Guid.Parse("13F73876-AE6A-4530-9482-DBE272F66300"), PersonName = "John Doe", Address = "Era Island", Email = "john@email.com", CountryID = Guid.Parse("18C77678-FB49-447D-9DC0-42442DADD11A"), Gender = "Male", DateOfBirth = DateTime.Parse("1995-05-05"), ReceiveNewsLetters = true},

                    new Person() {PersonID = Guid.Parse("26DE0B87-C73A-4422-B458-518E02476E91"), PersonName = "Shem Tov", Address = "Northern America", Email = "shem@email.com", CountryID = Guid.Parse("6AF61BFD-D839-400C-91DC-F4DC231F420E"), Gender = "Female", DateOfBirth = DateTime.Parse("2000-07-01"), ReceiveNewsLetters = false},

                    new Person() {PersonID = Guid.Parse("A397C3C5-A7AE-4AFB-AFC3-6F36C3D262AD"), PersonName = "Main Uddin", Address = "Feni", Email = "main@email.com", CountryID = Guid.Parse("FAFF4812-127B-45C2-9B77-E3972F61D434"), Gender = "Male", DateOfBirth = DateTime.Parse("1999-03-09"), ReceiveNewsLetters = true},

                    new Person() {PersonID = Guid.Parse("04AF0A9D-08F4-4542-9D32-2829DF05EE51"), PersonName = "Christopher", Address = "Era Island", Email = "ch@email.com", CountryID = Guid.Parse("18C77678-FB49-447D-9DC0-42442DADD11A"), Gender = "Male", DateOfBirth = DateTime.Parse("1988-09-01"), ReceiveNewsLetters = true},

                    new Person() {PersonID = Guid.Parse("5B89D3F5-37B4-452C-8230-4E1DB9A9E810"), PersonName = "Lowand", Address = "Saudi Arabia", Email = "lowand@email.com", CountryID = Guid.Parse("6AF61BFD-D839-400C-91DC-F4DC231F420E"), Gender = "Female", DateOfBirth = DateTime.Parse("2003-04-02"), ReceiveNewsLetters = false},

                    new Person() {PersonID = Guid.Parse("AF4A1979-BABF-4696-9199-F19F7D8C4D87"), PersonName = "Chan Soe", Address = "Era Island", Email = "chan@email.com", CountryID = Guid.Parse("18C77678-FB49-447D-9DC0-42442DADD11A"), Gender = "Male", DateOfBirth = DateTime.Parse("1995-05-05"), ReceiveNewsLetters = true},

                    new Person() {PersonID = Guid.Parse("7B85C697-787F-4D29-96BB-45370B0A7361"), PersonName = "Shein Loe", Address = "Northern America", Email = "Shein@email.com", CountryID = Guid.Parse("6AF61BFD-D839-400C-91DC-F4DC231F420E"), Gender = "Female", DateOfBirth = DateTime.Parse("2000-07-01"), ReceiveNewsLetters = false},

                    new Person() {PersonID = Guid.Parse("8D71340A-318C-4E5D-A777-9808135A8E59"), PersonName = "Zain Lee", Address = "Zaniaba", Email = "zain@email.com", CountryID = Guid.Parse("FAFF4812-127B-45C2-9B77-E3972F61D434"), Gender = "Female", DateOfBirth = DateTime.Parse("1997-03-09"), ReceiveNewsLetters = true},

                    new Person() {PersonID = Guid.Parse("C381EBE7-BB0A-4D5B-A706-C0C5E2E3C943"), PersonName = "Christopher Losen", Address = "Era Island", Email = "chrr@email.com", CountryID = Guid.Parse("18C77678-FB49-447D-9DC0-42442DADD11A"), Gender = "Female", DateOfBirth = DateTime.Parse("1989-09-01"), ReceiveNewsLetters = true}
                });
            }
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
            return _persons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? PersonID)
        {
            if (PersonID == null)
                return null;

            Person? person =_persons.FirstOrDefault(temp => temp.PersonID == PersonID);

            if (person == null)
                return null;

            return person.ToPersonResponse();
        }

        public List<PersonResponse> GetFilteredPersons(string? searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if(string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(temp => 
                    (!string.IsNullOrEmpty(temp.PersonName) ? 
                    temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Email) ?
                    temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(temp =>
                    (temp.DateOfBirth != null) ?
                    temp.DateOfBirth.Value.ToString("dd MMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Gender) ?
                    temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.CountryID):
                    matchingPersons = allPersons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Country) ?
                    temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Address) ?
                    temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                default: matchingPersons = allPersons; break;
            }
            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if(string.IsNullOrEmpty(sortBy)) return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder)
            switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),


                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

                _ => allPersons 
            };
            return sortedPersons;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            //check if object is null
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));

            //Model Validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            //Get matching person object to update
            Person? matchingPerson = _persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);
            if(matchingPerson == null)
            {
                throw new ArgumentException("Given PersonID is invalid");
            }

            //Update all details
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            return matchingPerson.ToPersonResponse();
        }

        public bool DeletePerson(Guid? personID)
        {
            if(personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }

            Person? person = _persons.FirstOrDefault(temp => temp.PersonID == personID);

            if (person == null)
                return false;

            _persons.RemoveAll(temp => temp.PersonID == personID);

            return true;
        }
    }
}
