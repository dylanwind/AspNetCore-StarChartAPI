using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        #region HttpGet
        [HttpGet("{id:int}",Name ="GetById")]
        public IActionResult GetById(int id)
        {
            var res = _context.CelestialObjects.Find(id);
           

            if (res == null)
            {
                return NotFound();
            }

            var satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id);
            res.Satellites = satellites == null ? null : satellites.ToList();

            return Ok(res);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var celestialObjs = _context.CelestialObjects.Where(e => e.Name == name);


            if (!celestialObjs.Any())
            {
                return NotFound();
            }

            foreach (var obj in celestialObjs)
            {
                var satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == obj.Id).ToList();
                obj.Satellites = satellites;
            }
            

            return Ok(celestialObjs);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjs = _context.CelestialObjects.ToList();

            foreach (CelestialObject celestial
                in celestialObjs)
            {
                var satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestial.Id).ToList();
                celestial.Satellites = satellites;
            }

            return Ok(celestialObjs);
        }

        #endregion

        #region HttpPost
        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject obj)
        {
            _context.CelestialObjects.Add(obj);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { obj.Id }, obj);
        }

        #endregion

        #region HttpPUT
        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject obj)
        {
            var celestial = _context.CelestialObjects.Find(id);

            if (celestial == null)
            {
                return NotFound();
            }

            celestial.Name = obj.Name;
            celestial.OrbitalPeriod = obj.OrbitalPeriod;
            celestial.OrbitedObjectId = obj.OrbitedObjectId;

            _context.Update(celestial);
            _context.SaveChanges();

            return NoContent();
        }
        #endregion

        #region HttpPatch
        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestial = _context.CelestialObjects.Find(id);

            if (celestial == null)
            {
                return NotFound();
            }

            celestial.Name = name;

            _context.Update(celestial);
            _context.SaveChanges();

            return NoContent();
        }
        #endregion

        #region HttpDelete
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestials = _context.CelestialObjects.Where(e => e.Id == id || e.OrbitedObjectId == id);
             

            if (!celestials.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestials);
            _context.SaveChanges();

            return NoContent();
        }
        #endregion
    }
}
