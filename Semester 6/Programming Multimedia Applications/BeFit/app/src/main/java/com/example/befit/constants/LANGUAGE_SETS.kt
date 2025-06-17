package com.example.befit.constants

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue

object Languages {
    const val ENGLISH = "English"
    const val POLISH = "Polish"
    const val SPANISH = "Spanish"
    const val GERMAN = "German"
    const val DEFAULT = ENGLISH

    val values = listOf(
        ENGLISH, POLISH, SPANISH, GERMAN
    )
}

var appLanguage by mutableStateOf(Languages.ENGLISH)

object Strings {
    private val currentSet: LanguageSet
        get() = LanguageSets.allLanguages[appLanguage] ?: LanguageSets.allLanguages[Languages.DEFAULT]!!

    val NOTHING_HERE_YET get() = currentSet.NOTHING_HERE_YET
    val DELETE get() = currentSet.DELETE
    val NAME get() = currentSet.NAME

    val YOUR_PROGRAMS get() = currentSet.YOUR_PROGRAMS
    val ADD_PROGRAM get() = currentSet.ADD_PROGRAM
    val PROGRAM_NAME get() = currentSet.PROGRAM_NAME
    val EDIT_PROGRAM get() = currentSet.EDIT_PROGRAM
    val ADD_TRAINING_DAY get() = currentSet.ADD_TRAINING_DAY
    val EDIT_TRAINING_DAY get() = currentSet.EDIT_TRAINING_DAY
    val ADD_EXERCISE get() = currentSet.ADD_EXERCISE
    val SELECT_EXERCISE get() = currentSet.SELECT_EXERCISE
    val REST_TIME get() = currentSet.REST_TIME
    val HOW_MANY_SETS get() = currentSet.HOW_MANY_SETS
    val WEIGHT_GYM get() = currentSet.WEIGHT_GYM
    val WEIGHT_BODY get() = currentSet.WEIGHT_BODY
    val EXERCISE_INFO get() = currentSet.EXERCISE_INFO
    val NOTES get() = currentSet.NOTES
    val EDIT_EXERCISE get() = currentSet.EDIT_EXERCISE
    val DELETE_EXERCISE get() = currentSet.DELETE_EXERCISE
    val SETTINGS get() = currentSet.SETTINGS
    val EDIT_EXERCISE_LIST get() = currentSet.EDIT_EXERCISE_LIST
    val EDIT_EXERCISES get() = currentSet.EDIT_EXERCISES
    val EXERCISE_NAME get() = currentSet.EXERCISE_NAME

    val ADD_STOPWATCH get() = currentSet.ADD_STOPWATCH
    val EDIT_STOPWATCH get() = currentSet.EDIT_STOPWATCH
    val STOPWATCH_NAME get() = currentSet.STOPWATCH_NAME
    val STOPWATCH get() = currentSet.STOPWATCH

    val HEALTH get() = currentSet.HEALTH
    val DIET_PLANS get() = currentSet.DIET_PLANS
    val WEIGHT_HISTORY get() = currentSet.WEIGHT_HISTORY
    val CALORIE_CALCULATOR get() = currentSet.CALORIE_CALCULATOR
    val BMI_CALCULATOR get() = currentSet.BMI_CALCULATOR
    val WATER_INTAKE_CALCULATOR get() = currentSet.WATER_INTAKE_CALCULATOR
    val ADD_WEIGHT get() = currentSet.ADD_WEIGHT
    val DATE get() = currentSet.DATE
    val EDIT_WEIGHT get() = currentSet.EDIT_WEIGHT
    val DELETE_WEIGHT get() = currentSet.DELETE_WEIGHT
    val AGE get() = currentSet.AGE
    val HEIGHT get() = currentSet.HEIGHT
    val SELECT_SEX get() = currentSet.SELECT_SEX
    val ACTIVITY_LEVEL get() = currentSet.ACTIVITY_LEVEL
    val CALCULATE get() = currentSet.CALCULATE
    val RESULT get() = currentSet.RESULT
    val YOUR_BMI_IS get() = currentSet.YOUR_BMI_IS
    val MAINTAIN_WEIGHT get() = currentSet.MAINTAIN_WEIGHT
    val BUILD_MUSCLE get() = currentSet.BUILD_MUSCLE
    val SLOW_WEIGHT_LOSS get() = currentSet.SLOW_WEIGHT_LOSS
    val WEIGHT_LOSS get() = currentSet.WEIGHT_LOSS
    val FAST_WEIGHT_LOSS get() = currentSet.FAST_WEIGHT_LOSS
    val WEEK get() = currentSet.WEEK
    val YOUR_BMR_IS get() = currentSet.YOUR_BMR_IS
    val YOU_SHOULD_DRINK get() = currentSet.YOU_SHOULD_DRINK
    val AT_LEAST get() = currentSet.AT_LEAST
    val LITERS get() = currentSet.LITERS
    val OF_WATER get() = currentSet.OF_WATER
    val EVERY_DAY get() = currentSet.EVERY_DAY

