using Offers.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Alliances
{
    public interface IGetAlliances
    {
        Task<List<AirlineRS>> GetAlliances(String[] Airline);

    }
}
