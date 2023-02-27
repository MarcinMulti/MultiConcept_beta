using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using MultiConcept.classes;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace MultiConcept.components
{
    public class AddShaft : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AddShaft class.
        /// </summary>
        public AddShaft()
          : base("AddShaft", "as",
              "Add shaft to existing structure",
              "Multiconsult", "Basic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("mBuilding", "mB", "multiconcept building", GH_ParamAccess.item); //0
            pManager.AddBrepParameter("brep", "br", "brep which defines shaft location", GH_ParamAccess.item); //1
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Mbuilding", "Mb", "Multiconsult Building class", GH_ParamAccess.item); //0
            pManager.AddTextParameter("Output", "info", "Multiconsult information", GH_ParamAccess.list);//01
            
            pManager.AddBrepParameter("slabs", "br", "brep which defines slabs", GH_ParamAccess.list);//02
            pManager.AddBrepParameter("shaftwalls", "sws", "brep which defines shaft location", GH_ParamAccess.list);//03
            pManager.AddCurveParameter("columns", "cs", "brep which defines shaft location", GH_ParamAccess.list);//04
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Building building = new Building();
            Brep shaft = new Brep();

            DA.GetData(0, ref building);
            DA.GetData(1, ref shaft);

            List<string> info = new List<string>() { "--- add shaft started ---" };

            //check intersection of floors with shaft
            List<Curve> columnCurves = new List<Curve>();
            List<Brep> slabBreps = new List<Brep>();
            List<Brep> wallBreps = new List<Brep>();



            foreach (var floor in building.floors)
            {
                if (floor.columns != null)
                foreach (var col in floor.columns)
                {
                    if (!shaft.IsPointInside(col.axis.PointAt(0.5), 0.0001, true))
                    {
                        columnCurves.Add( col.axis.ToNurbsCurve() );
                    }
                }
                Slab slab = floor.upperLevel.slab;
                Brep slabBrep = slab.brep;
                Curve slabSrf = slab.boundary;

                Brep[] result =slabBrep.Split(shaft, 0.000001) ;// Brep.CreateBooleanDifference(slabBrep, shaft, 0.00001) ;// 
                
                //slabBreps.Add(slabBrep);
                if (result != null)
                {
                    if (result.Length > 0)
                    {
                        slabBreps.Add(result[0]);
                    }
                }

                foreach (var wal in floor.walls)
                {
                    Brep wallBrep = wal.brep;
                    Brep[] resultW = wallBrep.Split(shaft, 0.000001);
                    if (resultW != null)
                    {
                        if (resultW.Length > 0)
                        {
                            foreach (var rw in resultW)
                            {
                                Point3d cenW = AreaMassProperties.Compute(rw).Centroid;
                                if (!shaft.IsPointInside(cenW, 0.0001, true))
                                    wallBreps.Add(rw);
                            }
                        }
                        else
                        {
                            wallBreps.Add(wallBrep);
                        }
                    }
                    
                }
            }

            List<Curve> shaftSections = new List<Curve>();

            foreach (var l in building.levels)
            {
                Plane pl = new Plane(new Point3d(0,0,l.levelValue), new Vector3d(0,0,1));
                Curve[] wcrvs;
                Point3d[] wpts;
                Intersection.BrepPlane(shaft, pl, 0.00001, out wcrvs, out wpts);
                shaftSections.Add(wcrvs[0]);

            }

            Brep wallShaft = Brep.CreateFromLoft(shaftSections, Point3d.Unset, Point3d.Unset, LoftType.Normal, false)[0];
            
            wallBreps.Add(wallShaft);


            Building building1 = new Building();
            building1.columns = columnCurves;
            building1.walls = wallBreps;
            building1.levels = new List<Level>();
            building1.floors = new List<Floor>();
            foreach (var l in building.levels)
                building1.levels.Add(new Level(l.id,l.name,l.levelValue,l.axes1,l.axes2));

            foreach (var f in building.floors)
            {
                building1.floors.Add(new Floor(f.id, f.name, f.upperLevel, f.lowerLevel));
                
            }
            

            DA.SetData(0, building1);
            DA.SetDataList(1, info);
            //slabs
            DA.SetDataList(2, slabBreps);
            DA.SetDataList(3, wallBreps);
            DA.SetDataList(4, columnCurves);
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
                return MultiConcept.Properties.Resources._25;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("B1592856-18BD-4DEB-9B6D-B229DC0E42FE"); }
        }
    }
}