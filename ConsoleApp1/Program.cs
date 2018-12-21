using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Application
{
    public class Employee
    {
        public string name { get; set; }
        public string department { get; set; }
        public string[] toppings { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Employee> employees = new List<Employee>();

            using (StreamReader r = new StreamReader("Data.json"))
            {
                string json = r.ReadToEnd();
                employees = JsonConvert.DeserializeObject<List<Employee>>(json);
            }

            // 1. Which department has the largest number of employees who like Pineapple on their pizzas?
            string DepartmentThatMostLikesPineapple = employees.Where(e => e.toppings.Contains("Pineapple"))
                                    .GroupBy(emp => emp.department)
                                    .OrderByDescending(gp => gp.Count())
                                    .Take(1)
                                    .Select(g => g.Key).ToList()[0];

            Console.WriteLine("Which department has the largest number of employees who like Pineapple on their pizzas?");
            Console.WriteLine(DepartmentThatMostLikesPineapple+"\n");

            // 2. How many pizzas would you need to order to feed the Engineering department, assuming a pizza feeds 4 people? Ignore personal preferences.
            int NumberofPizzasToFeedEngineeringDept = ((employees.Where(emp => emp.department.Contains("Engineering")).ToList().Count / 4) + 1);

            // Note, the + 1 is to feed the remaining employeees that cannot form a group of 4
            Console.WriteLine("How many pizzas would you need to order to feed the Engineering department, assuming a pizza feeds 4 people? Ignore personal preferences.");
            Console.WriteLine(NumberofPizzasToFeedEngineeringDept+"\n");

            // 3. Which pizza topping combination is the most popular in each department and how many employees prefer it?
            var departments = employees.Select(emp => emp).GroupBy(emp => emp.department);

            Console.WriteLine("Which pizza topping combination is the most popular in each department and how many employees prefer it?");

            foreach (var department in departments)
            {
                var employeesindept = employees.Where(emp => emp.department.Contains(department.Key)).ToList();

                List<string[]> toppingsList = new List<string[]>();
                foreach (var employee in employeesindept)
                {
                    toppingsList.Add(employee.toppings.OrderBy(s => s).ToArray());
                }

                var ingredients = toppingsList.SelectMany(a => a).Distinct();
                var pairs =
                    from p1 in ingredients
                    from p2 in ingredients
                    where p1.CompareTo(p2) < 0
                    select new { p1, p2 };
                var PizzaToppingCombination =
                    (from topping in toppingsList
                     from p in pairs
                     where topping.Contains(p.p1) && topping.Contains(p.p2)
                     group topping by p into g
                     orderby g.Count() descending
                     select g.Key)
                    .First();
                var NumberofEmployeesPreferIt =
                    (from topping in toppingsList
                     from p in pairs
                     where topping.Contains(p.p1) && topping.Contains(p.p2)
                     group topping by p into g
                     orderby g.Count() descending
                     select g.Count())
                     .First();

                Console.WriteLine("Department: " + department.Key);
                Console.WriteLine(PizzaToppingCombination.p1+", "+ PizzaToppingCombination.p2);
                Console.WriteLine(NumberofEmployeesPreferIt+"\n");

            }

            Console.ReadLine();
        }
    }
}