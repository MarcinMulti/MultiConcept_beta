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
    public class DecBuilding : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BrepToBuilding class.
        /// </summary>
        public DecBuilding()
          : base("DeconstructBuilding", "DecBuild",
            "Deconstruct Mconcept building",
            "Multiconsult", "DecMConcept")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Building", "BD", "MConcept building class", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Mbuilding", "Mb", "Multiconsult Building class", GH_ParamAccess.item); //0
            pManager.AddTextParameter("Output", "info", "Multiconsult information", GH_ParamAccess.list);//01
            pManager.AddGenericParameter("Axes", "axs", "Multiconsult information", GH_ParamAccess.list);//02
            pManager.AddGenericParameter("Floors", "axs", "Multiconsult information", GH_ParamAccess.list);//02
            pManager.AddGenericParameter("Levels", "axs", "Multiconsult information", GH_ParamAccess.list);//02

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //inputs
            
            Building bd = new Building();
            DA.GetData(0, ref bd);

            //variables
            List<string> ot = new List<string>() { "---code is running---" };
            List<Axis> allAxis = new List<Axis>();
            allAxis = bd.axes;
            //code

            ot.Add("span1 = " + bd.spanDirection1);
            ot.Add("span2 = " + bd.spanDirection2);
            ot.Add("floorHeight = " + bd.floorHeight);

            //final line of the code
            ot.Add("---code done---");

            //set data
            DA.SetData(0, bd);
            DA.SetDataList(1, ot);
            DA.SetDataList(2, allAxis);
            DA.SetDataList(3, bd.floors);
            DA.SetDataList(4, bd.levels);
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
                return MultiConcept.Properties.Resources._16;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("32915148-3434-40F6-A7A3-7AE4DA1A6107"); }
        }
    }
}