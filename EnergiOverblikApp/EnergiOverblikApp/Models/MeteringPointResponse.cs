using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergiOverblikApp.Models
{
    public class MeteringPointResponse
    {
        public List<MeteringPoint> Result { get; set; }
    }
    public class MeteringPoint
    {
        public string StreetCode { get; set; }
        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string FloorId { get; set; }
        public string RoomId { get; set; }
        public string CitySubDivisionName { get; set; }
        public string MunicipalityCode { get; set; }
        public string LocationDescription { get; set; }
        public string SettlementMethod { get; set; }
        public string MeterReadingOccurrence { get; set; }
        public string FirstConsumerPartyName { get; set; }
        public string SecondConsumerPartyName { get; set; }
        public string MeterNumber { get; set; }
        public DateTime ConsumerStartDate { get; set; }
        public string MeteringPointId { get; set; }
        public string TypeOfMP { get; set; }
        public string BalanceSupplierName { get; set; }
        public string Postcode { get; set; }
        public string CityName { get; set; }
        public bool HasRelation { get; set; }
        public string ConsumerCVR { get; set; }
        public string DataAccessCVR { get; set; }
        public List<MeteringPoint> ChildMeteringPoints { get; set; }
    }
}
