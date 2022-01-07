using BackProYsa.DataAccess.Domain;
using BackProYsa.DataAccess.Repository;
using BackProYsa.DataAccess.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.DataAccess.Services
{
    public class NeuralNodesService : INeuralNodesService
    {
        private readonly IRepository<NeuralNodes> _NeuralNodesRepository;
 

        public NeuralNodesService
            (IRepository<NeuralNodes> NeuralNodesRepository
            )
        {
            _NeuralNodesRepository = NeuralNodesRepository;
        
        }
        public void DeleteById(int Id)
        {
            _NeuralNodesRepository.DeleteById(Id);
        }

        public IQueryable<NeuralNodes> GetAll()
        {
            return _NeuralNodesRepository.All.Where(x => x.Active == true);
        }

        public List<NeuralNodes> GetList()
        {
            throw new NotImplementedException();
        }

        public void Save(NeuralNodes Mesai)
        {
            int EntityIdOld = Mesai.Id;
            if (Mesai.Id != 0)
            {
                _NeuralNodesRepository.Update(Mesai);
            }
            else
            {
                _NeuralNodesRepository.Insert(Mesai);

            }
            _NeuralNodesRepository.SaveChanges();
        }

        public NeuralNodes SelecetById(int Id)
        {
            return _NeuralNodesRepository.SelectById(Id);
        }
    }
}
