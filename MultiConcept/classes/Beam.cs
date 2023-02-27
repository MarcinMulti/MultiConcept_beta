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
    public class Beam
    {
        public int id;
        public string name;
        public string description;

        public List<Curve> columns;
        public List<Curve> beams;
        public List<Surface> floors;
        public List<Surface> walls;
        //info
        public string material;
        public string section;

        public Line axis;
        public List<Beam> subBeams;

        //creators
        public Beam()
        {
            //empty constructor
        }
        public Beam(int _id, string _name)
        {
            id = _id;
            name = _name;
        }
        public Beam(int _id, string _name, Line _axis)
        {
            id = _id;
            name = _name;
            axis = _axis;
        }
    }
}
