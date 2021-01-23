using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Teigha.DatabaseServices;
using Teigha.Geometry;
using Teigha.Runtime;
using Line = Autodesk.Revit.DB.Line;

namespace CodeInBuildingCoder1.LinkCadTextToModelText
{
    internal class ReadCADUtilsByDistance
    {
        public string method_0(ElementId cadLinkTypeId, Document revitDoc)
        {
            return
                ModelPathUtils.ConvertModelPathToUserVisiblePath((revitDoc
                             .GetElement(cadLinkTypeId) as CADLinkType)
                 .GetExternalFileReference().GetAbsolutePath());
        }

		public List<CADTextModel> GetCADTextInfo3(string dwgFile, XYZ location)
		{
			List<CADTextModel> list = new List<CADTextModel>();
			using (new Services())
			{
				using (Database database = new Database(false, false))
				{
					database.ReadDwgFile(dwgFile, FileOpenMode.OpenForReadAndAllShare, false, null);
					using (database.TransactionManager.StartTransaction())
					{
						using (BlockTable blockTable = (BlockTable)database.BlockTableId.GetObject(OpenMode.ForRead))
						{
							using (SymbolTableEnumerator enumerator = blockTable.GetEnumerator())
							{
								new StringBuilder();
								while (enumerator.MoveNext())
								{
									ObjectId objectId = enumerator.Current;
									using (BlockTableRecord blockTableRecord = (BlockTableRecord)objectId.GetObject(OpenMode.ForRead))
									{
										foreach (ObjectId objectId2 in blockTableRecord)
										{
											Entity entity = (Entity)objectId2.GetObject(OpenMode.ForRead, false, false);
											CADTextModel cadtextModel = new CADTextModel();
											string name = entity.GetRXClass().Name;
											if (name != null)
											{
												if (!(name == "AcDbText"))
												{
													if (name == "AcDbMText")
													{
														MText mtext = (MText)entity;
														cadtextModel.Location = this.ConverCADPointToRevitPoint3(mtext.Location);
														cadtextModel.Text = mtext.Text;
														cadtextModel.Angel = mtext.Rotation;
														cadtextModel.Layer = mtext.Layer;
														if (Line.CreateBound(cadtextModel.Location, location).Length != 0)
														{
															list.Add(cadtextModel);
														}
													}
												}
												else
												{
													DBText dbtext = (DBText)entity;
													cadtextModel.Location = this.ConverCADPointToRevitPoint3(dbtext.Position);
													cadtextModel.Text = dbtext.TextString;
													cadtextModel.Angel = dbtext.Rotation;
													cadtextModel.Layer = dbtext.Layer;
													if (Line.CreateBound(cadtextModel.Location, location).Length != 0)
													{
														list.Add(cadtextModel);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return list;
		}

		public IList<string> GetLayerName3(string dwgFile)
        {
            IList<string> list = new List<string>();
            using (new Services())
            {
                using (Database database = new Database(false, false))
                {
                    database.ReadDwgFile(dwgFile, FileShare.Read, true, "");
                    using (Teigha.DatabaseServices.Transaction transaction =
                        database.TransactionManager.StartTransaction())
                    {
                        using (LayerTable layerTable =
                            (LayerTable)
                            transaction.GetObject(database.LayerTableId,
                                                  OpenMode.ForRead))
                        {
                            foreach (ObjectId id in layerTable)
                            {
                                LayerTableRecord layerTableRecord =
                                    (LayerTableRecord)
                                    transaction.GetObject(id, OpenMode.ForRead);
                                list.Add(layerTableRecord.Name);
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
            return list;
        }

        private XYZ ConverCADPointToRevitPoint3(Point3d point)
        {
            return new XYZ(this.MillimetersToUnits3(point.X),
                           this.MillimetersToUnits3(point.Y),
                           this.MillimetersToUnits3(point.Z));
        }

        private double MillimetersToUnits3(double value)
        {
            return UnitUtils.ConvertToInternalUnits(value, (DisplayUnitType)2);
        }
    }
}