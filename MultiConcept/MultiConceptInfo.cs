using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace MultiConcept
{
    public class MultiConceptInfo : GH_AssemblyInfo
    {
        public override string Name => "MultiConcept";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("22EF06D3-78C2-411B-87F9-8E77BB5ADDB8");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}