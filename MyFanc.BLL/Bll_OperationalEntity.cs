using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyFanc.BusinessObjects;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.OperationalEntity;
using MyFanc.DTO.Internal.Users;
using MyFanc.Services;
using Newtonsoft.Json;

namespace MyFanc.BLL
{
    public partial class Bll
    {
        public async Task<IEnumerable<OperationalEntityDTO>> ListOperationalEntityAsync(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                GetOrganisationLinkInfoResult getOrganisationLinkInfoResult = await _fancRADApi.GetUserOrganisationLinks(new GetUserOrganisationLinksRequest()
                {
                    UserId = userId
                });
                var organisationLinkList = new List<OperationalEntityDTO>();
                if (getOrganisationLinkInfoResult.OrganisationLinks != null && getOrganisationLinkInfoResult.OrganisationLinks.Any())
                {
                    foreach (var item in getOrganisationLinkInfoResult.OrganisationLinks.SelectMany(x => x.Establishments))
                    {
                        var reference = !string.IsNullOrEmpty(item.FancOrganisationId) ? item.FancOrganisationId : item.EnterpriseCBENumber;
                        var orgaReference = await _fancRADApi.GetOrganisationEnterpriseReference(new GetOrganisationEnterpriseRequest() { Reference = reference });
                        if (orgaReference?.MainAddress?.PostalCode == null) continue;
                        item.DefaultLanguageCode = await GetDefaultLangCode(orgaReference.MainAddress.PostalCode);
                        var dto = _mapper.Map<OperationalEntityDTO>(item);
                        dto.Sectors = _nacabelHelper.GetMappedSectors(item.Nacabels, item.DefaultLanguageCode, item.FancOrganisationId);
                        organisationLinkList.Add(dto);
                    }
                    return organisationLinkList;
                }

                throw new KeyNotFoundException($"Cannot Get User organisation link {userId}. {Constants.UserOrganitationLinkNotFound}");
            }
            throw new ArgumentException($"Parameter is invalid (Value was: {userId}");
        }

        public async Task CreateOeAsync(CreateOeDTO createOeDTO)
        {
            if (createOeDTO == null)
                throw new ArgumentNullException(nameof(createOeDTO), "CreateOeDTO cannot be null");

            if (!string.IsNullOrWhiteSpace(createOeDTO?.EnterpriseCBENumber))
            {
                var createOe = _mapper.Map<UpdateOrganisationBusinessUnitInfoRequest>(createOeDTO);
                await _fancRADApi.UpdateOrganisationBusinessUnit(createOe);
            }
            else
            {
                throw new ArgumentNullException("EnterpriseCBENumber cannot be null or empty");
            }
        }

        public async Task<IEnumerable<OeUserDTO>> ListUserLinkedToOeAsync(string reference, string fancOrganisationId, bool includeMissingCbeBusinessUnits, string languageCode)
        {
            if (!string.IsNullOrEmpty(reference) && !string.IsNullOrEmpty(fancOrganisationId))
            {
                var getOrganisationLinkInfoResult = await _fancRADApi.GetOrganisationEnterpriseReference(new GetOrganisationEnterpriseRequest()
                {
                    IncludeMissingCbeBusinessUnits = includeMissingCbeBusinessUnits,
                    Reference = reference,
                    LanguageCode = languageCode
                });

                if (getOrganisationLinkInfoResult.Establishments != null)
                {
                    var establishment = getOrganisationLinkInfoResult.Establishments.Where(e => e.FancOrganisationId == fancOrganisationId).FirstOrDefault();
                    if (establishment != null)
                    {
                        List<OeUserDTO> result = _mapper.Map<List<OeUserDTO>>(establishment.Users);
                        return result;
                    }
                    throw new KeyNotFoundException($"Cannot get operational entity {fancOrganisationId}. {Constants.OperationalEntityLinkNotFound}");
                }

                throw new KeyNotFoundException($"No operational entity list found for current selected LE {reference}");
            }
            throw new ArgumentException($"Parameter is invalid (Value was: {reference}, {fancOrganisationId}");
        }

        public async Task ActivateOeAsync(ActivateOeDTO activateOeDTO)
        {
            if (activateOeDTO == null)
                throw new ArgumentNullException(nameof(activateOeDTO), "ActivateOeDTO cannot be null");

            if(activateOeDTO.FancOrganisationId != null)
                await _nacabelHelper.InsertOrUpdateNacabelSector(activateOeDTO.Sectors, activateOeDTO.FancOrganisationId);

            var activateOe = _mapper.Map<UpdateOrganisationBusinessUnitInfoRequest>(activateOeDTO);
            await _fancRADApi.UpdateOrganisationBusinessUnit(activateOe);
        }

