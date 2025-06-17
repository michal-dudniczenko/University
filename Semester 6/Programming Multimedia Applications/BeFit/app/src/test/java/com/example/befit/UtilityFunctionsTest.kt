package com.example.befit

import com.example.befit.common.floatToString
import com.example.befit.common.formatTime
import com.example.befit.common.isValidDate
import junit.framework.TestCase.assertEquals
import org.junit.Test
import java.text.SimpleDateFormat
import java.util.Locale

class UtilityFunctionsTest {
    @Test
    fun fullFloatToStringUtilityTest() {

        // actual to expected
        val testValues = mapOf(
            null to "",
            0f to "0",
            0.0f to "0",
            5f to "5",
            5.0f to "5",
            5.1f to "5.1",
            5.12f to "5.12",
            5.123f to "5.123",
            -5f to "-5",
            -5.0f to "-5",
            -5.1f to "-5.1",
            -5.12f to "-5.12",
            -5.123f to "-5.123"
        )

        testValues.forEach { (input, expected) ->
            val result = floatToString(input)
            assertEquals("Expected $expected but got $result for input $input", expected, result)
        }
    }

    @Test
    fun fullFormatTimeUtilityTest() {

        // actual to expected
        val testValues = mapOf(
            0 * 60 + 0L to "00:00",
            2 * 60 + 12L to "02:12",
            5 * 60 + 0L to "05:00",
            59 * 60 + 59L to "59:59",
            60 * 60 + 0L to "00:00",
            65 * 60 + 5L to "05:05",
            607 * 60 + 21L to "07:21"
        )

        testValues.forEach { (input, expected) ->
            val result = formatTime(input)
            assertEquals("Expected $expected but got $result for input $input", expected, result)
        }
    }

    @Test
    fun fullIsValidDateUtilityTest() {

        val dateFormat = SimpleDateFormat("dd.MM.yyyy", Locale.getDefault())
        dateFormat.isLenient = false

        // actual to expected
        val testValues = mapOf(
            "26.04.2001" to dateFormat.parse("26.04.2001"),
            "31.05.2007" to dateFormat.parse("31.05.2007"),
            "22.12.1973" to dateFormat.parse("22.12.1973"),
            "27.12.1949" to dateFormat.parse("27.12.1949"),
            "26:04:2001" to null,
            "32.04.2001" to null,
            "-26.04.2001" to null,
            "26.xx.2001" to null,
            "26.04.2001 r." to null,
            "26.04.2001r." to null,
            "26.04.2001r" to null,
            "randomtext123" to null,
            "7462194" to null,
            "2001.04.26" to null,
            "2001.26.04" to null,
            "04.26.2001" to null,
            "poniedziałek" to null,
            "dzisiaj" to null,
            "26 kwietnia 2001" to null,
            "26.04.97" to null
        )

        testValues.forEach { (input, expected) ->
            val result = isValidDate(input)
            assertEquals("Expected $expected but got $result for input $input", expected, result)

        }
    }
}