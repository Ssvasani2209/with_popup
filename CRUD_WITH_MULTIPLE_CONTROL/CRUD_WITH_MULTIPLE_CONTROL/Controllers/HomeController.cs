﻿using CRUD_WITH_MULTIPLE_CONTROL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRUD_WITH_MULTIPLE_CONTROL.Controllers
{
    public class HomeController : Controller
    {
        DEV_DB_CONTEXT dbcontext = new DEV_DB_CONTEXT();
        public ActionResult ListOfEmployee()
        {
            EmployeeViewModel empvm = new EmployeeViewModel();
            BindDDL();
            var list = from emp in dbcontext.Employees
                       join state in dbcontext.States on emp.StateId equals state.StateId
                       join city in dbcontext.Cities on emp.CityId equals city.CityId
                       join gender in dbcontext.Genders on emp.GenderId equals gender.GenderId
                       where (emp.IsDeleted == null)
                       select new EmployeeViewModel
                       {
                           EmpId = emp.EmpId,
                           FirstName = emp.FirstName,
                           LastName = emp.LastName,
                           MiddleName = emp.MiddleName,
                           StateName = state.StateName,
                           CityName = city.CityName,
                           DOB = emp.DOB,
                           Address = emp.Address,
                           Gender = gender.Gender1
                       };
            //ViewBag.ListOfEmployee = list;

            List<EmployeeViewModel> listofemp = new List<EmployeeViewModel>();
            foreach (var item in list)
            {
                EmployeeViewModel vm = new EmployeeViewModel();
                vm.EmpId = item.EmpId;
                vm.FirstName = item.FirstName;
                vm.LastName = item.LastName;
                vm.MiddleName = item.MiddleName;
                vm.Address = item.Address;
                vm.DOB = item.DOB;
                vm.CityName = item.CityName;
                vm.Gender = item.Gender;
                vm.StateName = item.StateName;
                listofemp.Add(vm); empvm.ListOfEmployee = listofemp;
            }

            return View(empvm);


        }
        public ActionResult CreateNewEmployee(int EmployeeId = 0)
        {
            if (EmployeeId == 0)
            {
                EmployeeViewModel Evmodel = new EmployeeViewModel();
                Evmodel.AddOrEdit = "add";
                BindDDL();
                return View(Evmodel);
            }
            else
            {

                Employee employeeDataFromDB = dbcontext.Employees.Where(data => data.EmpId == EmployeeId).FirstOrDefault();
                ViewBag.StateList = new SelectList(dbcontext.States.ToList(), "StateId", "StateName");
                ViewBag.CityList = new SelectList(dbcontext.Cities.Where(x => x.StateId == employeeDataFromDB.StateId).ToList(), "CityId", "CityName");
                EmployeeViewModel Evm = new EmployeeViewModel();
                Evm.AddOrEdit = "edit";
                Evm.EmpId = employeeDataFromDB.EmpId;
                Evm.FirstName = employeeDataFromDB.FirstName;
                Evm.MiddleName = employeeDataFromDB.MiddleName;
                Evm.LastName = employeeDataFromDB.LastName;
                Evm.Address = employeeDataFromDB.Address;
                Evm.StateId = Convert.ToInt32(employeeDataFromDB.StateId);
                Evm.CityId = Convert.ToInt32(employeeDataFromDB.CityId);
                Evm.GenderId = employeeDataFromDB.GenderId;
                Evm.DOB = employeeDataFromDB.DOB.Value;
                Evm.fileName = employeeDataFromDB.FilePath;
                Session["filePath"] = "/UploadedFiles/" + employeeDataFromDB.FilePath;
                // return View(Evm);
                return Json(Evm, JsonRequestBehavior.AllowGet);
            }
        }

        private void BindDDL()
        {
            ViewBag.StateList = new SelectList(dbcontext.States.ToList(), "StateId", "StateName");
            ViewBag.CityList = new SelectList(dbcontext.Cities.ToList(), "CityId", "CityName");
        }
        [HttpPost]
        public ActionResult CreateNewEmployee(EmployeeViewModel emp, string[] HobbiesCB)
        {
            try
            {

                if (emp.EmpId == 0)
                {
                    Employee e = new Employee();
                    Save_Update_Emp_Details(emp, e);
                    dbcontext.Employees.Add(e);
                    dbcontext.SaveChanges();
                    Save_Emp_Hobby_EmpId_Wise(HobbiesCB, e);
                    return Content("<script language='javascript' type='text/javascript'>alert('Employee created successfully.');window.location.href='/Home/ListOfEmployee';</script>");
                }
                else
                {
                    Employee e = dbcontext.Employees.Where(x => x.EmpId == emp.EmpId).FirstOrDefault();
                    List<SelectedHobbyOfEmployeeWise> HobbyListEmpIdWise = dbcontext.SelectedHobbyOfEmployeeWises.Where(X => X.EmployeeId == e.EmpId).ToList();
                    if (HobbyListEmpIdWise.Count > 0)
                    {
                        foreach (var item in HobbyListEmpIdWise)
                        {
                            dbcontext.SelectedHobbyOfEmployeeWises.Remove(item);
                            dbcontext.SaveChanges();
                        }
                    }
                    Save_Update_Emp_Details(emp, e);
                    dbcontext.SaveChanges();
                    Save_Emp_Hobby_EmpId_Wise(HobbiesCB, e);
                    return Content("<script language='javascript' type='text/javascript'>alert('Employee updated successfully.');window.location.href='/Home/ListOfEmployee';</script>");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return RedirectToAction("ListOfEmployee");
        }

        private void UploadFiles(HttpPostedFileBase File)
        {
            if (File != null)
            {
                string extension = Path.GetExtension(File.FileName);
                string Filepath = File.FileName;
                string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), Filepath);
                File.SaveAs(_path);
            }
        }

        private void Save_Emp_Hobby_EmpId_Wise(string[] HobbiesCB, Employee e)
        {
            foreach (var item in HobbiesCB)
            {
                SelectedHobbyOfEmployeeWise HobbyEmpIdWise = new SelectedHobbyOfEmployeeWise();
                HobbyEmpIdWise.EmployeeId = e.EmpId;
                HobbyEmpIdWise.HobbyId = Convert.ToInt32(item);
                dbcontext.SelectedHobbyOfEmployeeWises.Add(HobbyEmpIdWise);
                dbcontext.SaveChanges();
            }
        }

        private void Save_Update_Emp_Details(EmployeeViewModel emp, Employee e)
        {
            e.FirstName = emp.FirstName;
            e.MiddleName = emp.MiddleName;
            e.LastName = emp.LastName;
            e.Address = emp.Address;
            if (emp.Gender == "Male")
            {
                e.GenderId = 1;
            }
            else
            {
                e.GenderId = 2;
            }
            e.StateId = emp.StateId;
            e.CityId = emp.CityId;
            e.DOB = emp.DOB;
            UploadFiles(emp.fileUpload);
            if (emp.fileUpload != null)
            {
                e.FilePath = emp.fileUpload.FileName;
            }
        }
        public JsonResult BindCheckboxOfHobbies()
        {
            dbcontext.Configuration.ProxyCreationEnabled = false;
            List<Hobby> hb = dbcontext.Hobbies.ToList();
            return Json(hb, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillCityList(int StateId)
        {
            List<City> ct = new List<City>();
            Dictionary<int, string> CityList = new Dictionary<int, string>();
            if (StateId > 0)
            {
                ct = dbcontext.Cities.Where(datas => datas.StateId == StateId).ToList();
            }
            var result = (from r in ct
                          select new
                          {
                              id = r.CityId,
                              name = r.CityName
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetSelectedChechboxIdbyEmpId(int EmployeeId)
        {
            dbcontext.Configuration.ProxyCreationEnabled = false;
            List<SelectedHobbyOfEmployeeWise> selectedHobbybyEmpId = dbcontext.SelectedHobbyOfEmployeeWises.Where(x => x.EmployeeId == EmployeeId).ToList();
            return Json(selectedHobbybyEmpId, JsonRequestBehavior.AllowGet);

        }
        public JsonResult DeleteEmployeeIdWiseData(int? EmpId)
        {
            bool Result = false;
            dbcontext.Configuration.ProxyCreationEnabled = false;
            if (EmpId > 0)
            {
                List<SelectedHobbyOfEmployeeWise> dltHobbiesData = dbcontext.SelectedHobbyOfEmployeeWises.Where(x => x.EmployeeId == EmpId).ToList();
                if (dltHobbiesData.Count > 0)
                {
                    foreach (var item in dltHobbiesData)
                    {
                        dbcontext.SelectedHobbyOfEmployeeWises.Remove(item);
                        dbcontext.SaveChanges();
                    }
                    Employee existOrNot = dbcontext.Employees.Where(x => x.EmpId == EmpId).FirstOrDefault();
                    if (existOrNot != null)
                    {
                        existOrNot.IsDeleted = true;
                        dbcontext.SaveChanges();
                       
                    } 
                    Result = true;
                }
                

            }
            return Json(Result, JsonRequestBehavior.AllowGet);

        }
    }
}