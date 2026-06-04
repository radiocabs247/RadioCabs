namespace Radiocab.Models
{
    public class DriverViewModel
    {
        public Driver NewDriver { get; set; } = new Driver();
        public List<Driver> AllDrivers { get; set; } = new List<Driver>();
    }
}
