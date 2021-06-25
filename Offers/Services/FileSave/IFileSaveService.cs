using Offers.Models.BaseEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Offers.Services.FileSave
{
    public interface IFileSaveService<T> where T : FileSaveBaseEntity
    {
        public Task<Boolean> Save_v1(object FileSaveObject, string RequestName, string TraceID);
    }
}
