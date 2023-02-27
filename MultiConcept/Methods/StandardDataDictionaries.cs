using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
/*
            //types of buildings
                --- type 0  = 0 - passtopt
                --- type 1  = 1 - passtopt conform
                --- type 2  = 2 - passtopt conform spennarm
                --- type 3  = 3 - massivtre
                --- type 4  = 4 - hulldekker
*/


namespace MultiConcept.Methods
{
    public class StandardDataDictionaries
    {
        
       
        public static double getColumnDistribution(double _span, int _type)
        {
            double span = 0;

            Dictionary<double, double> columnSpansType0 = new Dictionary<double, double>();
            columnSpansType0.Add(5, 4);
            columnSpansType0.Add(6, 3.5);
            columnSpansType0.Add(7, 3.5);
            columnSpansType0.Add(8, 3);

            Dictionary<double, double> columnSpansType1 = new Dictionary<double, double>();
            columnSpansType1.Add(5, 4);
            columnSpansType1.Add(6, 4);
            columnSpansType1.Add(7, 4);
            columnSpansType1.Add(8, 3.5);
            columnSpansType1.Add(9, 3.5);
            columnSpansType1.Add(10, 3);
            columnSpansType1.Add(11, 3);
            columnSpansType1.Add(12, 2.5);
            columnSpansType1.Add(13, 2.5);

            Dictionary<double, double> columnSpansType2 = new Dictionary<double, double>();
            columnSpansType2.Add(5, 4);
            columnSpansType2.Add(6, 4);
            columnSpansType2.Add(7, 4);
            columnSpansType2.Add(8, 3.5);
            columnSpansType2.Add(9, 3.5);
            columnSpansType2.Add(10, 3);
            columnSpansType2.Add(11, 3);
            columnSpansType2.Add(12, 2.5);
            columnSpansType2.Add(13, 2.5);

            Dictionary<double, double> columnSpansType3 = new Dictionary<double, double>();
            columnSpansType3.Add(5, 3.5);
            columnSpansType3.Add(6, 3);
            columnSpansType3.Add(7, 3);
            columnSpansType3.Add(8, 3);
            columnSpansType3.Add(9, 2.5);
            columnSpansType3.Add(10, 2.5);

            Dictionary<double, double> columnSpansType4 = new Dictionary<double, double>();
            columnSpansType4.Add(5, 4);
            columnSpansType4.Add(6, 4);
            columnSpansType4.Add(7, 4);
            columnSpansType4.Add(8, 3.5);
            columnSpansType4.Add(9, 3.5);
            columnSpansType4.Add(10, 3);
            columnSpansType4.Add(11, 3);
            columnSpansType4.Add(12, 2.5);
            columnSpansType4.Add(13, 2.5);

            if (_type == 0)
            {
                span = columnSpansType0[getKeyFromDoubleDictionary(_span, columnSpansType0)];
            }
            else if (_type == 1)
            {
                span = columnSpansType1[getKeyFromDoubleDictionary(_span, columnSpansType1)];
            }
            else if (_type == 2)
            {
                span = columnSpansType2[getKeyFromDoubleDictionary(_span, columnSpansType2)];
            }
            else if (_type == 3)
            {
                span = columnSpansType3[getKeyFromDoubleDictionary(_span, columnSpansType3)];
            }
            else if (_type == 4)
            {
                span = columnSpansType4[getKeyFromDoubleDictionary(_span, columnSpansType4)];
            }

            return span;
        }

        public static double getSlabSection(double _span, int _type)
        {
            double thickness = 0;
            
            
            Dictionary<double, double> slabSectionType0 = new Dictionary<double, double>();
            slabSectionType0.Add(5, 180);
            slabSectionType0.Add(6, 200);
            slabSectionType0.Add(7, 250);
            slabSectionType0.Add(8, 270);

            Dictionary<double, double> slabSectionType1 = new Dictionary<double, double>();
            slabSectionType1.Add(5, 230);
            slabSectionType1.Add(6, 230);
            slabSectionType1.Add(7, 260);
            slabSectionType1.Add(8, 260);

            Dictionary<double, double> slabSectionType2 = new Dictionary<double, double>();
            slabSectionType2.Add(5, 230);
            slabSectionType2.Add(6, 230);
            slabSectionType2.Add(7, 230);
            slabSectionType2.Add(8, 260);
            slabSectionType2.Add(9, 260);
            slabSectionType2.Add(10, 260);
            slabSectionType2.Add(11, 290);
            slabSectionType2.Add(12, 290);
            slabSectionType2.Add(13, 290);

            Dictionary<double, double> slabSectionType3 = new Dictionary<double, double>();
            slabSectionType3.Add(5, 160);
            slabSectionType3.Add(6, 190);
            slabSectionType3.Add(7, 220);

            Dictionary<double, double> slabSectionType4 = new Dictionary<double, double>();
            slabSectionType4.Add(5, 200);
            slabSectionType4.Add(6, 200);
            slabSectionType4.Add(7, 220);
            slabSectionType4.Add(8, 220);
            slabSectionType4.Add(9, 265);
            slabSectionType4.Add(10, 320);
            slabSectionType4.Add(11, 320);
            slabSectionType4.Add(12, 400);
            slabSectionType4.Add(13, 500);

            if (_type == 0)
            {
                double k = getKeyFromDoubleDictionary(_span, slabSectionType0);
                if (k != -1)
                    thickness = slabSectionType0[getKeyFromDoubleDictionary(_span, slabSectionType0)];
                else
                    thickness = 0.1;
            }
            else if (_type == 1)
            {
                double k = getKeyFromDoubleDictionary(_span, slabSectionType1);
                if (k != -1)
                    thickness = slabSectionType1[getKeyFromDoubleDictionary(_span, slabSectionType1)];
                else
                    thickness = 0.1;
            }
            else if (_type == 2)
            {
                double k = getKeyFromDoubleDictionary(_span, slabSectionType2);
                if (k != -1)
                    thickness = slabSectionType2[getKeyFromDoubleDictionary(_span, slabSectionType2)];
                else
                    thickness = 0.1;
            }
            else if (_type == 3)
            {
                double k = getKeyFromDoubleDictionary(_span, slabSectionType3);
                if (k != -1)
                    thickness = slabSectionType3[getKeyFromDoubleDictionary(_span, slabSectionType3)];
                else
                    thickness = 0.1;
            }
            else if (_type == 4)
            {
                double k = getKeyFromDoubleDictionary(_span, slabSectionType4);
                if (k != -1)
                    thickness = slabSectionType4[getKeyFromDoubleDictionary(_span, slabSectionType4)];
                else
                    thickness = 0.1;
            }

            return thickness;
        }

