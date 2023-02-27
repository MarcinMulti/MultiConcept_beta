using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using MultiConcept.classes;
using Rhino.Geometry;

namespace MultiConcept.components
{
    public class PreviewBuilding : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PreviewBuilding class.
        /// </summary>
        public PreviewBuilding()
          : base("Preview Building", "prev",
            "Preview building for Rhino and GH",
            "Multiconsult", "Visualize")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("mBuilding", "mB","Multiconsult Building class",GH_ParamAccess.item) ;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("mBuilding", "mB", "Multiconsult Building class", GH_ParamAccess.item); //0
            pManager.AddTextParameter("output", "o", "Information about this algorithm", GH_ParamAccess.list); //1

            pManager.AddLineParameter("Columns","c","columns as lines",GH_ParamAccess.list);       //2
            pManager.AddCurveParameter("Beams", "b", "columns as lines", GH_ParamAccess.list);      //3
            pManager.AddBrepParameter("Slabs", "s", "slabs as surface", GH_ParamAccess.list);       //4
            pManager.AddBrepParameter("Walls", "w", "wall as surface", GH_ParamAccess.list);        //5

            pManager.AddMeshParameter("meshColumns", "mc", "columns as mesh", GH_ParamAccess.list);     //6
            pManager.AddMeshParameter("meshBeams", "mb", "columns as mesh", GH_ParamAccess.list);       //7
            pManager.AddMeshParameter("meshSlabs", "ms", "slabs as mesh", GH_ParamAccess.list);       //8
            pManager.AddMeshParameter("meshWalls", "mw", "wall as mesh", GH_ParamAccess.list);        //9
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Building building = new Building();
            DA.GetData(0, ref building);

            List<string> outputs = new List<string>();

            outputs.Add("--- preview algorithm started ---");
            
            

            outputs.Add("adding slabs as brep");
            List<Brep> slabs = new List<Brep>();
            outputs.Add("adding slabs as mesh");
            List<Mesh> mslabs = new List<Mesh>();
            foreach (var level in building.levels)
            {
                if(level.slab!=null)
                    slabs.Add(level.slab.brep);

                foreach (var slab in level.slabs)
                {
                    slabs.Add(slab.brep);
                    mslabs.Add(slab.slabMesh);
                }
                
            }
            
            
            outputs.Add("adding walls as brep");
            List<Brep> walls = new List<Brep>();
            outputs.Add("adding walls as mesh");
            List<Mesh> mwalls = new List<Mesh>();
            foreach (var wall in building.walls)
                walls.Add(wall);
            //foreach (var floor in building.floors)
             //   foreach (var wall in floor.walls)
             //       mwalls.Add(wall.wallMesh);



            outputs.Add("adding columns as lines");
            List<Line> columns = new List<Line>();
            outputs.Add("adding columns as mesh");
            List<Mesh> mcolumns = new List<Mesh>();
            foreach(var floor in building.floors)
                if (floor.columns != null)
                foreach(var col in floor.columns)
                    columns.Add(col.axis);




            outputs.Add("adding beams as mesh");
            List<Mesh> mbeams = new List<Mesh>();
            outputs.Add("adding beams as lines");
            List<Line> beams = new List<Line>();
            foreach (var floor in building.floors)
                if (floor.upperLevel.beams != null)
                { 
                foreach (var be in floor.upperLevel.beams)
                    beams.Add(be.axis);
                }
                

            outputs.Add("--- preview algorithm done ---");

            DA.SetData(0, building);   //0
            DA.SetData(1, outputs);   //1
            DA.SetDataList(2, columns);   //2
            DA.SetDataList(3, beams);   //3
            DA.SetDataList(4, slabs);   //4
            DA.SetDataList(5, walls);   //5
            DA.SetDataList(6, mcolumns);   //6
            DA.SetDataList(7, mbeams);   //7
            DA.SetDataList(8, mslabs);   //8
            DA.SetDataList(9, mwalls);   //9
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
                return MultiConcept.Properties.Resources._18 ;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("4E9CE475-A9EE-48A6-B459-5F118F86B577"); }
        }
    }
}