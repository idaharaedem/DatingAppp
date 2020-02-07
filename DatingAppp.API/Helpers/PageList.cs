using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingAppp.API.Helpers
{
    public class PageList<T> : List<T>
    {
        public int CurrentPage  { get; set; }   
        public int TotalPages { get; set; } 
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PageList(List<T> listItems, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count/ (double)pageSize);
            this.AddRange(listItems);
        }     

        public static async Task<PageList<T>> CreatAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();

            var listItems = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PageList<T>(listItems,count,pageNumber,pageSize);
        }
    }
}