        public static double getWallSection(int _type)
        {
            double thickness = 0;

            if (_type == 0)
            {
                thickness = 200;
            }
            else if (_type == 1)
            {
                thickness = 200;
            }
            else if (_type == 2)
            {
                thickness = 200;
            }
            else if (_type == 3)
            {
                thickness = 200;
            }
            else if (_type == 4)
            {
                thickness = 200;
            }

            return thickness;
        }

        public static string getBeamSection(double _span, int _type)
        {
            string section = "no";

            Dictionary<double, string> beamSectionType0 = new Dictionary<double, string>();
            beamSectionType0.Add(0, "no");

            Dictionary<double, string> beamSectionType1 = new Dictionary<double, string>();
            beamSectionType1.Add(5, "UPE" + 240);
            beamSectionType1.Add(6, "UPE" + 240);
            beamSectionType1.Add(7, "UPE" + 270);
            beamSectionType1.Add(8, "UPE" + 270);

            Dictionary<double, string> beamSectionType2 = new Dictionary<double, string>();
            beamSectionType2.Add(5, "UPE" + 240);
            beamSectionType2.Add(6, "UPE" + 240);
            beamSectionType2.Add(7, "UPE" + 270);
            beamSectionType2.Add(8, "UPE" + 270);
            beamSectionType2.Add(9, "UPE" + 270);
            beamSectionType2.Add(10, "UPE" + 270);
            beamSectionType2.Add(11, "UPE" + 300);
            beamSectionType2.Add(12, "UPE" + 300);
            beamSectionType2.Add(13, "UPE" + 300);

            Dictionary<double, string> beamSectionType3 = new Dictionary<double, string>();
            beamSectionType3.Add(5, "140x260");
            beamSectionType3.Add(6, "140x260");
            beamSectionType3.Add(7, "140x260");

            Dictionary<double, string> beamSectionType4 = new Dictionary<double, string>();
            beamSectionType4.Add(5, "200");
            beamSectionType4.Add(6, "200");
            beamSectionType4.Add(7, "220");
            beamSectionType4.Add(8, "220");
            beamSectionType4.Add(9, "265");
            beamSectionType4.Add(10, "320");
            beamSectionType4.Add(11, "320");
            beamSectionType4.Add(12, "400");
            beamSectionType4.Add(13, "500");
           
            if (_type == 0)
            {
                section = "no edge beams in this type of the structure";
            }
            else if (_type == 1)
            {
                section= beamSectionType1[getKeyFromStringDictionary(_span , beamSectionType1)];
            }
            else if (_type == 2)
            {
                section = beamSectionType2[getKeyFromStringDictionary(_span, beamSectionType2)];
            }
            else if (_type == 3)
            {
                section = beamSectionType3[getKeyFromStringDictionary(_span, beamSectionType3)];
            }
            else if (_type == 4)
            {
                section = beamSectionType4[getKeyFromStringDictionary(_span, beamSectionType4)];
            }

            return section;
        }


        public static double getKeyFromDoubleDictionary(double _value, Dictionary<double, double> _dic)
        {
            double correctKey = 0;

            List<double> keys = _dic.Keys.ToList();
            double k0 = keys.First();
            double k3 = keys.Last();
            if (_value <= k0)
            {
                correctKey = k0;
            }
            else if (_value > k3)
            {
                correctKey = -1; //no correct value
            }
            else
            {
                for (int i = 0; i < keys.Count - 1; i++)
                {
                    double k1 = keys[i];
                    double k2 = keys[i + 1];

                    if (_value > k1 && _value <= k2)
                    {
                        correctKey = k2;

                    }
                }     
            }
            return correctKey;
        }

        public static double getKeyFromStringDictionary(double _value, Dictionary<double, string> _dic)
        {
            double correctKey = 0;

            List<double> keys = _dic.Keys.ToList();
            double k0 = keys.First();
            double k3 = keys.Last();
            if (_value <= k0)
            {
                correctKey = k0;
            }
            else if (_value > k3)
            {
                correctKey = -1; //no correct value
            }
            else
            {
                for (int i = 0; i < keys.Count - 1; i++)
                {
                    double k1 = keys[i];
                    double k2 = keys[i + 1];

                    if (_value > k1 && _value <= k2)
                    {
                        correctKey = k2;

                    }
                }
            }
            return correctKey;
        }
    }
}