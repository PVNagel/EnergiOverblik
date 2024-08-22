using Newtonsoft.Json;

namespace EnergiOverblikApp.Models
{
    public class TimeSeriesResponse
    {
        [JsonProperty("result")]
        public List<TimeSeriesResult> Result { get; set; }
    }

    public class TimeSeriesResult
    {
        [JsonProperty("MyEnergyData_MarketDocument")]
        public MarketDocument MyEnergyDataMarketDocument { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("errorCode")]
        public int ErrorCode { get; set; }

        [JsonProperty("errorText")]
        public string ErrorText { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("stackTrace")]
        public string StackTrace { get; set; }
    }

    public class MarketDocument
    {
        [JsonProperty("mRID")]
        public string MRID { get; set; }

        [JsonProperty("createdDateTime")]
        public string CreatedDateTime { get; set; }

        [JsonProperty("sender_MarketParticipant.name")]
        public string SenderMarketParticipantName { get; set; }

        [JsonProperty("sender_MarketParticipant.mRID")]
        public MarketParticipantMRID SenderMarketParticipantMRID { get; set; }

        [JsonProperty("period.timeInterval")]
        public TimeInterval PeriodTimeInterval { get; set; }

        [JsonProperty("TimeSeries")]
        public List<TimeSeries> TimeSeries { get; set; }
    }

    public class MarketParticipantMRID
    {
        [JsonProperty("codingScheme")]
        public string CodingScheme { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class TimeInterval
    {
        [JsonProperty("start")]
        public string Start { get; set; }

        [JsonProperty("end")]
        public string End { get; set; }
    }

    public class TimeSeries
    {
        [JsonProperty("mRID")]
        public string MRID { get; set; }

        [JsonProperty("businessType")]
        public string BusinessType { get; set; }

        [JsonProperty("curveType")]
        public string CurveType { get; set; }

        [JsonProperty("measurement_Unit.name")]
        public string MeasurementUnitName { get; set; }

        [JsonProperty("MarketEvaluationPoint")]
        public MarketEvaluationPoint MarketEvaluationPoint { get; set; }

        [JsonProperty("Period")]
        public List<Period> Period { get; set; }
    }

    public class MarketEvaluationPoint
    {
        [JsonProperty("mRID")]
        public MRID MRID { get; set; }
    }

    public class MRID
    {
        [JsonProperty("codingScheme")]
        public string CodingScheme { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Period
    {
        [JsonProperty("resolution")]
        public string Resolution { get; set; }

        [JsonProperty("timeInterval")]
        public TimeInterval TimeInterval { get; set; }

        [JsonProperty("Point")]
        public List<Point> Points { get; set; }
    }

    public class Point
    {
        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("out_Quantity.quantity")]
        public string Quantity { get; set; } // Rename to match JSON field

        [JsonProperty("out_Quantity.quality")]
        public string Quality { get; set; }
    }
}
