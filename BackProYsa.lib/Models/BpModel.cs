using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.lib.Models
{


    public class BpModel
    {
        public double LearningRate { get; set; }
        public List<OutputLayerEntity> OutputLayerList { get; set; }
        public List<PreInputEntity> PreInputLayerList { get; set; }
        public List<InputLayerEntity> InputLayerList { get; set; }
        public List<HiddenLayerEntity> HiddenLayerList { get; set; }
        public int OutputNum { get; set; }
        public int HiddenNum { get; set; }
        public int InputNum { get; set; }
        public int PreInputNum { get; set; }



    }
    public class HiddenLayerEntity
    {     

        public double InputSum   { get; set; }
        public double Output     { get; set; }
        public double Error      { get; set; }
        public double[] Weights { get; set; }

    }
    public class InputLayerEntity
    {
        public double InputSum { get; set; }
        public double Output { get; set; }
        public double Error { get; set; }
        public double[] Weights { get; set; }

    }
    public class PreInputEntity
    {
        public double Value { get; set; }
        public double[] Weights { get; set; }

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
