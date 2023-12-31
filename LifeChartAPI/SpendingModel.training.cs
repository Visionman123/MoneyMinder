﻿﻿// This file was auto-generated by ML.NET Model Builder. 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.LightGbm;
using Microsoft.ML.Trainers;
using Microsoft.ML;

namespace SpendingBehavior
{
    public partial class SpendingModel
    {
        public static ITransformer RetrainPipeline(MLContext context, IDataView trainData)
        {
            var pipeline = BuildPipeline(context);
            var model = pipeline.Fit(trainData);

            return model;
        }

        /// <summary>
        /// build the pipeline that is used from model builder. Use this function to retrain model.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations
            var pipeline = mlContext.Transforms.ReplaceMissingValues(new []{new InputOutputColumnPair(@"ThreeMonthsPrior", @"ThreeMonthsPrior"),new InputOutputColumnPair(@"TwoMonthsPrior", @"TwoMonthsPrior"),new InputOutputColumnPair(@"OneMonthPrior", @"OneMonthPrior"),new InputOutputColumnPair(@"LimitThisMonth", @"LimitThisMonth")})      
                                    .Append(mlContext.Transforms.Concatenate(@"Features", new []{@"ThreeMonthsPrior",@"TwoMonthsPrior",@"OneMonthPrior",@"LimitThisMonth"}))      
                                    .Append(mlContext.Regression.Trainers.LightGbm(new LightGbmRegressionTrainer.Options(){NumberOfLeaves=157,MinimumExampleCountPerLeaf=3,NumberOfIterations=4,MaximumBinCountPerFeature=8,LearningRate=1F,LabelColumnName=@"ThisMonth",FeatureColumnName=@"Features",Booster=new GradientBooster.Options(){SubsampleFraction=0.553565797691701F,FeatureFraction=0.813241265960533F,L1Regularization=2E-10F,L2Regularization=20000000000F}}));

            return pipeline;
        }
    }
}
