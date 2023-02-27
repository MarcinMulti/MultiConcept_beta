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
    public class Wall
    {
        public int id;
        public string name;
        public string description;

        public Curve boundary;
        public List<Curve> subBoundaries;
        public Brep brep;

        public double thickness;
        public string material;

        public List<Curve> columns;
        public List<Curve> beams;
        public List<Surface> floors;
        public List<Surface> walls;
        public Surface surface;
        public Mesh wallMesh;

        //creators
        public Wall()
        {
        }
        
        public Wall(int _id, string _name)
        {
            id = _id;
            name = _name;
        }

        public Curve getCurve()
        {
            List<Curve> curves = new List<Curve>();

            foreach(var e in brep.Edges)
                curves.Add(e.EdgeCurve);

            Curve crv = Curve.JoinCurves(curves)[0];

            return crv;
        }

        public Mesh create3dMesh(double _thickness)
        {
            Mesh m = new Mesh();

            Point3d n0 = brep.Vertices[0].Location;
            Point3d n1 = brep.Vertices[1].Location;
            Point3d n2 = brep.Vertices[2].Location;
            Point3d n3 = brep.Vertices[3].Location;
            Point3d n4 = new Point3d(n0.X,n0.Y, n0.Z- _thickness);
            Point3d n5 = new Point3d(n1.X, n1.Y, n1.Z - _thickness);
            Point3d n6 = new Point3d(n2.X, n2.Y, n2.Z - _thickness);
            Point3d n7 = new Point3d(n3.X, n3.Y, n3.Z - _thickness);

            m.Vertices.Add(n0);
            m.Vertices.Add(n1);
            m.Vertices.Add(n2);
            m.Vertices.Add(n3);
            m.Vertices.Add(n4);
            m.Vertices.Add(n5);
            m.Vertices.Add(n6);
            m.Vertices.Add(n7);

            m.Faces.AddFace(0, 1, 2, 3);
            m.Faces.AddFace(0, 1, 5, 4);
            m.Faces.AddFace(1, 2, 6, 5);
            m.Faces.AddFace(2, 3, 7, 6);
            m.Faces.AddFace(3, 0, 4, 7);
            m.Faces.AddFace(4, 5, 6, 7);

            m.Normals.ComputeNormals();
            m.Compact();

            return m;
        
        }
    }
}
