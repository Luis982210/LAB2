using Lab2.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab2.Models
{
    public class GaseosaSettings : IGaseosaSettings
    {
        public ArbolAsterisco<string, Gaseosa> arbol { get; set; }

        public GaseosaSettings()
        {
            arbol = new ArbolAsterisco<string, Gaseosa>(4);// el 4 es el grado 
        }
        public void Create(Gaseosa nuevo)
        {
            arbol.Insertar(nuevo.Nombre,nuevo);
        }

        public List<Gaseosa> Get()
        {
            throw new NotImplementedException();
        }

        public Object GetOne(string nombre)
        {            
            return arbol.Search(nombre);
        }
    }
    public interface IGaseosaSettings
    {
        ArbolAsterisco<string, Gaseosa> arbol { get; set; }
        Object GetOne(string nombre);
        List<Gaseosa> Get();
        void Create(Gaseosa nuevo);
    }
}
