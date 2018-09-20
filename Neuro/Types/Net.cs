using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionTrackerApp
{

    public class Net
    {
        /// <summary>
        /// Dictionary of net layers
        /// </summary>
        public Dictionary<int, TMatrixElement> layerMatrixList = new Dictionary<int, TMatrixElement>();

        /// <summary>
        /// Dictionary of patterns
        /// </summary>
        public Dictionary<int, TPatternElement> patternList = new Dictionary<int, TPatternElement>();

        /// <summary>
        /// List of input neurons
        /// </summary>
        public List<double> neuronInp = new List<double>();

        /// <summary>
        /// List of origin input neurons
        /// </summary>
        public List<double> neuronOrigInp = new List<double>();

        /// <summary>
        /// List of temp. saved input neurons
        /// </summary>
        public List<double> neuronSaveInp = new List<double>();

        /// <summary>
        /// List of output neurons
        /// </summary>
        public List<double> neuronOut = new List<double>();

        /// <summary>
        /// List of temp. saved output neurons
        /// </summary>
        public List<double> neuronSaveOut = new List<double>();

        /// <summary>
        /// List of hidden neuron output values
        /// </summary>
        public List<double> neuronHiddenOut = new List<double>();

        /// <summary>
        /// List of aim values
        /// </summary>
        public List<double> neuronAim = new List<double>();

        /// <summary>
        /// List of netto values of neurons
        /// </summary>
        public List<double> neuronNetto = new List<double>();

        /// <summary>
        /// List of activation values of neurons
        /// </summary>
        public List<double> neuronActive = new List<double>();

        public int[,] p_innorm = new int[100, 2];
        public int[,] p_outnorm = new int[100, 2];
    }

    /// <summary>
    /// Single matrix element weight
    /// </summary>
    public class TMatrixElement
    {
        public int b_nr {get; set;}
        public double[,] ag_weight = null;
    }

    /// <summary>
    /// Pattern element with input and aim value lists
    /// </summary>
    public class TPatternElement
    {
        public int w_Nr { get; set; }
        public List<double> p_inp = new List<double>();   
        public List<double> p_aim = new List<double>();   
    }

    /// <summary>
    /// Net layer meta data
    /// </summary>
    public class LayerMetaData
    {
        public int layerNr { get; set; }
        public int neuroCellCount { get; set; }
        public string layerName { get; set; }
    }

    /// <summary>
    /// Enum for input method function
    /// </summary>
    public enum InputMethodTyp
    {
        Sum_Function = 1,
        Majority_Function = 2,
        Minumum_Function = 3,
        Maximum_Function =4
    }

    /// <summary>
    /// Enum for transfer method function
    /// </summary>
    public enum TransferMethodTyp
    {
        Linear_Function = 1,
        Signum_Function = 2,
        Linear_Threshold_Function = 3,
        Sigmoid_Function = 4
    }

    /// <summary>
    /// Enum for output method function
    /// </summary>
    public enum OutputMethodTyp
    {
        Ident_Function = 1,
        Signum_Function = 2
    }

}

