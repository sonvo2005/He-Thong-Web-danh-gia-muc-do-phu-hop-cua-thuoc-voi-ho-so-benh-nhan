using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebDanhGiaHs.Models;

namespace WebDanhGiaHs.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Mặc định vào Form BM01
        return View();
    }

    public IActionResult MucLucThuoc()
    {
        // Điều hướng sang Form BM02
        return View();
    }

    public IActionResult KetQuaDanhGia()
    {
        // Điều hướng sang Form BM03
        return View();
    }
}
