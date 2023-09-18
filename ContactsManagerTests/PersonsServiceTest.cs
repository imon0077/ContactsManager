﻿using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactsManagerTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));

            _personsService = new PersonsService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options), _countriesService);
            
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? request = null;

            //Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personsService.AddPerson(request);
            });
        }

        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest request = new PersonAddRequest { PersonName = null };

            //Act
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
               await _personsService.AddPerson(request);
            });
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails()
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
            PersonResponse personResponse = await _personsService.AddPerson(request);

            List<PersonResponse> person_list = await _personsService.GetAllPersons();

            //Assert
            Assert.True(personResponse.PersonID != Guid.Empty);

            Assert.Contains(personResponse, person_list);
        }

        #endregion

        #region GetPersonByPersonID

        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? PersonID = null;

            //Act
            PersonResponse? personResponse_from_get = await _personsService.GetPersonByPersonID(PersonID);

            //Assert
            Assert.Null(personResponse_from_get);

        }

        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

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

            PersonResponse personResponse_from_add = await _personsService.AddPerson(request);

            PersonResponse? personResponse_from_get = await _personsService.GetPersonByPersonID(personResponse_from_add.PersonID);

            //Assert
            Assert.Equal(personResponse_from_add, personResponse_from_get);
        }

        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by defualt
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Act
            List<PersonResponse> personResponses = await _personsService.GetAllPersons();

            //Assert
            Assert.Empty(personResponses);
        }

        //First, when we add some persons, it should return all
        [Fact]
        public async Task GetAllPersons_AddPersons()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Canada" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest() { PersonName = "Imon", Email = "imon@test.com", Address = "Ctg", CountryID = countryResponse1.CountryID, Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("1998-07-03"), ReceiveNewsLetters = true };

            PersonAddRequest personAddRequest2 = new PersonAddRequest() { PersonName = "Mohsin", Email = "mohsin@test.com", Address = "Feni", CountryID = countryResponse1.CountryID, Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("1988-01-01"), ReceiveNewsLetters = true };

            PersonAddRequest personAddRequest3 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@test.com", Address = "CA", CountryID = countryResponse2.CountryID, Gender = GenderOptions.Female, DateOfBirth = DateTime.Parse("1990-02-09"), ReceiveNewsLetters = false };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };
            List<PersonResponse> personResponses = new List<PersonResponse>();

            foreach (PersonAddRequest request in personAddRequests)
            {
                PersonResponse personResponse = await _personsService.AddPerson(request);
                personResponses.Add(personResponse);
            }

            //print personResponses using ItestOutputHelper
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse item in personResponses)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }


            //Act
            List<PersonResponse> persons_list_from_get = await _personsService.GetAllPersons();

            //print persons_list_from_get using ItestOutputHelper
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse item in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in personResponses)
            {
                Assert.Contains(person_response_from_add, persons_list_from_get);
            }
        }

        #endregion GetAllPersons

        #region GetFilteredPersons

        //If search text is empty, its should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Canada" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest() { PersonName = "Imon", Email = "imon@test.com", Address = "Ctg", CountryID = countryResponse1.CountryID, Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("1998-07-03"), ReceiveNewsLetters = true };

            PersonAddRequest personAddRequest2 = new PersonAddRequest() { PersonName = "Mohsin", Email = "mohsin@test.com", Address = "Feni", CountryID = countryResponse1.CountryID, Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("1988-01-01"), ReceiveNewsLetters = true };

            PersonAddRequest personAddRequest3 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@test.com", Address = "CA", CountryID = countryResponse2.CountryID, Gender = GenderOptions.Female, DateOfBirth = DateTime.Parse("1990-02-09"), ReceiveNewsLetters = false };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };
            List<PersonResponse> personResponses = new List<PersonResponse>();

            foreach (PersonAddRequest request in personAddRequests)
            {
                PersonResponse personResponse = await _personsService.AddPerson(request);
                personResponses.Add(personResponse);
            }

            //print personResponses using ItestOutputHelper
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse item in personResponses)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }


            //Act
            List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            //print persons_list_from_get using ItestOutputHelper
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse item in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in personResponses)
            {
                Assert.Contains(person_response_from_add, persons_list_from_search);
            }
        }

        //we will add some persons, then we will search based on personame & search string, its should return all matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "Canada" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest() { PersonName = "Imon", Email = "imon@test.com", Address = "Ctg", CountryID = countryResponse1.CountryID, Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("1998-07-03"), ReceiveNewsLetters = true };

            PersonAddRequest personAddRequest2 = new PersonAddRequest() { PersonName = "Mohsin", Email = "mohsin@test.com", Address = "Feni", CountryID = countryResponse1.CountryID, Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("1988-01-01"), ReceiveNewsLetters = true };

            PersonAddRequest personAddRequest3 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@test.com", Address = "CA", CountryID = countryResponse2.CountryID, Gender = GenderOptions.Female, DateOfBirth = DateTime.Parse("1990-02-09"), ReceiveNewsLetters = false };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };
            List<PersonResponse> personResponses = new List<PersonResponse>();

            foreach (PersonAddRequest request in personAddRequests)
            {
                PersonResponse personResponse = await _personsService.AddPerson(request);
                personResponses.Add(personResponse);
            }

            //print personResponses using ItestOutputHelper
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse item in personResponses)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }


            //Act
            List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "mo");

            //print persons_list_from_get using ItestOutputHelper
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse item in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //Assert
            foreach (PersonResponse person_response_from_add in personResponses)
            {
                if(person_response_from_add.PersonName != null) { 
                    if(person_response_from_add.PersonName.Contains("mo", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response_from_add, persons_list_from_search);
                    }
                }
            }
        }

        #endregion GetFilteredPersons

        #region GetSortedPersons

        //When we sort based on person name in DESC, it should return person list in descending order
        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = new() { CountryName = "USA" };
            CountryAddRequest countryAddRequest2 = new() { CountryName = "Canada" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest() { PersonName = "Imon", Email = "imon@test.com", Address = "Ctg", CountryID = countryResponse1.CountryID, Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("1998-07-03"), ReceiveNewsLetters = true };

            PersonAddRequest personAddRequest2 = new PersonAddRequest() { PersonName = "Mohsin", Email = "mohsin@test.com", Address = "Feni", CountryID = countryResponse1.CountryID, Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("1988-01-01"), ReceiveNewsLetters = true };

            PersonAddRequest personAddRequest3 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@test.com", Address = "CA", CountryID = countryResponse2.CountryID, Gender = GenderOptions.Female, DateOfBirth = DateTime.Parse("1990-02-09"), ReceiveNewsLetters = false };

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };
            List<PersonResponse> personResponses = new List<PersonResponse>();

            foreach (PersonAddRequest request in personAddRequests)
            {
                PersonResponse personResponse = await _personsService.AddPerson(request);
                personResponses.Add(personResponse);
            }

            //print personResponses using ItestOutputHelper
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse item in personResponses)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            List<PersonResponse> allPersons =  await _personsService.GetAllPersons();

            //Act
            List<PersonResponse> persons_list_from_sort = await _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            //print persons_list_from_get using ItestOutputHelper
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse item in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            personResponses = personResponses.OrderByDescending(temp =>  temp.PersonName).ToList();

            //Assert
            for (int i = 0; i < personResponses.Count; i++)
            {
                Assert.Equal(personResponses[i], persons_list_from_sort[i]);
            }
        }

        #endregion GetSortedPersons

        #region UpdatePerson
        //when we supply PersonUpdateRequest as null, then it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personsService.UpdatePerson(personUpdateRequest);
            });            
        }

        //when we supply InvalidPersonID, then it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest() { PersonID = Guid.NewGuid() };

            //Act
            await Assert.ThrowsAsync<ArgumentException>(async () => {
               await _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        //When we supply PersonName as null, then it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest() { PersonName = "John", CountryID = countryResponse.CountryID, Gender = GenderOptions.Male, Email = "john@email.com", Address = "ctg", ReceiveNewsLetters = true };

            PersonResponse personResponse_from_add = await _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponse_from_add.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => {
                //Act
               await _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        //add a new person and try to update the person name & email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest() { PersonName = "John", CountryID = countryResponse.CountryID, Gender = GenderOptions.Male, Address = "Abc road", DateOfBirth = DateTime.Parse("2000-01-04"), Email = "abc@email.com", ReceiveNewsLetters = true };

            PersonResponse personResponse_from_add = await _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponse_from_add.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "Willium";
            personUpdateRequest.Email = "willium@email.com";

            //Act
            PersonResponse personResponse_from_update = await _personsService.UpdatePerson(personUpdateRequest);

            PersonResponse? personResponse_from_get = await _personsService.GetPersonByPersonID(personResponse_from_update.PersonID);

            //Assert 
            Assert.Equal(personResponse_from_get, personResponse_from_update);
        }

        #endregion UpdatePerson

        #region DeletePerson

        //if you supply valid PersonID then it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest() { PersonName = "Imon", Address = "CTG", CountryID = countryResponse.CountryID, Email = "imon@email.com", DateOfBirth = Convert.ToDateTime("2010-02-02"), Gender = GenderOptions.Male, ReceiveNewsLetters = true };
            PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);

            //Act
            bool isDeleted = await _personsService.DeletePerson(personResponse.PersonID);

            //Assert
            Assert.True(isDeleted);
        }

        //if you supply invalid PersonID then it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDeleted);
        }

        #endregion
    }
}