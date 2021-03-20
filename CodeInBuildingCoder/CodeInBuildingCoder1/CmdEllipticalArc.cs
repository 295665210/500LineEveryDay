using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{

    //运行不出来？未将对象引用实例？
    [Transaction(TransactionMode.Manual)]
    class CmdEllipticalArc : IExternalCommand
    {
        Curve CreateEllipse(Application app) //Ellipse  /ɪˈlɪps/  椭圆
        {
            XYZ center = XYZ.Zero;
            double radX = 30;
            double radY = 50;

            XYZ xVec = XYZ.BasisX;
            XYZ yVec = XYZ.BasisY;

            double param0 = 0.0;
            double param1 = 2 * Math.PI;

            Curve c = Ellipse.CreateCurve(center, radX, radY, xVec, yVec, param0, param1);
            //Create a line from ellipse center in direction of target angle;

            double targetAngle = Math.PI / 3.0;
            XYZ direction = new XYZ(Math.Cos(targetAngle), Math.Sign(targetAngle), 0);

            Line line = Line.CreateBound(center, direction);

            //Find intersection between line and ellipse:
            IntersectionResultArray results;
            c.Intersect(line, out results);
            //Find shortest intersection segment:  //segment :段，部分；
            foreach (IntersectionResult result in results)
            {
                double p = result.UVPoint.U;
                if (p < param1)
                {
                    param1 = p;
                }
            }

            //Apply parameter to the ellipse:
            c.MakeBound(param0, param1);
            return c;
        }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Application app = commandData.Application.Application;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("arcTest");
                Curve c = CreateEllipse(app);
                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}