using EnergiOverblikApp.Models;
using EnergiOverblikApp.Services;
using Microsoft.Extensions.Primitives;
using System.Globalization;

namespace EnergiOverblikApp
{
    public class Program
    {
        private static ElOverblikService elOverblikService;
        private static string accessToken;
        private static List<MeteringPoint> meteringPoints;

        static async Task Main(string[] args)
        {
            elOverblikService = new ElOverblikService();
            accessToken = await elOverblikService.GetDataAccessTokenAsync();
            meteringPoints = await elOverblikService.GetMeteringPointsAsync(accessToken);

            while (true)
            {
                DisplayMenu();

                int choice = GetMenuChoice();
                await HandleMenuChoice(choice);
            }
        }

        static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("ENERGY OVERVIEW MENU");
            Console.WriteLine("1. Display Metering Points");
            Console.WriteLine("2. Display Meter Readings for a Metering Point");
            Console.WriteLine("3. Get Meter Reading Statistics");
            Console.WriteLine("4. Exit");
            Console.Write("Please enter your choice: ");
        }

        static int GetMenuChoice()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 4)
                {
                    return choice;
                }

                Console.Write("Invalid choice. Please enter a number between 1 and 4: ");
            }
        }

        static async Task HandleMenuChoice(int choice)
        {
            switch (choice)
            {
                case 1:
                    Console.Clear();
                    PrintMeteringPoints(meteringPoints);
                    break;

                case 2:
                    Console.Clear();
                    await FetchAndDisplayTimeSeriesDataMenu();
                    break;

                case 3:
                    Console.Clear();
                    await FetchAndDisplayStatisticsMenu();
                    break;

                case 4:
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Unexpected error. Exiting...");
                    Environment.Exit(1);
                    break;
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        static async Task FetchAndDisplayTimeSeriesDataMenu()
        {
            if (meteringPoints.Count > 0)
            {
                Console.WriteLine("Select a Metering Point from the list below:");

                for (int i = 0; i < meteringPoints.Count; i++)
                {
                    var point = meteringPoints[i];
                    Console.WriteLine($"{i + 1}. ID: {point.MeteringPointId}");
                    Console.WriteLine($"   Address: {point.CityName}, {point.StreetName} {point.BuildingNumber}, {point.FloorId}.{point.RoomId}");
                    Console.WriteLine($"   Consumer Start Date: {point.ConsumerStartDate:yyyy-MM-dd}");
                    Console.WriteLine($"   First Consumer Party Name: {point.FirstConsumerPartyName}");
                    Console.WriteLine("-------------------------------------------------------");
                }

                Console.Write("Enter the corresponding number: ");
                int index = GetMenuChoice() - 1;

                if (index < 0 || index >= meteringPoints.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    return;
                }

                MeteringPoint meteringPoint = meteringPoints[index];
                await FetchAndDisplayTimeSeriesData(accessToken, meteringPoint);
            }
            else
            {
                Console.WriteLine("No metering points available.");
            }
        }

        static async Task FetchAndDisplayTimeSeriesData(string accessToken, MeteringPoint meteringPoint)
        {
            DateTime meteringPointConsumerStartDate = meteringPoint.ConsumerStartDate;
            Console.Clear();
            Console.WriteLine($"Note: Consumer start date was {meteringPointConsumerStartDate:yyyy-MM-dd}, therefore the start date cannot be that or earlier.");

            DateTime startDate = meteringPointConsumerStartDate;
            bool isValidStartDate = false;

            while (!isValidStartDate)
            {
                Console.Write("Enter the start date (yyyy-MM-dd): ");
                string startDateInput = Console.ReadLine();

                if (DateTime.TryParse(startDateInput, out startDate))
                {
                    if (startDate >= meteringPointConsumerStartDate)
                    {
                        isValidStartDate = true;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine($"The start date cannot be {meteringPointConsumerStartDate:yyyy-MM-dd} or earlier. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid start date format. Please enter the date in the format yyyy-MM-dd.");
                }
            }

            DateTime endDate = DateTime.Today;
            bool isValidEndDate = false;

            while (!isValidEndDate)
            {
                Console.Write("Enter the end date (yyyy-MM-dd): ");
                string endDateInput = Console.ReadLine();

                if (DateTime.TryParse(endDateInput, out endDate))
                {
                    if (endDate <= DateTime.Today)
                    {
                        isValidEndDate = true;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine($"The end date cannot be later than today ({DateTime.Today:yyyy-MM-dd}). Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid end date format. Please enter the date in the format yyyy-MM-dd.");
                }
            }

            Console.WriteLine("Select a period:");
            Console.WriteLine("1. Hour");
            Console.WriteLine("2. Day");
            Console.WriteLine("3. Month");
            Console.WriteLine("4. Year");
            Console.Write("Enter your choice: ");

            string input = Console.ReadLine();
            bool isValid = int.TryParse(input, out int choice);

            string period = "Month";

            if (isValid && choice >= 1 && choice <= 4)
            {
                switch (choice)
                {
                    case 1:
                        period = "Hour";
                        break;
                    case 2:
                        period = "Day";
                        break;
                    case 3:
                        period = "Month";
                        break;
                    case 4:
                        period = "Year";
                        break;
                    default:
                        Console.WriteLine("Unexpected error. Exiting...");
                        Environment.Exit(1);
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid choice. Please select a number between 1 and 4.");
                Environment.Exit(1);
            }

            TimeSeriesResponse timeSeriesResponse = await elOverblikService.GetTimeSeriesAsync(accessToken, meteringPoint.MeteringPointId, startDate, endDate, period);
            PrintTimeSeriesResponse(timeSeriesResponse, period);
        }

        static void PrintMeteringPoints(List<MeteringPoint> meteringPoints)
        {
            if (meteringPoints == null || meteringPoints.Count == 0)
            {
                Console.WriteLine("No metering points available.");
                return;
            }

            foreach (var point in meteringPoints)
            {
                Console.WriteLine($"Street Code: {point.StreetCode}");
                Console.WriteLine($"Street Name: {point.StreetName}");
                Console.WriteLine($"Building Number: {point.BuildingNumber}");
                Console.WriteLine($"Floor ID: {point.FloorId}");
                Console.WriteLine($"Room ID: {point.RoomId}");
                Console.WriteLine($"City SubDivision Name: {point.CitySubDivisionName}");
                Console.WriteLine($"Municipality Code: {point.MunicipalityCode}");
                Console.WriteLine($"Location Description: {point.LocationDescription}");
                Console.WriteLine($"Settlement Method: {point.SettlementMethod}");
                Console.WriteLine($"Meter Reading Occurrence: {point.MeterReadingOccurrence}");
                Console.WriteLine($"First Consumer Party Name: {point.FirstConsumerPartyName}");
                Console.WriteLine($"Second Consumer Party Name: {point.SecondConsumerPartyName}");
                Console.WriteLine($"Meter Number: {point.MeterNumber}");
                Console.WriteLine($"Consumer Start Date: {point.ConsumerStartDate}");
                Console.WriteLine($"Metering Point ID: {point.MeteringPointId}");
                Console.WriteLine($"Type Of MP: {point.TypeOfMP}");
                Console.WriteLine($"Balance Supplier Name: {point.BalanceSupplierName}");
                Console.WriteLine($"Postcode: {point.Postcode}");
                Console.WriteLine($"City Name: {point.CityName}");
                Console.WriteLine($"Has Relation: {point.HasRelation}");
                Console.WriteLine($"Consumer CVR: {point.ConsumerCVR}");
                Console.WriteLine($"Data Access CVR: {point.DataAccessCVR}");

                Console.WriteLine("-------------------------------------------------------");
            }
        }

        static void PrintTimeSeriesResponse(TimeSeriesResponse response, string periodSelection)
        {
            Console.Clear();

            if (response?.Result == null || response.Result.Count == 0)
            {
                Console.WriteLine("No data available.");
                return;
            }

            foreach (var result in response.Result)
            {
                var marketDocument = result.MyEnergyDataMarketDocument;
                if (marketDocument == null)
                {
                    Console.WriteLine("Market Document is null.");
                    continue;
                }

                Console.WriteLine($"Document ID: {marketDocument.MRID}");
                Console.WriteLine($"Created DateTime: {marketDocument.CreatedDateTime}");
                Console.WriteLine($"Sender Name: {marketDocument.SenderMarketParticipantName}");

                if (marketDocument.SenderMarketParticipantMRID != null)
                {
                    Console.WriteLine($"Sender MRID Coding Scheme: {marketDocument.SenderMarketParticipantMRID.CodingScheme}");
                    Console.WriteLine($"Sender MRID Name: {marketDocument.SenderMarketParticipantMRID.Name}");
                }

                if (marketDocument.PeriodTimeInterval != null)
                {
                    Console.WriteLine($"Period Start: {marketDocument.PeriodTimeInterval.Start}");
                    Console.WriteLine($"Period End: {marketDocument.PeriodTimeInterval.End}");
                }

                if (marketDocument.TimeSeries != null)
                {
                    foreach (var timeSeries in marketDocument.TimeSeries)
                    {
                        Console.WriteLine($"Time Series ID: {timeSeries.MRID}");
                        Console.WriteLine($"Business Type: {timeSeries.BusinessType}");
                        Console.WriteLine($"Curve Type: {timeSeries.CurveType}");
                        Console.WriteLine($"Measurement Unit: {timeSeries.MeasurementUnitName}");
                        Console.WriteLine("-------------------------------------------------------");

                        if (timeSeries.Period != null)
                        {
                            foreach (var period in timeSeries.Period)
                            {
                                Console.WriteLine($"Measurement period: {period.TimeInterval?.Start} - {period.TimeInterval?.End}");

                                if (period.Points != null)
                                {
                                    foreach (var point in period.Points)
                                    {
                                        DateTime startDate;
                                        if (DateTime.TryParse(period.TimeInterval?.Start, out startDate))
                                        {
                                            if (periodSelection == "Month")
                                            {
                                                string month = startDate.ToString("MMMM yyyy", CultureInfo.GetCultureInfo("en-US"));
                                                Console.WriteLine($"  Month: {month}");
                                            }
                                            else if (periodSelection == "Day")
                                            {
                                                string day = startDate.ToString("dddd", CultureInfo.GetCultureInfo("en-US"));
                                                Console.WriteLine($"  Day: {day}");
                                            }
                                        }
                                        Console.WriteLine($"  Quantity: {point.Quantity} KWH");
                                        Console.WriteLine("-------------------------------------------------------");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static async Task FetchAndDisplayStatisticsMenu()
        {
            if (meteringPoints.Count > 0)
            {
                Console.WriteLine("Select a Metering Point from the list below:");

                for (int i = 0; i < meteringPoints.Count; i++)
                {
                    var point = meteringPoints[i];
                    Console.WriteLine($"{i + 1}. ID: {point.MeteringPointId}");
                    Console.WriteLine($"   Address: {point.CityName}, {point.StreetName} {point.BuildingNumber}, {point.FloorId}.{point.RoomId}");
                    Console.WriteLine($"   First Consumer Party Name: {point.FirstConsumerPartyName}");
                    Console.WriteLine($"   Consumer Start Date: {point.ConsumerStartDate:yyyy-MM-dd}");
                    Console.WriteLine("-------------------------------------------------------");
                }

                Console.Write("Enter the corresponding number: ");
                int index = GetMenuChoice() - 1;
                Console.Clear();

                if (index < 0 || index >= meteringPoints.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    return;
                }

                var selectedPoint = meteringPoints[index];
                string meteringPointId = selectedPoint.MeteringPointId;
                DateTime startDate = selectedPoint.ConsumerStartDate;
                DateTime endDate = DateTime.Now;

                TimeSeriesResponse timeSeriesResponse = await elOverblikService.GetTimeSeriesAsync(accessToken, meteringPointId, startDate, endDate, "Month");

                // Compute statistics
                if (timeSeriesResponse?.Result != null && timeSeriesResponse.Result.Count > 0)
                {
                    var monthlyUsage = new Dictionary<DateTime, double>();

                    foreach (var result in timeSeriesResponse.Result)
                    {
                        foreach (var timeSeries in result.MyEnergyDataMarketDocument.TimeSeries)
                        {
                            foreach (var timeSeriesPeriod in timeSeries.Period)
                            {
                                // Parse the timeInterval.start into a DateTime object
                                if (DateTime.TryParse(timeSeriesPeriod.TimeInterval.Start, out DateTime periodStart))
                                {
                                    var month = new DateTime(periodStart.Year, periodStart.Month, 1);

                                    // Convert quantity to double
                                    if (timeSeriesPeriod.Points != null && timeSeriesPeriod.Points.Count > 0)
                                    {
                                        var point = timeSeriesPeriod.Points.First(); // Assume only one point per period
                                        if (double.TryParse(point.Quantity, NumberStyles.Any, CultureInfo.InvariantCulture, out double quantity))
                                        {
                                            // Store the quantity in the dictionary
                                            monthlyUsage[month] = quantity;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Invalid quantity format: {point.Quantity}");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid date format.");
                                }
                            }
                        }
                    }

                    // Compute and display statistics
                    if (monthlyUsage.Count > 0)
                    {
                        double totalUsage = monthlyUsage.Values.Sum();
                        double averageMonthlyUsage = totalUsage / monthlyUsage.Count;
                        Console.WriteLine($"PERIOD: {startDate} to {endDate}");
                        Console.WriteLine("-------------------------------------------------------");
                        Console.WriteLine($"Total Usage: {totalUsage:F2} kWh");
                        Console.WriteLine($"Average Monthly Usage: {averageMonthlyUsage:F2} kWh");
                    }
                    else
                    {
                        Console.WriteLine("No monthly data available.");
                    }
                }
                else
                {
                    Console.WriteLine("No data available.");
                }
            }
            else
            {
                Console.WriteLine("No metering points available.");
            }
        }
    }
}