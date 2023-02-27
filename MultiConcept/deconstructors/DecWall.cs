using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using MultiConcept.classes;

namespace MultiConcept.deconstructors
{
    public class DecWall : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DecWall class.
        /// </summary>
        public DecWall()
          : base("DeconstructWall", "DecWall",
            "Deconstruct Wall",
            "Multiconsult", "DecMConcept")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("wall", "wl", "Multiconsult wall", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("wall", "wl", "Multiconsult wall", GH_ParamAccess.item);//0
            pManager.AddGenericParameter("info", "i", "Multiconsult wall info", GH_ParamAccess.list);//1
            pManager.AddBrepParameter("brep", "b", "Multiconsult wall", GH_ParamAccess.item);//2
            pManager.AddCurveParameter("curve", "c", "Multiconsult wall", GH_ParamAccess.item);//3
            pManager.AddMeshParameter("mesh", "m", "", GH_ParamAccess.item);//4
            pManager.AddTextParameter("name", "n", "", GH_ParamAccess.item); //5
            pManager.AddIntegerParameter("id", "i", "", GH_ParamAccess.item); //6
            pManager.AddTextParameter("section", "s", "", GH_ParamAccess.item); //7
            pManager.AddTextParameter("material", "m", "", GH_ParamAccess.item); //8
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Wall w = new Wall();

            DA.GetData(0, ref w);
            List<string> info = new List<string>();
            Brep b = new Brep();
            if (w.brep != null)
                b = w.brep;

            string name = "simple wall thickness=" + w.thickness;


            DA.SetData(0, w);
            DA.SetDataList(1, info);
            DA.SetData(2, b);
            //DA.SetData(3, w.boundary);
            //DA.SetData(4, w.wallMesh);
            DA.SetData(5, name);
            DA.SetData(6, w.id);
            DA.SetData(7, w.thickness);
            DA.SetData(8, w.material);
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
                return MultiConcept.Properties.Resources._6;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8B7F6C50-4D34-4038-912D-AAC1853DDF04"); }
        }
    }
}