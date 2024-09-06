using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace NASA_API
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Введите дату фотографии:");
            try
            {
                string date = Console.ReadLine();
                string imageDate=DateOnly.Parse(date).ToString("yyyy-MM-dd");
                await ProcesImage(imageDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            async Task ProcesImage(string date)
            {
                string apiKey = "fibtsDtrJgHcbqFy2bxY7EfNPk7Py3WGtQa3b44Q";
                string apiUrl = $"https://api.nasa.gov/planetary/apod" +
                $"?api_key={apiKey}&date={date}";
                using (var http = new HttpClient())
                {
                    var response = await http.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    string jsonString = await response.Content.ReadAsStringAsync();
                    var apodResponse = JsonSerializer.Deserialize<ImageResponse>(jsonString);

                    byte[] imageData = await DowloadImage(apodResponse.hdurl);

                    string savePath = Path.Combine(Environment.CurrentDirectory,
                        "Images/nasa_image.jpg");
                    File.WriteAllBytes(savePath, imageData);
                    Console.WriteLine($"Изображение сохранено: {savePath}");

                    Process.Start(new ProcessStartInfo { FileName= savePath,
                    UseShellExecute=true});
                }
            }

            async Task<byte[]> DowloadImage(string imageUrl)
            {
                using (var http = new HttpClient())
                {
                    var response = await http.GetAsync(imageUrl);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
        }
    }
}
