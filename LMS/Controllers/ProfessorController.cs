﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var query =
                from cl in db.Classes
                join e in db.Enrolleds
                    on cl.ClassId equals e.ClassId
                    into temp1
                
                from t1 in temp1
                join co in db.Courses
                    on cl.CourseId equals co.CourseId
                    into temp2
                
                from t2 in temp2
                join s in db.Students
                    on t1.Student equals s.UId
                    into enrollment
                
                from i in enrollment
                where cl.SemesterSeason == season && cl.SemesterYear == year && t2.Subject == subject && t2.Number == num
                select new
                {
                    fname = i.FirstName,
                    lname = i.LastName,
                    uid  = i.UId,
                    dob  = i.Dob,
                    grade = t1.Grade == null ? "--" : (string?) t1.Grade
                };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            if (category == null)
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
                        aname =  a.Name,
                        cname = t2.Name,
                        due = a.DueDate,
                        submissions = db.Submissions.Count(u => u.AssignmentId == a.AssignmentId)
                    };
                return Json(query.ToArray());
            }
            else
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
                    where t1.Subject == subject && t1.Number == num && cl.SemesterSeason == season && cl.SemesterYear == year && t2.Name == category
                    select new
                    {
                        aname =  a.Name,
                        cname = t2.Name,
                        due = a.DueDate,
                        submissions = db.Submissions.Count(u => u.AssignmentId == a.AssignmentId)
                    };
                return Json(query.ToArray());
            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query = from cl in db.Classes
                join co in db.Courses
                    on cl.CourseId equals co.CourseId
                    into temp

                from t in temp
                join ac in db.AssignmentCategories
                    on cl.ClassId equals ac.ClassId
                where t.Subject == subject && t.Number == num && cl.SemesterSeason == season && cl.SemesterYear == year
                select new
                {
                    name =  ac.Name,
                    weight = ac.Weight
                };
            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            try
            {
                var query = from cl in db.Classes
                    join co in db.Courses
                        on cl.CourseId equals co.CourseId
                    where co.Subject == subject && co.Number == num &&  cl.SemesterSeason == season && cl.SemesterYear == year
                    select cl.ClassId;
                
                if (query.Any())
                {
                    var ac = new AssignmentCategory { Weight = (byte)catweight, Name = category, ClassId = query.First() };
                    db.AssignmentCategories.Add(ac);
                    db.SaveChanges();
                }
                
                return Json(new { success = true });
            }
            catch 
            {
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            try
            {
                var query = from cl in db.Classes
                    join co in db.Courses
                        on cl.CourseId equals co.CourseId
                        into temp
                    
                    from t in temp
                    join ac in db.AssignmentCategories
                        on  cl.ClassId equals ac.ClassId
                    where t.Subject == subject && t.Number == num &&  cl.SemesterSeason == season && cl.SemesterYear == year && ac.Name == category
                    select ac.Acid;
                
                
                if (query.Any())
                {
                    var assignment = new Assignment { Name = asgname, MaxPoints = (uint) asgpoints, Contents = asgcontents, DueDate = asgdue, Acid = query.First() };
                    db.Assignments.Add(assignment);
                    db.SaveChanges();
                }
            }
            catch 
            {
                return Json(new { success = false });
            }
            
            //update all student's grades
            var q2 =
                from cl in db.Classes
                join e in db.Enrolleds
                    on cl.ClassId equals e.ClassId
                    into temp1

                from t1 in temp1
                join co in db.Courses
                    on cl.CourseId equals co.CourseId
                where cl.SemesterSeason == season && cl.SemesterYear == year && co.Subject == subject &&
                      co.Number == num
                select t1;
            
            foreach (Enrolled e in q2.ToArray())
            {
                UpdateGrade(subject, num, season, year, e.Student);
            }
            return Json(new { success = true });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
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
                    into temp3
                
                from t3 in temp3
                join s in db.Submissions
                    on t3.AssignmentId equals s.AssignmentId
                    into temp4 
                
                from t4 in temp4
                join st in db.Students
                    on t4.Student equals st.UId
                where t1.Subject == subject && t1.Number == num && cl.SemesterSeason == season && cl.SemesterYear == year && t2.Name == category && t3.Name == asgname
                select new
                {
                    fname = st.FirstName,
                    lname = st.LastName,
                    uid =  st.UId,
                    time = t4.SubmissionTime,
                    score = t4.Score,
                };
            return Json(query.ToArray());
            return Json(null);
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
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
                    into temp3
                
                from t3 in temp3
                join s in db.Submissions
                    on t3.AssignmentId equals s.AssignmentId
                    into temp4 
                
                from t4 in temp4
                join st in db.Students
                    on t4.Student equals st.UId
                where t1.Subject == subject && t1.Number == num && cl.SemesterSeason == season && cl.SemesterYear == year && t2.Name == category && t3.Name == asgname && st.UId == uid
                select t4;
            
            if (query.Any())
            {
                try
                {
                    var sub = query.First();
                    sub.Score = (uint) score;
                    db.SaveChanges();
                    UpdateGrade(subject, num, season, year, uid);
                    return Json(new { success = true });
                }
                catch 
                {
                    return Json(new { success = false });
                }
            }

            return Json(new { success = false });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query = from cl in db.Classes
                join co in db.Courses
                    on cl.CourseId equals co.CourseId
                where cl.Professor == uid
                select new
                {
                    subject = co.Subject,
                    number = co.Number,
                    name = co.Name,
                    season = cl.SemesterSeason,
                    year = cl.SemesterYear
                };
            return Json(query.ToArray());
        }


        private bool UpdateGrade(string subject, int num, string season, int year, string uid)
        {
            var query = from cl in db.Classes
                join co in db.Courses
                    on cl.CourseId equals co.CourseId
                    into temp1

                from t1 in temp1
                join ac in db.AssignmentCategories
                    on cl.ClassId equals ac.ClassId
                where t1.Subject == subject && t1.Number == num && cl.SemesterSeason == season && cl.SemesterYear == year
                select new
                {
                    classid =  cl.ClassId,
                    cat_weight = ac.Weight,
                    assignments = from a in ac.Assignments
                        select new
                        {
                            max = a.MaxPoints,
                            submission = from sub in a.Submissions
                                where sub.Student == uid && a.AssignmentId == sub.AssignmentId
                                select sub.Score
                        }
                };
            
            float cat_score = 0;
            float total_cat_weight = 0;
            foreach (var assignment_category in query.ToArray())
            {
                float cat_total = 0;
                float cat_max = 0;
                foreach (var assignment in assignment_category.assignments.ToArray())
                {
                    cat_max += (float)assignment.max;
                    if (assignment.submission.Any())
                    {
                        cat_total += (float)assignment.submission.First();
                        Console.WriteLine((float)assignment.submission.First() + " " + (float)assignment.max);
                    }
                    else
                    {
                        Console.WriteLine( (float)assignment.max);
                    }
                }

                cat_score += (cat_total / cat_max) * assignment_category.cat_weight;
                total_cat_weight += assignment_category.cat_weight;
                Console.WriteLine(cat_score + " " + total_cat_weight);
            }

            var scaling_factor = 100 / total_cat_weight;
            float score = scaling_factor * cat_score;
            Console.WriteLine(scaling_factor + " " + score);
            string grade;

            if (score >= 93)
                grade = "A";
            else if (score >= 90)
                grade = "A-";
            else if (score >= 87)
                grade = "B+";
            else if (score >= 83)
                grade = "B";
            else if (score >= 80)
                grade = "B-";
            else if (score >= 77)
                grade = "C+";
            else if (score >= 73)
                grade = "C";
            else if (score >= 70)
                grade = "C-";
            else if (score >= 67)
                grade = "D+";
            else if (score >= 63)
                grade = "D";
            else if (score >= 60)
                grade = "D-";
            else 
                grade = "E";


            var q2 = from e in db.Enrolleds
                where e.Student == uid && query.First().classid == e.ClassId
                select e;
            
            if (q2.Any())
            {
                try
                {
                    var enrolled = q2.First();
                    enrolled.Grade = grade;
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /*******End code to modify********/
    }
}

