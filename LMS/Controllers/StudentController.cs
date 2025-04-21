using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private LMSContext db;
        public StudentController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {           
            var query = from cl in db.Classes
                join co in db.Courses
                    on cl.CourseId equals co.CourseId
                    into temp1
                
                from t in temp1
                join e in db.Enrolleds
                    on cl.ClassId equals e.ClassId
                where e.Student == uid
                select new
                {
                    subject = t.Subject,
                    number = t.Number,
                    name = t.Name,
                    season = cl.SemesterSeason,
                    year = cl.SemesterYear,
                    grade = e.Grade == null ? "--" : (string?) e.Grade,
                };
            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {            
            var query = from cl in db.Classes
                join co in db.Courses
                    on cl.CourseId equals co.CourseId
                    into temp1

                from t1 in temp1
                join ac in db.AssignmentCategories
                    on cl.ClassId equals ac.ClassId
                    into temp2
                
                from t2 in temp2
                join a in db.Assignments
                    on t2.Acid equals a.Acid 
                where t1.Subject == subject && t1.Number == num && cl.SemesterSeason == season && cl.SemesterYear == year 
                select new
                {
                    aname = a.Name,
                    cname =  t2.Name,
                    due = a.DueDate,
                    score = from s in a.Submissions
                        where s.Student == uid && s.AssignmentId == a.AssignmentId
                            select s.Score
                };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            try
            {
                var q1 = from cl in db.Classes
                    join co in db.Courses
                        on cl.CourseId equals co.CourseId
                        into temp1

                    from t1 in temp1
                    join ac in db.AssignmentCategories
                        on cl.ClassId equals ac.ClassId
                        into temp2

                    from t2 in temp2
                    join a in db.Assignments
                        on t2.Acid equals a.Acid
                        into temp3

                    from t3 in temp3
                    join s in db.Submissions
                        on t3.AssignmentId equals s.AssignmentId
                        into temp4

                    from t4 in temp4
                    join st in db.Students
                        on t4.Student equals st.UId
                    where t1.Subject == subject && t1.Number == num && cl.SemesterSeason == season &&
                          cl.SemesterYear == year && t2.Name == category && t3.Name == asgname && st.UId == uid
                    select t4;

                if (q1.Any())
                {
                    Submission sub = q1.First();
                    sub.Contents = contents;
                    sub.SubmissionTime = DateTime.Now;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                
                var q2 = from cl in db.Classes
                    join co in db.Courses
                        on cl.CourseId equals co.CourseId
                        into temp1

                    from t1 in temp1
                    join ac in db.AssignmentCategories
                        on cl.ClassId equals ac.ClassId
                        into temp2

                    from t2 in temp2
                    join a in db.Assignments
                        on t2.Acid equals a.Acid
                    where t1.Subject == subject && t1.Number == num && cl.SemesterSeason == season &&
                          cl.SemesterYear == year && t2.Name == category && a.Name == asgname 
                    select a.AssignmentId;

                if (q2.Any())
                {
                    var sub = new Submission { AssignmentId = q2.First(), Student = uid , SubmissionTime = DateTime.Now , Contents = contents, Score = 0 };
                    db.Submissions.Add(sub);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            
                return Json(new { success = false });
            }
            catch
            {
                return Json(new { success = false });
            }
            
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            try
            {
                var query = from cl in db.Classes
                    join co in db.Courses
                        on cl.CourseId equals co.CourseId
                    where co.Subject == subject && co.Number == num && cl.SemesterSeason == season &&
                          cl.SemesterYear == year
                    select cl.ClassId;

                if (query.Any())
                {
                    var enrolled = new Enrolled { ClassId = query.First(), Student = uid, Grade = null };
                    db.Enrolleds.Add(enrolled);
                    db.SaveChanges();
                    return Json(new { success = true});
                }

                return Json(new { success = false});
            }
            catch
            {
                return Json(new { success = false});
            }
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {            
            var query = from e in db.Enrolleds
                where e.Student == uid
                    select e;
            
            if (!query.Any())
                return Json(new { gpa = 0.0 });

            double credit_hours = 0;
            double grade_points = 0;
            foreach (Enrolled e in query.ToArray())
            {
                if (e.Grade == null)
                    continue;
                
                string grade = e.Grade.ToString();
                credit_hours += 4;
                if (grade == "A")
                    grade_points += 4;
                if (grade == "A-")
                    grade_points += 3.7;
                if (grade == "B+")
                    grade_points += 3.3;
                if (grade == "B")
                    grade_points += 3;
                if (grade == "B-")
                    grade_points += 2.7;
                if (grade == "C+")
                    grade_points += 2.3;
                if (grade == "C")
                    grade_points += 2.0;
                if (grade == "C-")
                    grade_points += 1.7;
                if (grade == "D+")
                    grade_points += 1.3;
                if (grade == "D")
                    grade_points += 1.0;
                if (grade == "D-")
                    grade_points += 0.7;
            }

            var average = grade_points / credit_hours;
            return Json(new { gpa = average });
        }
                
        /*******End code to modify********/

    }
}

