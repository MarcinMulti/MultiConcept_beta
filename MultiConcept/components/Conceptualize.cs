using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using MultiConcept.classes;
using MultiConcept.Methods;
using System.Linq;
using Rhino.Geometry.Intersect;
using Rhino.Runtime;
using Eto.Forms;

namespace MultiConcept.components
{
    public class Conceptualize : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Conceptualize class.
        /// </summary>
        public Conceptualize()
          : base("MultiConcept_conceptualize", "MC_concept",
            "Conceptualize about multiconcept plugin",
            "Multiconsult", "Basic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("mBuilding","mB","multiconcept building",GH_ParamAccess.item); //0
            pManager.AddIntegerParameter("type", "tB", "multiconcept type [0-massivtre, 1-Hulldekker, 2-Plattedekker, 3-Plasstopt]", GH_ParamAccess.item, 0 ); //1
            pManager.AddIntegerParameter("topoType","tE","building edge type[0-walls, 1-walls+beams+columns, 2-beams+column, 3-columns]",GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Mbuilding", "Mb", "Multiconsult Building class", GH_ParamAccess.item); //0
            pManager.AddTextParameter("Output", "info", "Multiconsult information", GH_ParamAccess.list);//01
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Building building = new Building();
            int type = 0;
            int etype = 0;
            
            DA.GetData(0, ref building);
            DA.GetData(1, ref type);
            DA.GetData(2, ref etype);

            List<string> info = new List<string>() {"--- conceptualize started ---" };

            //code
            building.type = type;


            building.columns = new List<Curve>();
            building.walls = new List<Brep>();
            //add floor parts
            //add slabs

            if (etype == 0)
            {
                foreach (var level in building.levels)
                {
                    updateLevelWithSlab(level, building);
                }

                //add columns
                //updateBuildingWithColumns(building);
                
                updateBuildingWithWalls(building);
                building.columns.Clear();
                foreach (var floor in building.floors)
                    if (floor.columns != null)
                        floor.columns.Clear();
            }
            else if (etype == 1)
            {
                foreach (var level in building.levels)
                {
                    updateLevelWithSlab(level, building);
                }

                //add columns
                updateBuildingWithColumns12(building);
                updateBuildingWithWalls12(building);
            }
            else if (etype == 2)
            {
                foreach (var level in building.levels)
                {
                    updateLevelWithSlab(level, building);
                }

                //add columns
                updateBuildingWithColumns12(building);
                updateBuildingWithWalls12(building);
            }
            else if (etype == 3)
            {
                foreach (var level in building.levels)
                {
                    updateLevelWithSlab(level, building);
                }

                //add columns
                updateBuildingWithColumns(building);
                //updateBuildingWithWalls(building);
            }
            else {

                foreach (var level in building.levels)
                {
                    updateLevelWithSlab(level, building);
                }

                //add columns
                updateBuildingWithColumns(building);
                updateBuildingWithWalls(building);
            }


            foreach (var level in building.levels)
            {
                if (level.slab != null)
                level.slab.material = getSlabMaterial(type);
            }

            foreach (var floor in building.floors)
            {
                if (floor.walls!= null)
                foreach (var wall in floor.walls)
                { 
                    wall.material = getSlabMaterial(type);
                    wall.thickness = getWallSection(type);
                }
                if (floor.columns != null)
                foreach (var column in floor.columns)
                {
                    column.material = getColumnMaterial(type);
                    column.section = getColumnSection(type);
                }
            }


                //outputs
            DA.SetData(0, building);
        }

        void updateBuildingWithWalls12(Building building)
        {
            int id = 0;
            List<Brep> allWalls = new List<Brep>();
            foreach (var floor in building.floors)
            {

                var level1 = floor.upperLevel;
                var level0 = floor.lowerLevel;
                floor.walls = new List<Wall>();
                List<List<Brep>> brepsWalls = new List<List<Brep>>();
                List<List<Line>> floorColumns = new List<List<Line>>();
                for (int i = 0; i < level0.axes1.Count; i++)
                {


                    string axes1name = level0.axes1[i].name;
                    for (int j = 1; j < level1.axes1.Count-1; j++)
                    {

                        string axes2name = level1.axes1[j].name;
                        if (axes1name == axes2name)
                        {
                            List<Point3d> chPts0 = level0.axes1[i].characteristicPoints;
                            List<Point3d> chPts1 = level1.axes1[j].characteristicPoints;
                            List<Line> level0Lines = new List<Line>();
                            List<Brep> brepsWallsAxis = new List<Brep>();
                            //create column
                            foreach (Point3d cpt0 in chPts0)
                            {

                                foreach (Point3d cpt1 in chPts1)
                                {
                                    if (Math.Abs(cpt0.X - cpt1.X) < 0.0001 && Math.Abs(cpt0.Y - cpt1.Y) < 0.0001)
                                    {

                                        Line nline1 = new Line(cpt0, cpt1);
                                        level0Lines.Add(nline1);

                                    }
                                }
                            }
                            floorColumns.Add(level0Lines);
                            if (level0Lines.Count > 1)
                            {
                                Line sLine = level0Lines[0];
                                Line eLine = level0Lines[level0Lines.Count - 1];

                                Brep bwall = Brep.CreateFromCornerPoints(sLine.From, sLine.To, eLine.To, eLine.From, 0.00001);
                                brepsWallsAxis.Add(bwall);
                                allWalls.Add(bwall);

                                Wall w = new Wall(id++, "straight wall");
                                w.brep = bwall;
                                floor.walls.Add(w);

                            }
                        }
                    }
                }
                int n = floorColumns.Count;

                //now we have sets of lines


            }

            building.walls.AddRange(allWalls);
        }


        void updateBuildingWithWalls(Building building)
        {
            int id = 0;
            List<Brep> allWalls = new List<Brep>();
            foreach (var floor in building.floors)
            {

                var level1 = floor.upperLevel;
                var level0 = floor.lowerLevel;
                floor.walls = new List<Wall>();
                List<List<Brep>> brepsWalls = new List<List<Brep>>();
                List<List<Line>> floorColumns = new List<List<Line>>();
                for (int i = 0; i < level0.axes1.Count; i++)
                {
                    
                    
                    string axes1name = level0.axes1[i].name;
                    for (int j = 0; j < level1.axes1.Count; j++)
                    {
                        
                        string axes2name = level1.axes1[j].name;
                        if (axes1name == axes2name)
                        {
                            List<Point3d> chPts0 = level0.axes1[i].characteristicPoints;
                            List<Point3d> chPts1 = level1.axes1[j].characteristicPoints;
                            List<Line> level0Lines = new List<Line>();
                            List<Brep> brepsWallsAxis = new List<Brep>();
                            //create column
                            foreach (Point3d cpt0 in chPts0)
                            {
                                
                                foreach (Point3d cpt1 in chPts1)
                                {
                                    if (Math.Abs(cpt0.X - cpt1.X) < 0.0001 && Math.Abs(cpt0.Y - cpt1.Y) < 0.0001)
                                    {
                                        
                                        Line nline1 = new Line(cpt0, cpt1);
                                        level0Lines.Add(nline1); 

                                    }
                                }
                            }
                            floorColumns.Add(level0Lines);
                            if (level0Lines.Count > 1)
                            {
                                Line sLine = level0Lines[0];
                                Line eLine = level0Lines[level0Lines.Count - 1];

                                Brep bwall = Brep.CreateFromCornerPoints(sLine.From, sLine.To, eLine.To, eLine.From, 0.00001);
                                brepsWallsAxis.Add(bwall);
                                allWalls.Add(bwall);

                                Wall w = new Wall(id++, "straight wall");
                                w.brep = bwall;
                                floor.walls.Add(w);
                                
                            }
                        }
                    }
                }
                int n = floorColumns.Count;
                
                //now we have sets of lines
                

            }

            building.walls.AddRange(allWalls);
        }

        void updateBuildingWithColumns12(Building building)
        {
            foreach (var floor in building.floors)
            {

                var level1 = floor.upperLevel;
                var level0 = floor.lowerLevel;

                List<Column> floorColumns = new List<Column>();

                for (int i = 0; i < level0.axes1.Count; i++)
                {
                    string axes1name = level0.axes1[i].name;
                    for (int j = 0; j < level1.axes1.Count; j++)
                    {
                        if (i==0 || i== level0.axes1.Count-1)
                        { string axes2name = level1.axes1[j].name;
                        if (axes1name == axes2name)
                        {
                            List<Point3d> chPts0 = level0.axes1[i].characteristicPoints;
                            List<Point3d> chPts1 = level1.axes1[j].characteristicPoints;

                            //create column
                            foreach (Point3d cpt0 in chPts0)
                            {
                                foreach (Point3d cpt1 in chPts1)
                                {
                                    if (Math.Abs(cpt0.X - cpt1.X) < 0.0001 && Math.Abs(cpt0.Y - cpt1.Y) < 0.0001)
                                    {
                                        Column column = new Column(j, "straight column");
                                        column.axis = new Line(cpt0, cpt1);
                                        floorColumns.Add(column);

                                        building.columns.Add(new Line(cpt0, cpt1).ToNurbsCurve());
                                    }
                                }
                            }

                        }
                        }
                        
                    }
                }

                floor.columns = floorColumns;
            }
        }


        void updateBuildingWithColumns(Building building)
        {
            foreach (var floor in building.floors)
            {

                var level1 = floor.upperLevel;
                var level0 = floor.lowerLevel;

                List<Column> floorColumns = new List<Column>();

                for (int i = 0; i < level0.axes1.Count; i++)
                {
                    string axes1name = level0.axes1[i].name;
                    for (int j = 0; j < level1.axes1.Count; j++)
                    {
                        string axes2name = level1.axes1[j].name;
                        if (axes1name == axes2name)
                        {
                            List<Point3d> chPts0 = level0.axes1[i].characteristicPoints;
                            List<Point3d> chPts1 = level1.axes1[j].characteristicPoints;

                            //create column
                            foreach (Point3d cpt0 in chPts0)
                            {
                                foreach (Point3d cpt1 in chPts1)
                                {
                                    if (Math.Abs(cpt0.X - cpt1.X) < 0.0001 && Math.Abs(cpt0.Y - cpt1.Y) < 0.0001)
                                    {
                                        Column column = new Column(j, "straight column");
                                        column.axis = new Line(cpt0, cpt1);
                                        floorColumns.Add(column);

                                        building.columns.Add(new Line(cpt0, cpt1).ToNurbsCurve());
                                    }
                                }
                            }

                        }
                    }
                }

                floor.columns = floorColumns;
            }
        }

        public string getBeamSection(int _type, double span)
        {
            string section = "no";
            if (span > 0 && span <= 5)
            {
                if (_type == 0)
                {
                    section = "no";
                }
                else if (_type == 1)
                {
                    section = "UPE_240";
                }
                else if (_type == 2)
                {
                    section = "UPE_240";
                }
                else if (_type == 3)
                {
                    section = "Rect_140x260";
                }
                else if (_type == 4)
                {
                    section = "IPE_270";
                }
            }
            else if (span > 5 && span <= 6)
            {
                if (_type == 0)
                {
                    section = "no";
                }
                else if (_type == 1)
                {
                    section = "UPE_240";
                }
                else if (_type == 2)
                {
                    section = "UPE_240";
                }
                else if (_type == 3)
                {
                    section = "Rect_140x260";
                }
                else if (_type == 4)
                {
                    section = "IPE_270";
                }
            }
            else if (span > 6 && span <= 7)
            {
                if (_type == 0)
                {
                    section = "no";
                }
                else if (_type == 1)
                {
                    section = "UPE_240";
                }
                else if (_type == 2)
                {
                    section = "UPE_240";
                }
                else if (_type == 3)
                {
                    section = "Rect_140x260";
                }
                else if (_type == 4)
                {
                    section = "IPE_270";
                }
            }
            else if (span > 7 && span <= 8)
            {
                if (_type == 0)
                {
                    section = "no";
                }
                else if (_type == 1)
                {
                    section = "UPE_270";
                }
                else if (_type == 2)
                {
                    section = "UPE_270";
                }
                else if (_type == 3)
                {
                    section = "no";
                }
                else if (_type == 4)
                {
                    section = "IPE_270";
                }
            }
            else if (span > 8 && span <= 9)
            {
                if (_type == 0)
                {
                    section = "no";
                }
                else if (_type == 1)
                {
                    section = "UPE_270";
                }
                else if (_type == 2)
                {
                    section = "UPE_270";
                }
                else if (_type == 3)
                {
                    section = "no";
                }
                else if (_type == 4)
                {
                    section = "IPE_270";
                }
            }
            else if (span > 9 && span <= 10)
            {
                if (_type == 0)
                {
                    section = "no";
                }
                else if (_type == 1)
                {
                    section = "no";
                }
                else if (_type == 2)
                {
                    section = "UPE_270";
                }
                else if (_type == 3)
                {
                    section = "no";
                }
                else if (_type == 4)
                {
                    section = "IPE_270";
                }
            }
            else if (span > 10 && span <= 11)
            {
                if (_type == 0)
                {
                    section = "no";
                }
                else if (_type == 1)
                {
                    section = "no";
                }
                else if (_type == 2)
                {
                    section = "UPE_300";
                }
                else if (_type == 3)
                {
                    section = "no";
                }
                else if (_type == 4)
                {
                    section = "IPE_270";
                }
            }
            else if (span > 11 && span <= 12)
            {
                if (_type == 0)
                {
                    section = "no";
                }
                else if (_type == 1)
                {
                    section = "no";
                }
                else if (_type == 2)
                {
                    section = "UPE_300";
                }
                else if (_type == 3)
                {
                    section = "no";
                }
                else if (_type == 4)
                {
                    section = "IPE_270";
                }
            }
            else if (span > 12 && span <= 13)
            {
                if (_type == 0)
                {
                    section = "no";
                }
                else if (_type == 1)
                {
                    section = "no";
                }
                else if (_type == 2)
                {
                    section = "UPE_300";
                }
                else if (_type == 3)
                {
                    section = "no";
                }
                else if (_type == 4)
                {
                    section = "IPE_270";
                }
            }

            
            return section;
        }
        public double getColumnDivison(int _type, double span)
        {
            double divison = 1;
            if (span > 0 && span <= 5)
            {
                if (_type == 0)
                {
                    divison = 4;
                }
                else if (_type == 1)
                {
                    divison = 4;
                }
                else if (_type == 2)
                {
                    divison = 4 ;
                }
                else if (_type == 3)
                {
                    divison = 3.5;
                }
                else if (_type == 4)
                {
                    divison = 4;
                }
            }
            else if (span > 5 && span <= 6)
            {
                if (_type == 0)
                {
                    divison = 3.5;
                }
                else if (_type == 1)
                {
                    divison = 4;
                }
                else if (_type == 2)
                {
                    divison = 4;
                }
                else if (_type == 3)
                {
                    divison = 3;
                }
                else if (_type == 4)
                {
                    divison = 4;
                }
            }
            else if (span > 6 && span <= 7)
            {
                if (_type == 0)
                {
                    divison = 3.5;
                }
                else if (_type == 1)
                {
                    divison = 4;
                }
                else if (_type == 2)
                {
                    divison = 4;
                }
                else if (_type == 3)
                {
                    divison = 3;
                }
                else if (_type == 4)
                {
                    divison = 4;
                }
            }
            else if (span > 7 && span <= 8)
            {
                if (_type == 0)
                {
                    divison = 3;
                }
                else if (_type == 1)
                {
                    divison = 3.5;
                }
                else if (_type == 2)
                {
                    divison = 3.5;
                }
                else if (_type == 3)
                {
                    divison = 3;
                }
                else if (_type == 4)
                {
                    divison = 3.5;
                }
            }
            else if (span > 8 && span <= 9)
            {
                if (_type == 0)
                {
                    divison = 1;
                }
                else if (_type == 1)
                {
                    divison = 3.5;
                }
                else if (_type == 2)
                {
                    divison = 3.5;
                }
                else if (_type == 3)
                {
                    divison = 2.5;
                }
                else if (_type == 4)
                {
                    divison = 3.5;
                }
            }
            else if (span > 9 && span <= 10)
            {
                if (_type == 0)
                {
                    divison = 1;
                }
                else if (_type == 1)
                {
                    divison = 3;
                }
                else if (_type == 2)
                {
                    divison = 3;
                }
                else if (_type == 3)
                {
                    divison = 2.5;
                }
                else if (_type == 4)
                {
                    divison = 3;
                }
            }
            else if (span > 10 && span <= 11)
            {
                if (_type == 0)
                {
                    divison = 1;
                }
                else if (_type == 1)
                {
                    divison = 3;
                }
                else if (_type == 2)
                {
                    divison = 3;
                }
                else if (_type == 3)
                {
                    divison = 1;
                }
                else if (_type == 4)
                {
                    divison = 3;
                }
            }
            else if (span > 11 && span <= 12)
            {
                if (_type == 0)
                {
                    divison = 1;
                }
                else if (_type == 1)
                {
                    divison = 1;
                }
                else if (_type == 2)
                {
                    divison = 2.5;
                }
                else if (_type == 3)
                {
                    divison = 1;
                }
                else if (_type == 4)
                {
                    divison = 2.5;
                }
            }
            else if (span > 12 && span <= 13)
            {
                if (_type == 0)
                {
                    divison = 1;
                }
                else if (_type == 1)
                {
                    divison = 1;
                }
                else if (_type == 2)
                {
                    divison = 2.5;
                }
                else if (_type == 3)
                {
                    divison = 1;
                }
                else if (_type == 4)
                {
                    divison = 2.5;
                }
            }


            return divison;
        }
        public string getBeamMaterial(int _type)
        {
            string material = "no";
            if (_type == 0)
            {
                material = "C30/37";
            }
            else if (_type == 1)
            {
                material = "S355";
            }
            else if (_type == 2)
            {
                material = "S355";
            }
            else if (_type == 3)
            {
                material = "GL32c";
            }
            else if (_type == 4)
            {
                material = "S355";
            }
            return material;
        }
        public string getSlabMaterial(int _type)
        {
            string material = "no";
            if (_type == 0)
            {
                material = "C30/37";
            }
            else if (_type == 1)
            {
                material = "C30/37";
            }
            else if (_type == 2)
            {
                material = "C30/37";
            }
            else if (_type == 3)
            {
                material = "GL32c";
            }
            else if (_type == 4)
            {
                material = "C30/37";
            }
            return material;
        }
        public string getColumnSection(int _type)
        {
            string section = "no";
            if (_type == 0)
            {
                section = "concrete column";
            }
            else if (_type == 1)
            {
                section = "steel column";
            }
            else if (_type == 2)
            {
                section = "steel column";
            }
            else if (_type == 3)
            {
                section = "timber column";
            }
            else if (_type == 4)
            {
                section = "steel column";
            }
            return section;
        }
        public string getColumnMaterial(int _type)
        {
            string section = "no";
            if (_type == 0)
            {
                section = "C30/37";
            }
            else if (_type == 1)
            {
                section = "S355";
            }
            else if (_type == 2)
            {
                section = "S355";
            }
            else if (_type == 3)
            {
                section = "GL32c";
            }
            else if (_type == 4)
            {
                section = "S355";
            }
            return section;
        }

        public double getWallSection(int _type)
        {
            double section = 0;
            if (_type == 0)
            {
                section = 180;
            }
            else if (_type == 1)
            {
                section = 180;
            }
            else if (_type == 2)
            {
                section = 180;
            }
            else if (_type == 3)
            {
                section = 200;
            }
            else if (_type == 4)
            {
                section = 180;
            }
            return section;
        }

        public Mesh getMeshFromFlatBrep(Brep _brep, double _thickness)
            {
            Mesh mesh = new Mesh();

            var boundary = Curve.JoinCurves(_brep.Edges);
            Curve b = boundary[0];
            Polyline pl = new Polyline();
            b.TryGetPolyline(out pl);


            if (pl.Count == 5)
            {
                mesh.Vertices.Add(pl[0]);
                mesh.Vertices.Add(pl[1]);
                mesh.Vertices.Add(pl[2]);
                mesh.Vertices.Add(pl[3]);

                mesh.Vertices.Add(new Point3d(
                    pl[0].X,
                    pl[0].Y,
                    pl[0].Z - _thickness / 1000
                    ));
                mesh.Vertices.Add(new Point3d(
                    pl[1].X,
                    pl[1].Y,
                    pl[1].Z - _thickness/1000
                    ));
                mesh.Vertices.Add(new Point3d(
                    pl[2].X,
                    pl[2].Y,
                    pl[2].Z - _thickness/1000
                    ));
                mesh.Vertices.Add(new Point3d(
                    pl[3].X,
                    pl[3].Y,
                    pl[3].Z - _thickness/1000
                    ));

                mesh.Faces.AddFace(0, 1, 2, 3);
                mesh.Faces.AddFace(4, 5, 6, 7);
                mesh.Faces.AddFace(0, 1, 5, 4);
                mesh.Faces.AddFace(1, 2, 6, 5);
                mesh.Faces.AddFace(2, 3, 7, 6);
                mesh.Faces.AddFace(3, 0, 4, 7);

                mesh.Normals.ComputeNormals();
                mesh.Compact();
            }



            return mesh;
        }

        public void updateLevelWithSlab(Level level, Building building)
        {

            if (level.name != "ground level")
            {
                List<Point3d> nodes = level.nodes;
                Curve boundary = level.floorBoundaries;
                Brep brepBoundary = Brep.CreatePlanarBreps(boundary)[0];
                Surface srfBoundary = brepBoundary.Faces[0];

                string name = level.name;
                int id = level.id;

                Slab slab = new Slab( id , name );
                slab.boundary = boundary;
                slab.brep = brepBoundary;
                double thickness = MultiConcept.Methods.StandardDataDictionaries.getSlabSection(building.spanDirection1, building.type); ;
                slab.thickness = thickness;
                //slab.slabMesh = getMeshFromFlatBrep(brepBoundary, thickness);
                slab.name = "slab for level = 0 " + level.name;
                slab.id = level.id;
                slab.material = getSlabMaterial(building.type);

                level.slab = slab;
            }

        }
        void updateLevelWithSubSlabs(Level level, Building building)
        {

            if (level.name != "ground level")
            {
                List<Point3d> nodes = level.nodes;
                Curve boundary = level.floorBoundaries;
                List<Curve> axes1 = level.getAxisAsCurves(1);
                List<Curve> axes2 = level.getAxisAsCurves(2);
                Brep brepBoundary = Brep.CreatePlanarBreps(boundary)[0];
                Surface srfBoundary = brepBoundary.Faces[0];
                List<Brep> allSlabsBreps = new List<Brep>();
                Brep[] firstSplit = brepBoundary.Split(axes1, 0.00001);

                foreach (Brep bfs in firstSplit)
                {
                    Brep[] secondSplit = bfs.Split(axes2, 0.00001);
                    allSlabsBreps.AddRange(secondSplit.ToList());
                }

                List<Slab> slabsAtLevel = new List<Slab>();
                int slabi = -1;
                foreach (Brep brepSlab in allSlabsBreps)
                {
                    Slab slab = new Slab(slabi++, "Classic Slab");
                    //slab.brep = brepSlab;
                    double thickness = MultiConcept.Methods.StandardDataDictionaries.getSlabSection(building.spanDirection1, building.type);
                    slab.thickness = thickness;
                    slab.slabMesh = getMeshFromFlatBrep(brepSlab, thickness);
                    slab.name = "simple slab";
                    slab.id = slabi++;
                    slab.material = getSlabMaterial(building.type);
                    slab.brep = brepSlab;
                    slabsAtLevel.Add(slab);
                }

                level.slabs = slabsAtLevel;
            }

        }



        public Wall constructWall(Point3d chpt11, Point3d chpt12, Point3d chpt21, Point3d chpt22, int id, string name, double thickness)
        {

            //create bottom and top line of the wall
            Line bottomLine = new Line(chpt11, chpt12);
            Line topLine = new Line(chpt21, chpt22);
            //create vector with correct length for etrusion
            Vector3d dir = new Vector3d(bottomLine.FromX, bottomLine.FromY, topLine.FromZ - bottomLine.FromZ);

            //create Brep from 4 points
            Brep wallBrep = Brep.CreateFromCornerPoints(bottomLine.From, bottomLine.To, topLine.To, topLine.From, 0.0001);
            //find wall thickness depending on wall type
            double wallThickness = thickness;

            //create wall mesh
            Mesh wallMesh = new Mesh();
            wallMesh.Vertices.Add(bottomLine.From);
            wallMesh.Vertices.Add(bottomLine.To);
            wallMesh.Vertices.Add(topLine.To);
            wallMesh.Vertices.Add(topLine.From);

            wallMesh.Faces.AddFace(0, 1, 2, 3);
            wallMesh.Normals.ComputeNormals();
            wallMesh.Compact();

            //create wall class
            Wall wall = new Wall(id,name);
            wall.brep = wallBrep;
            wall.wallMesh = wallMesh;
            wall.description = "generic mesh";
            wall.thickness = wallThickness;

            return wall;
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return MultiConcept.Properties.Resources._14;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("D2F7B729-67A5-4556-868A-1E5240B29909"); }
        }
    }
}