// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Autodesk.Revit.ApplicationServices;
// using Autodesk.Revit.Attributes;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.DB.Plumbing;
// using Autodesk.Revit.UI;
// using Autodesk.Revit.UI.Selection;
//
// namespace RevitFoundation.CodeInQuick.Source.UpPipe
// {
// 	// Token: 0x02000014 RID: 20
// 	[Transaction(TransactionMode.Manual)]
// 	internal class UpPipe : IExternalCommand
// 	{
// 		// Token: 0x0600005A RID: 90 RVA: 0x00006A60 File Offset: 0x00004C60
// 		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
// 		{
// 			UIApplication application = commandData.Application;
// 			Application application2 = application.Application;
// 			Document document = application.ActiveUIDocument.Document;
// 			UIDocument activeUIDocument = commandData.Application.ActiveUIDocument;
// 			View activeView = activeUIDocument.ActiveView;
// 			try
// 			{
// 				string a = null;
// 				string text = this.ShowDialog(a);
// 				if (text == null)
// 				{
// 					return Result.Cancelled;
// 				}
// 				double num = Convert.ToDouble(text);
// 				double num2 = UnitUtils.Convert(num, DisplayUnitType.DUT_MILLIMETERS,    DisplayUnitType.DUT_DECIMAL_FEET);
// 				Reference reference = activeUIDocument.Selection.PickObject(ObjectType.Element, "请选择需要末端绘制立管的管道");
// 				XYZ globalPoint = reference.GlobalPoint;
// 				Pipe pipe = document.GetElement(reference) as Pipe;
// 				Transaction transaction = new Transaction(document, "自动生成立管");
// 				transaction.Start();
// 				ICollection<ElementId> source = ElementTransformUtils.CopyElement(document, pipe.Id, XYZ.Zero);
// 				LocationCurve locationCurve = pipe.Location as LocationCurve;
// 				Curve curve = locationCurve.Curve;
// 				XYZ endPoint = curve.GetEndPoint(0);
// 				XYZ endPoint2 = curve.GetEndPoint(1);
// 				Line line = Line.CreateBound(globalPoint, endPoint);
// 				Line line2 = Line.CreateBound(globalPoint, endPoint2);
// 				double length = line.Length;
// 				double length2 = line2.Length;
// 				XYZ xyz;
// 				if (length < length2)
// 				{
// 					xyz = endPoint;
// 				}
// 				else
// 				{
// 					xyz = endPoint2;
// 				}
// 				XYZ xyz2 = new XYZ(xyz.X, xyz.Y, xyz.Z + num2);
// 				new XYZ(0.0, 0.0, num2);
// 				Line line3 = Line.CreateBound(xyz, xyz2);
// 				this.ChangeLine(document.GetElement(source.ElementAt(0)), line3);
// 				document.Create.NewElbowFitting(this.GetConnector(document.GetElement(source.ElementAt(0)), xyz), this.GetConnector(pipe, xyz));
// 				transaction.Commit();
// 			}
// 			catch (Exception ex)
// 			{
// 				message = ex.Message;
// 				TaskDialog.Show("出错了！", string.Format("错误信息:\n{0}", message));
// 				return 0;
// 			}
// 			return 0;
// 		}
//
// 		// Token: 0x0600005B RID: 91 RVA: 0x00005968 File Offset: 0x00003B68
// 		private void ChangeLine(Element elem, Line line)
// 		{
// 			LocationCurve locationCurve = elem.Location as LocationCurve;
// 			locationCurve.Curve = line;
// 		}
//
// 		// Token: 0x0600005C RID: 92 RVA: 0x00004DC8 File Offset: 0x00002FC8
// 		private Connector GetConnector(Element elem, XYZ xyz)
// 		{
// 			Connector result = null;
// 			MEPCurve mepcurve = elem as MEPCurve;
// 			ConnectorSetIterator connectorSetIterator = mepcurve.ConnectorManager.Connectors.ForwardIterator();
// 			while (connectorSetIterator.MoveNext())
// 			{
// 				object obj = connectorSetIterator.Current;
// 				Connector connector = obj as Connector;
// 				if (connector.Origin.IsAlmostEqualTo(xyz))
// 				{
// 					result = connector;
// 					return result;
// 				}
// 			}
// 			return result;
// 		}
//
// 		// Token: 0x0600005D RID: 93 RVA: 0x00006C5C File Offset: 0x00004E5C
// 		private string ShowDialog(string a)
// 		{
// 			string text = null;
// 			UpPipeWpf upPipeWpf = new UpPipeWpf();
// 			bool? flag = upPipeWpf.ShowDialog();
// 			string result;
// 			if (flag.GetValueOrDefault() & flag != null)
// 			{
// 				text = upPipeWpf.TextboxHight.Text;
// 				result = text;
// 			}
// 			else
// 			{
// 				result = text;
// 			}
// 			return result;
// 		}
// 	}
// }
