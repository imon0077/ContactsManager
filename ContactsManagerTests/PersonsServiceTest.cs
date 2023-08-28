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
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Canada"};
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
    }
}
