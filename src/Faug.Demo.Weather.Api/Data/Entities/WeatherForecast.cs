namespace Faug.Demo.Weather.Api.Data.Entities
{
    public partial class WeatherForecast
    {
        public Guid Id { get; set; }

        public string City { get; set; }

        public DateTime DateTime { get; set; }

        public int TemperatureC { get; set; }

        public string? Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
