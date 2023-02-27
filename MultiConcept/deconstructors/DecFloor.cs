using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using MultiConcept.classes;

namespace MultiConcept.deconstructors
{
    public class DecFloor : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DecFloor class.
        /// </summary>
        public DecFloor()
          : base("DecFloor", "defloor",
              "Deconstruct floor",
            "Multiconsult", "DecMConcept")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Floor", "f", "Mconcept floor", GH_ParamAccess.item); 
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Floor", "f", "Mconcept floor", GH_ParamAccess.item);
            pManager.AddTextParameter("Output", "o", "Mconcept outputs", GH_ParamAccess.list);
            pManager.AddTextParameter("name", "n", "Mconcept floor", GH_ParamAccess.item); 
            pManager.AddTextParameter("id", "i", "Mconcept floor", GH_ParamAccess.item);
            pManager.AddGenericParameter("upperLevel", "uL", "Mconcept upper level", GH_ParamAccess.item);
            pManager.AddGenericParameter("lowerLevel", "lL", "Mconcept lower level", GH_ParamAccess.item);

            pManager.AddGenericParameter("columns", "cs", "Mconcept columns", GH_ParamAccess.list);
            pManager.AddGenericParameter("walls", "ws", "Mconcept walls", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Floor f = new Floor();
            DA.GetData(0, ref f);
            List<string> info = new List<string>();



            DA.SetData(0, f);
            DA.SetDataList(1, info);
            DA.SetData(2, f.name);
            DA.SetData(3, f.id);
            DA.SetData(4, f.upperLevel);
            DA.SetData(5, f.lowerLevel);


            DA.SetDataList(6, f.columns);
            DA.SetDataList(7, f.walls);
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
                return MultiConcept.Properties.Resources._2;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9936AE86-D2C6-4A8E-BC06-EFA3C611ED77"); }
        }
    }
}