using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to a list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>Return the country object after adding it(incliding newly generated country id)</returns>
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);
    }
}