using BackProYsa.DataAccess.Enums;
using BackProYsa.DataAccess.Services.Interfaces;
using BackProYsa.lib;
using BackProYsa.lib.Layers;
using BackProYsa.lib.Network;
using BackProYsa.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace BackProYsa.UI.Controllers
{
    public class BackProController : Controller
    {
        private Dictionary<string, double[]> TrainingSet;
        private BackProYsa.lib.Network.BackProNeuralNetwork<string> neuralNetwork;

        private readonly INeuralBpLayerService _NeuralBpLayerService;


        public BackProController(
            INeuralBpLayerService NeuralBpLayerService
            )
        {
            this._NeuralBpLayerService = NeuralBpLayerService;

        }
        public IActionResult Index()
        {
            var Model = new BackProAlgViewModel();

            //var AllTrainData = _NeuralBpLayerService.GetAll().ToList();
            var AllTrainData = _NeuralBpLayerService.GetSelectLists();
            Model.TrainTypeList = AllTrainData;

            Model.LayerTypeList = EnumHelper.GetSelectList(typeof(EnumCollection.LayerType)).ToList(); //EnumHelper.getTipList();
            return View(Model);
        }
        [HttpPost]
        public IActionResult Index(BackProAlgViewModel Model)
        {
            if (Model.FotoUpload == null)
            {
                ModelState.AddModelError("", "Hata: Tahmin Edilecek Foto Bulunamadı..");
                return View(Model);
            }
            if (Model.TrainTypeId <= 0)
            {
                ModelState.AddModelError("", "Hata:Train Modeli Bulunamadı..");
                return View(Model);
            }

            var TraininModel = _NeuralBpLayerService.SelecetById(Model.TrainTypeId);
            if (TraininModel == null)
            {
                ModelState.AddModelError("", "Hata:Train Modeli Bulunamadı..");
                return View(Model);
            }


            var av_ImageHeight = TraininModel.av_ImageHeight;
            var av_ImageWidth = TraininModel.av_ImageWidth;
            var NumOfPatterns = TraininModel.NumOfPatterns;
            var InputNum = TraininModel.InputNum;
            var HiddenNum = TraininModel.HiddenNum;


            //string[] Images = Directory.GetFiles("C:\\Avni\\School\\Ysa\\BPSimplified_src1\\BPSimplified\\bin\\Debug\\PATTERNS_Orj", "*.bmp");
            var FilePatternPath = DokumanHelper.GetServerMapPath("UserData/PATTERNS");
            string[] Images = Directory.GetFiles(FilePatternPath, "*.bmp");
            TrainingSet = new Dictionary<string, double[]>(Images.Length);
            foreach (string s in Images)
            {
                Bitmap Temp = new Bitmap(s);
                TrainingSet.Add(Path.GetFileNameWithoutExtension(s),
                    ImageProcessing.ToMatrix(Temp, av_ImageHeight, av_ImageWidth));
                Temp.Dispose();
            }



            if (Model.LayerType == 0)
            {

                neuralNetwork = new BackProNeuralNetwork<string>
                    (new BP1Layer<string>(av_ImageHeight * av_ImageWidth, NumOfPatterns), TrainingSet);
            }
            if (Model.LayerType == 1)
            {

                neuralNetwork = new BackProNeuralNetwork<string>
                    (new BP1Layer<string>(av_ImageHeight * av_ImageWidth, NumOfPatterns), TrainingSet);
            }
            else if (Model.LayerType == 2)
            {
                //int InputNum = Int16.Parse(Model.InputUnitCount);

                neuralNetwork = new BackProNeuralNetwork<string>
                    (new BP2Layer<string>(av_ImageHeight * av_ImageWidth, InputNum, NumOfPatterns), TrainingSet);


            }
            else if (Model.LayerType == 3)
            {


                neuralNetwork = new BackProNeuralNetwork<string>
                    (new BP3Layer<string>(av_ImageHeight * av_ImageWidth, InputNum, HiddenNum, NumOfPatterns), TrainingSet);
            }

            neuralNetwork.SetNetworkModel(new lib.Models.BpModel()
            {
                LearningRate = TraininModel.LearningRate,
                OutputLayerList = TraininModel.OutputLayerVirtualList.Select(x => new BackProYsa.lib.Models.OutputLayerEntity
                {
                    Error = x.Error,
                    Id = x.Id,
                    InputSum = x.InputSum,
                    output = x.output,
                    Target = x.Target,
                    Value = x.Value,

                }).ToList(),
                PreInputLayerList = TraininModel.PreInputLayerVirtualList.Select(x => new lib.Models.PreInputEntity
                {
                    Value = x.Value,
                    Weights = x.Weights,
                }).ToList(),
                HiddenLayerList = TraininModel.HiddenLayerVirtualList.Select(x => new lib.Models.HiddenLayerEntity
                {

                    InputSum = x.InputSum,
                    Output = x.Output,
                    Error = x.Error,
                    Weights = x.Weights,

                }).ToList(),
                InputLayerList = TraininModel.InputLayerVirtualList.Select(x => new lib.Models.InputLayerEntity
                {

                    InputSum = x.InputSum,
                    Output = x.Output,
                    Error = x.Error,
                    Weights = x.Weights,

                }).ToList(),

                OutputNum = TraininModel.OutputNum,
                PreInputNum = TraininModel.PreInputNum,
                HiddenNum = TraininModel.HiddenNum,
                InputNum = TraininModel.InputNum,


            });






            string MatchedHigh = "?", MatchedLow = "?";
            double OutputValueHight = 0, OutputValueLow = 0;
            //var FileName = "C:\\Users\\Inforce 2\\Desktop\\test2.bmp";

            //FotoSave
            var FilePath = DokumanHelper.GetServerMapPath("UserData/PredictImage");
            //var FilePath = Path.Combine(WebHostEnvironment.WebRootPath + "~/UserData/UserProfil/");
            //var OldimagePath = Path.Combine(FilePath, Model.FotoUpload.Name ?? "empty");
            //if (System.IO.File.Exists(OldimagePath) && Model.FotoUpload.Name != "no_image.png")
            //{ System.IO.File.Delete(OldimagePath); }

            var FotoName = (DateTime.Now.Ticks.ToString() + "_" + Model.FotoUpload.FileName);
            //user.ProfilFoto = foto.FileName;
            System.Drawing.Image image = System.Drawing.Image.FromStream(Model.FotoUpload.OpenReadStream());
            Size byt = ReSize(image);
            image = image.GetThumbnailImage(byt.Width, byt.Height, null, IntPtr.Zero);
            DokumanHelper.FolderExistOrCreate(FilePath);
            image.Save(Path.Combine(FilePath, FotoName));
            image.Dispose();

            //FotoSave
            var newFilePath = FilePath + "/" + FotoName;
            var Image = new Bitmap(new Bitmap(newFilePath), 153, 220);
            double[] input = ImageProcessing.ToMatrix(Image, av_ImageHeight, av_ImageWidth);

            neuralNetwork.Recognize(input, ref MatchedHigh, ref OutputValueHight,
                ref MatchedLow, ref OutputValueLow);

            var labelMatchedHigh = "Hight: " + MatchedHigh + " (%" + ((int)100 * OutputValueHight).ToString("##") + ")";
            var labelMatchedLow = "Low: " + MatchedLow + " (%" + ((int)100 * OutputValueLow).ToString("##") + ")";

            Model.labelMatchedLow= labelMatchedLow; 
            Model.labelMatchedHigh= labelMatchedHigh;
            Model.MatchedHigh= MatchedHigh;
            Model.MatchedHighLabel = MatchedHigh.Substring(0, 1);
            Model.MatchedLow= MatchedLow;

            return Json(new { Result = Model });

        }

        public Size ReSize(System.Drawing.Image org)
        {
            const int max = 200;// resmimizin max olacak boyutu.
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

    }
}
