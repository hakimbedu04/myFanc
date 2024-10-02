using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFanc.Api.Common;
using MyFanc.BLL;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.LegalEntity;
using System.ComponentModel.DataAnnotations;

namespace MyFanc.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LegalEntityController : Controller
    {
        private readonly IBll _bll;
        private readonly ILogger<LegalEntityController> _logger;

        public LegalEntityController(IBll bll, ILogger<LegalEntityController> logger)
        {
            _bll = bll;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLegalEntity([Required] string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var userRequest = new GetUserOrganisationLinksRequest()
                    {
                        UserId = id
                    };
                    var result = await _bll.GetLegalEntityListAsync(userRequest);
                    return Ok(result);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error stack trace {0}",ex);
				return await ex.ToActionResultAsync();
			}
        }
        [HttpPost("ActivateLegalEntity")]
        public async Task<IActionResult> ActivateLegalEntity(ActivateLeDTO activateLeDto)
        {
            try
            {
                await _bll.ActivateLegalEntities(activateLeDto);
                return Ok();
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }
    }
}
