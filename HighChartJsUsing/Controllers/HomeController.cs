using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HighChartJsUsing.Models;
using OfficeOpenXml;

namespace HighChartJsUsing.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string message = "Добро пожаловать")
        {
            ViewBag.Message = message;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private List<StudentExcelDiagram> GetNumberOfStudentsByExcel(string fileName)
        {
            List<StudentExcelDiagram> studentExcelDiagrams = new List<StudentExcelDiagram>();
            FileInfo f = new FileInfo(Server.MapPath("~/Files/" + fileName));
            using (ExcelPackage excelPackage = new ExcelPackage(f))
            {
                ExcelWorkbook excelWorkbook = excelPackage.Workbook;
                ExcelWorksheet excelWorkbookWorksheet = excelWorkbook.Worksheets[1];
                int startColumn = excelWorkbookWorksheet.Dimension.Start.Column,
                    startRow = excelWorkbookWorksheet.Dimension.Start.Row,
                    endColumn = excelWorkbookWorksheet.Dimension.End.Column, endRow = excelWorkbookWorksheet.Dimension.End.Row;
                ExcelRange excelRange =
                    excelWorkbookWorksheet.Cells[startRow + 1, startColumn + 1, endRow, endColumn];

                for (int i = startRow + 1; i < endRow; i++)
                {
                    StudentExcelDiagram diagram = new StudentExcelDiagram();
                    diagram.LevelOfEducationRus = excelRange[i, 1].Value.ToString();
                    diagram.LevelOfEducationKaz = excelRange[i, 2].Value.ToString();
                    diagram.LevelOfEducationEn = excelRange[i, 3].Value.ToString();
                    diagram.FacultyRus = excelRange[i, 4].Value.ToString();
                    diagram.FacultyKaz = excelRange[i, 5].Value.ToString();
                    diagram.FacultyEn = excelRange[i, 6].Value.ToString();
                    diagram.CountOfStudents = int.Parse(excelRange[i, 7].Value.ToString());
                    studentExcelDiagrams.Add(diagram);

                }
            }

            return studentExcelDiagrams;
        }
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase excelFile)
        {
            List<StudentExcelDiagram> studentExcelDiagrams = new List<StudentExcelDiagram>();
            try
            {
                string fileName = Path.GetFileName(excelFile.FileName);
                if (!System.IO.File.Exists(Server.MapPath("~/Files/" + fileName)))
                {
                    excelFile.SaveAs(Server.MapPath("~/Files/" + fileName));
                }
                studentExcelDiagrams = GetNumberOfStudentsByExcel(fileName);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home", new { message = e.Message });
            }
            return View(studentExcelDiagrams);
        }
        [HttpPost]
        public ActionResult ShowDiagram()
        {
            List<StudentExcelDiagram> studentExcelDiagrams;
            try
            {
                studentExcelDiagrams = GetNumberOfStudentsByExcel("задание в казну.xlsx");
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home", new { message = e.ToString() });
            }
            return View("FileUpload", studentExcelDiagrams);
        }
    }
}