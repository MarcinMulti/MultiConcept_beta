using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Utility;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System.Linq;
using Grasshopper.Kernel.Types.Transforms;
using System.Dynamic;
using MultiConcept.classes;

namespace MultiConcept.components
{
    public class DecAxis : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BrepToBuilding class.
        /// </summary>
        public DecAxis()
          : base("DeconstructAxis", "DeAxis",
            "Deconstruct Axis",
            "Multiconsult", "DecMConcept")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Axis", "A", "Axis of the building", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Axis", "A", "Axis of the building", GH_ParamAccess.item); //0
            pManager.AddTextParameter("Output", "info", "Multiconsult information", GH_ParamAccess.list);//01
            pManager.AddPointParameter("Pts", "pts", "Multiconsult information", GH_ParamAccess.list);//02
            pManager.AddLineParameter("Line", "l", "Multiconsult information", GH_ParamAccess.item);//03
            pManager.AddTextParameter("Name", "n", "Multiconsult information", GH_ParamAccess.item);//04
            pManager.AddIntegerParameter("Id", "i", "Multiconsult information", GH_ParamAccess.item);//05
            pManager.AddIntegerParameter("Direction", "d", "Multiconsult information", GH_ParamAccess.item);//06
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //inputs
            Axis ax = new Axis();

            DA.GetData(0, ref ax);

            //variables
            List<string> ot = new List<string>() { "---code is running---" };



            //final line of the code
            ot.Add("---code done---");

            //set data
            DA.SetData(0, ax);
            DA.SetDataList(1, ot);
            DA.SetDataList(2, ax.characteristicPoints);
            DA.SetData(3, ax.line);
            DA.SetData(4, ax.name);
            DA.SetData(5, ax.id);
            DA.SetData(6, ax.direction);
        }
        // create lines starting at spacing point with specifing length in two directions
       

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return MultiConcept.Properties.Resources._4;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("39915848-3434-40F6-A7A3-7AE4DA1A2307"); }
        }
    }
}