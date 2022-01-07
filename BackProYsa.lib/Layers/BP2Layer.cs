using BackProYsa.lib.Models;
using BackProYsa.lib.Propagation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.lib.Layers
{
    [Serializable]
    public class BP2Layer<T> : IBackPropagation<T> where T : IComparable<T>
    {
        private int PreInputNum;
        private int InputNum;
        private int OutputNum;

        private PreInput[] PreInputLayer;
        private Input[] InputLayer;
        private Output<string>[] OutputLayer;

        private double learningRate = 0.2;

        public BP2Layer(int preInputNum, int inputNum, int outputNum)
        {
            PreInputNum = preInputNum;
            InputNum = inputNum;
            OutputNum = outputNum;

            PreInputLayer = new PreInput[PreInputNum];
            InputLayer = new Input[InputNum];
            OutputLayer = new Output<string>[OutputNum];
        }
        public void SetInputNum(int Number)
        {

            InputNum = Number;
        }
        public int GetInputNum()
        {

            return InputNum;
        }


        public void SetHiddenNum(int Number)
        {
            //HiddenNum = Number;
            return;

        }


        public int GetHiddenNum()
        {
            //return HiddenNum;
            return 0;
        }

        public List<InputLayerEntity> GetInputLayerEntityList()
        {
            var ListModel = new List<InputLayerEntity>();
            var Model = new InputLayerEntity();

            //for(var i=0; i< PreInputLayer.Count(); i++)
            //{

            //}


            foreach (var _Input in InputLayer)
            {
                Model = new InputLayerEntity();
                Model.InputSum = _Input.InputSum;
                Model.Output = _Input.Output;
                Model.Error = _Input.Error;
                Model.Weights = _Input.Weights;
                ListModel.Add(Model);

            }
            return ListModel;

        }

        public List<HiddenLayerEntity> GetHiddenLayerEntityList()
        {
            //var ListModel = new List<HiddenLayerEntity>();
            //var Model = new HiddenLayerEntity();

            ////for(var i=0; i< PreInputLayer.Count(); i++)
            ////{

            ////}


            //foreach (var _Input in HiddenLayer)
            //{
            //    Model = new HiddenLayerEntity();
            //    Model.InputSum = _Input.InputSum;
            //    Model.Output = _Input.Output;
            //    Model.Error = _Input.Error;
            //    Model.Weights = _Input.Weights;
            //    ListModel.Add(Model);

            //}
            //return ListModel;

            return new List<HiddenLayerEntity>();
        }

        public void SetInputLayerEntityList(List<InputLayerEntity> setModel)
        {
            InputLayer = new Input[setModel.Count];
            for (var i = 0; i < setModel.Count; i++)
            {
                InputLayer[i].InputSum = setModel[i].InputSum;
                InputLayer[i].Output = setModel[i].Output;
                InputLayer[i].Error = setModel[i].Error;
                InputLayer[i].Weights = setModel[i].Weights;
            }

            return;
        }

        public void SetHiddenLayerEntityList(List<HiddenLayerEntity> setModel)
        {
            //HiddenLayer = new Hidden[setModel.Count];
            //for (var i = 0; i < setModel.Count; i++)
            //{
            //    HiddenLayer[i].InputSum = setModel[i].InputSum;
            //    HiddenLayer[i].Output = setModel[i].Output;
            //    HiddenLayer[i].Error = setModel[i].Error;
            //    HiddenLayer[i].Weights = setModel[i].Weights;
            //}

            return;
        }





        public void SetPreInputNum(int Number)
        {
            PreInputNum = Number;
        }

        public void SetOutputNum(int OutputNumber)
        {
            OutputNum = OutputNumber;
        }

        public void SetLearningRate(double LearningRate)
        {
            learningRate = LearningRate;
        }

        public void SetPreInputEntityList(List<PreInputEntity> SetModel)
        {
            PreInputLayer = new PreInput[SetModel.Count];
            for (var i = 0; i < SetModel.Count; i++)
            {
                PreInputLayer[i].Value = SetModel[i].Value;
                PreInputLayer[i].Weights = SetModel[i].Weights;
            }
        }

        public void SetOutputEntityList(List<OutputLayerEntity> SetModel)
        {
            OutputLayer = new Output<string>[SetModel.Count];
            for (var i = 0; i < SetModel.Count; i++)
            {
                OutputLayer[i].InputSum = SetModel[i].InputSum;
                OutputLayer[i].output = SetModel[i].output;
                OutputLayer[i].Error = SetModel[i].Error;
                OutputLayer[i].Target = SetModel[i].Target;
                OutputLayer[i].Value = SetModel[i].Value;
            }
        }
        public int GetPreInputNum()
        {
            return PreInputNum;
        }

        public int GetOutputNum()
        {
            return OutputNum;
        }

        public double GetLearningRate()
        {
            return learningRate;
        }

        public List<PreInputEntity> GetPreInputEntityList()
        {
            var ListModel = new List<PreInputEntity>();
            var Model = new PreInputEntity();

            //for(var i=0; i< PreInputLayer.Count(); i++)
            //{

            //}


            foreach (var _Input in PreInputLayer)
            {
                Model = new PreInputEntity();
                Model.Weights = _Input.Weights;
                Model.Value = _Input.Value;
                ListModel.Add(Model);

            }
            return ListModel;
        }

        public List<OutputLayerEntity> GetOutputLayerEntityList()
        {
            var ListModel = new List<OutputLayerEntity>();
            var Model = new OutputLayerEntity();

            //for(var i=0; i< PreInputLayer.Count(); i++)
            //{

            //}


            foreach (var _Input in OutputLayer)
            {
                Model = new OutputLayerEntity();
                Model.Id = 1;
                Model.InputSum = _Input.InputSum;
                Model.output = _Input.output;
                Model.Error = _Input.Error;
                Model.Target = _Input.Target;
                Model.Value = (_Input.Value != null) ? _Input.Value.ToString() : "";

                ListModel.Add(Model);

            }
            return ListModel;
        }

        #region IBackPropagation<T> Members
        public void BackPropagate()
        {
            int i, j;
            double total;

            ///Giriş Katmanının Hatasını Düzeltin
 
            for (i = 0; i < InputNum; i++)
            {
                total = 0.0;
                for (j = 0; j < OutputNum; j++)
                {
                    total += InputLayer[i].Weights[j] * OutputLayer[j].Error;
                }
                InputLayer[i].Error = total;
            }

            //İlk Katmanın Ağırlıklarını Güncelle
            for (i = 0; i < InputNum; i++)
            {
                for (j = 0; j < PreInputNum; j++)
                {
                    PreInputLayer[j].Weights[i] +=
                        learningRate * InputLayer[i].Error * PreInputLayer[j].Value;
                }
            }

            //İkinci Katmanın Ağırlıklarını Güncelle
            for (i = 0; i < OutputNum; i++)
            {
                for (j = 0; j < InputNum; j++)
                {
                    InputLayer[j].Weights[i] +=
                        learningRate * OutputLayer[i].Error * InputLayer[j].Output;
                }
            }
        }

        public double F(double x)
        {
            return (1 / (1 + Math.Exp(-x)));
        }

        public void ForwardPropagate(double[] pattern, string output)
        {
            int i, j;
            double total = 0.0;

            // Girişi ağa uygula
            for (i = 0; i < PreInputNum; i++)
            {
                PreInputLayer[i].Value = pattern[i];
            }

            //Birinci(Giriş) Katmanın Giriş ve Çıkışlarını Hesapla
            for (i = 0; i < InputNum; i++)
            {
                total = 0.0;
                for (j = 0; j < PreInputNum; j++)
                {
                    total += PreInputLayer[j].Value * PreInputLayer[j].Weights[i];
                }

                InputLayer[i].InputSum = total;
                InputLayer[i].Output = F(total);
            }

            //İkinci(Çıkış) Katmanın Girdilerini, Çıktılarını, Hedeflerini ve Hatalarını Hesaplayın
            for (i = 0; i < OutputNum; i++)
            {
                total = 0.0;
                for (j = 0; j < InputNum; j++)
                {
                    total += InputLayer[j].Output * InputLayer[j].Weights[i];
                }

                OutputLayer[i].InputSum = total;
                OutputLayer[i].output = F(total);
                OutputLayer[i].Target = OutputLayer[i].Value.CompareTo(output) == 0 ? 1.0 : 0.0;
                OutputLayer[i].Error = (OutputLayer[i].Target - OutputLayer[i].output) * (OutputLayer[i].output) * (1 - OutputLayer[i].output);
            }
        }

        public double GetError()
        {
            double total = 0.0;
            for (int j = 0; j < OutputNum; j++)
            {
                total += Math.Pow((OutputLayer[j].Target - OutputLayer[j].output), 2) / 2;
            }
            return total;
        }

        public void InitializeNetwork(Dictionary<string, double[]> TrainingSet)
        {
            int i, j;
            Random rand = new Random();
            for (i = 0; i < PreInputNum; i++)
            {
                PreInputLayer[i].Weights = new double[InputNum];
                for (j = 0; j < InputNum; j++)
                {
                    PreInputLayer[i].Weights[j] = 0.01 + ((double)rand.Next(0, 5) / 100);
                }
            }

            for (i = 0; i < InputNum; i++)
            {
                InputLayer[i].Weights = new double[OutputNum];
                for (j = 0; j < OutputNum; j++)
                {
                    InputLayer[i].Weights[j] = 0.01 + ((double)rand.Next(0, 5) / 100);
                }
            }

            int k = 0;
            
            foreach (KeyValuePair<string, double[]> p in TrainingSet)
            {
                OutputLayer[k++].Value = p.Key;
            }
        }

        public void Recognize(double[] Input, ref string MatchedHigh, ref double OutputValueHight, ref string MatchedLow, ref double OutputValueLow)
        {
            int i, j;
            double total = 0.0;
            double max = -1;

            /// Girişi ağa uygula
            for (i = 0; i < PreInputNum; i++)
            {
                PreInputLayer[i].Value = Input[i];
            }

            //Giriş Katmanının Giriş ve Çıkışlarını Hesapla
            for (i = 0; i < InputNum; i++)
            {
                total = 0.0;
                for (j = 0; j < PreInputNum; j++)
                {
                    total += PreInputLayer[j].Value * PreInputLayer[j].Weights[i];
                }
                InputLayer[i].InputSum = total;
                InputLayer[i].Output = F(total);
            }

            //[İki] En Yüksek Çıkışı Bul
            for (i = 0; i < OutputNum; i++)
            {
                total = 0.0;
                for (j = 0; j < InputNum; j++)
                {
                    total += InputLayer[j].Output * InputLayer[j].Weights[i];
                }
                OutputLayer[i].InputSum = total;
                OutputLayer[i].output = F(total);
                if (OutputLayer[i].output > max)
                {
                    MatchedLow = MatchedHigh;
                    OutputValueLow = max;
                    max = OutputLayer[i].output;
                    MatchedHigh = OutputLayer[i].Value;
                    OutputValueHight = max;
                }
            }
        }

        #endregion

        public double LearningRate
        {
            get { return learningRate; }
            set { learningRate = value; }
        }
    }
}
