using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigaApp
{
    public class Paginacao<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public Paginacao(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool TemPaginaAnterior
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool TemProximaPagina
        {
            get
            {
                return PageIndex < TotalPages;
            }
        }

        public static async Task<Paginacao<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Paginacao<T>(items, count, pageIndex, pageSize);
        }

        public static Paginacao<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new Paginacao<T>(items, count, pageIndex, pageSize);
        }

    }
}
