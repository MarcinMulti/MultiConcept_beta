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
    public class Axis
    {
        public int id;
        public string name;
        public string description;
        public int direction;
        public bool boundaryAxis;

        public List<Point3d> characteristicPoints;
        public Line line;

        public List<Beam> beams;

        //creators
        public Axis()
        { }
        public Axis(int _id, string _name)
        {
            id = _id;
            name = _name;
        }
        public Axis(int _id, string _name, int _direction, List<Point3d> _characteristicPoints, Line _line) 
        {
            id = _id;
            name = _name;
            direction = _direction;
            characteristicPoints = _characteristicPoints;
            line = _line;
        }
        public Axis(int _id, string _name, int _direction, Line _line)
        {
            id = _id;
            name = _name;
            direction = _direction;
            characteristicPoints = new List<Point3d>();
            line = _line;
        }
    }
}
