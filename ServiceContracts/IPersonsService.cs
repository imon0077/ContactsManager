
using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents Business Logic for manupulating Person entity
    /// </summary>
    public interface IPersonsService
    {
        /// <summary>
        /// Add a new person into the list of Persons
        /// </summary>
        /// <param name="personAddRequest">Person to add</param>
        /// <returns>Return same person details along with newly generated PersonID</returns>
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Return all persons
        /// </summary>
        /// <returns>Return a list of objects of PersonResponse type</returns>
        List<PersonResponse> GetAllPersons();

        /// <summary>
        /// Return the Person object based on given person id
        /// </summary>
        /// <param name="PersonID">Person Id to search</param>
        /// <returns>Return matching Person object</returns>
        PersonResponse? GetPersonByPersonID(Guid? PersonID);
    }
}