        public async Task DeleteUserFromOeAsync(string userId, string fancOrganisationId, string requestingUserId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException($"Parameter is invalid (Value was: {userId}");

            var userOrganisationLinks = await _fancRADApi.GetUserOrganisationLinks(new GetUserOrganisationLinksRequest()
            {
                UserId = userId
            });
            if (userOrganisationLinks?.OrganisationLinks == null)
                throw new KeyNotFoundException($"Delete user tried on unexisting organisations {fancOrganisationId}");

            var roles = GetUserRoleInOe(userOrganisationLinks.OrganisationLinks.ToList(), fancOrganisationId);
            UpdateOrganisationLinkInfoRequest requestBody = new UpdateOrganisationLinkInfoRequest()
            {
                FancOrganisationId = fancOrganisationId,
                Roles = roles,
                RequestingUserId = requestingUserId
            };
            UpdateUserRequest requestQuery = new UpdateUserRequest()
            {
                UserId = userId
            };
            await _fancRADApi.DeleteUserOrganisationLinks(requestBody, requestQuery);
            
            var userProfile = await GetUserInfoAsync(new GetUserRequest() { UserId = userId });
            if(userProfile != null && !string.IsNullOrEmpty(userProfile.Email))
                await _emailService.SendDeleteUserFromOeNotificationAsync(userProfile.Email, fancOrganisationId, roles, requestingUserId);
        }

        private IEnumerable<string> GetUserRoleInOe(List<OrganisationLink> organisationLinks, string fancOrganisationId)
        {
            var parentOrganisatios = organisationLinks.GroupBy(o => o.FancOrganisationId).Select(g => g.First()).ToDictionary(o => o.FancOrganisationId, o => o.Roles.Select(r => r.Role));
            var childOrganisations = organisationLinks.SelectMany(o => o.Establishments).GroupBy(o => o.FancOrganisationId).Select(g => g.First()).ToDictionary(o => o.FancOrganisationId, o => o.Roles.Select(r => r.Role));
            var selectedUserOrganitationsAndRoles = parentOrganisatios.Concat(childOrganisations).GroupBy(kv => kv.Key).ToDictionary(group => group.Key, group => group.First().Value);
            var result = selectedUserOrganitationsAndRoles[fancOrganisationId];
            if (result != null)
                return result;
            throw new KeyNotFoundException($"Can't find user roles for current fancOrganisationId {fancOrganisationId}");
        }
        public async Task SendInvitationMailAsync(SendInvitationDTO sendInvitationDTO)
        {
            foreach (var item in sendInvitationDTO.ListOe)
            {
                var token = GenerateToken(item.Role, item.FancOrganisationId, sendInvitationDTO.EmailTo);
                await _emailService.SendInvitationAsync(sendInvitationDTO.EmailTo, token, item.FancOrganisationId, item.Role);
            }
        }

        private string GenerateToken(string role, string fancOrganisationId, string emailTo)
        {
            var data = $"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss}|{role}|{fancOrganisationId}|{DateTime.UtcNow.AddDays(_tokenConfiguration.EpiredInDays):yyyy-MM-ddTHH:mm:ss}";
            var encryptedData = _aESEncryptService.EncryptString(data);
            return encryptedData;
        }

        public async Task<OrganisationEnterpriseDTO> GetOrganisationEnterpriseDetailAsync(string reference, bool includeMissingCbeBusinessUnits, 
            string languageCode, bool forceUpdate)
        {
            if (forceUpdate)
            {
				_sharedDataCache.ClearAllData();
            }

            var organisation = await _fancRADApi.GetOrganisationEnterpriseReference(new GetOrganisationEnterpriseRequest()
            {
                Reference = reference,
                IncludeMissingCbeBusinessUnits = includeMissingCbeBusinessUnits,
                LanguageCode = languageCode
            });

            if (organisation == null)
                throw new KeyNotFoundException($"Organisation with ID {reference} not found");

            var result = _mapper.Map<OrganisationEnterpriseDTO>(organisation);
            result.MainAddressIsInvoiceAddress = ObjectsAreEqual(result.MainAddress, result.InvoiceAddress);
            CheckEstablishmentInvoiceAddressAndMainAddress(result);
            //result.Sectore = _nacabelHelper.GetDescription(result.Nacebel2008Codes, languageCode);
            result.Sectors = _nacabelHelper.GetMappedSectors(result.Nacebel2008Codes, languageCode, result.EnterpriseCBENumber);
            return result;
        }

        private void CheckEstablishmentInvoiceAddressAndMainAddress(OrganisationEnterpriseDTO organisationEnterpriseDTO)
        {
            foreach (var establishment in organisationEnterpriseDTO.Establishments)
            {
                establishment.MainAddressIsInvoiceAddress = ObjectsAreEqual(establishment.MainAddress, establishment.InvoiceAddress);
            }
        }

        private bool ObjectsAreEqual(ProfileInfoAddressDTO? mainAddress, ProfileInfoAddressDTO? invoiceAddress)
        {
            var obj1Serialized = JsonConvert.SerializeObject(mainAddress);
            var obj2Serialized = JsonConvert.SerializeObject(invoiceAddress);
            return obj1Serialized == obj2Serialized;
        }

        public async Task<OrganisationDefaultLanguangeDTO> GetDefaultLanguageForOrganisationByPostalCodeAsync(string postCode)
        {
            if (!string.IsNullOrEmpty(postCode))
            {
                var cityList = await _fancRADApi.GetCityByCode(new GetCityRequest() { CityCode = postCode });
                if (cityList != null)
                {
                    var city = cityList.FirstOrDefault();
                    if (city != null)
                    {
                        return _mapper.Map<OrganisationDefaultLanguangeDTO>(city);
                    }
                    throw new KeyNotFoundException("Failed to get default language data.");

                }
                throw new KeyNotFoundException("Failed to retrieve the list of cities by postal code from RAD API.");
            }
            throw new InvalidOperationException("Postcode parameter can't be null or empty.");
        }
    }
}
