using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using MultiConcept.classes;
using Rhino.Geometry;

namespace MultiConcept.components
{
    public class PreviewResults : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the PreviewResults class.
        /// </summary>
        public PreviewResults()
          : base("Preview Results", "data",
            "Preview building results",
            "Multiconsult", "Visualize")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("mBuilding", "mB", "Multiconsult Building class", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("mBuilding", "mB", "Multiconsult Building class", GH_ParamAccess.item); //0
            pManager.AddTextParameter("output", "o", "Information about this algorithm", GH_ParamAccess.list); //1
            pManager.AddNumberParameter("emissions", "em", "Information about emissions", GH_ParamAccess.list); //2
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
            List<double> emissions = new List<double>();

            outputs.Add("--- result algorithm started ---");
            outputs.Add("building is type =" + building.type);
            outputs.Add("building name = " + building.name);
            outputs.Add("building id = " + building.id);
            double globalLength = 0;
            foreach (var column in building.columns)
                globalLength= globalLength + column.GetLength();
            outputs.Add("Building columns have got global length of [m] = " );
            outputs.Add(globalLength.ToString());
            outputs.Add("Building walls have got global area of [m2] = ");
            double wallarea = 0;
            foreach (var wall in building.walls)
                wallarea = wallarea + AreaMassProperties.Compute(wall).Area;
            outputs.Add(wallarea.ToString());
            outputs.Add("Building walls have got global volume of [m3] = ");
            double wallvolume = wallarea * 0.2;
            outputs.Add(wallvolume.ToString());
            outputs.Add("Building slabs have got global area of [m2] =");
            double slabArea = 0;
            foreach (var lev in building.levels)
             {
                if (lev.slab != null)
                    slabArea = slabArea + AreaMassProperties.Compute(lev.slab.brep).Area;
            
            }
                    
            outputs.Add(slabArea.ToString());
            outputs.Add("Building slabs have got global volume of [m3] =");
            double th = 0.2;// building.levels[1].slabs[0].thickness;
            double slabVolume = slabArea * th;
            outputs.Add(slabVolume.ToString());

            outputs.Add("--CO2 emission calculation--");
            var emission = calculateCO2emission(building);
            outputs.Add("column emission = " + emission[0] + "kg CO2");
            outputs.Add("beam emission = " + emission[1] + "kg CO2");
            outputs.Add("slab emission = " + emission[2] + "kg CO2");
            outputs.Add("wall emission = " + emission[3] + "kg CO2");
            outputs.Add("--- result algorithm ended ---");

            emissions.Add(emission[0]);
            emissions.Add(emission[1]);
            emissions.Add(emission[2]);
            emissions.Add(emission[3]);

            DA.SetData(0, building);   //0
            DA.SetDataList(1, outputs);   //1
            DA.SetDataList(2, emissions);   //2
        }
        List<double> calculateCO2emission(Building _building)
        { 
        List<double> co2emission = new List<double>();

            //material coefficients
            double columnCoef = 0;
            double beamCoef = 0;
            double slabCoef = 0;
            double wallCoef = 0;

            int type = _building.type;
            // steel section 24849,16
            // H235 153,03
            // tre 72
            // B35 350


            if (type == 0)
            {
                columnCoef = 0.00253* 24849;
                beamCoef = 0.00253 * 24849;
                slabCoef = 350;
                wallCoef = 350;
            }
            else if (type == 1)
            {
                columnCoef = 0.00253* 24849;
                beamCoef = 0.00253 * 24849;
                slabCoef = 350;
                wallCoef = 350;
            }
            else if (type == 2)
            {
                columnCoef = 0.00253 * 24849;
                beamCoef = 0.00253 * 24849;
                slabCoef = 350;
                wallCoef = 350;
            }
            else if (type == 3)
            {
                columnCoef = 0.2 * 0.2 * 72;
                beamCoef = 0.2 * 0.2 * 72;
                slabCoef = 72;
                wallCoef = 72;
            }
            else if (type == 4)
            {
                columnCoef = 0.00253 * 24849;
                beamCoef = 0.00253 * 24849;
                slabCoef = 153;
                wallCoef = 350;
            }

            //columns emission
            double colemission = 0;
            foreach (var c in _building.columns)
            {
                colemission = colemission + c.GetLength(0) * columnCoef;
            }
                //beams emission
                double beamemission = 0;
            if(_building.beams != null)
            { 
            foreach (var b in  _building.beams)
            {
                beamemission = beamemission + b.GetLength() * beamCoef;
            }
}
            //slab emission
            double slabemission = 0;
            foreach (var l in _building.levels)
            {
                if (l.slab != null)
                    slabemission = slabemission + l.slab.brep.GetArea() * l.slab.thickness* 0.001 * slabCoef;
                
            }
            //walls emission
            double wallemission = 0;
            foreach (var w in _building.walls)
            {
                wallemission = wallemission + w.GetArea() * 0.2 *wallCoef;
            }

            co2emission.Add(colemission);
            co2emission.Add(beamemission);
            co2emission.Add(slabemission);
            co2emission.Add(wallemission);

            return co2emission;
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
                return MultiConcept.Properties.Resources._19;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("505ED5AD-339D-4054-910A-FE6FD2A81CC4"); }
        }
    }
}