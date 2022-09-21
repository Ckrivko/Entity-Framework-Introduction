

namespace SoftUni
{
    using SoftUni.Data;
    using SoftUni.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            using var context = new SoftUniContext();

            string result = RemoveTown(context);
            Console.WriteLine(result);

        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();


            var employees = context.Employees.OrderBy(e => e.EmployeeId).ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} " +
                    $"{e.Salary.ToString("F2")}");

            }
            return sb.ToString().Trim();

        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();


            var employees = context.Employees
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary.ToString("F2")}");


            }
            return sb.ToString().Trim();

        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToArray();

            foreach (var e in employees)
            {

                sb.AppendLine($"{e.FirstName} {e.LastName} " +
                    $"from {e.DepartmentName} - ${e.Salary.ToString("F2")}");

            }
            return sb.ToString().Trim();

        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(newAddress);

            var nakovEmployee = context.Employees
                .FirstOrDefault(e => e.LastName == "Nakov");

            nakovEmployee.Address = newAddress;

            context.SaveChangesAsync();

            var employeesText = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => new
                {
                    e.Address.AddressText

                })
                .ToArray();

            foreach (var empl in employeesText)
            {
                sb.AppendLine(empl.AddressText);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.EmployeesProjects.Any(
                   ep => ep.Project.StartDate >= new DateTime(2001, 01, 01)
                    && ep.Project.StartDate <= new DateTime(2003, 12, 31)))
                    .Take(10)
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        ManagerFirstName = e.Manager.FirstName,
                        ManagerLastName = e.Manager.LastName,
                        AllProjects = e.EmployeesProjects.
                        Select(ep => new
                        {
                            ProjectName = ep.Project.Name,
                            StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                            EndDate = ep.Project.EndDate.HasValue ?
                            ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt")
                            : "not finished"

                        })
                        //.ToArray()

                    })
                    .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");


                foreach (var p in e.AllProjects)
                {
                    sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");


                }
            }

            return sb.ToString().Trim();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var adresses = context.Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town.Name)
                .ThenBy(a => a.AddressText).Take(10)
                .Select(a => new
                {
                    a.AddressText,
                    TownName = a.Town.Name,
                    EmployeeCount = a.Employees.Count
                });

            foreach (var a in adresses)
            {

                sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeeCount} employees");

            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employee = context.Employees.FirstOrDefault(e => e.EmployeeId == 147);
            var projects = context.EmployeesProjects.Where(p => p.EmployeeId == 147)
                .Select(pr => new
                {
                    ProjectName = pr.Project.Name
                })
                .OrderBy(p => p.ProjectName)
                .ToArray();


            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var p in projects)
            {
                sb.AppendLine($"{p.ProjectName}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    Employees = d.Employees.
                    OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToArray()
                })
                .ToArray();

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.Name} - {d.ManagerFirstName} {d.ManagerLastName}");

                foreach (var e in d.Employees)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");

                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var projects = context.Projects
               .OrderByDescending(p => p.StartDate)
               .Take(10)
               .Select(p => new
               {
                   p.Name,
                   p.Description,
                   p.StartDate
               })
               .OrderBy(p => p.Name)
               .ToArray();

            foreach (var p in projects)
            {

                sb.AppendLine($"{p.Name}");
                sb.AppendLine($"{p.Description}");
                sb.AppendLine($"{p.StartDate.ToString("M/d/yyyy h:mm:ss tt")}");

                //ToString("M/d/yyyy h:mm:ss tt")

            }

            return sb.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering"
                || e.Department.Name == "Tool Design"
                || e.Department.Name == "Marketing"
                || e.Department.Name == "Information Services")
           .OrderBy(e => e.FirstName)
           .ThenBy(e => e.LastName)
           .ToArray();

            foreach (var e in employees)
            {
                e.Salary *= (Decimal)(1.12);

                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary.ToString("F2")})");

            }
            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.FirstName.ToLower().StartsWith("sa"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary.ToString("F2")})");
            }
            return sb.ToString().TrimEnd();

        }

        public static string DeleteProjectById(SoftUniContext context)
        {

            StringBuilder sb = new StringBuilder();

            var project = context.Projects.First(p => p.ProjectId == 2);

            if (project != null)
            {
                var employees = project.EmployeesProjects.ToArray();

                context.EmployeesProjects.RemoveRange(employees);
                context.Projects.Remove(project);
                context.SaveChanges();

            }

            var projects = context.Projects.ToArray();
            foreach (var p in projects)
            {
                sb.AppendLine(p.Name);
            }
            return sb.ToString().TrimEnd();
        }

        public static string RemoveTown(SoftUniContext context)
        {
           
            var town = context.Towns.First(t => t.Name == "Seattle");
            var addresses = context.Addresses.Where(a => a.Town.Name == "Seattle").ToArray();

            var count = addresses.Length;
            foreach (var a in addresses)
            {
                var employees = context.Employees.Where(e => e.AddressId == a.AddressId).ToArray();

                foreach (var e in employees)
                {
                    e.AddressId = null;
                }
            }
            context.Addresses.RemoveRange(addresses);
            context.Towns.Remove(town);
            context.SaveChanges();


            return $"{count} addresses in Seattle were deleted";

        }

    }
}
