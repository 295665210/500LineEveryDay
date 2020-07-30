// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using Autodesk.Revit.DB;
// using Autodesk.Revit.UI;
//
// namespace CurvedBeam.ViewModel
// {
//     public class CurvedBeamViewModel
//     {
//       
//
// #region Class memeber variables
//         private UIApplication m_revit = null;
//         ArrayList m_beamMaps = new ArrayList(); //list of beams's type  
//         ArrayList m_levels = new ArrayList();   //list of levels
// #endregion
//
// #region Command class properties
//         //list of all type of beams
//         public ArrayList BeamMaps
//         {
//             get
//             {
//                 return m_beamMaps;
//             }
//         }
//         public ArrayList LevelMaps
//         {
//             get { return m_levels; }
//         }
// #endregion
//         
//         public  CurvedBeamViewModel(ExternalCommandData commandData)
//         {
//             m_revit = commandData.Application;
//          
//             InitializeData(); //初始化加载数据
//
//             StartCreateArcCommand = new DelegateCommand();
//             StartCreateArcCommand.ExecuteAction = new Action<object>(CreateBeamArc);
//
//             StartCreateBeamEllispeCommand = new DelegateCommand();
//             StartCreateBeamEllispeCommand.ExecuteAction = new Action<object>(CreateBeamEllispe);
//
//             // Transaction ts = new Transaction(doc, "mvvm里执行命令的测试");
//             // ts.Start();
//             // BeamType = BeamTypes.FirstOrDefault(x => x.Name == _currentSelectOfBeam);
//             // string info = BeamType.Name == null ? "选择的梁类型没有绑上" : BeamType.Name;
//             // TaskDialog.Show("tips", info);
//             // ts.Commit();
//         }
//
//          private bool Initialize()
//         {
//             try
//             {
//                 ElementClassFilter levelFilter = new ElementClassFilter(typeof(Level));
//                 ElementClassFilter famFilter = new ElementClassFilter(typeof(Family));
//                 LogicalOrFilter orFilter = new LogicalOrFilter(levelFilter, famFilter);
//                 FilteredElementCollector collector = new FilteredElementCollector(m_revit.ActiveUIDocument.Document);
//                 FilteredElementIterator i = collector.WherePasses(orFilter).GetElementIterator();
//                 i.Reset();
//                 bool moreElement = i.MoveNext();
//                 while (moreElement)
//                 {
//                     object o = i.Current;
//
//                     // add level to list
//                     Level level = o as Level;
//                     if (null != level)
//                     {
//                         m_levels.Add(new LevelMap(level));
//                         goto nextLoop;
//                     }
//
//                     // get
//                     Family f = o as Family;
//                     if (null == f)
//                     {
//                         goto nextLoop;
//                     }
//
//                     foreach (ElementId elementId in f.GetFamilySymbolIds())
//                     {
//                        object symbol = m_revit.ActiveUIDocument.Document.GetElement(elementId);
//                         FamilySymbol familyType = symbol as FamilySymbol;
//                         if (null == familyType)
//                         {
//                             goto nextLoop;
//                         }
//                         if (null == familyType.Category)
//                         {
//                             goto nextLoop;
//                         }
//
//                         // add symbols of beams and braces to lists 
//                         string categoryName = familyType.Category.Name;
//                         if ("Structural Framing" == categoryName)
//                         {
//                             m_beamMaps.Add(new SymbolMap(familyType));
//                         }
//                     }
//                 nextLoop:
//                     moreElement = i.MoveNext();
//                 }
//             }
//             catch (Exception ex)
//             {
//                 throw new Exception(ex.ToString());
//             }
//             return true;
//         }
//     }
//
//      public class SymbolMap
//     {
//         #region SymbolMap class member variables
//         string m_symbolName = "";
//         FamilySymbol m_symbol = null;
//         #endregion
//
//
//         /// <summary>
//         /// constructor without parameter is forbidden
//         /// </summary>
//         private SymbolMap()
//         {
//             // no operation 
//         }
//
//
//         /// <summary>
//         /// constructor
//         /// </summary>
//         /// <param name="symbol">family symbol</param>
//         public SymbolMap(FamilySymbol symbol)
//         {
//             m_symbol = symbol;
//             string familyName = "";
//             if (null != symbol.Family)
//             {
//                 familyName = symbol.Family.Name;
//             }
//             m_symbolName = familyName + " : " + symbol.Name;
//         }
//
//
//         /// <summary>
//         /// SymbolName property
//         /// </summary>
//         public string SymbolName
//         {
//             get
//             {
//                 return m_symbolName;
//             }
//         }
//
//
//         /// <summary>
//         /// ElementType property
//         /// </summary>
//         public FamilySymbol ElementType
//         {
//             get
//             {
//                 return m_symbol;
//             }
//         }
//     }
//
//
//     /// <summary>
//     /// assistant class contains level and it's name
//     /// </summary>
//     public class LevelMap
//     {
//         #region LevelMap class member variable
//         string m_levelName = "";
//         Level m_level = null;
//         #endregion
//
//
//         #region LevelMap Constructors
//         /// <summary>
//         /// constructor without parameter is forbidden
//         /// </summary>
//         private LevelMap()
//         {
//             // no operation
//         }
//
//
//         /// <summary>
//         /// constructor
//         /// </summary>
//         /// <param name="level">level</param>
//         public LevelMap(Level level)
//         {
//             m_level = level;
//             m_levelName = level.Name;
//         }
//         #endregion
//
//
//         #region LevelMap properties
//         /// <summary>
//         /// LevelName property
//         /// </summary>
//         public string LevelName
//         {
//             get
//             {
//                 return m_levelName;
//             }
//         }
//
//         /// <summary>
//         /// Level property
//         /// </summary>
//         public Level Level
//         {
//             get
//             {
//                 return m_level;
//             }
//         }
//         #endregion
//     }
// }