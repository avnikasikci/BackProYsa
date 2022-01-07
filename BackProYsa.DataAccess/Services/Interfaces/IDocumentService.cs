using BackProYsa.DataAccess.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.DataAccess.Services.Interfaces
{
    public interface IDocumentService
    {
        IQueryable<Document> GetAll();
        void DeleteById(int Id);
        void Save(Document Mesai);

        List<Document> GetList();
        Document SelecetById(int Id);
        List<SelectListItem> GetSelectLists();

    }
}
