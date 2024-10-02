using MyFanc.DTO.External.RADApi;
using MyFanc.DTO.Internal.LegalEntity;
using MyFanc.DTO.Internal.OperationalEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.BLL
{
    public partial interface IBll
    {
        Task<IEnumerable<LegalEntityDTO>> GetLegalEntityListAsync(GetUserOrganisationLinksRequest userRequest);
        Task<string> ActivateLegalEntities(ActivateLeDTO activateLeDTO);
    }
}
