using Grasshopper.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using MultiConcept.classes;

namespace MultiConcept.classes
{
    public class Floor
    {
        public int id;
        public string name;
        public string description;

        public Level upperLevel;
        public Level lowerLevel;
        public List<Column> columns;
        public List<Beam> bars;
        public List<Wall> walls;
        //creators
        public Floor()
        {
        }
        public Floor(int _id, string _name)
        {
            id = _id;
            name = _name;
        }
        public Floor(int _id, string _name, Level _upperLevel , Level _lowerLovel)
        {
            id = _id;
            name = _name;
            upperLevel = _upperLevel;
            lowerLevel = _lowerLovel;
        }
    }
}
