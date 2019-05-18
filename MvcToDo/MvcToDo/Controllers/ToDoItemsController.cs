using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcToDo.Models;
using MvcToDo.Services;

namespace MvcToDo.Controllers
{
    public class ToDoItemsController : Controller
    {
        private readonly ToDoListDbContext _context;
        private readonly IToDoListService<Guid, string> _toDo;

        public ToDoItemsController(ToDoListDbContext context, IToDoListService<Guid, string> toDo)
        {
            _context = context;
            _toDo = toDo;
        }



        // GET: ToDoItems
        public async Task<IActionResult> Index(string sort)
        {
            IEnumerable<Item<Guid, string>> items = Enumerable.Empty<Item<Guid, string>>();

            if (sort != null && sort.Trim().ToUpper() == "ASC")
            {
                items = await _toDo.GetItemsAsync(OrderBy.TitleAsc);
                //query = _context.ToDoItems.OrderBy(p => p.Title);
            }

            if (sort != null && sort.Trim().ToUpper() == "DESC")
            {
                items = await _toDo.GetItemsAsync(OrderBy.TitleDesc);
                //query = _context.ToDoItems.OrderByDescending(p => p.Title);
            }

            if (sort != null && sort.Trim().ToUpper() == "IDASC")
            {
                items = await _toDo.GetItemsAsync(OrderBy.IdDesc);
                //query = _context.ToDoItems.OrderBy(p => p.Title);
            }

            if (sort != null && sort.Trim().ToUpper() == "IDDESC")
            {
                items = await _toDo.GetItemsAsync(OrderBy.IdDesc);
                //query = _context.ToDoItems.OrderByDescending(p => p.Title);
            }



            if (sort == null)
            {
                items = await _toDo.GetItemsAsync();
            }

            //IQueryable<ToDoItem> query = _context.ToDoItems.AsQueryable();

            //if (sort != null && sort.Trim().ToUpper() == "ASC")
            //{
            //    query = _context.ToDoItems.OrderBy(p => p.Title);
            //}

            //if (sort != null && sort.Trim().ToUpper() == "DESC")
            //{
            //    query = _context.ToDoItems.OrderByDescending(p => p.Title);
            //}

            //var result = await query.ToListAsync();

            //return View(result);

            return View(items);
        }


        // GET: ToDoItems/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var toDoItem = await _toDo.GetItem(id.Value);

            var itemnew = new Item<Guid, string>
            {

                Id=toDoItem.Id,
                Color = toDoItem.Color,
                IsCompleted = toDoItem.IsCompleted,
                Title = toDoItem.Title

            };

 
            if (toDoItem == null)
            {
                return NotFound();
            }

            return View(itemnew);
        }

        // GET: ToDoItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ToDoItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Color,IsCompleted")] Item<Guid,string> toDoItem)
        {
            if (ModelState.IsValid)
            {
                await _toDo.AddAsync(new Item<Guid, string>
                {
                    Color = toDoItem.Color,
                    Title = toDoItem.Title,
                    IsCompleted = toDoItem.IsCompleted,
                });

                return RedirectToAction(nameof(Index));
                //toDoItem.Id = Guid.NewGuid();
                //_context.Add(toDoItem);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
            return View(toDoItem);
        }

        // GET: ToDoItems/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoItem = await _toDo.GetItem(id.Value);
            if (toDoItem == null)
            {
                return NotFound();
            }


            return View(toDoItem);
        }

        // POST: ToDoItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,IsCompleted,Color")] Item<Guid,string> toDoItem)
        {
            if (id != toDoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _toDo.EditAsync(new Item<Guid, string>
                    {
                        Id = toDoItem.Id,
                        Color = toDoItem.Color,
                        Title = toDoItem.Title,
                        IsCompleted = toDoItem.IsCompleted
                    });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoItemExists(toDoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(toDoItem);
        }

        // GET: ToDoItems/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDoItem = await _toDo.GetItem(id.Value);

            if (toDoItem == null)
            {
                return NotFound();
            }

            return View(new Item<Guid, string> 
            {
                Id = toDoItem.Id,
                Color = toDoItem.Color,
                IsCompleted = toDoItem.IsCompleted,
                Title = toDoItem.Title
            });

            
        }

        // POST: ToDoItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {

            var toDoItem = await _toDo.GetItem(id);
            await _toDo.DeleteAsync(toDoItem);
            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ToDoItemExists(Guid id)
        {
            return _context.ToDoItems.Any(e => e.Id == id);
        }
    }
}
