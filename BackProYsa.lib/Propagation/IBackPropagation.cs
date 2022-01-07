using BackProYsa.lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.lib.Propagation
{

    //[Serializable]
    public interface IBackPropagation<T>
    {
        int GetPreInputNum();
        int GetInputNum();
        int GetHiddenNum();
        int GetOutputNum();
        double GetLearningRate();
        List<PreInputEntity> GetPreInputEntityList();
        List<OutputLayerEntity> GetOutputLayerEntityList();
        List<InputLayerEntity> GetInputLayerEntityList();
        List<HiddenLayerEntity> GetHiddenLayerEntityList();

        void SetHiddenNum(int Number);
        void SetInputNum(int Number);
        void SetPreInputNum(int Number);
        void SetOutputNum(int OutputNumber);
        void SetLearningRate(double LearningRate);
        void SetPreInputEntityList(List<PreInputEntity> SetModel);   
        void SetOutputEntityList(List<OutputLayerEntity> setModel); 
        void SetInputLayerEntityList(List<InputLayerEntity> setModel); 
        void SetHiddenLayerEntityList(List<HiddenLayerEntity> setModel); 
       



        void BackPropagate();
        double F(double x);
        void ForwardPropagate(double[] pattern, string output);
        double GetError();
        void InitializeNetwork(Dictionary<string, double[]> TrainingSet);
        void Recognize(double[] Input, ref string MatchedHigh, ref double OutputValueHight,
                                        ref string MatchedLow, ref double OutputValueLow);
    }

}