    val APP_SETTINGS get() = currentSet.APP_SETTINGS

    val LITTLE_OR_NO_EXERCISE get() = currentSet.LITTLE_OR_NO_EXERCISE
    val LIGHT_EXERCISE get() = currentSet.LIGHT_EXERCISE
    val MODERATE_EXERCISE get() = currentSet.MODERATE_EXERCISE
    val INTENSE_EXERCISE get() = currentSet.INTENSE_EXERCISE
    val VERY_HARD_EXERCISE get() = currentSet.VERY_HARD_EXERCISE

    val MALE get() = currentSet.MALE
    val FEMALE get() = currentSet.FEMALE

    val NORMAL_WEIGHT get() = currentSet.NORMAL_WEIGHT
    val OVERWEIGHT get() = currentSet.OVERWEIGHT
    val OBESE_CLASS_I get() = currentSet.OBESE_CLASS_I
    val OBESE_CLASS_II get() = currentSet.OBESE_CLASS_II
    val OBESE_CLASS_III get() = currentSet.OBESE_CLASS_III
    val MILD_UNDERWEIGHT get() = currentSet.MILD_UNDERWEIGHT
    val MODERATE_UNDERWEIGHT get() = currentSet.MODERATE_UNDERWEIGHT
    val SEVERE_UNDERWEIGHT get() = currentSet.SEVERE_UNDERWEIGHT

    val LANGUAGE get() = currentSet.LANGUAGE
    val THEME get() = currentSet.THEME
}

data class LanguageSet(
    val NOTHING_HERE_YET: String,
    val DELETE: String,
    val NAME: String,

    val YOUR_PROGRAMS: String,
    val ADD_PROGRAM: String,
    val PROGRAM_NAME: String,
    val EDIT_PROGRAM: String,
    val ADD_TRAINING_DAY: String,
    val EDIT_TRAINING_DAY: String,
    val ADD_EXERCISE: String,
    val SELECT_EXERCISE: String,
    val REST_TIME: String,
    val HOW_MANY_SETS: String,
    val WEIGHT_GYM: String,
    val WEIGHT_BODY: String,
    val EXERCISE_INFO: String,
    val NOTES: String,
    val EDIT_EXERCISE: String,
    val DELETE_EXERCISE: String,
    val SETTINGS: String,
    val EDIT_EXERCISE_LIST: String,
    val EDIT_EXERCISES: String,
    val EXERCISE_NAME: String,

    val ADD_STOPWATCH: String,
    val EDIT_STOPWATCH: String,
    val STOPWATCH_NAME: String,
    val STOPWATCH: String,
    
    val HEALTH: String,
    val DIET_PLANS: String,
    val WEIGHT_HISTORY: String,
    val CALORIE_CALCULATOR: String,
    val BMI_CALCULATOR: String,
    val WATER_INTAKE_CALCULATOR: String,
    val ADD_WEIGHT: String,
    val DATE: String,
    val EDIT_WEIGHT: String,
    val DELETE_WEIGHT: String,
    val AGE: String,
    val HEIGHT: String,
    val SELECT_SEX: String,
    val ACTIVITY_LEVEL: String,
    val CALCULATE: String,
    val RESULT: String,
    val YOUR_BMI_IS: String,
    val MAINTAIN_WEIGHT: String,
    val BUILD_MUSCLE: String,
    val SLOW_WEIGHT_LOSS: String,
    val WEIGHT_LOSS: String,
    val FAST_WEIGHT_LOSS: String,
    val WEEK: String,
    val YOUR_BMR_IS: String,
    val YOU_SHOULD_DRINK: String,
    val AT_LEAST: String,
    val LITERS: String,
    val OF_WATER: String,
    val EVERY_DAY: String,
    
    val APP_SETTINGS: String,
    
    val LITTLE_OR_NO_EXERCISE: String,
    val LIGHT_EXERCISE: String,
    val MODERATE_EXERCISE: String,
    val INTENSE_EXERCISE: String,
    val VERY_HARD_EXERCISE: String,

    val MALE: String,
    val FEMALE: String,

    val NORMAL_WEIGHT: String,
    val OVERWEIGHT: String,
    val OBESE_CLASS_I: String,
    val OBESE_CLASS_II: String,
    val OBESE_CLASS_III: String,
    val MILD_UNDERWEIGHT: String,
    val MODERATE_UNDERWEIGHT: String,
    val SEVERE_UNDERWEIGHT: String,
    
    val LANGUAGE: String,
    val THEME: String
)

