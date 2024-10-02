using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFanc.Api.Common;
using MyFanc.BLL;

namespace MyFanc.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IBll _bll;
        public DataController(IBll bll)
        {
            _bll = bll;
        }

        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries() 
        {
            try
            {
                var countries = await _bll.GetCountriesAsync();
                return Ok(countries);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
            
        }

        [HttpGet("countries/{countryCode}")]
        public async Task<IActionResult> GetCountriesCode(string countryCode)
        {
            try
            {
                var country = await _bll.GetCountriesCodeAsync(countryCode);
                return Ok(country);
            }
            catch(Exception ex) 
            {
				return await ex.ToActionResultAsync();
			}

        }

        [HttpGet("sectors/{languageCode}")]
        public IActionResult GetSectors(string languageCode, string? nacabelCode, int pageSize = 10)
        {
            try
            {
                var sectors = _bll.GetSectors(languageCode, nacabelCode, pageSize);
                return Ok(sectors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            try
            {
                var cities = await _bll.GetCitiesAsync();
                return Ok(cities);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}

        }

        [HttpGet("cities/{cityCode}")]
        public async Task<IActionResult> GetCitiesByCode(string cityCode)
        {
            try
            {
                var cities = await _bll.GetCitiesByCodeAsync(cityCode);
                return Ok(cities);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}

        }

        [HttpGet("legalforms")]
        public async Task<IActionResult> GetLegalForms()
        {
            try
            {
                var legalForms = await _bll.GetLegalFormsAsync();
                return Ok(legalForms);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}

        }

        [HttpGet("legalforms/{code}")]
        public async Task<IActionResult> GetLegalFormsByCode(string code)
        {
            try
            {
                var legalForms = await _bll.GetLegalFormsByCodeAsync(code);
                return Ok(legalForms);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}

        }

        [HttpGet("personacategories")]
        public IActionResult GetPersonaCategories(int Type, string LanguageCode = "en")
        {
            try
            {
                var result = _bll.GetPersonaCategories(Type, LanguageCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
