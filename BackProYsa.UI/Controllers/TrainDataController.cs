using BackProYsa.DataAccess.Domain;
using BackProYsa.DataAccess.Services.Interfaces;
using BackProYsa.lib;
using BackProYsa.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace BackProYsa.UI.Controllers
{
    public class TrainDataController : Controller
    {
        private readonly INeuralBpLayerService _NeuralBpLayerService;
        private readonly IDocumentService _DocumentService;


        public TrainDataController(
            INeuralBpLayerService NeuralBpLayerService,
            IDocumentService DocumentService
            )
        {
            this._NeuralBpLayerService = NeuralBpLayerService;
            this._DocumentService = DocumentService;

        }
        public IActionResult Index()
        {
            var AllDocument = _DocumentService.GetAll().ToList();
            var Model = new TrainDataFileModel();
            Model.DocumentList = AllDocument;
            return View(Model);
        }

        [HttpPost]
        public IActionResult Index(TrainDataFileModel Model)
        {
            if(Model == null)
            {
                return View("Error");
            }
            var AllDocument = _DocumentService.GetAll().ToList();
            Model.DocumentList = AllDocument;
            //var AllDocument = _DocumentService.GetAll().ToList();
            //Model.DocumentList = AllDocument;

            if (Model.Name == null)
            {
                ModelState.AddModelError("", "Hata: İsim Alanı Boş olamaz");
                return View(Model);
            }
            if (Model.FotoUpload== null)
            {
                ModelState.AddModelError("", "Hata:Foto Boş Olamaz..");
                return View(Model);
            }
            foreach(var _FotoUpload in Model.FotoUpload)
            {
                //FotoSave
                var FilePath = DokumanHelper.GetServerMapPath("UserData/PATTERNS");
                //var FilePath = Path.Combine(WebHostEnvironment.WebRootPath + "~/UserData/UserProfil/");
                //var OldimagePath = Path.Combine(FilePath, Model.FotoUpload.Name ?? "empty");
                //if (System.IO.File.Exists(OldimagePath) && Model.FotoUpload.Name != "no_image.png")
                //{ System.IO.File.Delete(OldimagePath); }

                //var FotoName = (DateTime.Now.Ticks.ToString() + "_" + Model.FotoUpload.FileName);
                //var FotoName = Model.Name + "_" + DateTime.Now.Ticks.ToString() + ".bmp";
                var FotoName = _FotoUpload.FileName.Replace(".bmp","") + "_" + DateTime.Now.Ticks.ToString() + ".bmp";
                //user.ProfilFoto = foto.FileName;
                System.Drawing.Image image = System.Drawing.Image.FromStream(_FotoUpload.OpenReadStream());
                //Size byt = ReSize(image);
                //image = image.GetThumbnailImage(byt.Width, byt.Height, null, IntPtr.Zero);
                //System.Drawing.Image i = resizeImage(image, new Size(24, 33));
                image = resizeImage(image, new Size(24, 33));
                DokumanHelper.FolderExistOrCreate(FilePath);


                image.Save(Path.Combine(FilePath, FotoName));
                image.Dispose();
                //FotoSave

                var saveDocument = new Document();
                saveDocument.Name = Model.Name;
                /*/UserData/PATTERNS//3_1_637771417926972075.bmp*/
                var newFilePath = "/UserData/PATTERNS/" + FotoName;

                saveDocument.Path = newFilePath;
                saveDocument.Active = true;
                _DocumentService.Save(saveDocument);

            }







            //return View(Model);
            //return Json(new { Result = saveDocument,Message="Başarılı Şekilde Kaydedildi." });
            return View(Model);

        }
        //TrainDataDelete
        public JsonResult TrainDataDelete(int? Id)
        {

            if (Id.HasValue)
            {
                var Entity = _DocumentService.SelecetById(Id.Value);
                Entity.Active = false;
                _DocumentService.Save(Entity);
            }
            return Json(new { Result = "Silme İşlemi Başarılı", Answer = "Success" });

        }

        public Size ReSize(Image org)
        {


            const int max = 300;// resmimizin max olacak boyutu.
            int uzunluk = org.Width;// orjinal resmin uzunluğu
            int genislik = org.Height;// orjinal resmin genişliği
            double f;// küçültme oranımız.
            if (uzunluk > genislik)// hangi kenar büyük kontrolü
            {
                f = (double)max / uzunluk;//küçültme oranını hesapla
            }
            else
            {
                f = (double)max / genislik;//küçültme oranı hesapla.
            }
            return new Size((int)(uzunluk * f), (int)(genislik * f));
        }
        private static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, Size size)
        {
            //Get the image current width  
            int sourceWidth = imgToResize.Width;
            //Get the image current height  
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //Calulate  width with new desired size  
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //Calculate height with new desired size  
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width  
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height  
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height  
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }
    }
}
