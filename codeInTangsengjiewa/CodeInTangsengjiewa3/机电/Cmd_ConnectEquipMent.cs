using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CodeInTangsengjiewa3.BinLibrary.Extensions;

namespace CodeInTangsengjiewa3.机电
{
    /// <summary>
    /// 设备连接： 消火栓
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    class Cmd_ConnectEquipMent : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            View acview = uidoc.ActiveView;

            var familyInsRef = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is FamilyInstance));
            var pipeRef = sel.PickObject(ObjectType.Element, doc.GetSelectionFilter(m => m is Pipe));

            var pipe = pipeRef.GetElement(doc) as Pipe;
            var familyIns = familyInsRef.GetElement(doc) as FamilyInstance;
            var equipmentCons = familyIns.MEPModel.ConnectorManager.Connectors;
            var firstEquipCon = equipmentCons.Cast<Connector>().First(m =>
            {
                return m.ConnectorType == ConnectorType.Curve || m.ConnectorType == ConnectorType.End;
            });
            var conRadius = firstEquipCon.Radius;
            var condia = conRadius * 2;
            var origin = firstEquipCon.Origin;
            var conDir = firstEquipCon.CoordinateSystem.BasisZ;

            var pipelocationline = pipe.LocationLine();

            Transaction ts = new Transaction(doc, "设备连接");
            try
            {
                ts.Start();
                //有连接件，水平生成一段管
                var firstlineEnd1 = origin;
                var firstlineEnd2 = origin + 200d.MetricToFeet() * conDir;
                var firstline = Line.CreateBound(firstlineEnd1, firstlineEnd2);
                var firstPipe = Pipe.Create(doc, pipe.MEPSystem.GetTypeId(), pipe.GetTypeId(), pipe.ReferenceLevel.Id,
                    firstlineEnd1,
                    firstlineEnd2);

                firstPipe.LookupParameter("直径").Set(conRadius * 2);

                //生成垂直管道
                var secondlineEnd1 = firstlineEnd2;
                var secondlineEnd2 = secondlineEnd1 + XYZ.BasisZ * (pipelocationline.StartPoint().Z - secondlineEnd1.Z);
                var secondpipe = Pipe.Create(doc, pipe.MEPSystem.GetTypeId(), pipe.GetTypeId(), pipe.ReferenceLevel.Id,
                    secondlineEnd1, secondlineEnd2);

                secondpipe.LookupParameter("直径").Set(conRadius * 2);

                //生成第三根管道
                var thirdPipeEnd1 = secondlineEnd2;
                var thirdPipeEnd2 = thirdPipeEnd1.ProjectToXLine(pipelocationline);

                var thirdPipe = Pipe.Create(doc, pipe.MEPSystem.GetTypeId(), pipe.GetTypeId(), pipe.ReferenceLevel.Id,
                    thirdPipeEnd1, thirdPipeEnd2);

                thirdPipe.LookupParameter("直径").Set(conRadius * 2);

                //链接所有管道
                var firstpipeCons = firstPipe.ConnectorManager.Connectors;
                foreach (Connector item in firstpipeCons)
                {
                    if (item.ConnectorType == ConnectorType.End || ConnectorType.Curve == item.ConnectorType)
                    {
                        if (firstEquipCon.Origin.IsAlmostEqualTo(item.Origin))
                        {
                            item.ConnectTo(firstEquipCon);
                        }
                    }
                }
                firstPipe.ElbowConnect(secondpipe);
                secondpipe.ElbowConnect(thirdPipe);
                //链接剩余管道
                //do it yourself;

                ts.Commit();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                if (ts.GetStatus() == TransactionStatus.Started)
                {
                    ts.RollBack();
                }
            }
            return Result.Succeeded;
        }
    }
}