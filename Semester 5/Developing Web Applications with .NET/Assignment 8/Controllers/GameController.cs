using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace lista08.Controllers;

public class GameController : Controller {
    private const string RangeKey = "Range";
    private const string RandomValueKey = "RandomValue";
    private const string GuessCountKey = "GuessCount";
    private const string GuessesKey = "Guesses";

    private void EnsureSessionInitialized()
    {
        if (HttpContext.Session.GetInt32(RangeKey) == null)
        {
            HttpContext.Session.SetInt32(RangeKey, 30);
            DrawNewNumber();
        }
    }

    private void DrawNewNumber() {
        var range = HttpContext.Session.GetInt32(RangeKey) ?? 30;
        var generator = new Random();
        HttpContext.Session.SetInt32(RandomValueKey, generator.Next(0, range));
        HttpContext.Session.SetInt32(GuessCountKey, 0);
        HttpContext.Session.SetString(GuessesKey, JsonSerializer.Serialize(new List<int>()));
    }

    [Route("Set,{newRange}")]
    public IActionResult Set(int newRange) {
        HttpContext.Session.SetInt32(RangeKey, newRange);
        DrawNewNumber();
        return GameInfoPage($"Zakres został ustawiony na [0, {newRange - 1}]. Nowa liczba została wylosowana.");
    }

    [Route("Draw")]
    public IActionResult Draw() {
        EnsureSessionInitialized();
        DrawNewNumber();
        return GameInfoPage($"Nowa liczba z zakresu [0, {HttpContext.Session.GetInt32(RangeKey) - 1}] została wylosowana. Powodzenia!");
    }

    public IActionResult GameInfoPage(string infoMessage) {
        ViewData["info"] = infoMessage;
        return View("GameInfoPage");
    }

    [Route("Guess,{userGuess}")]
    public IActionResult Guess(int userGuess) {
        EnsureSessionInitialized();

        var range = HttpContext.Session.GetInt32(RangeKey) ?? 30;
        var randomValue = HttpContext.Session.GetInt32(RandomValueKey) ?? 0;
        var guessCount = HttpContext.Session.GetInt32(GuessCountKey) ?? 0;
        var guesses = JsonSerializer.Deserialize<List<int>>(HttpContext.Session.GetString(GuessesKey) ?? "[]") ?? [];

        guessCount++;
        guesses.Add(userGuess);

        HttpContext.Session.SetInt32(GuessCountKey, guessCount);
        HttpContext.Session.SetString(GuessesKey, JsonSerializer.Serialize(guesses));

        string message;
        string cssClass;

        ViewData["GuessCount"] = guessCount;
        ViewData["RandValue"] = randomValue;
        ViewData["Range"] = range;
        ViewData["GuessesHistory"] = String.Join(", ", guesses);


        if (userGuess < randomValue) {
            message = $"Za mało.";
            cssClass = "too-low";
        }
        else if (userGuess > randomValue) {
            message = $"Za dużo.";
            cssClass = "too-high";
        }
        else {
            message = $"Gratulacje! Udało ci się zgadnąć liczbę {randomValue}.";
            cssClass = "correct";
            DrawNewNumber();
        }

        ViewData["Message"] = message;
        ViewData["CssClass"] = cssClass;

        return View();
    }
}