using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BackProYsa.UI.Models
{
    public class BackProAlgViewModel
    {
        public int LayerType { get; set; }
        public List<SelectListItem> LayerTypeList { get; set; }
        public int TrainTypeId { get; set; }
        public List<SelectListItem> TrainTypeList { get; set; }
        public IFormFile FotoUpload { get; set; }
        public string labelMatchedHigh { get; set; }
        public string labelMatchedLow { get; set; }
        public string MatchedHighLabel { get; set; }
        public string MatchedHigh { get; set; }
        public string MatchedLow { get; set; }

    }
}
