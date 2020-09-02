using System;

namespace Models
{

    public enum PublicationType
    {
        Indefinido,
        Libro,
        Revista
    }

    public class Publications
    {
        // Titulo;Subtitulo;Autores;Editorial;Fecha_Publicacion;ISBN_13;ISBN_10;Paginas;Categorias;Tipo;Lenguaje;Imagen;Rating;Opiniones;Precio_Lista;Moneda_Lista;Precio_Venta;Moneda_Venta

        public int Id { get; set; }
        public string? Titulo { get; set; }
        public string? Subtitulo { get; set; }
        public string? Autores { get; set; }
        public string? Editorial { get; set; }
        public DateTime? Fecha_Publicacion { get; set; }
        public string? ISBN_13 { get; set; }
        public string? ISBN_10 { get; set; }
        public int? Paginas { get; set; }
        public string? Categorias { get; set; }
        public PublicationType? Tipo { get; set; }
        public string? Lenguaje { get; set; }
        public Uri? Imagen { get; set; }
        public string? Rating { get; set; }
        public string? Opiniones { get; set; }
        public decimal? Precio_Lista { get; set; }
        public string? Moneda_Lista { get; set; }
        public decimal? Precio_Venta { get; set; }
        public string? Moneda_Venta { get; set; }
    }
}

