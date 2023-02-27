using Grasshopper.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MultiConcept.classes
{
    public class Results
    {
        public int id;
        public string name;
        public string description;

        public List<Curve> columns;
        public List<Curve> beams;
        public List<Surface> floors;
        public List<Surface> walls;

        //creators
        public Results(int _id, string _name)
        {
            id = _id;
            name = _name;
        }
    }
}
