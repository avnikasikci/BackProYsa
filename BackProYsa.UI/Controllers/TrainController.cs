using BackProYsa.DataAccess.Domain;
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
    public class TrainController : Controller
    {
        private Dictionary<string, double[]> TrainingSet;
        private BackProYsa.lib.Network.BackProNeuralNetwork<string> neuralNetwork;

        private readonly INeuralBpLayerService _NeuralBpLayerService;


        public TrainController(
            INeuralBpLayerService NeuralBpLayerService
            )
        {
            this._NeuralBpLayerService = NeuralBpLayerService;

        }



        public IActionResult Index()
        {
            var Model = new TrainModel();

            var AllTrainData = _NeuralBpLayerService.GetAll().ToList();
            Model.NeuralBpLayerList = AllTrainData;
            Model.LayerTypeList = EnumHelper.GetSelectList(typeof(EnumCollection.LayerType)).ToList(); //EnumHelper.getTipList();



            return View(Model);
        }

        public JsonResult TrainDelete(int? Id)
        {

            if (Id.HasValue)
            {
                var Entity = _NeuralBpLayerService.SelecetById(Id.Value);
                Entity.Active = false;
                _NeuralBpLayerService.Save(Entity);
            }
            return Json(new { Result = "Silme İşlemi Başarılı", Answer = "Success" });

        }




        [HttpPost]
        public IActionResult TrainSave(TrainModel Model)
        {
            var SaveNeural = new NeuralBpLayer();
            SaveNeural.Name = Model.Name;
            SaveNeural.LayerType = Model.LayerType;


            try
            {
                //NameValueCollection AppSettings = ConfigurationManager.AppSettings;

                //comboBoxLayers.SelectedIndex = (Int16.Parse(AppSettings["NumOfLayers"]) - 1);
                //textBoxTrainingBrowse.Text = Path.GetFullPath(AppSettings["PatternsDirectory"]);
                //textBoxMaxError.Text = AppSettings["MaxError"];
                //Model.LayersNumber = TrainSet.LayerType;
                Model.MaxErrorCount = "1.1";

                //string[] Images = Directory.GetFiles("C:\\Avni\\School\\Ysa\\BPSimplified_src1\\BPSimplified\\bin\\Debug\\PATTERNS", "*.bmp");
                

                var PredictPath = DokumanHelper.GetServerMapPath("UserData/PATTERNS");
                string[] Images = Directory.GetFiles(PredictPath, "*.bmp");
                var NumOfPatterns = Images.Length;

                var av_ImageHeight = 0;
                var av_ImageWidth = 0;

                foreach (string s in Images)
                {
                    Bitmap Temp = new Bitmap(s);
                    av_ImageHeight += Temp.Height;
                    av_ImageWidth += Temp.Width;
                    Temp.Dispose();
                }
                av_ImageHeight /= NumOfPatterns;
                av_ImageWidth /= NumOfPatterns;

                int networkInput = av_ImageHeight * av_ImageWidth;

                Model.InputUnitCount = ((int)((double)(networkInput + NumOfPatterns) * .33)).ToString();
                Model.HiddenUnitCount = ((int)((double)(networkInput + NumOfPatterns) * .11)).ToString();
                Model.OutputCount = NumOfPatterns.ToString();


                //string[] Patterns = Directory.GetFiles("C:\\Avni\\School\\Ysa\\BPSimplified_src1\\BPSimplified\\bin\\Debug\\PATTERNS_Orj", "*.bmp");


                TrainingSet = new Dictionary<string, double[]>(Images.Length);
                foreach (string s in Images)
                {
                    Bitmap Temp = new Bitmap(s);
                    TrainingSet.Add(Path.GetFileNameWithoutExtension(s),
                        ImageProcessing.ToMatrix(Temp, av_ImageHeight, av_ImageWidth));
                    Temp.Dispose();
                }


                int InputNum = Int16.Parse(Model.InputUnitCount);
                int HiddenNum = Int16.Parse(Model.HiddenUnitCount);

                if (TrainingSet == null)
                    throw new Exception("Unable to Create Neural Network As There is No Data to Train..");
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

                neuralNetwork.TrainNetwork();
                var GetTrainModel = neuralNetwork.GetNetWorkModel();


                ///Save network
                ///

                SaveNeural.LearningRate = GetTrainModel.LearningRate;
                //SaveNeural.OutputLayerList = GetTrainModel.OutputLayerList.Select(x => new OutputLayerEntity()
                SaveNeural.OutputLayerVirtualList = GetTrainModel.OutputLayerList.Select(x => new OutputLayerEntity()
                {
                    Error = x.Error,
                    Id = x.Id,
                    InputSum = x.InputSum,
                    output = x.output,
                    Target = x.Target,
                    Value = x.Value,
                }).ToList();
                //SaveNeural.PreInputLayerList = GetTrainModel.PreInputLayerList.Select(x => new PreInputEntity()
                SaveNeural.PreInputLayerVirtualList = GetTrainModel.PreInputLayerList.Select(x => new PreInputEntity()
                {
                    Value = x.Value,
                    Weights = x.Weights,
                }).ToList();

                SaveNeural.InputLayerVirtualList=GetTrainModel.InputLayerList.Select(x=> new InputEntity()
                {
           
                    InputSum = x.InputSum,
                    Output = x.Output,
                    Error = x.Error,
                    Weights = x.Weights,
            
                }).ToList();
                SaveNeural.HiddenLayerVirtualList = GetTrainModel.HiddenLayerList.Select(x => new HiddenEntity()
                {

                    InputSum = x.InputSum,
                    Output = x.Output,
                    Error = x.Error,
                    Weights = x.Weights,

                }).ToList();

                SaveNeural.OutputNum = GetTrainModel.OutputNum;
                SaveNeural.PreInputNum = GetTrainModel.PreInputNum;
                ///Save network
                ///

                //Save Setting
                SaveNeural.av_ImageHeight = av_ImageHeight;
                SaveNeural.av_ImageWidth = av_ImageWidth;
                
                SaveNeural.InputNum = InputNum;
                SaveNeural.HiddenNum = HiddenNum;

                SaveNeural.InputNum = GetTrainModel.InputNum;
                SaveNeural.HiddenNum = GetTrainModel.HiddenNum;

                SaveNeural.NumOfPatterns = NumOfPatterns;
                SaveNeural.Active = true;
                _NeuralBpLayerService.Save(SaveNeural);

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error Initializing Settings: " + ex.Message, "Error",
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ModelState.AddModelError("", "Hata: InputLayerCount Boş Olamaz.." + ex.Message);
                return View(Model);
            }





            //var ModelSetting = InitializeSettings(Model);


            return View(Model);
        }
        private SettingsViewModel InitializeSettings(TrainModel TrainSet)
        {

            var Model = new SettingsViewModel();


            var SaveNeural = new NeuralBpLayer();
            SaveNeural.Name = TrainSet.Name;
            SaveNeural.LayerType = TrainSet.LayerType;

            //textBoxState.AppendText("Initializing Settings..");

            try
            {
                //NameValueCollection AppSettings = ConfigurationManager.AppSettings;

                //comboBoxLayers.SelectedIndex = (Int16.Parse(AppSettings["NumOfLayers"]) - 1);
                //textBoxTrainingBrowse.Text = Path.GetFullPath(AppSettings["PatternsDirectory"]);
                //textBoxMaxError.Text = AppSettings["MaxError"];
                Model.LayersNumber = TrainSet.LayerType;
                Model.MaxErrorCount = "1.1";

                //string[] Images = Directory.GetFiles("C:\\Avni\\School\\Ysa\\BPSimplified_src1\\BPSimplified\\bin\\Debug\\PATTERNS", "*.bmp");
                string[] Images = Directory.GetFiles("C:\\Avni\\School\\Ysa\\BPSimplified_src1\\BPSimplified\\bin\\Debug\\PATTERNS_Orj", "*.bmp");

                var NumOfPatterns = Images.Length;

                var av_ImageHeight = 0;
                var av_ImageWidth = 0;

                foreach (string s in Images)
                {
                    Bitmap Temp = new Bitmap(s);
                    av_ImageHeight += Temp.Height;
                    av_ImageWidth += Temp.Width;
                    Temp.Dispose();
                }
                av_ImageHeight /= NumOfPatterns;
                av_ImageWidth /= NumOfPatterns;

                int networkInput = av_ImageHeight * av_ImageWidth;

                Model.InputUnitCount = ((int)((double)(networkInput + NumOfPatterns) * .33)).ToString();
                Model.HiddenUnitCount = ((int)((double)(networkInput + NumOfPatterns) * .11)).ToString();
                Model.OutputCount = NumOfPatterns.ToString();


                //string[] Patterns = Directory.GetFiles("C:\\Avni\\School\\Ysa\\BPSimplified_src1\\BPSimplified\\bin\\Debug\\PATTERNS_Orj", "*.bmp");


                TrainingSet = new Dictionary<string, double[]>(Images.Length);
                foreach (string s in Images)
                {
                    Bitmap Temp = new Bitmap(s);
                    TrainingSet.Add(Path.GetFileNameWithoutExtension(s),
                        ImageProcessing.ToMatrix(Temp, av_ImageHeight, av_ImageWidth));
                    Temp.Dispose();
                }


                int InputNum = Int16.Parse(Model.InputUnitCount);
                int HiddenNum = Int16.Parse(Model.HiddenUnitCount);

                if (TrainingSet == null)
                    throw new Exception("Unable to Create Neural Network As There is No Data to Train..");
                if (Model.LayersNumber == 0)
                {

                    neuralNetwork = new BackProNeuralNetwork<string>
                        (new BP1Layer<string>(av_ImageHeight * av_ImageWidth, NumOfPatterns), TrainingSet);
                }
                else if (Model.LayersNumber == 1)
                {
                    //int InputNum = Int16.Parse(Model.InputUnitCount);

                    neuralNetwork = new BackProNeuralNetwork<string>
                        (new BP2Layer<string>(av_ImageHeight * av_ImageWidth, InputNum, NumOfPatterns), TrainingSet);


                }
                else if (Model.LayersNumber == 2)
                {


                    neuralNetwork = new BackProNeuralNetwork<string>
                        (new BP3Layer<string>(av_ImageHeight * av_ImageWidth, InputNum, HiddenNum, NumOfPatterns), TrainingSet);
                }

                neuralNetwork.TrainNetwork();
                var GetTrainModel = neuralNetwork.GetNetWorkModel();


                ///Save network
                ///

                SaveNeural.LearningRate = GetTrainModel.LearningRate;
                //SaveNeural.OutputLayerList = GetTrainModel.OutputLayerList.Select(x => new OutputLayerEntity()
                SaveNeural.OutputLayerVirtualList = GetTrainModel.OutputLayerList.Select(x => new OutputLayerEntity()
                {
                    Error = x.Error,
                    Id = x.Id,
                    InputSum = x.InputSum,
                    output = x.output,
                    Target = x.Target,
                    Value = x.Value,
                }).ToList();
                //SaveNeural.PreInputLayerList = GetTrainModel.PreInputLayerList.Select(x=> new PreInputEntity()
                SaveNeural.PreInputLayerVirtualList = GetTrainModel.PreInputLayerList.Select(x => new PreInputEntity()
                {
                    Value = x.Value,
                    Weights = x.Weights,
                }).ToList();
                SaveNeural.OutputNum = GetTrainModel.OutputNum;
                SaveNeural.PreInputNum = GetTrainModel.PreInputNum;
                ///Save network
                ///

                //Save Setting
                SaveNeural.av_ImageHeight = av_ImageHeight;
                SaveNeural.av_ImageWidth = av_ImageWidth;
                SaveNeural.InputNum = InputNum;
                SaveNeural.HiddenNum = HiddenNum;
                SaveNeural.NumOfPatterns = NumOfPatterns;
                _NeuralBpLayerService.Save(SaveNeural);

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error Initializing Settings: " + ex.Message, "Error",
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return Model;
        }

    }
}
