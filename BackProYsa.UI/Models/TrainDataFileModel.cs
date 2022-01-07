using BackProYsa.DataAccess.Domain;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace BackProYsa.UI.Models
{
    public class TrainDataFileModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List< IFormFile> FotoUpload { get; set; }
        public List<Document> DocumentList { get; set; }


    }
}
