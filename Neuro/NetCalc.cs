using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace MotionTrackerApp
{
    /// <summary>
    /// Artificial neuronal net class
    /// </summary>
    public class NetCalc
    {
        #region Events

        /// <summary>
        /// Event for signaling net error
        /// </summary>
        public event NetErrEventHandler PublishNetError;

        /// <summary>
        /// Event for signaling learn cycles
        /// </summary>
        public event NetCycleEventHandler PublishLearnCycle;

        public event NetErrPointsEventHandler PublishLearnCyclePoints;

        #endregion

        #region Member

        /// <summary>
        /// Current artificial net
        /// </summary>
        private Net currentNet = null;

        /// <summary>
        /// Dictionary of net layer meta data
        /// </summary>
        private Dictionary<int, LayerMetaData> layerMetaData = new Dictionary<int, LayerMetaData>();

        /// <summary>
        /// Squared error of one pattern over all neurons
        /// </summary>
        private double patError = 0.0;  

        /// <summary>
        /// Learning rate
        /// </summary>
        private double learnRate = 0.89;  

        /// <summary>
        /// Over-all net toleranz (max error of all neurons over all pattern)
        /// </summary>
        private double epsilon = 0.4;

        /// <summary>
        /// Use bias neuron
        /// </summary>
        private bool withBias = true;

        private Dictionary<string, double> deltaValuesOutput = new Dictionary<string, double>();
        private Dictionary<string, double> deltaValuesHidden = new Dictionary<string, double>();

        #endregion

        #region Properties

        /// <summary>
        /// Learn rate
        /// </summary>
        public double LearnRate
        {
            get { return learnRate; }
            set { learnRate = value; }
        }

        /// <summary>
        /// Max. over-all net error
        /// </summary>
        public double Epsilon
        {
            get
            {
                return epsilon;
            }
            set
            {
                epsilon = value;
            }
        }

        /// <summary>
        /// Current net object
        /// </summary>
        public Net CurrentNet
        {
            get
            {
                return this.currentNet;
            }
            set
            {
                this.currentNet = value;
            }
        }

        /// <summary>
        /// Flag for stopping learning process
        /// </summary>
        public bool StopLearning { get; set; }

        /// <summary>
        /// Publish current net error
        /// </summary>
        /// <param name="arg"></param>
        private void OnPublishNetError(NetErrEventArgs arg)
        {
            if (this.PublishNetError != null)
            {
                this.PublishNetError(this, arg);
            }
        }

        /// <summary>
        /// Publish current net learn cycle
        /// </summary>
        /// <param name="arg"></param>
        private void OnPublishNetLearnCycle(NetCycleEventArgs arg)
        {
            if (this.PublishLearnCycle != null)
            {
                this.PublishLearnCycle(this, arg);
            }
        }

        private void OnPublishLearnCyclePoints(NetErrPointsEventArgs arg)
        {
            if (this.PublishLearnCyclePoints != null)
            {
                this.PublishLearnCyclePoints(this, arg);
            }
        }

        /// <summary>
        /// Property to use bias neuron
        /// </summary>
        public bool WithBias
        {
            get
            { 
                return withBias; 
            }
            set 
            {
                withBias = value; 
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public NetCalc()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear net structures
        /// </summary>
        public void DelVectors()
        {
			for (int i = 0; i <= this.currentNet.neuronInp.Count - 1; i++)
            {
                this.currentNet.neuronInp[i] = 0.0;
            }

            for (int i = 0; i <= this.currentNet.neuronOrigInp.Count - 1; i++)
            {
                this.currentNet.neuronOrigInp[i] = 0.0;
            }

            for (int i = 0; i <= this.currentNet.neuronNetto.Count - 1; i++)
            {
                this.currentNet.neuronNetto[i] = 0.0;
            }

            for (int i = 0; i <= this.currentNet.neuronActive.Count - 1; i++)
            {
                this.currentNet.neuronActive[i] = 0.0;
            }

            for (int i = 0; i <= this.currentNet.neuronOut.Count - 1; i++)
            {
                this.currentNet.neuronOut[i] = 0.0;
            }

            for (int i = 0; i <= this.currentNet.neuronAim.Count - 1; i++)
            {
                this.currentNet.neuronAim[i] = 0.0;
            }

            for (int i = 0; i <= this.currentNet.neuronHiddenOut.Count - 1; i++)
            {
                this.currentNet.neuronHiddenOut[i] = 0.0;
            }

            for (int i = 0; i <= this.currentNet.neuronSaveInp.Count - 1; i++)
            {
                this.currentNet.neuronSaveInp[i] = 0.0;
            }

            for (int i = 0; i <= this.currentNet.neuronSaveOut.Count - 1; i++)
            {
                this.currentNet.neuronSaveOut[i] = 0.0;
            }


        }



        /// <summary>
        /// Calculate neuron netto values with given input function
        /// </summary>
        /// <param name="b_InpKind"></param>
        /// <param name="b_anzi"></param>
        /// <param name="b_anzj"></param>
        /// <param name="netLayerNr"></param>
        public void EvalNettoVec(InputMethodTyp InputMethodID, int b_anzi, int b_anzj, int netLayerNr)
        {
			double r_max = 0;
            double r_min = 0;


            if (InputMethodID == InputMethodTyp.Minumum_Function)
            {
                r_min = Int32.MaxValue;
            }

            if (InputMethodID == InputMethodTyp.Maximum_Function)
            {
                r_max = Int32.MinValue;
            }

            for (int j = 0; j <= b_anzj - 1; j++)
            {
                this.currentNet.neuronNetto[j] = 0.0;

                for (int i = 0; i <= b_anzi - 1; i++)
                {

                    switch (InputMethodID)
                    {
                        // Sum function
                        case InputMethodTyp.Sum_Function:
                            this.currentNet.neuronNetto[j] = this.currentNet.neuronNetto[j] + this.currentNet.layerMatrixList[netLayerNr].ag_weight[i, j] * this.currentNet.neuronInp[i];
                            break;

                        // Majority function
                        case InputMethodTyp.Majority_Function:
                            if (this.currentNet.layerMatrixList[netLayerNr].ag_weight[i, j] * this.currentNet.neuronInp[i] > 0)
                            {
                                this.currentNet.neuronNetto[j] = this.currentNet.neuronNetto[j] + 1;
                            }
                            else
                            {
                                this.currentNet.neuronNetto[j] = this.currentNet.neuronNetto[j] - 1;
                            }
                            break;

                        // Minumum function
                        case InputMethodTyp.Minumum_Function:
                            if (this.currentNet.layerMatrixList[netLayerNr].ag_weight[i, j] * this.currentNet.neuronInp[i] <= r_min)
                            {
                                r_min = this.currentNet.layerMatrixList[netLayerNr].ag_weight[i, j] * this.currentNet.neuronInp[i];
                            }
                            break;


                        // Maximum function
                        case InputMethodTyp.Maximum_Function:
                            if (this.currentNet.layerMatrixList[netLayerNr].ag_weight[i, j] * this.currentNet.neuronInp[i] >= r_max)
                            {
                                r_max = this.currentNet.layerMatrixList[netLayerNr].ag_weight[i, j] * this.currentNet.neuronInp[i];
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            // Set neuronNetto vector to minimum
            if (InputMethodID == InputMethodTyp.Minumum_Function)
            {
                for (int j = 0; j <= b_anzj - 1; j++)
                {
                    this.currentNet.neuronNetto[j] = r_min;
                }
            }

            // Set neuronNetto vector to maximum
            if (InputMethodID == InputMethodTyp.Maximum_Function)
            {
                for (int j = 0; j <= b_anzj - 1; j++)
                {
                    this.currentNet.neuronNetto[j] = r_max;
                }
            }

        }

        /// <summary>
        /// Calculate activation values of neuron with given activation function
        /// </summary>
        /// <param name="b_TransKind"></param>
        /// <param name="b_countj"></param>
        public void EvalActivVec(TransferMethodTyp TransferMethodID, int b_countj)
        {
			switch (TransferMethodID)
            {
                // Linear function
                case TransferMethodTyp.Linear_Function:
                    for (int j = 0; j <= b_countj - 1; j++)
                    {
                        this.currentNet.neuronActive[j] = this.currentNet.neuronNetto[j];
                    }
                    break;


                // Signum function
                case TransferMethodTyp.Signum_Function:
                    for (int j = 0; j <= b_countj - 1; j++)
                    {
                        if (this.currentNet.neuronNetto[j] > 0)
                        {
                            this.currentNet.neuronActive[j] = 1.0;
                        }
                        else
                        {
                            this.currentNet.neuronActive[j] = -1.0;
                        }
                    }
                    break;


                // linear threshold function
                case TransferMethodTyp.Linear_Threshold_Function:
                    for (int j = 0; j <= b_countj - 1; j++)
                    {
                        if (this.currentNet.neuronNetto[j] > 0)
                        {
                            this.currentNet.neuronActive[j] = this.currentNet.neuronNetto[j];
                        }
                        else
                        {
                            this.currentNet.neuronActive[j] = -1.0;
                        }
                    }
                    break;

                // Sigmoid function
                case TransferMethodTyp.Sigmoid_Function:
                    for (int j = 0; j <= b_countj - 1; j++)
                    {
                        this.currentNet.neuronActive[j] = 1 / (1 + Math.Exp(-1 * this.currentNet.neuronNetto[j]));
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Calculate output neuron values with given output function
        /// </summary>
        /// <param name="b_OutKind"></param>
        /// <param name="b_countj"></param>
        public void EvalOutputVec(OutputMethodTyp OutputMethodID, int b_countj)
        {
			switch (OutputMethodID)
            {

                // Ident function
                case OutputMethodTyp.Ident_Function:
                    for (int j = 0; j <= b_countj - 1; j++)
                    {
                        this.currentNet.neuronOut[j] = this.currentNet.neuronActive[j];
                    }
                    break;

                    // Signum function
                case OutputMethodTyp.Signum_Function:
                    for (int j = 0; j <= b_countj - 1; j++)
                    {
                        if (this.currentNet.neuronActive[j] > 0)
                        {
                            this.currentNet.neuronOut[j] = 1.0;
                        }
                        else
                        {
                            this.currentNet.neuronOut[j] = -1.0;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Create net structure (init weights with random values)
        /// </summary>
        public void MakeNetStruc()
        {
			int cellMaxCount = (from p in this.currentNet.patternList
				//select p.Value.p_aim.Count).Max();
				select p.Value.p_inp.Count).Max();

			LayerMetaData metadata_LayerInput = new LayerMetaData();
			metadata_LayerInput.layerNr = 1;
			metadata_LayerInput.neuroCellCount = cellMaxCount;
			metadata_LayerInput.layerName = "Input_Layer";

			LayerMetaData metadata_LayerHidden = new LayerMetaData();
			metadata_LayerHidden.layerNr = 2;
			metadata_LayerHidden.neuroCellCount = cellMaxCount;
			metadata_LayerHidden.layerName = "Hidden_Layer";

			LayerMetaData metadata_LayerOutput = new LayerMetaData();
			metadata_LayerOutput.layerNr = 3;
			metadata_LayerOutput.neuroCellCount = 4;
			metadata_LayerOutput.layerName = "Output_Layer";

			this.layerMetaData.Add(0, metadata_LayerInput);
			this.layerMetaData.Add(1, metadata_LayerHidden);
			this.layerMetaData.Add(2, metadata_LayerOutput);

			var random = new Random();

            this.currentNet.layerMatrixList.Clear();

            // Init matrix weights between hidden and input layer
            var matrixInputHidden = new TMatrixElement
                                               {
                                                   ag_weight = new double[this.layerMetaData[0].neuroCellCount + 1, this.layerMetaData[1].neuroCellCount]
                                               };

            for (int j = 0; j <= this.layerMetaData[1].neuroCellCount - 1; j++)
            {
                for (int i = 0; i <= this.layerMetaData[0].neuroCellCount; i++)
                {
                    matrixInputHidden.ag_weight[i, j] = random.Next(0, 10) / 10.0;
                }
            }

            // Init matrix weights between output and hidden layer
            var matrixHiddenOutput = new TMatrixElement
                                     {
                                         ag_weight = new double[this.layerMetaData[2].neuroCellCount + 1, this.layerMetaData[1].neuroCellCount]
                                     };

            for (int j = 0; j <= this.layerMetaData[2].neuroCellCount - 1; j++)
            {

                for (int i = 0; i <= this.layerMetaData[1].neuroCellCount; i++)
                {
                    matrixHiddenOutput.ag_weight[i, j] = random.Next(0, 10) / 10.0;
                }
            }

            // Add layers to net
            this.currentNet.layerMatrixList.Add(0, matrixInputHidden);
            this.currentNet.layerMatrixList.Add(1, matrixHiddenOutput);

            // Init Norm vectors
            for (int i = 0; i <= this.layerMetaData[0].neuroCellCount - 1; i++)
            {
                this.currentNet.p_innorm[i, 0] = -1;
                this.currentNet.p_innorm[i, 1] = 1;
            }

            for (int i = 0; i <= this.layerMetaData[2].neuroCellCount - 1; i++)
            {
                this.currentNet.p_outnorm[i, 0] = -1;
                this.currentNet.p_outnorm[i, 1] = 1;
            }


            // Init input neurons
            for (int i = 0; i <= this.layerMetaData[0].neuroCellCount; i++)
            {
                this.currentNet.neuronInp.Add(0.0);
                this.currentNet.neuronNetto.Add(0.0);
                this.currentNet.neuronActive.Add(0.0);
                this.currentNet.neuronOrigInp.Add(0.0);
                this.currentNet.neuronSaveInp.Add(0.0);
            }

            // Init hidden neurons
            for (int i = 0; i <= this.layerMetaData[1].neuroCellCount; i++)
            {
                this.currentNet.neuronHiddenOut.Add(0.0);
            }

            // Init output neurons
            for (int i = 0; i <= this.layerMetaData[2].neuroCellCount-1; i++)
            {
                this.currentNet.neuronOut.Add(0.0);
                this.currentNet.neuronAim.Add(0.0);
                this.currentNet.neuronSaveOut.Add(0.0);
            }
        }


        /// <summary>
        /// Copy patNr pattern input vector into input layer neuron vector and
        /// pattern aim vector into output layer vector
        /// </summary>
        public void CopyInpAim(int patNr)
        {
			for (int i = 0; i <= this.layerMetaData[0].neuroCellCount - 1; i++)
            {
                this.currentNet.neuronInp[i] = this.currentNet.patternList[patNr].p_inp[i];
            }

            for (int i = 0; i <= this.layerMetaData[2].neuroCellCount - 1; i++)
            {
                this.currentNet.neuronAim[i] = this.currentNet.patternList[patNr].p_aim[i];
            }   
        }


        /// <summary>
        /// Recall net
        /// </summary>
        /// <param name="b_InpKind"></param>
        /// <param name="b_TransKind"></param>
        /// <param name="b_OutKind"></param>
        public void BackRecall(InputMethodTyp InputMethodID, TransferMethodTyp TransferMethodID, OutputMethodTyp OutputMethodID)
        {
			// Set Bias for hidden neurons to 1 
            this.currentNet.neuronInp[this.layerMetaData[0].neuroCellCount] = 1.0;

            // Calculate netto input vector
            this.EvalNettoVec(InputMethodID, this.layerMetaData[0].neuroCellCount + 1, this.layerMetaData[1].neuroCellCount, 0);

            // Calculate activation vector
            this.EvalActivVec(TransferMethodID, this.layerMetaData[1].neuroCellCount);

            // Calculate output vector  
            this.EvalOutputVec(OutputMethodID, this.layerMetaData[1].neuroCellCount);

            //  Save input Vector in p_saveinp because its reused
            for (int i = 0; i <= this.layerMetaData[0].neuroCellCount; i++)
            {
                this.currentNet.neuronSaveInp[i] = this.currentNet.neuronInp[i];
            }

            // Copy content of output vector (output of hidden layer) into input vector
            // Copy content of output vector in hidden layer output vector for error calculation
            for (int i = 0; i <= this.layerMetaData[2].neuroCellCount - 1; i++)
            {
                this.currentNet.neuronInp[i] = this.currentNet.neuronOut[i];
                this.currentNet.neuronHiddenOut[i] = this.currentNet.neuronOut[i];
            }

            // Set Bias for output layer to 1
            this.currentNet.neuronInp[this.layerMetaData[1].neuroCellCount] = 1.0;
            this.currentNet.neuronHiddenOut[this.layerMetaData[1].neuroCellCount] = 1.0;

            // Calculate netto input vector
            this.EvalNettoVec(InputMethodID, this.layerMetaData[1].neuroCellCount + 1, this.layerMetaData[2].neuroCellCount, 1);

            // Calculate activation vector
            this.EvalActivVec(TransferMethodID, this.layerMetaData[2].neuroCellCount);

            // Calculate output vector  
            this.EvalOutputVec(OutputMethodID, this.layerMetaData[2].neuroCellCount);

            // Reconstruct saved input vector from saved input vector
            for (int i = 0; i <= this.layerMetaData[0].neuroCellCount; i++)
            {
                this.currentNet.neuronInp[i] = this.currentNet.neuronSaveInp[i];
            }
        }

        /// <summary>
        /// Learn method (adapt matrix weights)
        /// </summary>
        public void BackLearn ()
		{
			var err_sum = new double[100];
            double err = 0.0;

            this.deltaValuesOutput.Clear();
            this.deltaValuesHidden.Clear();

            // Set sum of errors of output layer to 0 
            for (int i = 0; i <= this.layerMetaData[1].neuroCellCount; i++)
            {
                err_sum[i] = 0.0;
            }

            // Set pattern error to 0
            this.patError = 0.0;

            // Loop thru all output neurons
            for (int j = 0; j <= this.layerMetaData[2].neuroCellCount - 1; j++)
            {
                // Calculate max. output layer error
                if (Math.Abs(this.currentNet.neuronAim[j] - this.currentNet.neuronOut[j]) > this.patError)
                {
                    this.patError = Math.Abs(this.currentNet.neuronAim[j] - this.currentNet.neuronOut[j]);
                }

                // Calculate pattern error of output neurons for matrix adjustment (1st derivation of Sigmoid function)
                err = (this.currentNet.neuronAim[j] - this.currentNet.neuronOut[j]) * this.currentNet.neuronOut[j] * (1 - this.currentNet.neuronOut[j]);


                // Add all errors of output layer for adjustment of weights of hidden layer
                for (int i = 0; i <= this.layerMetaData[1].neuroCellCount; i++)
                {
					this.deltaValuesOutput.Add(String.Concat(j.ToString("0000"), i.ToString("0000")), this.learnRate * err * this.currentNet.neuronHiddenOut[i]);

                    err_sum[i] = err_sum[i] + (this.currentNet.layerMatrixList[1].ag_weight[i, j] * err);

                    // Weight = old value + learn rate * error of output layer * output of hidden layer
                    //this.currentNet.layerMatrixList[1].ag_weight[i, j] = this.currentNet.layerMatrixList[1].ag_weight[i, j] + (this.learnRate * err * this.currentNet.neuronHiddenOut[i]);
                }
            }

            // Switch to  1st layer matrix

            // Loop thru all hidden neurons
            for (int j = 0; j <= this.layerMetaData[1].neuroCellCount - 1; j++)
            {
                // Calculate pattern error of hidden neurons for maxtrix weights adjustment
                err = err_sum[j] * this.currentNet.neuronHiddenOut[j] * (1 - this.currentNet.neuronHiddenOut[j]);

                // Loop thru all input neurons
                for (int i = 0; i <= this.layerMetaData[0].neuroCellCount; i++)
                {

					this.deltaValuesHidden.Add(String.Concat(j.ToString("0000"), i.ToString("0000")), this.learnRate * err * this.currentNet.neuronInp[i]);

                    // Weight = old value + learn rate * error of hidden layer * output of input layer
                    //this.currentNet.layerMatrixList[0].ag_weight[i, j] = this.currentNet.layerMatrixList[0].ag_weight[i, j] + (this.learnRate * err * this.currentNet.neuronInp[i]);
                }
            }
        }


        /// <summary>
        /// Adjust weights with saved delta values FORWARD
        /// </summary>
        private void AdjustWeights()
        {
			// Forward adjusting weights

            // Matrix : Input => Hidden
            // ========================

            // Loop thru all hidden neurons
            for (int j = 0; j <= this.layerMetaData[1].neuroCellCount - 1; j++)
            {
                // Loop thru all input neurons
                for (int i = 0; i <= this.layerMetaData[0].neuroCellCount; i++)
                {
                    // Weight = old value + learn rate * error of hidden layer * output of input layer
					this.currentNet.layerMatrixList[0].ag_weight[i, j] = this.currentNet.layerMatrixList[0].ag_weight[i, j] + this.deltaValuesHidden[String.Concat(j.ToString("0000"), i.ToString("0000"))];
                }
            }

            // Matrix : Hidden => Output
            // ========================

            // Loop thru all output neurons
            for (int j = 0; j <= this.layerMetaData[2].neuroCellCount - 1; j++)
            {
                // Add all errors of output layer for adjustment of weights of hidden layer
                for (int i = 0; i <= this.layerMetaData[1].neuroCellCount; i++)
                {
                    // Weight = old value + learn rate * error of output layer * output of hidden layer
					this.currentNet.layerMatrixList[1].ag_weight[i, j] = this.currentNet.layerMatrixList[1].ag_weight[i, j] + this.deltaValuesOutput[String.Concat(j.ToString("0000"), i.ToString("0000"))];
                }
            }
        }

        //public void BackLearn()
        //{
        //    double[] err_sum = new double[100];
        //    double err = 0.0;


        //    // Set sum of errors of output layer to 0 
        //    for (int i = 0; i <= this.layerMetaData[1].neuroCellCount; i++)
        //    {
        //        err_sum[i] = 0.0;
        //    }

        //    // Set pattern error to 0
        //    this.patError = 0.0;

        //    // Loop thru all output neurons
        //    for (int j = 0; j <= this.layerMetaData[2].neuroCellCount - 1; j++)
        //    {
        //        // Calculate max. output layer error
        //        if (Math.Abs(this.currentNet.neuronAim[j] - this.currentNet.neuronOut[j]) > this.patError)
        //        {
        //            this.patError = Math.Abs(this.currentNet.neuronAim[j] - this.currentNet.neuronOut[j]);
        //        }

        //        // Calculate pattern error of output neurons for matrix adjustment (1st derivation of Sigmoid function)
        //        err = (this.currentNet.neuronAim[j] - this.currentNet.neuronOut[j]) * this.currentNet.neuronOut[j] * (1 - this.currentNet.neuronOut[j]);

        //        // Add all errors of output layer for adjustment of weights of hidden layer
        //        for (int i = 0; i <= this.layerMetaData[1].neuroCellCount; i++)
        //        {
        //            err_sum[i] = err_sum[i] + (this.currentNet.layerMatrixList[1].ag_weight[i, j] * err);

        //            // Weight = old value + learn rate * error of output layer * output of hidden layer
        //            this.currentNet.layerMatrixList[1].ag_weight[i, j] = this.currentNet.layerMatrixList[1].ag_weight[i, j] + (this.learnRate * err * this.currentNet.neuronHiddenOut[i]);
        //        }
        //    }

        //    // Switch to  1st layer matrix

        //    // Loop thru all hidden neurons
        //    for (int j = 0; j <= this.layerMetaData[1].neuroCellCount - 1; j++)
        //    {
        //        // Calculate pattern error of hidden neurons for maxtrix weights adjustment
        //        err = err_sum[j] * this.currentNet.neuronHiddenOut[j] * (1 - this.currentNet.neuronHiddenOut[j]);

        //        // Loop thru all input neurons
        //        for (int i = 0; i <= this.layerMetaData[0].neuroCellCount; i++)
        //        {
        //            // Weight = old value + learn rate * error of hidden layer * output of input layer
        //            this.currentNet.layerMatrixList[0].ag_weight[i, j] = this.currentNet.layerMatrixList[0].ag_weight[i, j] + (this.learnRate * err * this.currentNet.neuronInp[i]);
        //        }
        //    }
        //}


		public Task BackTrainAsync()
		{
			return Task.Run(() => BackTrain());
		}

        /// <summary>
        /// Train net
        /// </summary>
		public void BackTrain()
        {
            Int64 cycle = 0;             // Over all cyclus (learn all pattern one time)
            double overAllPatErr = 0.0;  // Sum of error over all pattern
            int currentPatternNr = 0;    // Number of pattern
           // List<Point> learnPointsOutput = new List<Point>();
           // List<Point> learnPointsAim = new List<Point>();

            // Clear vectors
            this.DelVectors();

            cycle = 1;

            // Loop until over-all error of net > epsilon
            do
            {
                // Set over-all net error to 0
                overAllPatErr = 0.0;

                // Set squared error of one pattern over all neurons to 0
                this.patError = 0.0;

                // Set pattern number to 0
                currentPatternNr = 0;

                // Loop thru all pattern
                do
                {
                    // Get next pattern
                    this.CopyInpAim(currentPatternNr);

                    // Start Net recall to invoke learning process
                    this.BackRecall(InputMethodTyp.Sum_Function, TransferMethodTyp.Sigmoid_Function, OutputMethodTyp.Ident_Function);

                    // Learn net (adjust matrix weights)
                    this.BackLearn();

                    this.AdjustWeights();

                    // Set over-all error = max. error of one output neuron
                    if (this.patError > overAllPatErr)
                    {
                        overAllPatErr = this.patError;
                    }

                    //this.BackTrainDisplay(learnPointsOutput, learnPointsAim);

                    // Switch to next pattern...
                    currentPatternNr = currentPatternNr + 1;

                    // until all pattern have been learned
                } while (currentPatternNr <= this.currentNet.patternList.Count - 1);

                // Increase learn cyclus
                cycle = cycle + 1;

                // Publish net error 
                NetErrEventArgs arg = new NetErrEventArgs();
                arg.AllPatErr = overAllPatErr;
                this.OnPublishNetError(arg);

                // Publish learn cycle
                NetCycleEventArgs arg2 = new NetCycleEventArgs();
                arg2.LearnCycle = cycle;
                this.OnPublishNetLearnCycle(arg2);

                //this.BackTrainDisplay(learnPointsOutput, learnPointsAim);
			               

                // Learn until over-all net error < epsilon
            } while ((overAllPatErr > this.epsilon) && (this.StopLearning != true));
	
        }


        private void BackTrainDisplay(List<Point> learnPointsOutput, List<Point> learnPointsAim)
        {
            learnPointsOutput.Clear();
            for (int k = 0; k <= this.layerMetaData[2].neuroCellCount - 1; k++)
            {
                learnPointsOutput.Add(new Point(Convert.ToInt32(100 * k), Convert.ToInt32(this.currentNet.neuronOut[k] * 100)));
            }

            learnPointsAim.Clear();
            for (int k = 0; k <= this.currentNet.neuronAim.Count - 1; k++)
            {
                learnPointsAim.Add(new Point(Convert.ToInt32(100 * k), Convert.ToInt32(this.currentNet.neuronAim[k] * 100)));
            }

            NetErrPointsEventArgs arg3 = new NetErrPointsEventArgs();
            arg3.pointListOutput = learnPointsOutput;
            arg3.pointListAim = learnPointsAim;
            this.OnPublishLearnCyclePoints(arg3);
        }


        /// <summary>
        /// Normalize value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="g_x1"></param>
        /// <param name="g_x2"></param>
        /// <param name="g_y1"></param>
        /// <param name="g_y2"></param>
        /// <returns></returns>
        public double Norm(double g_value, double g_x1, double g_x2, double g_y1, double g_y2)
        {
            return ((g_y1 - g_y2) / (g_x1 - g_x2)) * g_value + (((g_x1 * g_y2) - (g_x2 * g_y1)) / (g_x1 - g_x2));
        }


		/// <summary>
		/// Create sample pattern set
		/// </summary>
		public void CreateMovingPatternSet (List<PhoneMotion> phoneMotionList)
		{
			this.currentNet.patternList.Clear ();

			int patternID = 0;

			int maxCount = 0;

			foreach (var phoneMotionItem in phoneMotionList)
			{
				foreach (List<double> list in phoneMotionItem.AccelerometerDict.Values)
				{
					if (list.Count > maxCount)
					{
						maxCount = list.Count;
					}
				}
			}


			foreach (var phoneMotionItem in phoneMotionList)
			{
				for (int i = 0; i <= phoneMotionItem.AccelerometerDict.Keys.Count - 1; i++)
				{
					List<double> valueList = phoneMotionItem.AccelerometerDict [i];

					TPatternElement pattern = new TPatternElement ();

					for (int j = 0; j <= maxCount - 1; j++)
					{						
						if (j <= valueList.Count - 1)
						{
							pattern.w_Nr = j + 1;
							pattern.p_inp.Add (valueList [j]);
							//pattern.p_aim.Add (valueList [j]);
							pattern.p_aim.Add (0.0);
						}
						else
						{
							pattern.w_Nr = j + 1;
							pattern.p_inp.Add (0.0);
							pattern.p_aim.Add (0.0);
						}
					}

					//for (int x = 0; x <= pattern.p_aim.Count - 1; x++) 
					for (int x = 0; x <= 4; x++) 
					{
					  pattern.p_aim[x] = 0.0;
					}
					
					pattern.p_aim[0] = 4.0;
					pattern.p_aim[1] = 5.0;
					pattern.p_aim[2] = 0.0;
					pattern.p_aim[3] = 1.0;
					

					this.currentNet.patternList.Add (patternID, pattern);
					patternID++;
				}

				for (int i = 0; i <= phoneMotionItem.GyroscopeDict.Keys.Count - 1; i++) 
				{
					List<double> valueList = phoneMotionItem.GyroscopeDict [i];

					TPatternElement pattern = new TPatternElement ();

					for (int j = 0; j <= maxCount - 1; j++) 
					{						
						if (j <= valueList.Count - 1)
						{
							pattern.w_Nr = j + 1;
							pattern.p_inp.Add (valueList [j]);
							pattern.p_aim.Add (valueList [j]);
						}
						else
						{
							pattern.w_Nr = j + 1;
							pattern.p_inp.Add (0.0);
							pattern.p_aim.Add (0.0);
						}
					}

					this.currentNet.patternList.Add (patternID, pattern);
					patternID++;
				}
			}				 
		}

        /// <summary>
        /// Create sample pattern set
        /// </summary>
        public void CreateSamplePatternSet()
        {
            this.currentNet.patternList.Clear();

            //for (int i = 0; i <= 14; i++)
            //{
            //    TPatternElement pattern = new TPatternElement();
            //    pattern.w_Nr = i + 1;
            //    pattern.p_inp.Add(Convert.ToDouble(i + 1));

            //    if ((i % 2) == 0)
            //    {
            //        pattern.p_aim.Add(0.5);
            //    }
            //    else
            //    {
            //        pattern.p_aim.Add(-0.5);
            //    }

            //    this.currentNet.p_GoP.Add(i, pattern);
            //}

            TPatternElement pattern = new TPatternElement();
            pattern.w_Nr = 1;
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(1.0);

            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(1.0);

            this.currentNet.patternList.Add(0, pattern);

            // ========

            pattern = new TPatternElement();
            pattern.w_Nr = 2;
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(1.0);
            pattern.p_inp.Add(0.0);

            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(1.0);
            pattern.p_aim.Add(0.0);

            this.currentNet.patternList.Add(1, pattern);

            // ========

            pattern = new TPatternElement();
            pattern.w_Nr = 3;
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(1.0);
            pattern.p_inp.Add(1.0);

            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(1.0);
            pattern.p_aim.Add(1.0);

            this.currentNet.patternList.Add(2, pattern);

            // ========

            pattern = new TPatternElement();
            pattern.w_Nr = 4;
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(1.0);
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(0.0);

            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(1.0);
            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(0.0);

            this.currentNet.patternList.Add(3, pattern);

            // ========

            pattern = new TPatternElement();
            pattern.w_Nr = 5;
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(1.0);
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(1.0);

            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(1.0);
            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(1.0);

            this.currentNet.patternList.Add(4, pattern);

            // ========

            pattern = new TPatternElement();
            pattern.w_Nr = 6;
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(1.0);
            pattern.p_inp.Add(1.0);
            pattern.p_inp.Add(0.0);

            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(1.0);
            pattern.p_aim.Add(1.0);
            pattern.p_aim.Add(0.0);

            this.currentNet.patternList.Add(5, pattern);

            // ========

            pattern = new TPatternElement();
            pattern.w_Nr = 7;
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(1.0);
            pattern.p_inp.Add(1.0);
            pattern.p_inp.Add(1.0);

            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(1.0);
            pattern.p_aim.Add(1.0);
            pattern.p_aim.Add(1.0);

            this.currentNet.patternList.Add(6, pattern);

            // ========

            pattern = new TPatternElement();
            pattern.w_Nr = 8;
            pattern.p_inp.Add(1.0);
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(0.0);
            pattern.p_inp.Add(0.0);

            pattern.p_aim.Add(1.0);
            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(0.0);
            pattern.p_aim.Add(0.0);

            this.currentNet.patternList.Add(7, pattern);

        }

        /// <summary>
        /// Recall one pattern
        /// </summary>
        /// <param name="neuroValueList"></param>
        /// <returns></returns>
        public List<double> SetOneRecallPattern(List<double> neuroValueList)
        {
            List<double> outputNeuronListValues = new List<double>();

            for (int i = 0; i <= neuroValueList.Count - 1; i++)
            {
                this.currentNet.neuronInp[i] = neuroValueList[i];
            }

            this.BackRecall(InputMethodTyp.Sum_Function, TransferMethodTyp.Sigmoid_Function, OutputMethodTyp.Ident_Function);

            for (int i = 0; i <= this.currentNet.neuronOut.Count - 1; i++)
            {
                if (this.currentNet.neuronOut[i] != 0.0)
                {
                    outputNeuronListValues.Add(this.currentNet.neuronOut[i]);
                }
            }

            return outputNeuronListValues;
        }

        #endregion

    }
}


