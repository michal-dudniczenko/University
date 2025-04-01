using Microsoft.AspNetCore.Mvc;

namespace lista08.Controllers;

public class ToolController : Controller {

    [Route("Tool/Solve/{a=2}/{b=3}/{c=4}")]
    public IActionResult Solve(double a, double b, double c) {
        string result;
        string cssClass;

        double[]? wynik = CalculateRoot(a, b, c);

        if (wynik == null) {
            result = "Nieskończenie wiele rozwiązań.";
            cssClass = "tozsamosc";
        } else if (wynik.Length == 0) {
            result = "Brak rozwiązań.";
            cssClass = "brak-rozwiazan";
        } else if (wynik.Length == 1) {
            result = $"Równanie liniowe, rozwiązanie: x = {wynik[0]:F2}";
            cssClass = "jedno-rozwiazanie";       
        } else {
            double x1 = wynik[0];
            double x2 = wynik[1];
            result = $"Dwa rozwiązania: x<sub>1</sub> = {x1:F2}, x<sub>2</sub> = {x2:F2}";
            cssClass = "dwa-rozwiazania";
        }

        ViewData["Result"] = result;
        ViewData["CssClass"] = cssClass;

        ViewData["a"] = a;
        ViewData["b"] = b;
        ViewData["c"] = c;

        return View(); 
    }

    static double[]? CalculateRoot(double a, double b, double c) {
        if (a == 0) {
            if (b == 0) {
                if (c == 0) {
                    return null;
                } else {
                    return [];
                }
            } else {
                return [(-1 * c) / b];
            }
        } else {
            double delta = b * b - 4 * a * c;
            if (delta < 0) {
                return [];
            } else if (delta == 0) {
                return [(-1 * b) / (2 * a)];
            } else {
                return [(-1 * b) / (2 * a) - Math.Sqrt(delta), (-1 * b) / (2 * a) + Math.Sqrt(delta)];
            }
        }
    } 
}