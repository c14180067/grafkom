using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using System;

namespace Tugas1
{
    class Program
    {
        static void Main(string[] args)
        {
            var ourWindow = new NativeWindowSettings()
            {
                Size = new Vector2i(400, 400),
                Title = "Tugas 1 - Grafkom"
            };

            using (var window = new Window(GameWindowSettings.Default, ourWindow))
            {
                window.Run();
            }
        }
    }
}
