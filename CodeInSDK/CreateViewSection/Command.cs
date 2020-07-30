using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Transform = Autodesk.Revit.DB.Transform;
using View = Autodesk.Revit.DB.View;

namespace CreateViewSection
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Command : IExternalCommand
    {
        //private Members
        UIDocument m_project;      //store the current document in revit
        string m_errorInformation; //store the error information
        const Double precision = 0.000000001;

        BoundingBoxXYZ m_box;       //store the BoundingBoxXYZ reference used in creation
        Element m_currentComponent; //Store the selected element
        SelectType m_type;

        // 0 - wall ; 1- beam; 2 -floor;  -1 -invalid
        const Double Length = 10; //define half length and width of BoundingBoxXYZ
        const Double Height = 5;  //define height of the BoundingBoxXYZ

        //define a enum to indicate the selected element type
        enum SelectType
        {
            WALL = 0,
            BEAM = 1,
            FLOOR = 2,
            INVALID = -1
        }

        //Methods
        public Command()
        {
            m_type = SelectType.INVALID;
        }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                m_project = commandData.Application.ActiveUIDocument;
                //get the selected element and store it to data member.
                if (!GetSelectedElement())
                {
                    message = m_errorInformation;
                    return Result.Failed;
                }

                //create a BoundingBoxXYZ instance which used in NewViewSection() method
                if (!GenerateBoundingBoxXYZ())
                {
                    message = m_errorInformation;
                    return Result.Failed;
                }

                //create a section view.
                Transaction transaction = new Transaction(m_project.Document, "CreateSectionView");
                transaction.Start();
                //
                ElementId DetailViewId = new ElementId(-1);
                IList<Element> elems = new FilteredElementCollector(m_project.Document).OfClass(typeof(ViewFamilyType))
                    .ToElements();
                foreach (Element e in elems)
                {
                    ViewFamilyType v = e as ViewFamilyType;
                    if (v != null && v.ViewFamily == ViewFamily.Detail)
                    {
                        DetailViewId = e.Id;
                        break;
                    }
                }

                ViewSection section = ViewSection.CreateDetail(m_project.Document, DetailViewId, m_box);
                if (null == section)
                {
                    message = "Canot create the ViewSection.";
                    return Result.Failed;
                }

                //modify some parameter to make it look better.
                section.get_Parameter(BuiltInParameter.VIEW_DETAIL_LEVEL).Set(2);
                transaction.Commit();

                //if everything goes right,give successful information and return succeeded.
                TaskDialog.Show("Revit", "Create view section succeeded.");
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }

        Boolean GetSelectedElement()
        {
            //first get the selection ,and make sure only one element in it.
            ElementSet collection = new ElementSet();
            foreach (ElementId elementId in m_project.Selection.GetElementIds())
            {
                collection.Insert(m_project.Document.GetElement(elementId));
            }
            if (1 != collection.Size)
            {
                m_errorInformation = "Please select only ont element,such as a wall,a beam or a floor.";
                return false;
            }

            //Get the selected element.
            foreach (Element e in collection)
            {
                m_currentComponent = e;
            }

            //make sure the element to be a wall ,beam,or a floor.
            if (m_currentComponent is Wall)
            {
                //check whether the wall is a linear wall
                LocationCurve location = m_currentComponent.Location as LocationCurve;
                if (null == location)
                {
                    m_errorInformation = "Please selected wall should be linear.";
                    return false;
                }

                if (location.Curve is Line)
                {
                    m_type = SelectType.WALL; //when the element is a linear wall.
                    return true;
                }
                else
                {
                    m_errorInformation = "The selected wall should be linear.";
                }
            }

            FamilyInstance beam = m_currentComponent as FamilyInstance;
            if (null != beam && StructuralType.Beam == beam.StructuralType)
            {
                m_type = SelectType.BEAM; //when the element is a beam
                return true;
            }

            //if it is not a wall ,a beam or a floor,give error information
            m_errorInformation = "Please select an element, such as a wall,a beam or a floor.";
            return false;
        }

        Boolean GenerateBoundingBoxXYZ()
        {
            Transaction transaction = new Transaction(m_project.Document, "GenerateBoundingBox");
            transaction.Start();
            //First new a BoundingBoxXYZ,and set the max and min property.
            m_box = new BoundingBoxXYZ();
            m_box.Enabled = true;
            XYZ maxPoint = new XYZ(Length, Length, 0);
            XYZ minPoint = new XYZ(-Length, -Length, -Height);
            m_box.Max = maxPoint;
            m_box.Min = minPoint;

            //set transform property is the most important thing.
            //it define the origin and directions(include rightDirection,UpDirection and View Direction)of the created view.
            Transform transform = GenerateTransform();
            if (null == transform)
            {
                return false;
            }
            m_box.Transform = transform;
            transaction.Commit();

            //if all went well return true.
            return true;
        }

        /// <summary>
        /// Generate a Transform instance which as Transform property of BoundingBoxXYZ
        /// </summary>
        /// <returns></returns>
        Transform GenerateTransform()
        {
            //Because different element have different ways to create Transform.
            //so , this method just call corresponding（corresponding 相应的） method.
            if (SelectType.WALL == m_type)
            {
                return GenerateWallTransform();
            }
            else if (SelectType.BEAM == m_type)
            {
                return GenerateBeamTransform();
            }
            else if (SelectType.FLOOR == m_type)
            {
                return GenerateFloorTransform();
            }
            else
            {
                m_errorInformation = "The programe should never go here.";
                return null;
            }
        }

        /// <summary>
        /// generate a transform instance which as transform property of BoundingBoxXYZ
        /// </summary>
        /// <returns></returns>
        Transform GenerateWallTransform()
        {
            Transform transform = null;
            Wall wall = m_currentComponent as Wall;

            //because the architecture wall and curtain wall don't have analytical model line.
            //so use location property of wall object is better choice.
            //first get the location line of the wall
            LocationCurve location = wall.Location as LocationCurve;
            Line locationLine = location.Curve as Line;
            transform = Transform.Identity;

            //second find the middle point of the wall and set it as Origin property.
            XYZ mPoint = XYZMath.FindMidPoint(locationLine.GetEndPoint(0), locationLine.GetEndPoint(1));
            //midPoint is mid point of the wall location,but not the wall's.
            //the different is the elevation of the point. Then change it.
            XYZ midPoint = new XYZ(mPoint.X, mPoint.Y, mPoint.Z + GetWallMidOffsetFromLocation(wall));

            transform.Origin = midPoint;

            //at last find out the directions of the created view,and set it as Basis property
            XYZ basisZ = XYZMath.FindDirection(locationLine.GetEndPoint(0), locationLine.GetEndPoint(1));
            XYZ basisX = XYZMath.FindRightDirection(basisZ);
            XYZ basisY = XYZMath.FindUpDirection(basisZ);

            transform.set_Basis(0, basisX);
            transform.set_Basis(1, basisY);
            transform.set_Basis(2, basisZ);
            return transform;
        }

        Transform GenerateBeamTransform()
        {
            Transform transform = null;
            FamilyInstance instance = m_currentComponent as FamilyInstance;

            //first check whether the beam is horizontal.
            //in order to predigest the calculation, only allow it to be horizontal.
            double startOffset = instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).AsDouble();
            double endOffset = instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).AsDouble();
            if (-precision > startOffset - endOffset || precision < startOffset - endOffset)
            {
                m_errorInformation = "Please select a horizontal beam.";
                return transform;
            }

            //second get the analytical model line.
            AnalyticalModel model = instance.GetAnalyticalModel();
            if (null == model)
            {
                m_errorInformation = "The selected beam doesn't have analytical Model line.";
                return transform;
            }
            Curve curve = model.GetCurve();
            if (null == curve)
            {
                m_errorInformation = " The program shoule never go here.";
                return transform;
            }

            //Now I am sure I can create a transform instance.
            transform = Transform.Identity;

            //third find the middle point of the line and set it as origin property.
            XYZ startPoint = curve.GetEndPoint(0);
            XYZ endPoint = curve.GetEndPoint(1);
            XYZ midPoint = XYZMath.FindMidPoint(startPoint, endPoint);
            transform.Origin = midPoint;

            //at last find out the direction of the created view,and set it as basis property.
            XYZ basisZ = XYZMath.FindDirection(startPoint, endPoint);
            XYZ basisX = XYZMath.FindRightDirection(basisZ);
            XYZ basisY = XYZMath.FindUpDirection(basisZ);

            transform.set_Basis(0, basisX);
            transform.set_Basis(1, basisY);
            transform.set_Basis(2, basisZ);

            return transform;
        }

        Transform GenerateFloorTransform()
        {
            Transform transform = null;
            Floor floor = m_currentComponent as Floor;

            // First get the Analytical Model lines
            AnalyticalModel model = floor.GetAnalyticalModel();
            if (null == model)
            {
                m_errorInformation = "Please select a structural floor.";
                return transform;
            }

            CurveArray curves = m_project.Document.Application.Create.NewCurveArray();
            IList<Curve> curveList = model.GetCurves(AnalyticalCurveType.ActiveCurves);
            foreach (Curve curve in curveList)
            {
                curves.Append(curve);
            }

            if (null == curves || true == curves.IsEmpty)
            {
                m_errorInformation = "The program should never go here.";
                return transform;
            }

            // Now I am sure I can create a transform instance.
            transform = Transform.Identity;

            // Third find the middle point of the floor and set it as Origin property.
            Autodesk.Revit.DB.XYZ midPoint = XYZMath.FindMiddlePoint(curves);
            transform.Origin = midPoint;

            // At last find out the directions of the created view, and set it as Basis property.
            Autodesk.Revit.DB.XYZ basisZ = XYZMath.FindFloorViewDirection(curves);
            Autodesk.Revit.DB.XYZ basisX = XYZMath.FindRightDirection(basisZ);
            Autodesk.Revit.DB.XYZ basisY = XYZMath.FindUpDirection(basisZ);

            transform.set_Basis(0, basisX);
            transform.set_Basis(1, basisY);
            transform.set_Basis(2, basisZ);
            return transform;
        }

        Double GetWallMidOffsetFromLocation(Wall wall)
        {
            // First get the "Base Offset" property.
            Double baseOffset = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();

            // Second get the "Unconnected Height" property. 
            Double height = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();

            // Get the middle of of wall elevation from the wall location.
            // The elevation of wall location equals the elevation of "Base Constraint" level
            Double midOffset = baseOffset + height / 2;
            return midOffset;
        }
    }


    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateDraftingView : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Document doc = commandData.Application.ActiveUIDocument.Document;
                Transaction transaction = new Transaction(doc, "CreateDraftingView");
                transaction.Start();

                ViewFamilyType viewFamilyType = null;
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                var viewFamilyTypes = collector.OfClass(typeof(ViewFamilyType)).ToElements();
                foreach (Element e in viewFamilyTypes)
                {
                    ViewFamilyType v = e as ViewFamilyType;
                    if (v.ViewFamily == ViewFamily.Drafting)
                    {
                        viewFamilyType = v;
                        break;
                    }
                }

                ViewDrafting drafting = ViewDrafting.Create(doc, viewFamilyType.Id);
                if (null == drafting)
                {
                    message = "Can't create the ViewDrafting.";
                    return Result.Failed;
                }
                transaction.Commit();
                TaskDialog.Show("Tips", "Create view drafting succeeded.");
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }
    }
}