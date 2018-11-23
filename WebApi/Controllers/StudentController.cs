using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiConsumer.Controllers
{
    public class StudentController : ApiController
    {

 //---------------------------------------- Get - To Extract Student Details (Address if true) ------------------------------------------//

        // api/stu      ||         api/stu/includeAddress=true
        [HttpGet]
        public IHttpActionResult GetAllStudents(bool includeAddress = false)
        {
            IList<Models.StudentViewModel> students = null;

            using (var ctx = new SchoolDBEntities())
            {
                students = ctx.Students.Include("StudentAddress").Select(s => new Models.StudentViewModel()
                {
                    Id = s.StudentId,
                    FirstNameStu = s.FirstName,
                    LastNameStu = s.LastName,
                    Address = s.StudentAddress == null || includeAddress == false ? null : new Models.AddressViewModel()
                    {
                        StudentId = s.StudentAddress.StudentId,
                        Address1 = s.StudentAddress.Address1,
                        Address2 = s.StudentAddress.Address2,
                        City = s.StudentAddress.City,
                        State = s.StudentAddress.State
                    }
                }).ToList<Models.StudentViewModel>();
            }

            if (students == null)
            {
                return NotFound();
            }

            return Ok(students);
        }

 //--------------------------------------- Get - To Extract Student Details Using Id ---------------------------------------------------//

        // api/stu?id=<StudentId>
        [HttpGet]
        public IHttpActionResult GetStudentById(int id)
        {
            Models.StudentViewModel student = null;

            using (var ctx = new SchoolDBEntities())
            {
                student = ctx.Students.Include("StudentAddress")
                    .Where(s => s.StudentId == id)
                    .Select(s => new Models.StudentViewModel()
                    {
                        Id = s.StudentId,
                        FirstNameStu = s.FirstName,
                        LastNameStu = s.LastName
                    }).FirstOrDefault<Models.StudentViewModel>();
            }

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

 //--------------------------------------- Get - To Extract Student Details Using Name ---------------------------------------------------//

        // api/stu?name=<StudentName>
        [HttpGet]
        public IHttpActionResult GetAllStudents(string name)
        {
            IList<Models.StudentViewModel> students = null;

            using (var ctx = new SchoolDBEntities())
            {
                students = ctx.Students.Include("StudentAddress")
                    .Where(s => s.FirstName.ToLower() == name.ToLower())
                    .Select(s => new Models.StudentViewModel()
                    {
                        Id = s.StudentId,
                        FirstNameStu = s.FirstName,
                        LastNameStu = s.LastName,
                        Address = s.StudentAddress == null ? null : new Models.AddressViewModel()
                        {
                            StudentId = s.StudentAddress.StudentId,
                            Address1 = s.StudentAddress.Address1,
                            Address2 = s.StudentAddress.Address2,
                            City = s.StudentAddress.City,
                            State = s.StudentAddress.State
                        }
                    }).ToList<Models.StudentViewModel>();
            }

            if (students.Count == 0)
            {
                return NotFound();
            }

            return Ok(students);
        }

 //--------------------------------------- Get - To Extract Student Details Using Student Id ---------------------------------------------//

        // api/stu?standardId=<StandardId>
        [HttpGet]
        public IHttpActionResult GetAllStudentsInSameStandard(int standardId)
        {
            IList<Models.StudentViewModel> students = null;

            using (var ctx = new SchoolDBEntities())
            {
                students = ctx.Students.Include("StudentAddress").Include("Standard").Where(s => s.StandardId == standardId)
                            .Select(s => new Models.StudentViewModel()
                            {
                                Id = s.StudentId,
                                FirstNameStu = s.FirstName,
                                LastNameStu = s.LastName,
                                Address = s.StudentAddress == null ? null : new Models.AddressViewModel()
                                {
                                    StudentId = s.StudentAddress.StudentId,
                                    Address1 = s.StudentAddress.Address1,
                                    Address2 = s.StudentAddress.Address2,
                                    City = s.StudentAddress.City,
                                    State = s.StudentAddress.State
                                },
                                Standard = new Models.StandardViewModel()
                                {
                                    StandardId = s.Standard.StandardId,
                                    Name = s.Standard.StandardName
                                }
                            }).ToList<Models.StudentViewModel>();
            }

            if (students.Count == 0)
            {
                return NotFound();
            }

            return Ok(students);
        }

 //------------------------------------------ Post - To Add New Student Details ------------------------------------------------------//

        // api/stu
        [HttpPost]
        public IHttpActionResult PostNewStudent(Models.StudentViewModel student)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            using (var ctx = new SchoolDBEntities())
            {
                ctx.Students.Add(new Student()
                {
                    StudentId = student.Id,
                    FirstName = student.FirstNameStu,
                    LastName = student.LastNameStu
                });

                ctx.SaveChanges();
            }

            return Ok();
        }

        //------------------------------------------ Put - To Edit Student Details ----------------------------------------------------------//

        // api/stu
        [HttpPut]
        public IHttpActionResult Put(Models.StudentViewModel student)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            using (var ctx = new SchoolDBEntities())
            {
                var existingStudent = ctx.Students.Where(s => s.StudentId == student.Id)
                                                        .FirstOrDefault<Student>();

                if (existingStudent != null)
                {
                    existingStudent.FirstName = student.FirstNameStu;
                    existingStudent.LastName = student.LastNameStu;

                    ctx.SaveChanges();
                }
                else
                {
                    return NotFound();
                }
            }

            return Ok();
        }

 //------------------------------------------ Delete - To Delete a Student Entry --------------------------------------------------------//

        // api/stu
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Not a valid student id");

            using (var ctx = new SchoolDBEntities())
            {
                var student = ctx.Students
                    .Where(s => s.StudentId == id)
                    .FirstOrDefault();

                ctx.Entry(student).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
            }

            return Ok();
        }
    }
}
