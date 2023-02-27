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
    public class Level
    {
        public int id;
        public string name;
        public string description;
        public double levelValue;

        public List<Axis> axes1;
        public List<Axis> axes2;
        public List<Axis> boundaryAxes;
        public Curve floorBoundaries;
        public List<Point3d> nodes;

        public Slab slab;
        public List<Slab> slabs;
        public List<Beam> beams;
        //creators
        public Level()
        {
        }
        public Level(int _id, string _name)
        {
            id = _id;
            name = _name;
            slabs = new List<Slab>();
        }
        public Level(int _id, string _name, double _levelValue)
        {
            id = _id;
            name = _name;
            levelValue = _levelValue;
            slabs = new List<Slab>();
        }
        public Level(int _id, string _name, double _levelValue, List<Axis> _axes1, List<Axis> _axes2)
        {
            id = _id;
            name = _name;
            levelValue = _levelValue;
            axes1 = _axes1;
            axes2 = _axes2; 
            slabs = new List<Slab>();
        }


        public List<Line> getAxisAsLines(int _direction)
        { 
            List<Line> lines = new List<Line>();

            if (_direction==1)
                foreach (var ax in axes1)
                    lines.Add(ax.line);

            if (_direction == 2)
                foreach (var ax in axes2)
                    lines.Add(ax.line);

            return lines;
        }

        public List<Curve> getAxisAsCurves(int _direction)
        {
            List<Curve> lines = new List<Curve>();

            if (_direction == 1)
                foreach (var ax in axes1)
                    lines.Add(ax.line.ToNurbsCurve());

            if (_direction == 2)
                foreach (var ax in axes2)
                    lines.Add(ax.line.ToNurbsCurve());

            return lines;
        }
    }
}
