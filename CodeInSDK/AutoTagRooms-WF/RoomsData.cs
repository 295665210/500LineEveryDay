using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace AutoTagRooms_WF
{
    // this class can get all the rooms, room tags, room tag types and levels 
    public class RoomsData
    {
        //store the reference of application in revit
        private UIApplication m_revit;
        //store all levels which have rooms in the current document
        List<Level> m_levels = new List<Level>();
        //store all the rooms
        List<Room> m_rooms = new List<Room>();
        //store all the RoomTagTypes
        List<RoomTagType> m_roomTagTypes = new List<RoomTagType>();
        //store the room ID and all tags which  tagged to that room
        Dictionary<int, List<RoomTag>> m_roomWithTags = new Dictionary<int, List<RoomTag>>();

        //constructor of roomsData


        public RoomsData(ExternalCommandData commandData)
        {
            m_revit = commandData.Application;
            GetRooms();
            GetRoomTagsTypes();
            GetRoomWithTags();
        }

        //get all the rooms in the current document
        public IReadOnlyCollection<Room> Rooms
        {
            get
            {
                return new ReadOnlyCollection<Room>(m_rooms);
            }
        }

        //get all the levels which have rooms in the current document
        public ReadOnlyCollection<Level> Levels
        {
            get
            {
                return new ReadOnlyCollection<Level>(m_levels);
            }
        }

        public ReadOnlyCollection<RoomTagType> RoomTagTypes
        {
            get
            {
                return new ReadOnlyCollection<RoomTagType>(m_roomTagTypes);
            }
        }


        /// <summary>
        /// find all the rooms in the current document
        /// </summary>
        private void GetRooms()
        {
            Document doc = m_revit.ActiveUIDocument.Document;
            foreach (PlanTopology planTopology in doc.PlanTopologies)
            {
                if (planTopology.GetRoomIds().Count != 0 && planTopology.Level != null)
                {
                    m_levels.Add(planTopology.Level);
                    foreach (ElementId elementId in planTopology.GetRoomIds())
                    {
                        Room tmpRoom = doc.GetElement(elementId) as Room;
                        if (doc.GetElement(tmpRoom.LevelId) != null &&
                            m_roomWithTags.ContainsKey(tmpRoom.Id.IntegerValue) == false)
                        {
                            m_rooms.Add(tmpRoom);
                            m_roomWithTags.Add(tmpRoom.Id.IntegerValue, new List<RoomTag>());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// get all the roomTagTypes in the current document            
        /// </summary>
        private void GetRoomTagsTypes()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_revit.ActiveUIDocument.Document);
            collector.OfClass(typeof(FamilySymbol));
            collector.OfCategory(BuiltInCategory.OST_RoomTags);
            m_roomTagTypes = collector.Cast<RoomTagType>().ToList<RoomTagType>();
        }

        /// <summary>
        /// get all the room tags which tagged rooms
        /// </summary>
        private void GetRoomWithTags()
        {
            Document doc = m_revit.ActiveUIDocument.Document;
            IEnumerable<RoomTag> roomTags =
                from elem in ((new FilteredElementCollector(doc)).WherePasses(new RoomTagFilter()).ToElements())
                let roomTag = elem as RoomTag
                where (roomTag != null) && (roomTag.Room != null)
                select roomTag;

            foreach (RoomTag roomTag in roomTags)
            {
                if (m_roomWithTags.ContainsKey(roomTag.Room.Id.IntegerValue))
                {
                    List<RoomTag> tmpList = m_roomWithTags[roomTag.Id.IntegerValue];
                    tmpList.Add(roomTag);
                }
            }
        }

        /// <summary>
        /// auto tag rooms with specified roomTagType in a level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="tagType"></param>
        public void AutoTagRooms(Level level, RoomTagType tagType)
        {
            PlanTopology planTopology = m_revit.ActiveUIDocument.Document.get_PlanTopology(level);
            SubTransaction subTransaction = new SubTransaction(m_revit.ActiveUIDocument.Document);

            subTransaction.Start();

            foreach (ElementId eid in planTopology.GetRoomIds())
            {
                Room tmpRoom = m_revit.ActiveUIDocument.Document.GetElement(eid) as Room;
                if (m_revit.ActiveUIDocument.Document.GetElement(tmpRoom.LevelId) != null && tmpRoom.Location != null)
                {
                    //create a specified type roomTag to tag a room
                    LocationPoint locationPoint = tmpRoom.Location as LocationPoint;
                    UV point = new UV(locationPoint.Point.X, locationPoint.Point.Y);
                    RoomTag newTag =m_revit.ActiveUIDocument.Document.Create.NewRoomTag(new LinkElementId(tmpRoom.Id), point, null);
                    newTag.RoomTagType = tagType;
                    List<RoomTag> tagListInTheRoom = m_roomWithTags[newTag.Room.Id.IntegerValue];
                    tagListInTheRoom.Add(newTag);
                }
            }
            subTransaction.Commit();
        }



        public int GetTagNumber(Room room, RoomTagType tagType)
        {
            int count = 0;
            List<RoomTag> tagListInTheRoom = m_roomWithTags[room.Id.IntegerValue];
            foreach (RoomTag roomTag in tagListInTheRoom)
            {
                if (roomTag.RoomTagType.Id.IntegerValue == tagType.Id.IntegerValue)
                {
                    count++;
                }
            }
            return count;
        }


    }
}