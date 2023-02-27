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
    public class BrepToBuilding : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BrepToBuilding class.
        /// </summary>
        public BrepToBuilding()
          : base("BrepToMultiBuilding", "MC_Build",
            "create basic class multibuilding",
            "Multiconsult", "Basic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "Br", "Brep", GH_ParamAccess.item);
            pManager.AddNumberParameter("Spacing1", "s1", "spacing between structural axes in 1 direction[m]", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("Spacing2", "s2", "spacing between structural axes in 2 direction[m]", GH_ParamAccess.item, 7);
            pManager.AddNumberParameter("Floors", "fH", "spacing between floors[m]", GH_ParamAccess.item, 3.2);
            pManager.AddBooleanParameter("Adjust","ad","adjust span exactly to brep boundary",GH_ParamAccess.item, false);
            pManager.AddNumberParameter("axisTransposeX", "axTX", "offset of the basde point[m]", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("axisTransposeY", "axtY", "offset of the basde point[m]", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
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
            //inputs
            Brep br = new Brep();
            double s1 = 0;
            double s2 = 0;
            double fH = 0;
            bool ad = false;
            double mbptX = 0;
            double mbptY = 0;

            DA.GetData(0, ref br);
            DA.GetData(1, ref s1);
            DA.GetData(2, ref s2);
            DA.GetData(3, ref fH);
            DA.GetData(4, ref ad);
            DA.GetData(5, ref mbptX);
            DA.GetData(6, ref mbptY);

            //variables
            List<string> ot = new List<string>() { "---code is running---" };
            ot.Add("adjust mode = " + ad);

            //get center of brep
            var amp = AreaMassProperties.Compute(br);
            Point3d cpt = amp.Centroid;
            BoundingBox bb = br.GetBoundingBox(true);
            Point3d bpt = new Point3d(cpt.X, cpt.Y, bb.Min.Z);
            Point3d tpt = new Point3d(cpt.X, cpt.Y, bb.Max.Z);
            double floorLevel = bpt.Z;
            double maxLevel = tpt.Z;


            //create new floor height value if user want to adjust to brep
            if (ad)
            {
                double buildingHeight = maxLevel - floorLevel;
                double newfh = Math.Round(buildingHeight / fH, 0);
                fH = buildingHeight / newfh;
            }


            //make planes for floor height
            ot.Add("--creating floor levels--");
            List<Plane> levels = createLevels(floorLevel, maxLevel, fH, cpt);

            //intersection of floor levels and brep
            List<Curve> floorBoundaries = createBoundaries(levels, br);

            //find the biggest floor, it will be reference for the global system
            Surface unMaxSrf = findTheBiggestFloor(floorBoundaries);

            //check the longest domain to check the main direction
            List<Vector3d> sVecs = findMainVectors(unMaxSrf);
            Vector3d sVec1 = sVecs[0];
            Vector3d sVec2 = sVecs[1];

            ot.Add("--direction Vectors--");
            ot.Add("vec1 = " + sVec1.X + " , " + sVec1.Y + " , " + sVec1.Z);
            ot.Add("vec2 = " + sVec2.X + " , " + sVec2.Y + " , " + sVec2.Z);

            //create axis lines in both direction in a specific range 
            // this algorithm is not checking if lines are in the floor plan!
            var mainAxes = createAxisSystem(sVecs, bpt, floorBoundaries[0], ad, s1, s2, 0, 0);

            List<Axis> mainAxes1 = mainAxes[0];     //axes in 1 direction
            List<Axis> mainAxes2 = mainAxes[1];     //axes in 2 direction

            //intersection between axes and floor plans
            List<List<Axis>> mainTouchAxes1 = new List<List<Axis>>() { };
            List<List<Axis>> mainTouchAxes2 = new List<List<Axis>>() { };
            findTouchingAxes(floorBoundaries, mainAxes1, mainAxes2, out mainTouchAxes1, out mainTouchAxes2);

            //Create levels, axes with points, and list of all characteristic points
            List<Point3d> allAxPts = new List<Point3d>();
            List<Level> mainLevels = new List<Level>();
            //make all characteristic points and Levels, in levels the axes class is made!
            makeCharastericPointsAndMainLevels(floorBoundaries, mainTouchAxes1, mainTouchAxes2, out allAxPts, out mainLevels);

            //add axis axis points to levels as charasteric points
            addCharasteristicPointsToLevels(mainLevels);

            //add ground level, copy the first level to ground
            List<Level> buildingLevels = addGroundLevel(mainLevels, bpt);

            //create floors
            List<Floor> floors = new List<Floor>();
            for (int i = 1; i < buildingLevels.Count; i++)
            {
                Floor f = new Floor(i - 1, "building floor", buildingLevels[i], buildingLevels[i-1]);
                floors.Add(f);
            }


            //code
            ot.Add("--- creating building class --- ");
            Building bd = new Building(0, "type1");
            bd.brep = br;
            ot.Add("    -- adding floors -- ");
            bd.floors = floors;
            ot.Add("    -- adding levels -- ");
            bd.levels = buildingLevels;
            ot.Add("    -- adding direction Vectors and spans -- ");
            bd.direction1 = sVec1;
            ot.Add("     - direction vector1 added - vec1 = [" + sVec1.X + "," + sVec1.Y + "," + sVec1.Z );
            bd.direction2 = sVec2;
            ot.Add("     - direction vector2 added - vec2 = [" + sVec2.X + "," + sVec2.Y + "," + sVec2.Z);
            bd.spanDirection1 = s1;
            ot.Add("     - span in direction1 added -  " + s1 + "[m]");
            bd.spanDirection2 = s2;
            ot.Add("     - span in direction2 added -  " + s2 + "[m]");
            bd.floorHeight = fH;
            ot.Add("     - span in vertical direction added -  " + fH + "[m]");
            //final line of the code
            ot.Add("---code done---");

            //set data
            DA.SetData(0, bd);
            DA.SetDataList(1, ot);
        }
        // create lines starting at spacing point with specifing length in two directions
        Line createLineAtPoint(Point3d _basePoint, Vector3d _dirVect, double _length)
        {

            Vector3d negDirVect = Vector3d.Negate(_dirVect);
            Vector3d dirVect = Vector3d.Multiply(_dirVect, _length / 2);
            Vector3d ndirVect = Vector3d.Multiply(_dirVect, -_length / 2);
            Point3d spt = Point3d.Add(_basePoint, ndirVect);
            Point3d ept = Point3d.Add(_basePoint, dirVect);
            Line line = new Line(spt, ept);

            return line;
        }

        List<Plane> createLevels(double floorLevel, double maxLevel, double fH, Point3d cpt)
        {

            List<double> levelsh = new List<double>();
            List<Plane> levels = new List<Plane>();
            while (floorLevel <= maxLevel - fH)
            {
                floorLevel = floorLevel + fH;
                levelsh.Add(floorLevel);
                levels.Add(new Plane(new Point3d(cpt.X, cpt.Y, floorLevel), new Vector3d(0, 0, 1)));
            }

            return levels;
        }
        List<Curve> createBoundaries(List<Plane> levels, Brep br)
        {
            int n = levels.Count;
            List<Curve> floorBoundaries = new List<Curve>();

            //intersection of floor levels and brep
            for (int i = 0; i < n; i++)
            {
                Curve[] floorCurves;
                Point3d[] floorPoints;
                Plane pl = levels[i];
                Intersection.BrepPlane(br, pl, 0.000001, out floorCurves, out floorPoints);
                Curve[] floorBoundary = Curve.JoinCurves(floorCurves);
                floorBoundaries.Add(floorBoundary[0]);
            }
            return floorBoundaries;
        }

        Surface findTheBiggestFloor(List<Curve> floorBoundaries)
        {
            Dictionary<Curve, double> fboundaries = new Dictionary<Curve, double>();
            foreach (Curve fb in floorBoundaries)
            {
                fboundaries.Add(fb, AreaMassProperties.Compute(fb).Area);
            }
            var sortedFBs = fboundaries.OrderBy(x => x.Value).ToList();
            Curve minFb = sortedFBs[0].Key;
            Curve maxFb = sortedFBs[sortedFBs.Count - 1].Key;
            List<Curve> minmaxFB = new List<Curve>() { minFb, maxFb };

            //create planar floor
            Brep maxSrf = Brep.CreatePlanarBreps(maxFb)[0];
            Surface unMaxSrf = maxSrf.Faces[0].UnderlyingSurface();
            return unMaxSrf;
        }


        List<Vector3d> findMainVectors(Surface unMaxSrf)
        {
            double dl1 = unMaxSrf.Domain(0).Length;
            double dl2 = unMaxSrf.Domain(1).Length;


            Vector3d sVec1 = unMaxSrf.CurvatureAt(0, 0).Direction(0);
            Vector3d sVec2 = unMaxSrf.CurvatureAt(0, 0).Direction(1);

            if (dl1 > dl2)
            {
                sVec1 = unMaxSrf.CurvatureAt(0, 0).Direction(0);
                sVec2 = unMaxSrf.CurvatureAt(0, 0).Direction(1);
            }
            else
            {
                sVec1 = unMaxSrf.CurvatureAt(0, 0).Direction(1);
                sVec2 = unMaxSrf.CurvatureAt(0, 0).Direction(0);
            }

            return new List<Vector3d>() { sVec1, sVec2 };
        }

        List<List<Axis>> createAxisSystem(
            List<Vector3d> sVecs, Point3d bpt, Curve boundary, bool ad, 
            double s1, double s2, double transposeBptX, double transposeBptY)
        {
            var sVec1 = sVecs[0];
            var sVec2 = sVecs[1];
            Line line11 = new Line(bpt, sVec1, 100);
            Line line12 = new Line(bpt, sVec1, -100);
            Line line1 = new Line(line11.To, line12.To);

            Line line21 = new Line(bpt, sVec2, 100);
            Line line22 = new Line(bpt, sVec2, -100);
            Line line2 = new Line(line21.To, line22.To);

            List<Line> mainAxes1 = new List<Line>() { };
            List<Line> mainAxes2 = new List<Line>() { };

            //if user choose adjust to brep new spacing 1 and 2 have to made
            if (ad)
            {
                double s1len;
                double s2len;
                Curve floorProjB = Rhino.Geometry.Curve.ProjectToPlane(boundary, Plane.WorldXY);
                var intCrvLin = Intersection.CurveLine(floorProjB, line1, 0.0001, 0.0001);
                Point3d pt11 = intCrvLin[0].PointA;
                Point3d pt12 = intCrvLin[1].PointA;
                s1len = new Line(pt11, pt12).Length;
                var intCrvLin2 = Intersection.CurveLine(floorProjB, line2, 0.0001, 0.0001);
                Point3d pt21 = intCrvLin2[0].PointA;
                Point3d pt22 = intCrvLin2[1].PointA;
                s2len = new Line(pt21, pt22).Length;
                double s1int = Math.Round(s1len / s1, 0);
                double s2int = Math.Round(s2len / s2, 0);

                s1 = s1len / s1int;
                s2 = s2len / s2int;
            }

            sVec1 = Vector3d.Multiply(sVec1, s1);
            sVec2 = Vector3d.Multiply(sVec2, s2);
            Vector3d sVec3 = Vector3d.Negate(sVec1);
            Vector3d sVec4 = Vector3d.Negate(sVec2);

            List<Line> axis11 = new List<Line>();
            List<Line> axis12 = new List<Line>();
            List<Line> axis21 = new List<Line>();
            List<Line> axis22 = new List<Line>();

            //hopefully nobody will cross more than 40 axes in any direction
            int numberOfAxes = 40;
            for (int i = 0; i < numberOfAxes; i++)
            {
                Point3d nextbpt11 = Point3d.Add(new Point3d(bpt.X + transposeBptX, bpt.Y+ transposeBptY, bpt.Z), i * sVec1);
                axis11.Add(createLineAtPoint(nextbpt11, sVec2, 1000));
                Point3d nextbpt12 = Point3d.Add(new Point3d(bpt.X + transposeBptX, bpt.Y + transposeBptY, bpt.Z), i * sVec1 * -1);
                axis12.Add(createLineAtPoint(nextbpt12, sVec2, 1000));
                Point3d nextbpt21 = Point3d.Add(new Point3d(bpt.X + transposeBptX, bpt.Y + transposeBptY, bpt.Z), i * sVec2);
                axis21.Add(createLineAtPoint(nextbpt21, sVec1, 1000));
                Point3d nextbpt22 = Point3d.Add(new Point3d(bpt.X + transposeBptX, bpt.Y + transposeBptY, bpt.Z), i * sVec2 * -1);
                axis22.Add(createLineAtPoint(nextbpt22, sVec1, 1000));
            }

            axis12.Reverse();
            axis11.RemoveAt(0);
            axis22.Reverse();
            axis21.RemoveAt(0);

            //axes in 1 direction
            mainAxes1.AddRange(axis12);
            mainAxes1.AddRange(axis11);
            //axes in 2 direction
            mainAxes2.AddRange(axis22);
            mainAxes2.AddRange(axis21);

            
            List<Axis> axes1 = new List<Axis>();
            List<Axis> axes2 = new List<Axis>();

            int ia1 = 0;
            foreach (Line l1 in mainAxes1)
            {
                ia1++;
                string aname = "X" + ia1;

                axes1.Add(new Axis(ia1, aname, 1, new Line(l1.From, l1.To)));
            
            }
            int ia2 = 0;
            foreach (Line l2 in mainAxes2)
            {
                ia2++;
                string aname = "Y" + ia2;

                axes2.Add(new Axis(ia2, aname, 2, new Line(l2.From, l2.To)));

            }
            List<List<Axis>> allAx = new List<List<Axis>>();
            allAx.Add(axes1);
            allAx.Add(axes2);
            return allAx;
        }

        void findTouchingAxes(List<Curve> floorBoundaries, List<Axis> mainAxes1, List<Axis> mainAxes2, out List<List<Axis>> mainTouchAxes1, out List<List<Axis>> mainTouchAxes2)
        {
            //intersection between axes and floor plans
            mainTouchAxes1 = new List<List<Axis>>() { };
            mainTouchAxes2 = new List<List<Axis>>() { };

            //number of axes and floors
            int numberOfAxes = mainAxes1.Count;
            int numberOfFloors = floorBoundaries.Count;

            for (int i = 0; i < numberOfFloors; i++)
            {
                Curve floorB = floorBoundaries[i];

                List<Axis> mta1 = new List<Axis>();
                List<Axis> mta2 = new List<Axis>();

                for (int j = 0; j < numberOfAxes; j++)
                {
                    Curve axis1 = mainAxes1[j].line.ToNurbsCurve();
                    Curve axis2 = mainAxes2[j].line.ToNurbsCurve();
                    Curve floorProjB = Rhino.Geometry.Curve.ProjectToPlane(floorB, Plane.WorldXY);
                    Curve axisPro1 = Rhino.Geometry.Curve.ProjectToPlane(axis1, Plane.WorldXY);
                    Curve axisPro2 = Rhino.Geometry.Curve.ProjectToPlane(axis2, Plane.WorldXY);

                    var intC1 = Intersection.CurveCurve(floorProjB, axisPro1, 0.00001, 0.00001);
                    if (intC1.Count != 0)
                    {
                        mta1.Add(mainAxes1[j]);

                    }
                    var intC2 = Intersection.CurveCurve(floorProjB, axisPro2, 0.00001, 0.00001);
                    if (intC2.Count != 0)
                    {
                        mta2.Add(mainAxes2[j]);
                    }
                }
                mainTouchAxes1.Add(mta1);
                mainTouchAxes2.Add(mta2);
            }
        }

        void makeCharastericPointsAndMainLevels(List<Curve> floorBoundaries, List<List<Axis>> mainTouchAxes1, List<List<Axis>> mainTouchAxes2, 
            out List<Point3d> allAxPts, out List<Level> mainLevels)
        {
            //number floors
            int numberOfFloors = floorBoundaries.Count;
            //make characteristic points
            allAxPts = new List<Point3d>();
            //make list of level class
            mainLevels = new List<Level>();

            for (int i = 0; i < numberOfFloors; i++)
            {
                Curve floorB = floorBoundaries[i];
                double zlevel = AreaMassProperties.Compute(floorB).Centroid.Z;
                List<Axis> thisFloorAxes1 = mainTouchAxes1[i];
                List<Axis> thisFloorAxes2 = mainTouchAxes2[i];
                List<Axis> axes1 = new List<Axis>();
                List<Axis> axes2 = new List<Axis>();
                for (int j = 0; j < thisFloorAxes1.Count; j++)
                {
                    //make axis in first direction
                    Axis axis1 = new Axis(thisFloorAxes1[j].id, thisFloorAxes1[j].name, 0,
                       new Line(
                           new Point3d(thisFloorAxes1[j].line.FromX, thisFloorAxes1[j].line.FromY, zlevel),
                           new Point3d(thisFloorAxes1[j].line.ToX, thisFloorAxes1[j].line.ToY, zlevel)));
                    axes1.Add(axis1);
                }
                for (int k = 0; k < thisFloorAxes2.Count; k++)
                {
                    //make axis in second direction
                    Axis axis2 = new Axis(thisFloorAxes2[k].id, thisFloorAxes2[k].name, 1, new Line(
                            new Point3d(thisFloorAxes2[k].line.FromX, thisFloorAxes2[k].line.FromY, zlevel),
                            new Point3d(thisFloorAxes2[k].line.ToX, thisFloorAxes2[k].line.ToY, zlevel)));
                    axes2.Add(axis2);
                }
                Level level = new Level(i + 1, "building level", zlevel, axes1, axes2);
                level.floorBoundaries = floorB;
                mainLevels.Add(level);
            }
        }


        void addCharasteristicPointsToLevels(List<Level> mainLevels)
        {
            //add middle characteristic points to levels
            foreach (Level l in mainLevels)
            {
                Level level = l;
                List<Point3d> chpts = new List<Point3d>();
                for (int j = 0; j < l.axes1.Count; j++)
                {

                    //make axis in first direction
                    Curve ax1 = l.axes1[j].line.ToNurbsCurve();
                    for (int k = 0; k < l.axes2.Count; k++)
                    {
                        Curve ax2 = l.axes2[k].line.ToNurbsCurve();
                        //make axis in second direction
                        var aax12 = Intersection.CurveCurve(ax1, ax2, 0.00001, 0.00001);
                        Point3d chPt = new Point3d(aax12[0].PointA.X, aax12[0].PointA.Y, level.levelValue);
                        chpts.Add(chPt);
                        Curve boundary = level.floorBoundaries;
                        if (boundary.Contains(chPt) == PointContainment.Inside || boundary.Contains(chPt) == PointContainment.Coincident)
                        { 
                            l.axes1[j].characteristicPoints.Add(chPt);
                            l.axes2[k].characteristicPoints.Add(chPt);
                        }
                        
                    }
                }
                level.nodes = chpts;
            }
        }

        List<Level> addGroundLevel(List<Level> mainLevels , Point3d bpt)
        {
            //copy the lowest floor to ground floor
            Level level0 = new Level(0, "ground level", bpt.Z);
            List<Axis> axesG1 = new List<Axis>();
            List<Axis> axesG2 = new List<Axis>();
            foreach (var ax1 in mainLevels[0].axes1)
            {
                Axis a1 = new Axis(ax1.id, ax1.name, ax1.direction,
                    new Line(
                        new Point3d(ax1.line.FromX, ax1.line.FromY, bpt.Z),
                        new Point3d(ax1.line.ToX, ax1.line.ToY, bpt.Z)
                        )
                    );
                foreach (Point3d a1p in ax1.characteristicPoints)
                {
                    a1.characteristicPoints.Add(new Point3d(a1p.X, a1p.Y, bpt.Z));
                }
                axesG1.Add(a1);
            }


            foreach (var ax2 in mainLevels[0].axes2)
            {
                Axis a2 = new Axis(ax2.id, ax2.name, ax2.direction,
                    new Line(
                        new Point3d(ax2.line.FromX, ax2.line.FromY, bpt.Z),
                        new Point3d(ax2.line.ToX, ax2.line.ToY, bpt.Z)
                        )
                    );
                foreach (Point3d a2p in ax2.characteristicPoints)
                {
                    a2.characteristicPoints.Add(new Point3d(a2p.X, a2p.Y, bpt.Z));
                }
                axesG2.Add(a2);
            }

            level0.nodes = new List<Point3d>();
            foreach (Point3d pt in mainLevels[0].nodes)
            {
                level0.nodes.Add(new Point3d(pt.X, pt.Y, bpt.Z));
            }

            level0.axes1 = axesG1;
            level0.axes2 = axesG2;
            List<Level> buildingLevels = new List<Level>() { level0 };
            buildingLevels.AddRange(mainLevels);

            List<Axis> axes = new List<Axis>();
            foreach (var l in buildingLevels)
            {
                axes.AddRange(l.axes1);
                axes.AddRange(l.axes2);
            }

            return buildingLevels;
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
                return MultiConcept.Properties.Resources._13;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("34915848-3434-40F6-A7A3-7AE4DA1A6107"); }
        }
    }
}