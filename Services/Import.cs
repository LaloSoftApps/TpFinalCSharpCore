using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Models;
using System.Globalization;
using Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class Import
    {
        private readonly IConfiguration AppConfig;
        public Import(IConfiguration Config)
        {
            AppConfig = Config;
        }

        public string File { get; set; }

        public void ImportCsv()
        {
            FileInfo fi = new FileInfo(File);

            if (fi.Exists)
            {
                Console.WriteLine($"Fichero: {fi.Name} | Tamaño: {fi.Length} | Fecha de Acceso: {fi.LastAccessTime:yyyy-MM-dd hh:mm:ss}");

                List<Publications> LstPublications = new List<Publications>();
                var strFile = fi.OpenText();
                Int32 line = 1;
                List<Int32> LstError = new List<Int32>();

                // Set Format Culture and Style from PublicationPrice
                var StylePubPrice = NumberStyles.Number;
                var CulturePubPrice = CultureInfo.CreateSpecificCulture("en-GB");

                // Set Format Culture and Style from PublicacionDate
                var CulturePubDate = new CultureInfo("en-US");
                string[] FormatPubDate  = { "yyyy", "yyyy-MM", "yyyy-MM-dd" };

                while (!strFile.EndOfStream)
                {
                    string strLine = strFile.ReadLine();
                    string[] strFields = strLine?.Split(';');

                    // Exclude Header
                    if (strFields[0].Trim() == "Titulo") continue;

                    Publications oPublication = new Publications();
                    line++;

                    try
                    {
                        Decimal.TryParse(strFields[14], StylePubPrice, CulturePubPrice, out decimal dPrecio_Lista);
                        Decimal.TryParse(strFields[16], StylePubPrice, CulturePubPrice, out decimal dPrecio_Venta);

                        if (DateTime.TryParseExact(strFields[4], FormatPubDate, CulturePubDate, DateTimeStyles.None, out DateTime oPubDate))
                        {
                            oPublication.Fecha_Publicacion = oPubDate;
                        }
                        else {
                            oPublication.Fecha_Publicacion = null;
                        }
                        
                        oPublication.Titulo = strFields[0];
                        oPublication.Subtitulo = strFields[1];
                        oPublication.Autores = strFields[2];
                        oPublication.Editorial = strFields[3];

                        oPublication.ISBN_13 = strFields[5];
                        oPublication.ISBN_10 = strFields[6];
                        oPublication.Paginas = Int32.Parse(strFields[7]);
                        oPublication.Categorias = strFields[8];

                        oPublication.Lenguaje = strFields[10];
                        oPublication.Imagen = (strFields[11].Trim() != "") ? new Uri(strFields[11]) : null;
                        oPublication.Rating = strFields[12];
                        oPublication.Opiniones = strFields[13];
                        oPublication.Precio_Lista = dPrecio_Lista;
                        oPublication.Moneda_Lista = strFields[15];
                        oPublication.Precio_Venta = dPrecio_Venta;
                        oPublication.Moneda_Venta = strFields[17];

                        oPublication.Tipo = strFields[9].ToUpper() switch
                        {
                            "BOOK" => PublicationType.Libro,
                            "MAGAZINE" => PublicationType.Revista,
                            _ => PublicationType.Indefinido
                        };

                        // Check ISBN not exist OR ISBN empty
                        if (strFields[6] == "" || (!(LstPublications.Exists(r => r.ISBN_10 == strFields[6]))) ) {
                            LstPublications.Add(oPublication);
                        }
                        

                    } catch(Exception)
                    {
                        LstError.Add(line);
                    }
                    
                }

                // ShowDataList(LstPublications);
                // ShowErrorList(LstError);
                SaveData(LstPublications);
                ShowInfo();

            }
            else
            {
                throw new Exception(message: "El Archivo no Existe");
            }

        }


        public void SaveData(List<Publications> Publication)
        {
            PubContext Pubctx = new PubContext(AppConfig);
            Pubctx.Database.ExecuteSqlRaw("truncate table Publications");

            foreach (Publications oPub in Publication)
            {
                Pubctx.Add(oPub);     
            }
            Pubctx.SaveChanges();

        }

        public void ShowErrorList(List<Int32> LstError) {
            Console.WriteLine("--------------------------------- Lista de Errores ----------------------------------");
            Console.WriteLine($"+ Se encontraron {LstError.Count} errores.");
            foreach (Int32 oList in LstError)
            {
                Console.WriteLine($"|_ ERROR al intentar parsear la linea [{oList}]");
            }
        }


        public void ShowDataList(List<Publications> LstPublications) {
            foreach (Publications oList in LstPublications)
            {
                Console.WriteLine($"| {oList.Titulo} | {oList.Subtitulo} | {oList.Autores} | ");
            }
        }

        public void ShowInfo() {
            Console.WriteLine($"------------------------------------------------------------------------------------");
            Console.WriteLine($" + Resumen:");

            PubContext PubCtx = new PubContext(AppConfig);

            // Average Publication Books with > 500 pages
            var AvgPrice = PubCtx.Publications
                 .Where(oPub => oPub.Paginas > 500)
                 .Where(oPub => oPub.Precio_Venta != 0)
                 .Where(oPub => oPub.Tipo == PublicationType.Libro)
                 .Select(oPub => oPub.Precio_Venta).Average();
           Console.WriteLine($" |_ El Promedio de Libros con mas de 500 Paginas es de ${AvgPrice}");

            // Minimun Book Price
            var MinPrice = (PubCtx.Publications
                 .Where(oPub => oPub.Precio_Venta != 0)
                 .Where(oPub => oPub.Tipo == PublicationType.Libro)
                 .Select(oPub => oPub.Precio_Venta)).Min();
            Console.WriteLine($" |_ El Precio Minimo de Libros es de ${MinPrice}");

            // Maximun Book Price
            var MaxPrice = (PubCtx.Publications
                 .Where(oPub => oPub.Precio_Venta != 0)
                 .Where(oPub => oPub.Tipo == PublicationType.Libro)
                 .Select(oPub => oPub.Precio_Venta)).Max();
            Console.WriteLine($" |_ El Precio Maximo de Libros es de ${MaxPrice}");


            // Rating List
            IEnumerable<string> LstRating = PubCtx.Publications
                 .Where(oPub => oPub.Rating != "")
                 .Where(oPub => oPub.Tipo == PublicationType.Libro)
                 .OrderByDescending(oPub => oPub.Rating)
                 .Select(oPub => oPub.Titulo);

            Console.WriteLine("");
            Console.WriteLine("[INFO] Presione un Boton para ver la Lista de Ratings...");
            Console.ReadKey();
            Console.WriteLine("");

            foreach (string item in LstRating)
            {
                Console.WriteLine($" |_ {item}");
            }
        }

    }
}

