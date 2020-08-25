using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodeInTangsengjiewa3.BinLibrary.Extensions
{
  public static class ConnectorExtension
    {
        public static Connector GetConnectedCon(this Connector connector)
        {
            var result = default(Connector);
            var connectors = connector.AllRefs;
            var connectordir = connector.CoordinateSystem.BasisZ;
            var connectorOrigin = connector.Origin;

            foreach (Connector con in connectors)
            {
                if (con.ConnectorType == ConnectorType.End || con.ConnectorType == ConnectorType.Curve)
                {
                    var conOrigin = con.Origin;
                    var condir = con.CoordinateSystem.BasisZ;
                    if (connectorOrigin.IsAlmostEqualTo(conOrigin) && connectordir.IsOppositeDirection(condir))
                    {
                        result = con;
                    }
                }
            }
            return result;
        }
    }
}
