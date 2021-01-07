using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace FamilyManagerUI
{   
    /// <summary>
    /// 分页类
    /// </summary>
    class Paging
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        List<FamilyObject> PagedList = new List<FamilyObject>();//初始化

        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="需要分页的对象"></param>
        /// <param name="每页个数"></param>
        /// <returns> List<FamilyObject></returns>
        public List<FamilyObject> Previous(List<FamilyObject> ListToPage, int RecordsPerPage)
        {
            PageIndex--;
            if (PageIndex <= 0)
            {
                PageIndex = 0;
            }
            PagedList = SetPaging(ListToPage, RecordsPerPage);
            return PagedList;
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="需要分页的对象"></param>
        /// <param name="每页个数"></param>
        /// <returns> List<FamilyObject></returns>
        public List<FamilyObject> Next(List<FamilyObject> ListToPage, int RecordsPerPage)
        {
            PageIndex++;
            if (PageIndex >= ListToPage.Count / RecordsPerPage)
            {
                PageIndex = ListToPage.Count / RecordsPerPage;
            }
            PagedList = SetPaging(ListToPage, RecordsPerPage);
            return PagedList;
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="需要分页的对象"></param>
        /// <param name="每页个数"></param>
        /// <returns> List<FamilyObject></returns>
        public  List<FamilyObject> First( List<FamilyObject> ListToPage, int RecordsPerPage)
        {
            PageIndex = 0;
            PagedList = SetPaging(ListToPage, RecordsPerPage);
            return PagedList;
        }

        /// <summary>
        /// 尾页
        /// </summary>
        /// <param name="需要分页的对象"></param>
        /// <param name="每页个数"></param>
        /// <returns> List<FamilyObject></returns>
        public  List<FamilyObject> Last( List<FamilyObject> ListToPage, int RecordsPerPage)
        {
            PageIndex = ListToPage.Count / RecordsPerPage;
            PagedList = SetPaging(ListToPage, RecordsPerPage);
            return PagedList;
        }

        /// <summary>
        /// 获取对应页的对象<FamilyObject>
        /// </summary>
        /// <param name="需要分页的对象"></param>
        /// <param name="每页个数"></param>
        /// <returns> List<FamilyObject></returns>
		public  List<FamilyObject> SetPaging( List<FamilyObject> ListToPage, int RecordsPerPage)
        {
            int PageGroup = PageIndex * RecordsPerPage;
            PagedList = ListToPage.Skip(PageGroup).Take(RecordsPerPage).ToList(); //跳过指定位置，然后取后面多少个对象
            return PagedList;
        }
    }
} 
