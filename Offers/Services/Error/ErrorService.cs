using Microsoft.Extensions.Logging;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Error
{
    public class ErrorService : IErrorService<ErrorBaseEntity>
    {
        private readonly ILogger _logger;
        public ILogger<ErrorService> _Logger { get; }

        public ErrorService(ILogger<ErrorService> Logger)
        {
            _logger = Logger;
        }

        public async Task<ErrorRs> ErrorLog(string Title, string Detail)
        {
            ErrorRs ErrorRsObj = new ErrorRs();
            ErrorRsObj.title = Title;
            ErrorRsObj.detail = Detail;
            //Log(Title + " - " + Detail);
            return await Task.FromResult(ErrorRsObj);
        }

        public void Log(string Err)
        {
            _Logger.LogWarning(Err);
        }
       
    }
}
