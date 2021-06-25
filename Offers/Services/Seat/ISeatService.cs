using Offers.Models.BaseEntities;
using Offers.Models.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Offers.Services.Seat
{
    public interface ISeatService<T> where T : SeatBaseEntity
    {
        public Task<List<SeatsRs>> GetSeats(String[] Id,string TransactionId,string context);
        public Task<SeatsRs> GetSeat(String Id, string TransactionId, string context);
    }
}
