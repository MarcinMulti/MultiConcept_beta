using Grasshopper.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

/*
            //types of buildings
                --- type 0  = 0 - passtopt
                --- type 1  = 1 - passtopt conform
                --- type 2  = 2 - passtopt conform spennarm
                --- type 3  = 3 - massivtre
                --- type 4  = 4 - hulldekker
*/

namespace MultiConcept.classes
{
    public class Building
    {
        public int id;
        public string name;
        public string description;

        public List<Axis> axes;
        public List<Floor> floors;
        public List<Level> levels;

        public List<Curve> columns;
        public List<Curve> beams;
        
        public List<Brep> walls;

        public Vector3d direction1;
        public Vector3d direction2;

        public double floorHeight;
        public double spanDirection1;
        public double spanDirection2;
        public Brep brep;

        public int type;
        //creators
        public Building()
        {
        }
        public Building(int _id, string _name)
        {
            id = _id;
            name = _name;
        }
    }
}
