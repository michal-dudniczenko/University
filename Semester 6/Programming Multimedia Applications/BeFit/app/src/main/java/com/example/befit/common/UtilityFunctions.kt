package com.example.befit.common

import java.text.SimpleDateFormat
import java.util.Date
import java.util.Locale
import java.util.regex.Pattern

fun formatDateFromLong(timestamp: Long): String {
    val formatter = SimpleDateFormat("dd.MM.yyyy", Locale.getDefault())
    return formatter.format(Date(timestamp))
}

fun floatToString(float: Float?): String {
    if (float == null) return ""
    return if (float % 1 == 0f) {
        float.toInt().toString()
    } else {
        float.toString()
    }
}

fun isValidDate(dateString: String): Date? {
    val regex = "^(0[1-9]|[12][0-9]|3[01])\\.(0[1-9]|1[0-2])\\.\\d{4}$"

    if (!Pattern.matches(regex, dateString)) {
        return null
    }

    val dateFormat = SimpleDateFormat("dd.MM.yyyy", Locale.getDefault())
    dateFormat.isLenient = false // Strict parsing
    try {
        val date: Date = dateFormat.parse(dateString) ?: return null
        return date
    } catch (_: Exception) {
        return null
    }
}

fun formatTime(seconds: Long): String {
    val minutes = (seconds % 3600) / 60
    val secs = seconds % 60
    return String.format(Locale.getDefault(), "%02d:%02d", minutes, secs)
}
