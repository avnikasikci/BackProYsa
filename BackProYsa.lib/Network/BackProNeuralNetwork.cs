using BackProYsa.lib.Layers;
using BackProYsa.lib.Models;
using BackProYsa.lib.Propagation;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.lib.Network
{
    [Serializable]
    public class BackProNeuralNetwork<T> where T : IComparable<T>
    {
        private IBackPropagation<T> NeuralNet;
        private double maximumError = 1.0;
        private int maximumIteration = 10000;
        Dictionary<string, double[]> TrainingSet;

        public delegate void IterationChangedCallBack(object o, NeuralEventArgs args);
        public event IterationChangedCallBack IterationChanged = null;

        public BackProNeuralNetwork(IBackPropagation<T> IBackPro, Dictionary<string, double[]> trainingSet)
        {
            NeuralNet = IBackPro;
            TrainingSet = trainingSet;
            NeuralNet.InitializeNetwork(TrainingSet);
        }

        //public NeuralNetwork(BP1Layer<string> bP1Layer, Dictionary<string, double[]> trainingSet)
        //{
        //    NeuralNet = IBackPro;
        //    TrainingSet = trainingSet;
        //    NeuralNet.InitializeNetwork(TrainingSet);
        //}

        public bool TrainNetwork()
        {
            double currentError = 0;
            int currentIteration = 0;
            NeuralEventArgs Args = new NeuralEventArgs();

            do
            {
                currentError = 0;
                foreach (KeyValuePair<string, double[]> p in TrainingSet)
                {
                    NeuralNet.ForwardPropagate(p.Value, p.Key);
                    NeuralNet.BackPropagate();
                    currentError += NeuralNet.GetError();
                }

                currentIteration++;

                if (IterationChanged != null && currentIteration % 5 == 0)
                {
                    Args.CurrentError = currentError;
                    Args.CurrentIteration = currentIteration;
                    IterationChanged(this, Args);
                }

            } while (currentError > maximumError && currentIteration < maximumIteration && !Args.Stop);

            if (IterationChanged != null)
            {
                Args.CurrentError = currentError;
                Args.CurrentIteration = currentIteration;
                IterationChanged(this, Args);
            }

            if (currentIteration >= maximumIteration || Args.Stop)
                return false;//Eğitim başarılı değil

            return true;
        }

        public void Recognize(double[] Input, ref string MatchedHigh, ref double OutputValueHight,
            ref string MatchedLow, ref double OutputValueLow)
        {
            NeuralNet.Recognize(Input, ref MatchedHigh, ref OutputValueHight, ref MatchedLow, ref OutputValueLow);
        }
        public BpModel GetNetWorkModel() {

            var Model = new BpModel();
            Model.LearningRate = NeuralNet.GetLearningRate();
            
            Model.PreInputLayerList = NeuralNet.GetPreInputEntityList();
            Model.InputLayerList=NeuralNet.GetInputLayerEntityList();
            Model.HiddenLayerList = NeuralNet.GetHiddenLayerEntityList();
            Model.OutputLayerList = NeuralNet.GetOutputLayerEntityList();

            Model.PreInputNum = NeuralNet.GetPreInputNum();
            Model.OutputNum = NeuralNet.GetOutputNum();          
            Model.HiddenNum = NeuralNet.GetHiddenNum();          
            Model.OutputNum = NeuralNet.GetOutputNum();          
            

            return Model;
        }
        public void SetNetworkModel(BpModel Model)
        {
            if (Model == null) { return; }

            NeuralNet.SetLearningRate(Model.LearningRate);

            NeuralNet.SetPreInputNum(Model.PreInputNum);    
            NeuralNet.SetOutputNum(Model.OutputNum);
            NeuralNet.SetInputNum(Model.InputNum);
            NeuralNet.SetHiddenNum(Model.HiddenNum);    

            NeuralNet.SetOutputEntityList(Model.OutputLayerList);
            NeuralNet.SetHiddenLayerEntityList(Model.HiddenLayerList);
            NeuralNet.SetInputLayerEntityList(Model.InputLayerList);
            NeuralNet.SetPreInputEntityList(Model.PreInputLayerList);
            
        }

        public void SaveNetwork(string path)
        {

            var SaveData = new BpModel();


            try
            {

                var FilePath2 = DokumanHelper.GetServerMapPath("UserData\\saveNeural.txt");
                BinaryFormatter binFormatter = new BinaryFormatter();

                FileStream writerFileStream2 = new FileStream(FilePath2, FileMode.Create, System.IO.FileAccess.Write);
                binFormatter.Serialize(writerFileStream2, NeuralNet);


                System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                System.IO.Stream stream = new System.IO.MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, NeuralNet);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    //return (T)formatter.Deserialize(stream);                    
                }


               

                stream.CopyTo(writerFileStream2);
                writerFileStream2.Dispose();




                // Create a FileStream that will write data to file. 
                FileStream writerFileStream = new FileStream(FilePath2, FileMode.Create, System.IO.FileAccess.Write);
                binFormatter.Serialize(writerFileStream, NeuralNet);
                // Close the writerFileStream when we are done. 
                writerFileStream.Close();

                var FilePath3 = "C:\\Avni";

                StreamWriter sw = new StreamWriter(FilePath2);
                //Write a line of text
                //sw.WriteLine("Hello World!!");
                ////Write a second line of text
                //sw.WriteLine("From the StreamWriter class");
                //Close the file
                //BinaryFormatter BF3 = new BinaryFormatter();
                //BF3.Serialize(sw, NeuralNet);
                //sw.Close();


                var ds = new DataSet();
                var table = new DataTable("Moo");
                table.Columns.Add("Field1", typeof(string));
                table.Columns.Add("Field2", typeof(int));

                table.Rows.Add("Value1", 1);
                ds.Tables.Add(table);
                ds.AcceptChanges();

                var bf = new BinaryFormatter();

                //using (var stream = File.OpenWrite(FilePath2))
                //{
                //    bf.Serialize(stream, ds);
                //}

                using (FileStream stream2 = new FileStream(FilePath3, FileMode.OpenOrCreate))
                {
                    BinaryFormatter BF2 = new BinaryFormatter();
                    BF2.Serialize(stream2, NeuralNet);
                    stream2.Close();
                    //uploadedFiles.Add(fileName);
                }

                System.IO.Stream ms = File.OpenWrite(path);

                //FileStream FS = new FileStream(path, FileMode.Create);
                BinaryFormatter BF = new BinaryFormatter();
                BF.Serialize(ms, NeuralNet);
                ms.Flush();
                ms.Close();
                ms.Dispose();
                //FS.Close();
            }
            catch (Exception e)
            {

                var error = e.Message;
            }
   
        }

        public void LoadNetwork(string path)
        {
            

            FileStream FS = new FileStream(path, FileMode.Open);
            BinaryFormatter BF = new BinaryFormatter();
            NeuralNet = (IBackPropagation<T>)BF.Deserialize(FS);
            FS.Close();
        }

        public double MaximumError
        {
            get { return maximumError; }
            set { maximumError = value; }
        }

        public int MaximumIteration
        {
            get { return maximumIteration; }
            set { maximumIteration = value; }
        }
    }
}
