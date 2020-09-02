using System;
using System.IO;
using Microsoft.Extensions.Configuration;


namespace ConsoleClient
{
    public class Program
    {
        /// <summary>
        /// Punto de Entrada del Sistema
        /// </summary>

        static void Main()
        {
            // Load Config File
            var AppConfig = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json")
            .Build();

            Aplication app = new Aplication(AppConfig);

            try
            {
                app.Run();
                
                Console.WriteLine("------------------------------------------------------------------------------------");
                Console.WriteLine("Finalizado OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------------------------------------------------------------------------");
                Console.WriteLine("Finalizado con Errores:");
                Console.WriteLine(ex.Message);
                Console.WriteLine("------------------------------------------------------------------------------------");
            }


            Console.ReadKey();
        }
    }
}
