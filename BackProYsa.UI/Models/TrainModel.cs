using BackProYsa.DataAccess.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BackProYsa.UI.Models
{
    public class TrainModel
    {
        public string Name { get; set; }
        public int LayerType { get; set; }
        public List<SelectListItem> LayerTypeList { get; set; }
        public List<NeuralBpLayer> NeuralBpLayerList { get; set; }
        public string InputUnitCount { get; set; }
        public string HiddenUnitCount { get; set; }
        public string OutputCount { get; set; }
        public string MaxErrorCount { get; set; }
    }
}
