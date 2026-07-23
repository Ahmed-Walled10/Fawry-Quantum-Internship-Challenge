
 namespace Fawry_Assessment.Core
{
    public class Observation
    {
        public string PlateNumber { get; set; }
        public DateTime DateTime { get; set; }
        public CarType CarType { get; set; }
        public int Speed { get; set; }
        public bool SeatbeltStatus { get; set; }

        public Observation(string plateNumber, DateTime dateTime, CarType carType, int speed, bool seatbeltStatus)
        {
            PlateNumber = plateNumber;
            DateTime = dateTime;
            CarType = carType;
            Speed = speed;
            SeatbeltStatus = seatbeltStatus;
        }

    }
}
