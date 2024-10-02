using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFanc.Api.Common;
using MyFanc.BLL;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.Persona;
using MyFanc.DTO.Internal.Users;
using MyFanc.Services;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MyFanc.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IBll _bll;
        private readonly ILogger<UserController> _logger;

        public UserController(IBll bll, ILogger<UserController> logger)
        {
            _bll = bll;
            _logger = logger;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UpdateProfileDTO user)
        {
            string errors;
            try
            {
                if (String.IsNullOrEmpty(id))
                {
                    errors = Constants.InputMadantoryValues;
                    return BadRequest(errors);
                }
                else
                {
                    if (user == null)
                    {
                        errors = Constants.UpdateUser;
                        return BadRequest(errors);
                    }
                    var result = await _bll.UpdateUserAsync(id, user);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([Required] string id)
        {
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    var userRequest = new GetUserRequest()
                    {
                        UserId = id
                    };
                    var userAPI = await _bll.GetUserInfoAsync(userRequest);
                    return Ok(userAPI);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}

        }

        [HttpGet("{id}/AuthenticationRedirection")]
        public async Task<IActionResult> GetAuthRedirection(string id)
        {
            try
            {
                var result = new AuthRedirectionDTO();
                if (!string.IsNullOrEmpty(id))
                {
					var idp = HttpContext?.User?.Identity?.AsClaimIdentity().IdentityProvider();
					result = await _bll.GetAuthRedirectionAsync(id, idp);

                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}

        }

        [HttpGet("{id}/Synchronize")]
        public async Task<IActionResult> SynchProfile(string id)
        {
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    var userRequest = new GetUserRequest()
                    {
                        UserId = id,
                        ForceUpdate = true
                    };
                    var userAPI = await _bll.GetUserInfoAsync(userRequest);
                    return Ok(userAPI);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        
        [HttpGet("{id}/Identity")]
        public async Task<IActionResult> Identity([Required] string id)
        {
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    var userRequest = new GetUserRequest()
                    {
                        UserId = id
                    };
                    var idp = HttpContext?.User?.Identity?.AsClaimIdentity().IdentityProvider();
                    var userIndentity = await _bll.GetUserIdentityAsync(userRequest, idp);
                    return Ok(userIndentity);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPost("{userId}/VerifyEmail")]
        public async Task<IActionResult> ForceEmailVerification(string userId)
        {
            try
            {
                if(string.IsNullOrEmpty(userId))
                    return BadRequest("Invalid user ID.");
                var verificationResult = await _bll.ForceEmailVerificationAsync(userId);
                if (verificationResult)
                {
                    return Ok(verificationResult);
                }
                return BadRequest("Failed to force email verification");
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPost("{userId}/AcceptInvitation")]
        public async Task<IActionResult> AcceptInvitation(string userId, UpdateOrganisationLinkInfoRequest userRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return BadRequest("Invalid user ID.");
                var result = await _bll.AcceptInvitation(userId, userRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpDelete("{userId}/RefuseInvitation")]
        public async Task<IActionResult> RefuseInvitation(string userId, UpdateOrganisationLinkInfoRequest userRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return BadRequest("Invalid user ID.");
                var result = await _bll.RefuseInvitation(userId, userRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpGet("Roles")]
        public async Task<IActionResult> GetRoles(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return BadRequest("Invalid user ID.");
                var userRequest = new GetUserOrganisationLinksRequest()
                {
                    UserId = userId
                };
                var result = await _bll.GetUserRoles(userRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error stack trace {0}", ex);
				return await ex.ToActionResultAsync();
			}
        }

        [HttpPut("{userId}/UpdateCurrentSelector")]
        public async Task<IActionResult> UpdateCurrentSelector(string userId, SelectorUserDTO userRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return BadRequest("Invalid user ID.");
                var result = await _bll.UpdateCurrentSelector(userId, userRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
				return await ex.ToActionResultAsync();
			}
        }

        [HttpGet("{userId}/Personas")]
        public IActionResult GetPersona(string userId, Guid organisationFancId, string languageCode = "en")
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return BadRequest("Invalid user ID.");
                if (organisationFancId == Guid.Empty)
                    return BadRequest("Invalid organisation ID.");
                var result = _bll.GetUserCompanyPersona(userId, organisationFancId, languageCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{userId}/Personas")]
        public async Task<IActionResult> SavePersona(string userId, AddOrUpdatePersonaDTO param)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return BadRequest("Invalid user ID.");
                var result = await _bll.SaveUserAndCompanyPersona(userId, param);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
