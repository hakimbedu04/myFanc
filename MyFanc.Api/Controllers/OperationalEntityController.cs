using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFanc.Api.Common;
using MyFanc.BLL;
using MyFanc.DTO.Internal.OperationalEntity;
using MyFanc.Services;

namespace MyFanc.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OperationalEntityController : ControllerBase
    {
        private readonly IBll _bll;
        private readonly ILogger<OperationalEntityController> _logger;
        public OperationalEntityController(IBll bll, ILogger<OperationalEntityController> logger)
        {
            _bll = bll;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("User/{userId}/OperationalEntities")]
        public async Task<IActionResult> GetUserOperationalEntities(string userId)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    var operationalEntities = await _bll.ListOperationalEntityAsync(userId);
                    return Ok(operationalEntities);
                }

                return BadRequest();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPost]
        public async Task<IActionResult> CreateOe(CreateOeDTO createOeDTO)
        {
            try
            {
                await _bll.CreateOeAsync(createOeDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
				return await ex.ToActionResultAsync();
			}
        }

        [HttpGet("{fancOrganisationId}/users")]
        public async Task<IActionResult> GetOperationalEntityUser(string fancOrganisationId, [FromQuery]string reference, [FromQuery]bool includeMissingCbeBusinessUnits = false, [FromQuery]string languageCode = "en")
        {
            try
            {
                if (!string.IsNullOrEmpty(fancOrganisationId) && !string.IsNullOrEmpty(reference))
                {
                    var users = await _bll.ListUserLinkedToOeAsync(reference, fancOrganisationId, includeMissingCbeBusinessUnits, languageCode);
                    return Ok(users);
                }

                return BadRequest();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
				return await ex.ToActionResultAsync();
			}
        }

        [HttpDelete("User/{userId}/OperationalEntities")]
        public async Task<IActionResult> DeleteUserFromOe(string userId, string fancOrganisationId, string requestingUserId)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId)
                    && !string.IsNullOrEmpty(fancOrganisationId)
                    && !string.IsNullOrEmpty(requestingUserId))
                {
                    await _bll.DeleteUserFromOeAsync(userId, fancOrganisationId, requestingUserId);
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPost("SendJoinOeInvitation")]
        public async Task<IActionResult> SendJoinOeInvitation(SendInvitationDTO sendInvitationDTO)
        {
            try
            {
                await _bll.SendInvitationMailAsync(sendInvitationDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPut("Activate")]
        public async Task<IActionResult> ActivateOe(ActivateOeDTO activateOeDTO)
        {
            try
            {
                await _bll.ActivateOeAsync(activateOeDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
				return await ex.ToActionResultAsync();
			}
        }

        [HttpGet("{reference}")]
        public async Task<IActionResult> GetOrganisationEnterprise(string reference,
            bool includeMissingCbeBusinessUnits, string languageCode,
            bool forceUpdate = false)
        {
            try
            {
                if (string.IsNullOrEmpty(reference))
                {
                    var errors = Constants.InputMadantoryValues;
                    return BadRequest(errors);
                }
                
                var result = await _bll.GetOrganisationEnterpriseDetailAsync(reference, 
                    includeMissingCbeBusinessUnits, languageCode, forceUpdate);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

		[HttpGet("defaultlanguage/{postCode}")]
        public async Task<IActionResult> GetCompanyDefaultLanguageByPostCode(string postCode)
        {
            try
            {
                var cities = await _bll.GetDefaultLanguageForOrganisationByPostalCodeAsync(postCode);
                return Ok(cities);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}

        }
    }
}
