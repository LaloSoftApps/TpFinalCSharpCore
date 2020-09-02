using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Services;

namespace ConsoleClient
{
    public class Aplication
    {
        private readonly IConfiguration AppConfig;

        public Aplication(IConfiguration Config)
        {
            AppConfig = Config;
        }

        /// <summary>
        /// Ejecuta la app principal
        /// </summary>
        public void Run()
        {
            Import imp = new Import(AppConfig);

            imp.File = AppConfig["FileToImport"];
            imp.ImportCsv();
        }
    }
}
