using BackProYsa.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackProYsa.lib.Network;
using BackProYsa.lib;
using BackProYsa.lib.Layers;
using System.Runtime.Serialization.Formatters.Binary;
using BackProYsa.DataAccess.Services.Interfaces;

namespace BackProYsa.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;        
        //private BackProYsa.lib.Network.NeuralNetwork<string> neuralNetwork = null;
        private BackProYsa.lib.Network.BackProNeuralNetwork<string> neuralNetwork;

        //Data Members Required For Neural Network
        private Dictionary<string, double[]> TrainingSet;
        //private Dictionary<string, double[]> TrainingSet = null;
        private int av_ImageHeight = 0;
        private int av_ImageWidth = 0;
        private int NumOfPatterns = 0;

        //For Asynchronized Programming Instead of Handling Threads
        private delegate bool TrainingCallBack();
        private AsyncCallback asyCallBack = null;
        private IAsyncResult res = null;
        private ManualResetEvent ManualReset = null;
        SettingsViewModel DefaultModel =new SettingsViewModel();

        private DateTime DTStart;


        private readonly INeuralBpLayerService _NeuralBpLayerService;


        public HomeController(
            INeuralBpLayerService NeuralBpLayerService,
            ILogger<HomeController> logger
            )
        {
            this._NeuralBpLayerService = NeuralBpLayerService;
            _logger = logger;


        }

        private SettingsViewModel InitializeSettings()
        {
            var Model = DefaultModel;
            //textBoxState.AppendText("Initializing Settings..");

            try
            {
                //NameValueCollection AppSettings = ConfigurationManager.AppSettings;

                //comboBoxLayers.SelectedIndex = (Int16.Parse(AppSettings["NumOfLayers"]) - 1);
                //textBoxTrainingBrowse.Text = Path.GetFullPath(AppSettings["PatternsDirectory"]);
                //textBoxMaxError.Text = AppSettings["MaxError"];
                Model.LayersNumber = 0;
                Model.MaxErrorCount = "1.1";

                //string[] Images = Directory.GetFiles("C:\\Avni\\School\\Ysa\\BPSimplified_src1\\BPSimplified\\bin\\Debug\\PATTERNS", "*.bmp");
                string[] Images = Directory.GetFiles("C:\\Avni\\School\\Ysa\\BPSimplified_src1\\BPSimplified\\bin\\Debug\\PATTERNS_Orj", "*.bmp");

                NumOfPatterns = Images.Length;

                av_ImageHeight = 0;
                av_ImageWidth = 0;

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


            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error Initializing Settings: " + ex.Message, "Error",
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //textBoxState.AppendText("Done!\r\n");
            return Model;
        }


        public IActionResult Index()
        {
            //var Model = new SettingsViewModel();

            //InitializeComponent();
            var Model =InitializeSettings();

            GenerateTrainingSet(Model);
            CreateNeuralNetwork(Model);

            asyCallBack = new AsyncCallback(TraningCompleted);
            ManualReset = new ManualResetEvent(false);


            return View(Model);
        }
        private void TraningCompleted(IAsyncResult result)
        {
            if (result.AsyncState is TrainingCallBack)
            {
                TrainingCallBack TR = (TrainingCallBack)result.AsyncState;

                bool isSuccess = TR.EndInvoke(res);
                if (isSuccess)
                {
                    //UpdateState("Completed Training Process Successfully\r\n");

                }
                else
                {
                    //UpdateState("Training Process is Aborted or Exceed Maximum Iteration\r\n");
                }
                //SetButtons(true);
                //timer1.Stop();
            }
        }
        
        [HttpPost]
        public IActionResult Index(HomeViewModel Model)
        {
            return View(Model);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public string CallTranin()
        {
            //var path = "C:\\Avni\\School\\Ysa\\BackProYsa\\BackProYsa.Web\\BackProYsa.UI\\wwwroot";

            //var FilePath2 = DokumanHelper.GetServerMapPath("NetworkSave\\test.net");
            //DokumanHelper.FolderExistOrCreate(FilePath2);
            //System.IO.File.Create(FilePath2);
            //using (var fs = new FileStream("C:\\inetpub\\wwwroot", FileMode.Open, FileAccess.Read))
            //{

            //}

            //using (Stream stream = new MemoryStream()) {


            //    BinaryFormatter BF2 = new BinaryFormatter();
            //    BF2.Serialize(stream, neuralNetwork);
                
            //}
            

            //using (FileStream stream = new FileStream(path, FileMode.Create))
            //{
            //    //BinaryFormatter BF2 = new BinaryFormatter();
            //    //BF2.Serialize(stream, NeuralNet);
            //    stream.Close();
            //    //uploadedFiles.Add(fileName);
            //}
            //UpdateState("Began Training Process..\r\n");
            //SetButtons(false);
            //ManualReset.Reset();

            //TrainingCallBack TR = new TrainingCallBack(neuralNetwork.Train);
            //res = TR.BeginInvoke(asyCallBack, TR);
            //DTStart = DateTime.Now;
            //timer1.Start();
            var Model=InitializeSettings();            

            GenerateTrainingSet(Model);
            CreateNeuralNetwork(Model);
            neuralNetwork.TrainNetwork();
            var SaveModel = neuralNetwork.GetNetWorkModel();

            this.buttonRecognize_Click();

            //var FilePath = DokumanHelper.GetServerMapPath("NetworkSave");

            //neuralNetwork.SaveNetwork(FilePath);


            //buttonRecognize_Click();
            ////DokumanHelper.FolderExistOrCreate(FilePath);


            //////string uploadPath = Server.MapPath("~/uploads");



            return "fonk Başlatıldı...";
        }

        public string buttonRecognize_Click()
        {
            string MatchedHigh = "?", MatchedLow = "?";
            double OutputValueHight = 0, OutputValueLow = 0;
            //var FileName = "C:\\Users\\Inforce 2\\Desktop\\test2.bmp";
            var FileName = "C:\\Avni\\School\\Ysa\\BPSimplified_src1\\BPSimplified\\bin\\Debug\\PATTERNS\\0.bmp";
            var Image=new Bitmap(new Bitmap(FileName), 153, 220);
            double[] input = ImageProcessing.ToMatrix(Image       ,         av_ImageHeight, av_ImageWidth);

            neuralNetwork.Recognize(input, ref MatchedHigh, ref OutputValueHight,
                ref MatchedLow, ref OutputValueLow);

            ShowRecognitionResults(MatchedHigh, MatchedLow, OutputValueHight, OutputValueLow);
            return "";

        }
        private void ShowRecognitionResults(string MatchedHigh, string MatchedLow, double OutputValueHight, double OutputValueLow)
        {
            //labelMatchedHigh.Text = "Hight: " + MatchedHigh + " (%" + ((int)100 * OutputValueHight).ToString("##") + ")";
            var labelMatchedHigh = "Hight: " + MatchedHigh + " (%" + ((int)100 * OutputValueHight).ToString("##") + ")";
            var labelMatchedLow = "Low: " + MatchedLow + " (%" + ((int)100 * OutputValueLow).ToString("##") + ")";


            //var pictureBoxInputImage= new Bitmap(drawingPanel1.ImageOnPanel, pictureBoxInput.Width, pictureBoxInput.Height);
            //pictureBoxInput.Image = new Bitmap(drawingPanel1.ImageOnPanel,
            //    pictureBoxInput.Width, pictureBoxInput.Height);

            //if (MatchedHigh != "?")
            //    var pictureBoxMatchedHighImage = new Bitmap(new Bitmap(textBoxTrainingBrowse.Text + "\\" + MatchedHigh + ".bmp"),
            //    //pictureBoxMatchedHigh.Image = new Bitmap(new Bitmap(textBoxTrainingBrowse.Text + "\\" + MatchedHigh + ".bmp"),
            //        12, 12);

            //if (MatchedLow != "?")
            //    pictureBoxMatchedLow.Image = new Bitmap(new Bitmap(textBoxTrainingBrowse.Text + "\\" + MatchedLow + ".bmp"),
            //        pictureBoxMatchedLow.Width, pictureBoxMatchedLow.Height);
        }


        public IActionResult Settings()
        {
            var Model=new SettingsViewModel();
            Model.LayersNumber = 1;
            Model.MaxErrorCount = "1.1";
            Model = DefaultModel;
            return View(Model);
        }



        [HttpPost]
        public IActionResult Settings(SettingsViewModel Model)

        {
            //string[] Images = Directory.GetFiles("C:\\Avni\\School\\Ysa\\BPSimplified_src1\\BPSimplified\\bin\\Debug\\PATTERNS", "*.bmp");
            //int NumOfPattern = Images.Length;
            


            if (Model == null) return View();
            if (!string.IsNullOrEmpty(Model.InputUnitCount) || Model.InputUnitCount=="0") {

                ModelState.AddModelError("", "Hata: InputLayerCount Boş Olamaz..");
                return View(Model);
            }

            


            string[] Images = Directory.GetFiles("C:\\Avni\\School\\Ysa\\BPSimplified_src1\\BPSimplified\\bin\\Debug\\PATTERNS", "*.bmp");
            NumOfPatterns = Images.Length;

            av_ImageHeight = 0;
            av_ImageWidth = 0;

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
            Model.InputUnitCount = ((int)((double)(networkInput + NumOfPatterns) * .5)).ToString();
            Model.HiddenUnitCount = ((int)((double)(networkInput + NumOfPatterns) * .3)).ToString();
            Model.OutputCount = NumOfPatterns.ToString();
            //textBoxInputUnit.Text = ((int)((double)(networkInput + NumOfPatterns) * .5)).ToString();
            //textBoxHiddenUnit.Text = ((int)((double)(networkInput + NumOfPatterns) * .3)).ToString();
            //textBoxOutputUnit.Text = NumOfPatterns.ToString();


            //buttonRecognize.Enabled = false;
            //buttonSave.Enabled = false;

            //textBoxState.AppendText("Done!\r\n");

            GenerateTrainingSet(Model);
            CreateNeuralNetwork(Model);


            return View(Model);
        }
        private void GenerateTrainingSet(SettingsViewModel Model)
        {
            //textBoxState.AppendText("Generating Training Set..");

            //string[] Patterns = Directory.GetFiles(textBoxTrainingBrowse.Text, "*.bmp");
            string[] Patterns = Directory.GetFiles("C:\\Avni\\School\\Ysa\\BPSimplified_src1\\BPSimplified\\bin\\Debug\\PATTERNS_Orj", "*.bmp");


            TrainingSet = new Dictionary<string, double[]>(Patterns.Length);
            foreach (string s in Patterns)
            {
                Bitmap Temp = new Bitmap(s);
                TrainingSet.Add(Path.GetFileNameWithoutExtension(s),
                    ImageProcessing.ToMatrix(Temp, av_ImageHeight, av_ImageWidth));
                Temp.Dispose();
            }

            //textBoxState.AppendText("Done!\r\n");
        }
        private void CreateNeuralNetwork(SettingsViewModel Model)
        {
            if (TrainingSet == null)
                throw new Exception("Unable to Create Neural Network As There is No Data to Train..");
            if (Model.LayersNumber == 0)
            {
                
                neuralNetwork = new BackProNeuralNetwork<string>
                    (new BP1Layer<string>(av_ImageHeight * av_ImageWidth, NumOfPatterns), TrainingSet);
            }
            else if (Model.LayersNumber == 1)
            {
                int InputNum = Int16.Parse(Model.InputUnitCount);

                neuralNetwork = new BackProNeuralNetwork<string>
                    (new BP2Layer<string>(av_ImageHeight * av_ImageWidth, InputNum, NumOfPatterns), TrainingSet);


            }
            else if (Model.LayersNumber == 2)
            {
                int InputNum = Int16.Parse(Model.InputUnitCount);
                int HiddenNum = Int16.Parse(Model.HiddenUnitCount);

                neuralNetwork = new BackProNeuralNetwork<string>
                    (new BP3Layer<string>(av_ImageHeight * av_ImageWidth, InputNum, HiddenNum, NumOfPatterns), TrainingSet);
            }
            //neuralNetwork.IterationChanged +=
            //    new NeuralNetwork<string>.IterationChangedCallBack(neuralNetwork_IterationChanged);

            //neuralNetwork.MaximumError = Double.Parse(textBoxMaxError.Text);

            //if (comboBoxLayers.SelectedIndex == 0)
            //{

            //    neuralNetwork = new NeuralNetwork<string>
            //        (new BP1Layer<string>(av_ImageHeight * av_ImageWidth, NumOfPatterns), TrainingSet);

            //}
            //else if (comboBoxLayers.SelectedIndex == 1)
            //{
            //    int InputNum = Int16.Parse(textBoxInputUnit.Text);

            //    neuralNetwork = new NeuralNetwork<string>
            //        (new BP2Layer<string>(av_ImageHeight * av_ImageWidth, InputNum, NumOfPatterns), TrainingSet);

            //}
            //else if (comboBoxLayers.SelectedIndex == 2)
            //{
            //    int InputNum = Int16.Parse(textBoxInputUnit.Text);
            //    int HiddenNum = Int16.Parse(textBoxHiddenUnit.Text);

            //    neuralNetwork = new NeuralNetwork<string>
            //        (new BP3Layer<string>(av_ImageHeight * av_ImageWidth, InputNum, HiddenNum, NumOfPatterns), TrainingSet);

            //}

            //neuralNetwork.IterationChanged +=
            //    new NeuralNetwork<string>.IterationChangedCallBack(neuralNetwork_IterationChanged);

            //neuralNetwork.MaximumError = Double.Parse(textBoxMaxError.Text);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