object LanguageSets {
    val allLanguages: Map<String, LanguageSet> = mapOf(
        Languages.ENGLISH to LanguageSet(
            NOTHING_HERE_YET = "Nothing here yet!",
            YOUR_PROGRAMS = "Your Programs",
            DELETE = "Delete",
            NAME = "Name",
            ADD_PROGRAM = "Add program",
            PROGRAM_NAME = "Program name",
            EDIT_PROGRAM = "Edit program",
            ADD_TRAINING_DAY = "Add training day",
            EDIT_TRAINING_DAY = "Edit training day",
            ADD_EXERCISE = "Add exercise",
            SELECT_EXERCISE = "Select exercise",
            REST_TIME = "Rest time",
            HOW_MANY_SETS = "How many sets",
            WEIGHT_GYM = "Weight",
            WEIGHT_BODY = "Weight",
            EXERCISE_INFO = "Exercise info",
            NOTES = "Notes",
            EDIT_EXERCISE = "Edit exercise",
            DELETE_EXERCISE = "Delete exercise",
            SETTINGS = "Settings",
            EDIT_EXERCISE_LIST = "Edit exercise list",
            EDIT_EXERCISES = "Edit exercises",
            EXERCISE_NAME = "Exercise name",
            ADD_STOPWATCH = "Add stopwatch",
            EDIT_STOPWATCH = "Edit stopwatch",
            STOPWATCH_NAME = "Stopwatch name",
            STOPWATCH = "Stopwatch",
            HEALTH = "Health",
            DIET_PLANS = "Diet Plans",
            WEIGHT_HISTORY = "Weight History",
            CALORIE_CALCULATOR = "Calorie Calculator",
            BMI_CALCULATOR = "BMI Calculator",
            WATER_INTAKE_CALCULATOR = "Water Intake Calculator",
            ADD_WEIGHT = "Add weight",
            DATE = "Date",
            EDIT_WEIGHT = "Edit weight",
            DELETE_WEIGHT = "Delete weight",
            AGE = "Age",
            HEIGHT = "Height",
            SELECT_SEX = "Select sex",
            ACTIVITY_LEVEL = "Activity level",
            CALCULATE = "Calculate",
            RESULT = "Result",
            YOUR_BMI_IS = "Your BMI is:",
            MAINTAIN_WEIGHT = "Maintain weight:",
            BUILD_MUSCLE = "Build muscle:",
            SLOW_WEIGHT_LOSS = "Slow weight loss:",
            WEIGHT_LOSS = "Weight loss:",
            FAST_WEIGHT_LOSS = "Fast weight loss:",
            WEEK = "week",
            YOUR_BMR_IS = "Your BMR is:",
            YOU_SHOULD_DRINK = "You should drink",
            AT_LEAST = "at least",
            LITERS = "liters",
            OF_WATER = "of water",
            EVERY_DAY = "every day",
            APP_SETTINGS = "App Settings",
            LITTLE_OR_NO_EXERCISE = "Little or no exercise",
            LIGHT_EXERCISE = "Light exercise 1-3 days/week",
            MODERATE_EXERCISE = "Moderate exercise 3-5 days/week",
            INTENSE_EXERCISE = "Intense exercise 6-7 days/week",
            VERY_HARD_EXERCISE = "Very hard daily exercise or physical job",
            MALE = "Male",
            FEMALE = "Female",
            NORMAL_WEIGHT = "Normal weight",
            OVERWEIGHT = "Overweight",
            OBESE_CLASS_I = "Obese class I",
            OBESE_CLASS_II = "Obese class II",
            OBESE_CLASS_III = "Obese class III",
            MILD_UNDERWEIGHT = "Mild underweight",
            MODERATE_UNDERWEIGHT = "Moderate underweight",
            SEVERE_UNDERWEIGHT = "Severe underweight",
            LANGUAGE = "Language",
            THEME = "Theme"
        ),
        Languages.POLISH to LanguageSet(
            NOTHING_HERE_YET = "Jeszcze nic tu nie ma!",
            YOUR_PROGRAMS = "Plany treningowe",
            DELETE = "Usuń",
            NAME = "Nazwa",
            ADD_PROGRAM = "Dodaj plan",
            PROGRAM_NAME = "Nazwa planu",
            EDIT_PROGRAM = "Edytuj plan",
            ADD_TRAINING_DAY = "Dodaj dzień treningowy",
            EDIT_TRAINING_DAY = "Edytuj dzień treningowy",
            ADD_EXERCISE = "Dodaj ćwiczenie",
            SELECT_EXERCISE = "Wybierz ćwiczenie",
            REST_TIME = "Czas przerwy",
            HOW_MANY_SETS = "Liczba serii",
            WEIGHT_GYM = "Ciężar",
            WEIGHT_BODY = "Waga",
            EXERCISE_INFO = "Szczegóły ćwiczenia",
            NOTES = "Notatki",
            EDIT_EXERCISE = "Edytuj ćwiczenie",
            DELETE_EXERCISE = "Usuń ćwiczenie",
            SETTINGS = "Ustawienia",
            EDIT_EXERCISE_LIST = "Edytuj listę ćwiczeń",
            EDIT_EXERCISES = "Edycja ćwiczeń",
            EXERCISE_NAME = "Nazwa ćwiczenia",
            ADD_STOPWATCH = "Dodaj stoper",
            EDIT_STOPWATCH = "Edytuj stoper",
            STOPWATCH_NAME = "Nazwa stopera",
            STOPWATCH = "Stoper",
            HEALTH = "Zdrowie",
            DIET_PLANS = "Plany Dietetyczne",
            WEIGHT_HISTORY = "Historia Masy Ciała",
            CALORIE_CALCULATOR = "Kalkulator Kalorii",
            BMI_CALCULATOR = "Kalkulator BMI",
            WATER_INTAKE_CALCULATOR = "Kalkulator nawodnienia",
            ADD_WEIGHT = "Dodaj wagę",
            DATE = "Data",
            EDIT_WEIGHT = "Edytuj wagę",
            DELETE_WEIGHT = "Usuń wagę",
            AGE = "Wiek",
            HEIGHT = "Wzrost",
            SELECT_SEX = "Wybierz płeć",
            ACTIVITY_LEVEL = "Poziom aktywności",
            CALCULATE = "Oblicz",
            RESULT = "Wynik",
            YOUR_BMI_IS = "Twoje BMI to:",
            MAINTAIN_WEIGHT = "Utrzymanie wagi:",
            BUILD_MUSCLE = "Budowanie mięśni:",
            SLOW_WEIGHT_LOSS = "Wolne odchudzanie:",
            WEIGHT_LOSS = "Odchudzanie:",
            FAST_WEIGHT_LOSS = "Szybkie odchudzanie:",
            WEEK = "tydzień",
            YOUR_BMR_IS = "Twoje BMR to:",
            YOU_SHOULD_DRINK = "Powinieneś pić",
            AT_LEAST = "przynajmniej",
            LITERS = "litrów",
            OF_WATER = "wody",
            EVERY_DAY = "dziennie",
            APP_SETTINGS = "Ustawienia aplikacji",
            LITTLE_OR_NO_EXERCISE = "Brak lub bardzo mała aktywność fizyczna",
            LIGHT_EXERCISE = "Lekki trening 1–3 dni w tygodniu",
            MODERATE_EXERCISE = "Umiarkowany trening 3–5 dni w tygodniu",
            INTENSE_EXERCISE = "Intensywny trening 6–7 dni w tygodniu",
            VERY_HARD_EXERCISE = "Bardzo intensywny trening codziennie lub praca fizyczna",
            MALE = "Mężczyzna",
            FEMALE = "Kobieta",
            NORMAL_WEIGHT = "Waga prawidłowa",
            OVERWEIGHT = "Nadwaga",
            OBESE_CLASS_I = "Otyłość I stopnia",
            OBESE_CLASS_II = "Otyłość II stopnia",
            OBESE_CLASS_III = "Otyłość III stopnia",
            MILD_UNDERWEIGHT = "Lekka niedowaga",
            MODERATE_UNDERWEIGHT = "Niedowaga",
            SEVERE_UNDERWEIGHT = "Poważna niedowaga",
            LANGUAGE = "Język",
            THEME = "Motyw"
        ),
        Languages.SPANISH to LanguageSet(
            NOTHING_HERE_YET = "¡Aún no hay nada!",
            YOUR_PROGRAMS = "Tus programas",
            DELETE = "Eliminar",
            NAME = "Nombre",
            ADD_PROGRAM = "Agregar programa",
            PROGRAM_NAME = "Nombre del programa",
            EDIT_PROGRAM = "Editar programa",
            ADD_TRAINING_DAY = "Agregar día de entrenamiento",
            EDIT_TRAINING_DAY = "Editar día de entrenamiento",
            ADD_EXERCISE = "Agregar ejercicio",
            SELECT_EXERCISE = "Seleccionar ejercicio",
            REST_TIME = "Tiempo de descanso",
            HOW_MANY_SETS = "Cuántas series",
            WEIGHT_GYM = "Peso",
            WEIGHT_BODY = "Peso corporal",
            EXERCISE_INFO = "Info del ejercicio",
            NOTES = "Notas",
            EDIT_EXERCISE = "Editar ejercicio",
            DELETE_EXERCISE = "Eliminar ejercicio",
            SETTINGS = "Ajustes",
            EDIT_EXERCISE_LIST = "Editar lista de ejercicios",
            EDIT_EXERCISES = "Editar ejercicios",
            EXERCISE_NAME = "Nombre del ejercicio",
            ADD_STOPWATCH = "Agregar cronómetro",
            EDIT_STOPWATCH = "Editar cronómetro",
            STOPWATCH_NAME = "Nombre del cronómetro",
            STOPWATCH = "Cronómetro",
            HEALTH = "Salud",
            DIET_PLANS = "Planes de dieta",
            WEIGHT_HISTORY = "Historial de peso",
            CALORIE_CALCULATOR = "Calculadora de calorías",
            BMI_CALCULATOR = "Calculadora IMC",
            WATER_INTAKE_CALCULATOR = "Calculadora de agua",
            ADD_WEIGHT = "Agregar peso",
            DATE = "Fecha",
            EDIT_WEIGHT = "Editar peso",
            DELETE_WEIGHT = "Eliminar peso",
            AGE = "Edad",
            HEIGHT = "Altura",
            SELECT_SEX = "Seleccionar sexo",
            ACTIVITY_LEVEL = "Nivel de actividad",
            CALCULATE = "Calcular",
            RESULT = "Resultado",
            YOUR_BMI_IS = "Tu IMC es:",
            MAINTAIN_WEIGHT = "Mantener peso:",
            BUILD_MUSCLE = "Ganar músculo:",
            SLOW_WEIGHT_LOSS = "Pérdida lenta:",
            WEIGHT_LOSS = "Pérdida de peso:",
            FAST_WEIGHT_LOSS = "Pérdida rápida:",
            WEEK = "semana",
            YOUR_BMR_IS = "Tu TMB es:",
            YOU_SHOULD_DRINK = "Deberías beber",
            AT_LEAST = "al menos",
            LITERS = "litros",
            OF_WATER = "de agua",
            EVERY_DAY = "cada día",
            APP_SETTINGS = "Ajustes de la app",
            LITTLE_OR_NO_EXERCISE = "Poca o ninguna actividad",
            LIGHT_EXERCISE = "Ejercicio ligero 1–3 días/sem",
            MODERATE_EXERCISE = "Ejercicio moderado 3–5 días/sem",
            INTENSE_EXERCISE = "Ejercicio intenso 6–7 días/sem",
            VERY_HARD_EXERCISE = "Ejercicio diario muy intenso o trabajo físico",
            MALE = "Hombre",
            FEMALE = "Mujer",
            NORMAL_WEIGHT = "Peso normal",
            OVERWEIGHT = "Sobrepeso",
            OBESE_CLASS_I = "Obesidad clase I",
            OBESE_CLASS_II = "Obesidad clase II",
            OBESE_CLASS_III = "Obesidad clase III",
            MILD_UNDERWEIGHT = "Delgadez leve",
            MODERATE_UNDERWEIGHT = "Delgadez moderada",
            SEVERE_UNDERWEIGHT = "Delgadez severa",
            LANGUAGE = "Idioma",
            THEME = "Tema"
        ),
        Languages.GERMAN to LanguageSet(
            NOTHING_HERE_YET = "Noch nichts hier!",
            YOUR_PROGRAMS = "Deine Programme",
            DELETE = "Löschen",
            NAME = "Name",
            ADD_PROGRAM = "Programm hinzufügen",
            PROGRAM_NAME = "Programmname",
            EDIT_PROGRAM = "Programm bearbeiten",
            ADD_TRAINING_DAY = "Trainingstag hinzufügen",
            EDIT_TRAINING_DAY = "Trainingstag bearbeiten",
            ADD_EXERCISE = "Übung hinzufügen",
            SELECT_EXERCISE = "Übung auswählen",
            REST_TIME = "Pausezeit",
            HOW_MANY_SETS = "Wie viele Sätze",
            WEIGHT_GYM = "Gewicht",
            WEIGHT_BODY = "Gewicht",
            EXERCISE_INFO = "Übungsinformationen",
            NOTES = "Notizen",
            EDIT_EXERCISE = "Übung bearbeiten",
            DELETE_EXERCISE = "Übung löschen",
            SETTINGS = "Einstellungen",
            EDIT_EXERCISE_LIST = "Übungsliste bearbeiten",
            EDIT_EXERCISES = "Übungen bearbeiten",
            EXERCISE_NAME = "Übungsname",
            ADD_STOPWATCH = "Stoppuhr hinzufügen",
            EDIT_STOPWATCH = "Stoppuhr bearbeiten",
            STOPWATCH_NAME = "Name der Stoppuhr",
            STOPWATCH = "Stoppuhr",
            HEALTH = "Gesundheit",
            DIET_PLANS = "Ernährungspläne",
            WEIGHT_HISTORY = "Gewichtsverlauf",
            CALORIE_CALCULATOR = "Kalorienrechner",
            BMI_CALCULATOR = "BMI-Rechner",
            WATER_INTAKE_CALCULATOR = "Wasseraufnahme-Rechner",
            ADD_WEIGHT = "Gewicht hinzufügen",
            DATE = "Datum",
            EDIT_WEIGHT = "Gewicht bearbeiten",
            DELETE_WEIGHT = "Gewicht löschen",
            AGE = "Alter",
            HEIGHT = "Größe",
            SELECT_SEX = "Geschlecht auswählen",
            ACTIVITY_LEVEL = "Aktivitätsniveau",
            CALCULATE = "Berechnen",
            RESULT = "Ergebnis",
            YOUR_BMI_IS = "Dein BMI ist:",
            MAINTAIN_WEIGHT = "Gewicht halten:",
            BUILD_MUSCLE = "Muskeln aufbauen:",
            SLOW_WEIGHT_LOSS = "Langsamer Gewichtsverlust:",
            WEIGHT_LOSS = "Gewichtsverlust:",
            FAST_WEIGHT_LOSS = "Schneller Gewichtsverlust:",
            WEEK = "Woche",
            YOUR_BMR_IS = "Dein BMR ist:",
            YOU_SHOULD_DRINK = "Du solltest trinken",
            AT_LEAST = "mindestens",
            LITERS = "Liter",
            OF_WATER = "Wasser",
            EVERY_DAY = "jeden Tag",
            APP_SETTINGS = "App-Einstellungen",
            LITTLE_OR_NO_EXERCISE = "Wenig oder kein Sport",
            LIGHT_EXERCISE = "Leichte Aktivität 1–3 Tage/Woche",
            MODERATE_EXERCISE = "Mäßige Aktivität 3–5 Tage/Woche",
            INTENSE_EXERCISE = "Intensive Aktivität 6–7 Tage/Woche",
            VERY_HARD_EXERCISE = "Sehr harte tägliche Aktivität oder körperliche Arbeit",
            MALE = "Männlich",
            FEMALE = "Weiblich",
            NORMAL_WEIGHT = "Normalgewicht",
            OVERWEIGHT = "Übergewicht",
            OBESE_CLASS_I = "Adipositas Klasse I",
            OBESE_CLASS_II = "Adipositas Klasse II",
            OBESE_CLASS_III = "Adipositas Klasse III",
            MILD_UNDERWEIGHT = "Leichtes Untergewicht",
            MODERATE_UNDERWEIGHT = "Mäßiges Untergewicht",
            SEVERE_UNDERWEIGHT = "Schweres Untergewicht",
            LANGUAGE = "Sprache",
            THEME = "Thema"
        ),
    )
}