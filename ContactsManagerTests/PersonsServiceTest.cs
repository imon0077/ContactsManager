using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;

namespace ContactsManagerTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        public PersonsServiceTest()
        {
            _personsService = new PersonsService();
            _countriesService = new CountriesService();
        }

        #region AddPerson
        [Fact]
        public void AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? request = null;

            //Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                _personsService.AddPerson(request);
            });
        }

        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest request = new PersonAddRequest { PersonName = null };

            //Act
            Assert.Throws<ArgumentException>(() =>
            {
                _personsService.AddPerson(request);
            });
        }

        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest request = new PersonAddRequest
            {
                PersonName = "Person name...",
                Email = "person@email.com",
                Address = "Sample Address ",
                CountryID = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };

            //Act
            PersonResponse personResponse = _personsService.AddPerson(request);

            List<PersonResponse> person_list = _personsService.GetAllPersons();

            //Assert
            Assert.True(personResponse.PersonID != Guid.Empty);

            Assert.Contains(personResponse, person_list);
        }

        #endregion

        #region GetPersonByPersonID

        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? PersonID = null;

            //Act
            PersonResponse? personResponse_from_get = _personsService.GetPersonByPersonID(PersonID);

            //Assert
            Assert.Null(personResponse_from_get);

        }

        [Fact]
        public void GetPersonByPersonID_WithPersonID()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

            //Act
            PersonAddRequest request = new()
            {
                PersonName = "Person name...",
                Email = "person@email.com",
                Address = "Sample Address ",
                CountryID = countryResponse.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };

            PersonResponse personResponse_from_add = _personsService.AddPerson(request);

            PersonResponse? personResponse_from_get = _personsService.GetPersonByPersonID(personResponse_from_add.PersonID);

            //Assert
            Assert.Equal(personResponse_from_add, personResponse_from_get);
        }

        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by defualt
        [Fact]
        public void GetAllPersons_EmptyList()
        {
            //Act
            List<PersonResponse> personResponses = _personsService.GetAllPersons();

            //Assert
            Assert.Empty(personResponses);
        }

        //First, when we add some persons, it should return all
        [Fact]
        public void GetAllPersons_AddPersons()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Canada" };

            CountryResponse countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest() { PersonName = "Imon", Email = "imon@test.com", Address = "Ctg", CountryID = countryResponse1.CountryID, Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("1998-07-03"), ReceiveNewsLetters = true };

            PersonAddRequest personAddRequest2 = new PersonAddRequest() { PersonName = "Mohsin", Email = "mohsin@test.com", Address = "Feni", CountryID = countryResponse1.CountryID, Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("1988-01-01"), ReceiveNewsLetters = true };

            PersonAddRequest personAddRequest3 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@test.com", Address = "CA", CountryID = countryResponse2.CountryID, Gender = GenderOptions.Female, DateOfBirth = DateTime.Parse("1990-02-09"), ReceiveNewsLetters = false };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };
            List<PersonResponse> personResponses = new List<PersonResponse>();

            foreach(PersonAddRequest request in personAddRequests)
            {
                PersonResponse personResponse = _personsService.AddPerson(request);
                personResponses.Add(personResponse);
            }

            //Act
            List<PersonResponse> persons_list_from_get = _personsService.GetAllPersons();

            //Assert
            foreach(PersonResponse person_response_from_add in personResponses)
            {
                Assert.Contains(person_response_from_add, persons_list_from_get);
            }

            #endregion


        }
    }
}