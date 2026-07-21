using Microsoft.AspNetCore.Mvc;
using bibliotecaMVC.Models;
using System.Collections.Generic;
using System;

namespace bibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        public IActionResult Index()
        {
            // Crear una lista de autores de ejemplo
            List<Autor> Autores = new List<Autor>
            {
                new Autor { Id = 1, Nombre = "Gabriel", Apellido = "García Márquez", Nacionalidad = "Colombiano", FechaNacimiento = new DateTime(1927, 3, 6), Activo = true },
                new Autor { Id = 2, Nombre = "Isabel", Apellido = "Allende", Nacionalidad = "Chilena", FechaNacimiento = new DateTime(1942, 8, 2), Activo = true },
                new Autor { Id = 3, Nombre = "Mario", Apellido = "Vargas Llosa", Nacionalidad = "Peruano", FechaNacimiento = new DateTime(1936, 3, 28), Activo = false },
                new Autor { Id = 4, Nombre = "Jorge Luis", Apellido = "Borges", Nacionalidad = "Argentino", FechaNacimiento = new DateTime(1899, 8, 24), Activo = false },
                new Autor { Id = 5, Nombre = "Pablo", Apellido = "Neruda", Nacionalidad = "Chileno", FechaNacimiento = new DateTime(1904, 7, 12), Activo = false }

            };  


            return View(Autores);
        }
    }
}
