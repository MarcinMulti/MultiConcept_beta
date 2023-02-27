using System;
using System.Collections.Generic;
using MultiConcept.classes;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MultiConcept.deconstructors
{
    public class DecLevel : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DecLevel class.
        /// </summary>
        public DecLevel()
          : base("DecLevel", "DeLevel",
              "Deconstruct Level",
            "Multiconsult", "DecMConcept")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Level", "l", "Mconcept level", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Level", "l", "Mconcept level", GH_ParamAccess.item);
            pManager.AddTextParameter("Output", "o", "Mconcept outputs", GH_ParamAccess.list);
            pManager.AddTextParameter("name", "n", "Mconcept level", GH_ParamAccess.item);
            pManager.AddTextParameter("id", "i", "Mconcept level", GH_ParamAccess.item);
            pManager.AddGenericParameter("axes1", "a1", "Mconcept level", GH_ParamAccess.list);
            pManager.AddGenericParameter("axes2", "a2", "Mconcept level", GH_ParamAccess.list);
            pManager.AddPointParameter("nodes", "ns", "Mconcept level", GH_ParamAccess.list);
            pManager.AddGenericParameter("slabs", "ss", "Mconcept slabs", GH_ParamAccess.list);
            pManager.AddGenericParameter("slab", "s", "Mconcept slabs", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Level  l = new Level();
            DA.GetData(0, ref l);
            List<string> info = new List<string>();



            DA.SetData(0, l);
            DA.SetDataList(1, info);
            DA.SetData(2, l.name);
            DA.SetData(3, l.id);
            DA.SetDataList(4, l.axes1);
            DA.SetDataList(5, l.axes2);
            DA.SetDataList(6, l.nodes);
            DA.SetDataList(7, l.slabs);
            DA.SetData(8, l.slab);
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
                return MultiConcept.Properties.Resources._3;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9A8D2392-B55D-49EB-A73B-D1CE65ACFC05"); }
        }
    }
}