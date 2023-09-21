using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PersonsDbContext _db;

        public CountriesService(PersonsDbContext personsDbContext)
        {
            _db = personsDbContext;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation: ContryAddRequest parameter can't be null
            if(countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validation: CountrName can't be null
            if(countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //Validation: CountryName can't be duplicate
            if(await _db.Countries.CountAsync(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("Given country name already exists");
            }

            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            //Generate CountryID
            country.CountryID = Guid.NewGuid();

            //Add counrty object into _countries
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if(countryID == null)
                return null;

            Country? country_response_from_list = await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID ==  countryID);

            if (country_response_from_list == null)
                return null;

            return country_response_from_list.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcel(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countriesInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = excelWorksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(excelWorksheet.Cells[row, 1].Value);

                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;

                        if (_db.Countries.Where(temp => temp.CountryName == countryName).Count() == 0)
                        {
                            Country country = new Country() { CountryName = countryName };
                            _db.Countries.Add(country);
                            await _db.SaveChangesAsync();

                            countriesInserted++;
                        }
                    }
                }
            }
            return countriesInserted;
        }
    }
}