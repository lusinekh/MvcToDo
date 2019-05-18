using Microsoft.EntityFrameworkCore;
using MvcToDo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcToDo.Services
{
    public class ToDoListService : IToDoListService<Guid, string>
    {

        private readonly ToDoListDbContext _context;

        public ToDoListService(ToDoListDbContext context)
        {
            _context = context;
        }

        public async Task<Item<Guid, string>> AddAsync(Item<Guid, string> item)
        {

            var entity = new ToDoItem
            {
                Title = item.Title,
                Color = item.Color,
                Completed = item.IsCompleted
            };

            await _context.ToDoItems.AddAsync(entity);
            await _context.SaveChangesAsync();

            item.Id = entity.Id;

            return item;
        }

        public async Task DeleteAsync(Item<Guid, string> item)
        {

            var entity = new ToDoItem
            {
                Id = item.Id,
                Title = item.Title,
                Color = item.Color,
                Completed = item.IsCompleted
            };

            var e = await _context.ToDoItems.SingleOrDefaultAsync(p => p.Id == item.Id);
            _context.ToDoItems.Remove(e);
         await   _context.SaveChangesAsync();
        }

        public async  Task EditAsync(Item<Guid, string> item)
        {
            var entity =  await _context.ToDoItems.SingleOrDefaultAsync(p => p.Id == item.Id);

            entity.Id = item.Id;
            entity.Title = item.Title;
            entity.Color = item.Color;
            entity.Completed = item.IsCompleted;
            _context.Update(entity);
           await   _context.SaveChangesAsync();
        }

        public async Task<Item<Guid, string>> GetItem(Guid id)
        {
            var entity = await _context.ToDoItems.SingleOrDefaultAsync(p => p.Id == id);

            return new Item<Guid, string>
            {
                Id = entity.Id,
                Color = entity.Color,
                IsCompleted = entity.Completed,
                Title = entity.Title
            };

        }

        public async Task<IEnumerable<Item<Guid, string>>> GetItemsAsync(OrderBy orderBy = OrderBy.None,
            int offset = 0, 
            int limit = 10,
            bool? completed = null,
            string colorFilter = null, 
            string titleFilter = null)
        {

            var query = _context.ToDoItems.AsQueryable();

            switch (orderBy)
            {
                case OrderBy.TitleAsc:
                    query = query.OrderBy(p => p.Title);
                    break;
                case OrderBy.TitleDesc:
                    query = query.OrderByDescending(p => p.Title);
                    break;
                case OrderBy.IdAsc:
                    query = query.OrderBy(p => p.Id);
                    break;
                case OrderBy.IdDesc:
                    query = query.OrderByDescending(p => p.Id);
                    break;
                default:
                    break;
            }


            query = query.Skip(offset).Take(limit);

            if (completed.HasValue)
            {
                query = query.Where(p => p.Completed == completed);
            }

            if (!string.IsNullOrWhiteSpace(colorFilter))
            {
                query = query.Where(p => p.Color == colorFilter);
            }

            if (!string.IsNullOrWhiteSpace(titleFilter))
            {
                query = query.Where(p => p.Title.ToUpper().Contains(titleFilter.ToUpper()));
            }

            var entities = await query.ToListAsync();


            return entities.Select(entity => new Item<Guid, string>
            {
                Id = entity.Id,
                Color = entity.Color,
                IsCompleted = entity.Completed,
                Title = entity.Title
            });

        }
    }
}
