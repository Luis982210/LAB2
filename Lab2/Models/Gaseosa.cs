using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab2.Models
{
    public class Gaseosa: IComparable<Gaseosa>
    {
        public string Nombre { get; set; }
        public string Sabor { get; set; }
        public string Volumen { get; set; }
        public int Precio { get; set; }
        public string Productora { get; set; }

        public int CompareTo(Gaseosa other)
        {
            return Nombre.CompareTo(other.Nombre);
        }
    }
}
