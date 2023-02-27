using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using MultiConcept.classes;

namespace MultiConcept.deconstructors
{
    public class DecColumn : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DecColumn class.
        /// </summary>
        public DecColumn()
          : base("DeconstructColumn", "DeColumn",
            "Deconstruct Column",
            "Multiconsult", "DecMConcept")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Column", "c", "Mconcept floor", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Column", "c", "Mconcept floor", GH_ParamAccess.item);
            pManager.AddTextParameter("Output", "o", "Mconcept outputs", GH_ParamAccess.list);
            pManager.AddTextParameter("name", "n", "Mconcept floor", GH_ParamAccess.item);
            pManager.AddTextParameter("id", "i", "Mconcept floor", GH_ParamAccess.item);
            pManager.AddCurveParameter("axis", "ax", "Mconcept axis", GH_ParamAccess.item);
            pManager.AddTextParameter("section", "s", "Mconcept floor", GH_ParamAccess.item);
            pManager.AddTextParameter("material", "m", "Mconcept floor", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Column c = new Column();
            DA.GetData(0, ref c);
            List<string> info = new List<string>();



            DA.SetData(0, c);
            DA.SetDataList(1, info);
            DA.SetData(2, c.name);
            DA.SetData(3, c.id);
            DA.SetData(4, c.axis);
            DA.SetData(5, c.section);
            DA.SetData(6, c.material);
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
            get { return new Guid("D175F7E6-5A91-4562-9D2A-7C7C27240B0C"); }
        }
    }
}