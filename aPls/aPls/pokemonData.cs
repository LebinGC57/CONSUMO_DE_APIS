public class WeatherData
{
    public Main main { get; set; }
    public List<Weather> weather { get; set; }
}

public class Main
{
    public double temp { get; set; }
    public double feels_like { get; set; }
    public int humidity { get; set; }
}

public class Weather
{
    public string description { get; set; }
}
