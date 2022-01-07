using BackProYsa.Infrastructure.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.DataAccess.Domain
{
    public class NeuralBpLayer
    {
        //can be entered by the user 
        public int Id { get; set; }
        public string Name { get; set; }
        public int LayerType { get; set; }
        public string InputUnitCount { get; set; }
        public string HiddenUnitCount { get; set; }
        public string OutputCount { get; set; }
        public string MaxErrorCount { get; set; }
        //can be entered by the user 


        //can not be entered by the user 

        public int av_ImageHeight { get; set; }
        public int av_ImageWidth { get; set; }
        public int InputNum { get; set; }
        public int HiddenNum { get; set; }
        public int NumOfPatterns { get; set; }


        public double LearningRate { get; set; }
        //public List<OutputLayerEntity> OutputLayerList { get; set; }
        //public List<PreInputEntity> PreInputLayerList { get; set; }
        public int OutputNum { get; set; }
        public int PreInputNum { get; set; }

        public bool Active { get; set; }
        //can not be entered by the user 

        public string InputLayerListStr { get; set; }
        [NotMapped]
        public virtual IList<InputEntity> InputLayerVirtualList
        {
            get => (UtilityJson.JsonDeserialize<IList<InputEntity>>(InputLayerListStr));
            set { InputLayerListStr = UtilityJson.JsonSerialize(value); }
        }

        public string HiddenLayerListStr { get; set; }
        [NotMapped]
        public virtual IList<HiddenEntity> HiddenLayerVirtualList
        {
            get => (UtilityJson.JsonDeserialize<IList<HiddenEntity>>(HiddenLayerListStr));
            set { HiddenLayerListStr = UtilityJson.JsonSerialize(value); }
        }



        public string OutputLayerListStr { get; set; }
        [NotMapped]
        public virtual IList<OutputLayerEntity> OutputLayerVirtualList
        {
            get => (UtilityJson.JsonDeserialize<IList<OutputLayerEntity>>(OutputLayerListStr));
            set { OutputLayerListStr = UtilityJson.JsonSerialize(value); }
        }


        public string PreInputLayerListStr { get; set; }
        [NotMapped]
        public virtual IList<PreInputEntity> PreInputLayerVirtualList
        {
            get => (UtilityJson.JsonDeserialize<IList<PreInputEntity>>(PreInputLayerListStr));
            set { PreInputLayerListStr = UtilityJson.JsonSerialize(value); }
        }



    }

    public class HiddenEntity
    {

        public double InputSum { get; set; }
        public double Output { get; set; }
        public double Error { get; set; }
        public string WeightsStr { get; set; }
        [NotMapped]
        public double[] Weights
        {
            /*double[] doubles = Array.ConvertAll(sarray.split(','), Double.Parse);*/
            get => (WeightsStr.Split(',').Select(Double.Parse).ToArray());
            set { WeightsStr = string.Join(',', value); }
        }
    }
    public class InputEntity
    {
        public double InputSum { get; set; }
        public double Output { get; set; }
        public double Error { get; set; }
        public string WeightsStr { get; set; }
        [NotMapped]
        public double[] Weights
        {
            /*double[] doubles = Array.ConvertAll(sarray.split(','), Double.Parse);*/
            get => (WeightsStr.Split(',').Select(Double.Parse).ToArray());
            set { WeightsStr = string.Join(',', value); }
        }

    }

    public class PreInputEntity
    {
        public double Value { get; set; }
        public string WeightsStr { get; set; }
        [NotMapped]
        public double[] Weights
        {
            /*double[] doubles = Array.ConvertAll(sarray.split(','), Double.Parse);*/
            get => (WeightsStr.Split(',').Select(Double.Parse).ToArray());
            set { WeightsStr = string.Join(',', value); }
        }

    }
    public class OutputLayerEntity
    {
        public int Id { get; set; }
        public double InputSum { get; set; }
        public double output { get; set; }
        public double Error { get; set; }
        public double Target { get; set; }
        public string Value { get; set; }
    }
}
