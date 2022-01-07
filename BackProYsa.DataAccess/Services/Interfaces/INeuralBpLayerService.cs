using BackProYsa.DataAccess.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.DataAccess.Services.Interfaces
{
    public interface INeuralBpLayerService
    {
        IQueryable<NeuralBpLayer> GetAll();
        void DeleteById(int Id);
        void Save(NeuralBpLayer Mesai);

        List<NeuralBpLayer> GetList();
        NeuralBpLayer SelecetById(int Id);
        List<SelectListItem> GetSelectLists();


    }

}
