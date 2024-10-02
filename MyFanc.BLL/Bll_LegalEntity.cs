using Microsoft.Extensions.Logging;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.Data;
using MyFanc.DTO.Internal.LegalEntity;
using MyFanc.Services;

namespace MyFanc.BLL
{
    public partial class Bll : IBll
    {
        public async Task<IEnumerable<LegalEntityDTO>> GetLegalEntityListAsync(GetUserOrganisationLinksRequest userRequest)
        {
            if (!string.IsNullOrWhiteSpace(userRequest?.UserId))
            {
                GetOrganisationLinkInfoResult getOrganisationLinkInfoResult = await _fancRADApi.GetUserOrganisationLinks(userRequest);
                var organisationLinkList = new List<OrganisationLink>();
                if (getOrganisationLinkInfoResult != null && getOrganisationLinkInfoResult.OrganisationLinks.Any())
                {
                    foreach (var item in getOrganisationLinkInfoResult.OrganisationLinks)
                    {
                        var reference = !string.IsNullOrEmpty(item.FancOrganisationId) ? item.FancOrganisationId : item.EnterpriseCBENumber;
                        var orgaReference = await _fancRADApi.GetOrganisationEnterpriseReference(new GetOrganisationEnterpriseRequest() { Reference = reference });
                        if (orgaReference == null) continue;
                        item.Name = item.Name ?? orgaReference.Name;
                        //remove the validation (TP#448873 https://voxteneo.tpondemand.com/entity/448873-user-29-missing-an-le)
                        //if (orgaReference?.MainAddress?.PostalCode == null) continue;
                        item.DefaultLanguageCode = orgaReference?.MainAddress?.PostalCode != null ? await GetDefaultLangCode(orgaReference.MainAddress.PostalCode) : "";
                        item.Nacabels = orgaReference.Nacebel2008Codes;
                        item.Sectors = _nacabelHelper.GetMappedSectors(orgaReference.Nacebel2008Codes, item.DefaultLanguageCode, item.EnterpriseCBENumber);
                        organisationLinkList.Add(item);
                    }
                    var result = _mapper.Map<IEnumerable<LegalEntityDTO>>(organisationLinkList);
                    return result;
                }

                _logger.LogError("Cannot Get LE list {0}. {1}", userRequest?.UserId, Constants.UserNotFound);
                throw new KeyNotFoundException($"Cannot Get LE list {userRequest?.UserId}. {Constants.UserNotFound}");
            }
            _logger.LogError("Parameter is invalid (Value was: {0}). {1}", userRequest?.UserId, Constants.UserRequest);
            throw new ArgumentException($"Parameter is invalid (Value was: {userRequest?.UserId}). {Constants.UserRequest}", nameof(userRequest));
        }

        private async Task<string> GetDefaultLangCode(string postalCode)
        {
            var result = string.Empty;
            // TODO : get user language code from city
            var cityList = await _fancRADApi.GetCityByCode(new GetCityRequest() { CityCode = postalCode });
            if (cityList != null)
            {
                var city = cityList.FirstOrDefault();
                if (city != null)
                {
                    result = city?.OfficialLangCode1 ?? city?.OfficialLangCode2;
                }
            }
            return result;
        }

        public async Task<string> ActivateLegalEntities(ActivateLeDTO activateLeDTO)
        {
            if (!string.IsNullOrWhiteSpace(activateLeDTO?.EnterpriseCBENumber))
            {
                var OrganisationEnterpriseRequest = _mapper.Map<UpdateOrganisationEnterpriseInfoRequest>(activateLeDTO);
                // TODO: update call rad api
                await _fancRADApi.UpdateOrganisationEnterprise(OrganisationEnterpriseRequest);

                // TODO: Insert Nacabel 
                await _nacabelHelper.InsertOrUpdateNacabelSector(OrganisationEnterpriseRequest.Tags, OrganisationEnterpriseRequest.EnterpriseCBENumber);
                return Constants.SuccessActivateLE;
            }
            _logger.LogError("Parameter is invalid (Value was: {0}). {1}", activateLeDTO?.EnterpriseCBENumber, Constants.OrganisationId);
            throw new Exception("Something went wrong, activation failed for some reason!");
        }
    }
}
