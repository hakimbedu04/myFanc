﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFanc.DTO.External.RADApi
{
    public class GetOrganisationLinkInfoResult
    {
        public IEnumerable<OrganisationLink> OrganisationLinks { get; set; } = Enumerable.Empty<OrganisationLink>();
    }
}