using System;
using System.Collections.Generic;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class MLModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ModelType Type { get; set; }
        public string Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastTrainedAt { get; set; }
        public string ModelPath { get; set; }
        public Dictionary<string, string> Hyperparameters { get; set; }
        public ModelStatus Status { get; set; }
        public double Accuracy { get; set; }
        public string CreatedBy { get; set; }
        
        public virtual ICollection<ModelPrediction> Predictions { get; set; }
        public virtual ICollection<ModelTrainingHistory> TrainingHistory { get; set; }
        public virtual ICollection<ModelFeature> Features { get; set; }
    }

    public class ModelPrediction
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public double Confidence { get; set; }
        public bool WasCorrect { get; set; }
        public string FeedbackNotes { get; set; }
        
        public int MLModelId { get; set; }
        public virtual MLModel MLModel { get; set; }
    }

    public class ModelTrainingHistory
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string DatasetVersion { get; set; }
        public Dictionary<string, double> Metrics { get; set; }
        public string TrainingNotes { get; set; }
        public TrainingStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        
        public int MLModelId { get; set; }
        public virtual MLModel MLModel { get; set; }
    }

    public class ModelFeature
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public FeatureType Type { get; set; }
        public bool IsRequired { get; set; }
        public string ValidationRules { get; set; }
        public double ImportanceScore { get; set; }
        
        public int MLModelId { get; set; }
        public virtual MLModel MLModel { get; set; }
    }

    public enum ModelType
    {
        AnomalyDetection,
        BehavioralAnalysis,
        RiskAssessment,
        PatternRecognition,
        ThreatDetection
    }

    public enum ModelStatus
    {
        InDevelopment,
        Training,
        Active,
        Deprecated,
        Failed
    }

    public enum TrainingStatus
    {
        Queued,
        InProgress,
        Completed,
        Failed,
        Cancelled
    }

    public enum FeatureType
    {
        Numeric,
        Categorical,
        Temporal,
        Text,
        Binary
    }
}
