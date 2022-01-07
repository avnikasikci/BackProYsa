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
    public class DocumentService : IDocumentService
    {
        private readonly IRepository<Document> _DocumentRepository;


        public DocumentService
            (IRepository<Document> DocumentRepository
            )
        {
            _DocumentRepository = DocumentRepository;

        }
        public void DeleteById(int Id)
        {
            _DocumentRepository.DeleteById(Id);
        }

        public IQueryable<Document> GetAll()
        {
            return _DocumentRepository.All.Where(x => x.Active == true);
        }

        public List<Document> GetList()
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

        public void Save(Document Mesai)
        {
            int EntityIdOld = Mesai.Id;
            if (Mesai.Id != 0)
            {
                _DocumentRepository.Update(Mesai);
            }
            else
            {
                _DocumentRepository.Insert(Mesai);

            }
            _DocumentRepository.SaveChanges();
        }

        public Document SelecetById(int Id)
        {
            return _DocumentRepository.SelectById(Id);
        }
    }

}
