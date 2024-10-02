using MyFanc.BusinessObjects;
using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BLL
{
    public partial class Bll
    {
        private async Task<GetOrganisationEnterpriseInfoResult> StoreOrganisation(string organisationId)
        {
            const string cacheKey = "Organisations";
            //string reference = $"Enterprise/{organisationId}";
            var organisation = await _fancRADApi.GetOrganisationEnterpriseReference(new GetOrganisationEnterpriseRequest
            {
                Reference = organisationId
            });
            if (organisation != null)
            {
                var organisations = _sharedDataCache.GetData<List<GetOrganisationEnterpriseInfoResult>>(cacheKey) 
                    ?? new List<GetOrganisationEnterpriseInfoResult>();

                var existingOrganisation = organisations.FirstOrDefault(o => o.FancOrganisationId == organisation.FancOrganisationId);

                if(existingOrganisation != null)
                {
                    existingOrganisation.Name = organisation.Name;
                    existingOrganisation.MainAddress = organisation.MainAddress;
                    existingOrganisation.InvoiceAddress = organisation.InvoiceAddress;
                    existingOrganisation.Nacebel2008Codes = organisation.Nacebel2008Codes;
                    existingOrganisation.Establishments = organisation.Establishments;
                }
                else
                {
                    organisations.Add(organisation);
                }

                _sharedDataCache.SetData(cacheKey, organisations);

                return organisation;
            }
            else
            {
                throw new InvalidOperationException("Failed to retrieve organisation details from RAD API.");
            }
        }

        public async Task StoreUserOrganisation(string userId)
        {
            var organisationLinks = await _fancRADApi.GetUserOrganisationLinks(new GetUserOrganisationLinksRequest
            {
                UserId = userId
            });
            string cacheKey = $"User-{userId}-Organisations";
            string cacheOrganisationKey = "OrganisationUsers";

            var organisationUsers = new List<OrganisationUsersDTO>();

            foreach (var organisationLink in organisationLinks.OrganisationLinks)
            {
                if (string.IsNullOrEmpty(organisationLink.FancOrganisationId)) continue;
                var storeOrganisation = await StoreOrganisation(organisationLink.FancOrganisationId);

                if (storeOrganisation != null)
                {
                    var organisationUser = new OrganisationUsersDTO
                    {
                        OrganisationId = storeOrganisation.FancOrganisationId,
                        EstabishmentId = storeOrganisation.Establishments.FirstOrDefault()?.FancOrganisationId,
                        UserId = userId
                    };
                    organisationUsers.Add(organisationUser);
                }
                CleanUserOrganisationCache(organisationLink.FancOrganisationId, organisationLink.Establishments.FirstOrDefault()?.FancNumber);
            }

            _sharedDataCache.SetData(cacheKey, organisationUsers);

            /*var existingOrganisationUsers = _sharedDataCache.GetData<List<OrganisationUsersDTO>>(cacheOrganisationKey);
            if (existingOrganisationUsers != null)
            {
                existingOrganisationUsers.RemoveAll(link => link.UserId == userId);
                existingOrganisationUsers.AddRange(organisationUsers);
                _sharedDataCache.SetData(cacheOrganisationKey, existingOrganisationUsers);
            }
            else
            {
                _sharedDataCache.SetData(cacheOrganisationKey, organisationUsers);
            }*/


        }

        private void CleanUserOrganisationCache(string organisationId, string? establishmentId = null)
        {
            var organisationUsersCacheKey = "OrganisationUsers";
            var organisationUsers = _sharedDataCache.GetData<List<OrganisationUsersDTO>>(organisationUsersCacheKey);

            if (organisationUsers != null)
            {
                var linksToRemove = organisationUsers
                    .Where(link => link.OrganisationId == organisationId && (establishmentId == null || link.EstabishmentId == establishmentId))
                    .ToList();

                foreach (var link in linksToRemove)
                {
                    var userOrganisationsCacheKey = $"User-{link.UserId}-Organisations";
                    _sharedDataCache.RemoveData<List<OrganisationUsersDTO>>(userOrganisationsCacheKey);
                }
                organisationUsers.RemoveAll(link => linksToRemove.Contains(link));


                _sharedDataCache.SetData(organisationUsersCacheKey, organisationUsers);
            }
        }
    }
}
