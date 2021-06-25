using Newtonsoft.Json;
using Offers.Models.BaseEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.FileSave
{
    public class FileSaveService : IFileSaveService<FileSaveBaseEntity>
    {
        #region Save
        public async Task<bool> Save_v1(object FileSaveObject, string FileName, string TraceID)
        {
            try
            {
                String Json = await Log(FileSaveObject);
                string FolderPath = @"/" + "SystemLogs" + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/" + FileName.Split("_").First();
                if (Directory.Exists(FolderPath))
                {
                    string FilePath = FolderPath + "/" + TraceID + "_" + FileName + "_" + DateTime.Now.ToString("yyyy_MM_ddTHH_mm_ss") + ".json";
                    File.WriteAllText(FilePath, Json);
                    return await Task.FromResult(true);

                }
                else
                {
                    Directory.CreateDirectory(FolderPath);
                    string FilePath = FolderPath + "/" + TraceID + "_" + FileName + "_" + DateTime.Now.ToString("yyyy_MM_ddTHH_mm_ss") + ".json";
                    File.WriteAllText(FilePath, Json);
                    return await Task.FromResult(true);
                }
            }
            catch (Exception ErrorMessage)
            {
                Console.WriteLine(ErrorMessage.Message);
                return await Task.FromResult(false);
            }
        }
        #endregion

        public Task<string> Log(object Obj)
        {
            String SaveFile = JsonConvert.SerializeObject(Obj, Formatting.Indented);
            return Task.FromResult(SaveFile);
        }

    }
}
