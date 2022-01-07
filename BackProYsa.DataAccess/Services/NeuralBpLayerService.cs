using BackProYsa.DataAccess.Domain;
using BackProYsa.DataAccess.Repository;
using BackProYsa.DataAccess.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.DataAccess.Services
{
    public class NeuralBpLayerService : INeuralBpLayerService
    {
        private readonly IRepository<NeuralBpLayer> _NeuralBpLayerRepository;
 

        public NeuralBpLayerService
            (IRepository<NeuralBpLayer> NeuralBpLayerRepository
            )
        {
            _NeuralBpLayerRepository = NeuralBpLayerRepository;
        
        }
        public void DeleteById(int Id)
        {
            _NeuralBpLayerRepository.DeleteById(Id);
        }

        public IQueryable<NeuralBpLayer> GetAll()
        {
            return _NeuralBpLayerRepository.All.Where(x => x.Active == true);
        }

        public List<NeuralBpLayer> GetList()
        {
            throw new NotImplementedException();
        }

        public List<SelectListItem> GetSelectLists()
        {
            var _MesaiList = from item in GetAll().Where(x => x.Active == true)
                                 //get { return $"{PersonName}  {PersonSurName}"; }


                             select new SelectListItem
                             {

                                 Text = item.Name /*+item.StartTime.ToString("HH 'hrs' mm 'mins' ss 'secs'")*/, // select içerisinde tostring ve diğer formatlar çalışmadığı için aşağıda manuel olarak yapıldı.
                                 Value = ((int)item.Id).ToString()
                             };

            return new SelectList(_MesaiList, "Value", "Text").ToList();
        }

        public void Save(NeuralBpLayer Mesai)
        {
            int EntityIdOld = Mesai.Id;
            if (Mesai.Id != 0)
            {
                _NeuralBpLayerRepository.Update(Mesai);
            }
            else
            {
                _NeuralBpLayerRepository.Insert(Mesai);

            }
            _NeuralBpLayerRepository.SaveChanges();
        }

        public NeuralBpLayer SelecetById(int Id)
        {
            return _NeuralBpLayerRepository.SelectById(Id);
        }
    }
}
