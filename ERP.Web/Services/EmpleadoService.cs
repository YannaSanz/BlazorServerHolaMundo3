using ERP.Web.Data;
using ERP.Web.Domain.Dto;
using ERP.Web.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Services
{
    public interface IEmpleadoService
    {
        Task<List<EmpleadoDto>> Consultar(string filtro);
        Task<bool> Crear(EmpleadoDto request);
        Task<bool> Eliminar(int Id);
        Task<bool> Modificar(EmpleadoDto request);
    }

    public class EmpleadoService : IEmpleadoService
    {
        private readonly AppDbContext _context;
        //public List<EmpleadoDto> empleado;

        public EmpleadoService(AppDbContext context)
        {
            _context = context;
        }
        ///Consultar los clientes existentes
        public async Task<List<EmpleadoDto>> Consultar(string filtro)
        {
            var empleado = await
                _context.Empleados
                .Include(c => c.DatosPersonales)
                .Where(c => c.DatosPersonales.Nombre.Contains(filtro))
                .Select(
                    c =>
                    new EmpleadoDto()
                    {
                        Id = c.Id,
                        PersonaId = c.PersonaId,
                        Sueldo = c.Sueldo,
                        DatosPersonales = new PersonaDto()
                        {
                            Id = c.DatosPersonales.Id,
                            Nombre = c.DatosPersonales.Nombre,
                            FechaDeNacimiento = c.DatosPersonales.FechaDeNacimiento
                        }
                    }
                )
                .ToListAsync();
            return empleado;
        }
        public async Task<bool> Crear(EmpleadoDto request)
        {
            var empleado = Empleado.Create(
                request.DatosPersonales.Nombre,
                request.DatosPersonales.FechaDeNacimiento,
                request.Sueldo
            );
            _context.Empleados.Add(empleado);
            ;
            return (await _context.SaveChangesAsync()) > 0;
        }
        public async Task<bool> Modificar(EmpleadoDto request)
        {
            //1. Busco el empleado
            var empleado = await _context.Empleados
                .Include(c => c.DatosPersonales)
                .FirstOrDefaultAsync(c => c.Id == request.Id);
            //2. Modifico el empleado
            empleado!.DatosPersonales.Nombre = request.DatosPersonales.Nombre;
            empleado!.DatosPersonales.FechaDeNacimiento = request.DatosPersonales.FechaDeNacimiento;
            empleado!.Sueldo = request.Sueldo;
            //Guardo los cambios
            return (await _context.SaveChangesAsync()) > 0;
        }
        public async Task<bool> Eliminar(int Id)
        {
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(c => c.Id == Id);

            _context.Empleados.Remove(empleado!);

            return (await _context.SaveChangesAsync()) > 0;
        }


    }

}
