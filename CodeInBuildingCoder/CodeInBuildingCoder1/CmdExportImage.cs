using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeInBuildingCoder1
{
    [Transaction(TransactionMode.Manual)]
    class CmdExportImage : IExternalCommand
    {
        #region SetWhiteRenderBackground
        void SetWhiteRenderBackground(View3D view)
        {
            RenderingSettings rs = view.GetRenderingSettings();
            rs.BackgroundStyle = BackgroundStyle.Color;

            ColorBackgroundSettings cbs = (ColorBackgroundSettings) rs.GetBackgroundSettings();
            cbs.Color = new Color(255, 0, 0);
            rs.SetBackgroundSettings(cbs);
            view.SetRenderingSettings(rs);
        }

        static string ExportToImage(Document doc)
        {
            var tempFileName = Path.ChangeExtension(Path.GetRandomFileName(), "png");
            string tempImageFile;
            try
            {
                tempImageFile = Path.Combine(Path.GetTempPath(), tempFileName);
            }
            catch (Exception e)
            {
                return null;
            }
            IList<ElementId> views = new List<ElementId>();

            try
            {
                var collector = new FilteredElementCollector(doc);
                var viewFamilyType = collector.OfClass(typeof(ViewFamilyType)).OfType<ViewFamilyType>()
                    .FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);

                var view3D = (viewFamilyType != null) ? View3D.CreateIsometric(doc, viewFamilyType.Id) : null;
                if (view3D != null)
                {
                    Color white = new Color(255, 255, 255);
                    view3D.SetBackground(ViewDisplayBackground.CreateGradient(white, white, white));
                    views.Add(view3D.Id);

                    var graphicDisplayOptions = view3D.get_Parameter(BuiltInParameter.MODEL_GRAPHICS_STYLE);

                    graphicDisplayOptions.Set(6);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            var ieo = new ImageExportOptions()
            {
                FilePath = tempImageFile, FitDirection = FitDirectionType.Horizontal,
                HLRandWFViewsFileType = ImageFileType.PNG,
                ImageResolution = ImageResolution.DPI_150,
                ShouldCreateWebSite = false
            };
            if (views.Count > 0)
            {
                ieo.SetViewsAndSheets(views);
                ieo.ExportRange = ExportRange.SetOfViews;
            }
            else
            {
                ieo.ExportRange = ExportRange.VisibleRegionOfCurrentView;
            }

            ieo.ZoomType = ZoomFitType.FitToPage;
            ieo.ViewName = "tmp";

            if (ImageExportOptions.IsValidFileName(tempImageFile))
            {
                try
                {
                    doc.ExportImage(ieo);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                return string.Empty;
            }

            var files = Directory.GetFiles(Path.GetTempPath(),
                                           string.Format("{0}*.*", Path.GetFileNameWithoutExtension(tempFileName)));
            return files.Length > 0 ? files[0] : string.Empty;
        }

        /// <summary>
        /// Wrapper for old sample code.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        static Result ExportToImage2(Document doc)
        {
            Result r = Result.Failed;
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Export Image");
                string filepath = ExportToImage(doc);
                tx.RollBack();
                if (0 < filepath.Length)
                {
                    Process.Start(filepath);
                    r = Result.Succeeded;
                }
            }
            return r;
        }

        /// <summary>
        /// New code as described in Revit API discussion
        /// on how to export an image from a specific view using revit API C#
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        static Result ExportToImage3(Document doc)
        {
            Result r = Result.Failed;
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Export Image");
                string destop_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                View view = doc.ActiveView;
                string filepath = Path.Combine(destop_path, view.Name);
                ImageExportOptions img = new ImageExportOptions();

                img.ZoomType = ZoomFitType.FitToPage;
                img.PixelSize = 512;
                img.ImageResolution = ImageResolution.DPI_600;
                img.FitDirection = FitDirectionType.Horizontal;
                img.ExportRange = ExportRange.CurrentView;
                img.HLRandWFViewsFileType = ImageFileType.PNG;
                img.FilePath = filepath;
                img.ShadowViewsFileType = ImageFileType.PNG;

                doc.ExportImage(img);
                tx.RollBack();

                filepath = Path.ChangeExtension(filepath, "png");
                Process.Start(filepath);
            }
            return r;
        }
        #endregion

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            bool use_old_code = false;

            Result r = use_old_code ? ExportToImage2(doc) : ExportToImage3(doc);
            return r;
        }
    }
}