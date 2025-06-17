package com.example.befit

import com.example.befit.database.UserDataDao
import junit.framework.TestCase
import kotlinx.coroutines.test.runTest
import org.junit.Assert.assertEquals
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.mock
import org.mockito.kotlin.whenever

class HealthViewModelTest {

    private lateinit var userDataDao: UserDataDao
    private lateinit var viewModel: TestHealthViewModel

    @Before
    fun setup() = runTest {
        userDataDao = mock()
        whenever(userDataDao.getAll()).thenReturn(emptyList())

        viewModel = TestHealthViewModel(userDataDao)
    }

    @Test
    fun fullBmiCalculatorTest() {
        val testCases = listOf(
            Triple(0, 70f, 0f),
            Triple(170, 0f, 0f),
            Triple(0, 0f, 0f),
            Triple(-170, 70f, 0f),
            Triple(170, -70f, 0f),
            Triple(180, 81f, 25.0f),
            Triple(160, 100f, 39.06f),
            Triple(190, 60f, 16.63f),
            Triple(150, 45f, 20.0f),
            Triple(200, 100f, 25.0f),
            Triple(123, 45.6f, 30.14f),
        )

        testCases.forEach { (height, weight, expected) ->
            val result = viewModel.calculateBmi(height, weight)
            assertEquals(
                "BMI for height=$height and weight=$weight should be $expected but got $result",
                expected, result, 0.01f
            )
        }
    }

    @Test
    fun fullWaterIntakeCalculatorTest() {
        val testCases = mapOf(
            0f to 0f,
            -1f to 0f,
            -100f to 0f,
            1f to 0.035f,
            2.5f to 0.0875f,
            10f to 0.35f,
            50f to 1.75f,
            60f to 2.1f,
            70f to 2.45f,
            80f to 2.8f,
            90f to 3.15f,
            100f to 3.5f,
            120f to 4.2f,
            150f to 5.25f,
            200f to 7.0f,
            65.5f to 2.2925f,
            73.3f to 2.5655f,
            85.75f to 3.00125f
        )

        testCases.forEach { (input, expected) ->
            val result = viewModel.calculateWaterIntake(input)
            TestCase.assertEquals(
                "Expected $expected but got $result for input $input",
                expected,
                result
            )
        }
    }
}