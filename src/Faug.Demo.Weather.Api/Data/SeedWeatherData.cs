using Faug.Demo.Weather.Api.Data.Entities;

namespace Faug.Demo.Weather.Api.Data
{
    internal static class SeedWeatherData
    {
        public static List<WeatherForecast> WeatherForecasts()
        {
            var summaries = new[]
            {
                "Freezing", "Chilly", "Cool", "Mild"
            };
            var cities = new[]
            {
                "Akaa", "Alajärvi", "Alavus", "Espoo", "Forssa", "Haapajärvi", "Haapavesi", "Hamina", "Hanko", "Harjavalta", "Heinola", "Helsinki", "Huittinen", "Hyvinkää", "Hämeenlinna", "Iisalmi", 
                "Ikaalinen", "Imatra", "Jakobstad (Pietarsaari)", "Joensuu", "Jyväskylä", "Jämsä", "Järvenpää", "Kaarina", "Kajaani", "Kalajoki", "Kangasala", "Kankaanpää", "Kannus", "Karkkila", "Kaskinen", 
                "Kauhajoki", "Kauhava", "Kauniainen", "Kemi", "Kemijärvi", "Kerava", "Keuruu", "Kitee", "Kiuruvesi", "Kokemäki", "Kokkola", "Kotka", "Kouvola", "Kristiinankaupunki", "Kuhmo", "Kuopio", "Kurikka", 
                "Kuusamo", "Lahti", "Laitila", "Lappeenranta", "Lapua", "Lieksa", "Lohja", "Loimaa", "Loviisa", "Maarianhamina", "Mikkeli", "Mänttä-Vilppula", "Naantali", "Nivala", "Nokia", "Nurmes", "Uusikaarlepyy",
                "Närpiö", "Orimattila", "Orivesi", "Oulainen", "Oulu", "Outokumpu", "Paimio", "Parainen", "Parkano", "Pieksämäki", "Pori", "Porvoo", "Pudasjärvi", "Pyhäjärvi", "Raahe", "Raasepori", "Raisio", "Rovaniemi",
                "Salo", "Sastamala", "Savonlinna", "Seinäjoki", "Somero", "Tampere", "Tornio", "Turku", "Vaasa", "Valkeakoski", "Vantaa", "Varkaus", "Viitasaari", "Ylivieska", "Ylöjärvi"
            };

            var weatherForecasts = new List<WeatherForecast>();

            foreach (var city in cities)
            {
                for (int i = 0; i < 10; i++)
                {
                    weatherForecasts.Add(new WeatherForecast()
                    {
                        Id = Guid.NewGuid(),
                        City = city,
                        DateTime = DateTime.Now.AddDays(i),
                        TemperatureC = Random.Shared.Next(-5, 5),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    });
                }
            }            

            return weatherForecasts;
        }
    }
}
