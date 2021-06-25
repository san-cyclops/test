using Offers.Models.BaseEntities;
using Offers.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Error
{
    public interface IErrorService<T> where T : ErrorBaseEntity
    {
        public Task<ErrorRs> ErrorLog(string Title,string Detail);  
    }
     
}
