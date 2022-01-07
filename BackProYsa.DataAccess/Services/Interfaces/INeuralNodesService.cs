using BackProYsa.DataAccess.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.DataAccess.Services.Interfaces
{
    public interface INeuralNodesService
    {
        IQueryable<NeuralNodes> GetAll();
        void DeleteById(int Id);
        void Save(NeuralNodes Mesai);

        List<NeuralNodes> GetList();
        NeuralNodes SelecetById(int Id);

    }

}
