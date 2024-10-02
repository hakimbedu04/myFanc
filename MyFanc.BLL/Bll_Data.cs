using Microsoft.EntityFrameworkCore;
using MyFanc.BusinessObjects;
using MyFanc.Contracts.Services;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.Persona;
using MyFanc.Services.FancRadApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static MyFanc.Core.Enums;

namespace MyFanc.BLL
{
    public partial class Bll
    {
        public async Task<IEnumerable<CountryDTO>> GetCountriesAsync()
        {
            const string cacheKey = "CountryList";
            var cacheCountryInfo = _sharedDataCache.GetData<List<CountryDTO>>(cacheKey);
            if (cacheCountryInfo != null && cacheCountryInfo.Any())
            {
                return cacheCountryInfo;
            }
            else
            {
                var countryList = await _fancRADApi.GetCountry();
                if (countryList != null)
                {
                    var countryDTO = _mapper.Map<List<CountryDTO>>(countryList);
                    if(countryDTO != null)
                    {
                        _sharedDataCache.SetData(cacheKey, countryList);
                        return countryDTO;
                    }
                    throw new InvalidOperationException("Failed to map country data to DTO.");
                    
                }
                throw new InvalidOperationException("Failed to retrieve the list of countries from RAD API.");
            }
        }


        public async Task<CountryDTO> GetCountriesCodeAsync(string countryCode) 
        {   
            var allCountries = await GetCountriesAsync();
            var countryDto = allCountries.FirstOrDefault(x=>x.ContinentCode == countryCode);
            if(countryDto != null)
            {
                return countryDto;
            }
            throw new InvalidOperationException("Failed to retrieve the list of countries from RAD API.");
            
        }

        public IEnumerable<SectorDTO> GetSectors(string languageCode, string nacabelCode, int pageSize)
        {
            return _nacabelHelper.GetSectors(languageCode, nacabelCode, pageSize);
        }

        public IEnumerable<NacabelsCodeDTO> GetMappedSectors(IEnumerable<string> nacabelCode, string languageCode, string cbeNumber)
        {
            return _nacabelHelper.GetMappedSectors(nacabelCode, languageCode, cbeNumber);
        }

        public async Task<IEnumerable<CityDTO>> GetCitiesAsync()
        {
            var cityList = await _fancRADApi.GetCities();
            if (cityList != null)
            {
                return _mapper.Map<List<CityDTO>>(cityList);
            }
            throw new KeyNotFoundException("Failed to retrieve the list of cities from RAD API.");
        }

        public async Task<IEnumerable<CityDTO>> GetCitiesByCodeAsync(string cityCode)
        {
            if (!string.IsNullOrEmpty(cityCode))
            {
                var cityList = await _fancRADApi.GetCityByCode(new GetCityRequest() { CityCode = cityCode });
                if (cityList != null)
                {
                    return _mapper.Map<List<CityDTO>>(cityList);
                }
                throw new KeyNotFoundException("Failed to retrieve the list of cities from RAD API.");
            }
            throw new InvalidOperationException("City code parameter can't be null or empty.");
        }

        public async Task<IEnumerable<LegalFormDTO>> GetLegalFormsAsync()
        {
            var legalFormList = await _fancRADApi.GetLegalForms();
            if (legalFormList != null)
            {
                return _mapper.Map<List<LegalFormDTO>>(legalFormList);
            }
            throw new KeyNotFoundException("Failed to retrieve the list of legal forms from RAD API.");
        }

        public async Task<IEnumerable<LegalFormDTO>> GetLegalFormsByCodeAsync(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var legalFormList = await _fancRADApi.GetLegalFormByCode(new GetLegalFormRequest() { Code = code });
                if (legalFormList != null)
                {
                    return _mapper.Map<List<LegalFormDTO>>(legalFormList);
                }
                throw new KeyNotFoundException("Failed to retrieve the list of legal forms from RAD API.");
            }
            throw new InvalidOperationException("Legal form code parameter can't be null or empty.");
        }

        public PersonaCategoriesDTO GetPersonaCategories(int type, string languageCode)
        {
            try
            {
                var result = new PersonaCategoriesDTO();
                var personaCategories = _personaCategoriesRepository.Find(f => !f.DeletedTime.HasValue && f.ParentId == null && f.Type == type)
                                                   .AsSplitQuery()
                                                   .Include(x => x.Labels.Where(y => y.LanguageCode == languageCode))
                                                   .Include(x => x.Nacebel!).ThenInclude(n => n.NacabelTranslation.Where(y => y.LanguageCode == languageCode))
                                                   .ToList();

                foreach (var personaCategory in personaCategories)
                {
					PopulateChildPersona(personaCategory, type, languageCode);
				}
                
                result.Categories = _mapper.Map<List<CategoryDTO>>(personaCategories);
                
                return result;
            }
            catch (Exception)
            {
				throw new InvalidOperationException("Failed to retrieve the list of persona categories.");
			}
        }

        private void PopulateChildPersona(PersonaCategories category, int type, string languageCode)
        {
            var subcategories = _personaCategoriesRepository.Find(f => !f.DeletedTime.HasValue && f.ParentId == category.Id && f.Type == type)
                                                   .AsSplitQuery()
                                                   .Include(x => x.Labels.Where(y => y.LanguageCode == languageCode))
                                                   .Include(x => x.Nacebel!).ThenInclude(n => n.NacabelTranslation.Where(y => y.LanguageCode == languageCode))
                                                   .ToList();

            if (subcategories == null || !subcategories.Any())
            {
                return;
            }

            category.Children = subcategories;

            foreach (var subcategory in subcategories)
            {
                PopulateChildPersona(subcategory, type, languageCode); 
            }
        }
    }
}
