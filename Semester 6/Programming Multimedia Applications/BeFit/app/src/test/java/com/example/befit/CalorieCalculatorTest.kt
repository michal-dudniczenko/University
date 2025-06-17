package com.example.befit

import com.example.befit.database.UserDataDao
import com.example.befit.health.caloriecalculator.ActivityLevel
import com.example.befit.health.caloriecalculator.ActivityLevels
import com.example.befit.health.caloriecalculator.CalorieCalculatorResult
import com.example.befit.health.caloriecalculator.CalorieCalculatorViewModel
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.ExperimentalCoroutinesApi
import kotlinx.coroutines.test.StandardTestDispatcher
import kotlinx.coroutines.test.resetMain
import kotlinx.coroutines.test.runTest
import kotlinx.coroutines.test.setMain
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.mock
import org.mockito.kotlin.whenever

data class CalorieCalculatorTestData(
    val age: Int,
    val height: Int,
    val weight: Float,
    val isMale: Boolean,
    val activityLevel: ActivityLevel,

    val bmr: Int,
    val maintain: Int,
    val gainWeight: Int,
    val slowWeightLoss: Int,
    val weightLoss: Int,
    val fastWeightLoss: Int
)

@OptIn(ExperimentalCoroutinesApi::class)
class CalorieCalculatorTest {

    private lateinit var userDataDao: UserDataDao
    private lateinit var healthViewModel: TestHealthViewModel
    private lateinit var viewModel: CalorieCalculatorViewModel

    private val testDispatcher = StandardTestDispatcher()

    @Before
    fun setup() = runTest {
        Dispatchers.setMain(testDispatcher)

        userDataDao = mock()
        whenever(userDataDao.getAll()).thenReturn(emptyList())
        healthViewModel = TestHealthViewModel(userDataDao)
        viewModel = CalorieCalculatorViewModel(healthViewModel)
    }

    @After
    fun tearDown() {
        Dispatchers.resetMain()
    }

    @Test
    fun fullCalorieCalculatorTest() {
        val activityLevels: List<ActivityLevel> = ActivityLevels.getLevels()

        val testCases = listOf(
            CalorieCalculatorTestData(
                age = 24,
                height = 175,
                weight = 80f,
                isMale = true,
                activityLevel = activityLevels[1], // 1.375
                bmr = 1779,
                maintain = 2446,
                gainWeight = 2746,
                slowWeightLoss = 2196,
                weightLoss = 1946,
                fastWeightLoss = 1446
            ),
            CalorieCalculatorTestData(
                age = 30,
                height = 185,
                weight = 70f,
                isMale = false,
                activityLevel = activityLevels[0], // 1.2
                bmr = 1545,
                maintain = 1854,
                gainWeight = 2154,
                slowWeightLoss = 1604,
                weightLoss = 1354,
                fastWeightLoss = 854
            ),
            CalorieCalculatorTestData(
                age = 52,
                height = 155,
                weight = 41.5f,
                isMale = false,
                activityLevel = activityLevels[2], // 1.55
                bmr = 963,
                maintain = 1493,
                gainWeight = 1793,
                slowWeightLoss = 1243,
                weightLoss = 993,
                fastWeightLoss = 493
            ),
            CalorieCalculatorTestData(
                age = 16,
                height = 215,
                weight = 75.87f,
                isMale = true,
                activityLevel = activityLevels[4], // 1.9
                bmr = 2027,
                maintain = 3851,
                gainWeight = 4151,
                slowWeightLoss = 3601,
                weightLoss = 3351,
                fastWeightLoss = 2851
            ),
            CalorieCalculatorTestData(
                age = 82,
                height = 164,
                weight = 66f,
                isMale = false,
                activityLevel = activityLevels[3], // 1.725
                bmr = 1114,
                maintain = 1922,
                gainWeight = 2222,
                slowWeightLoss = 1672,
                weightLoss = 1422,
                fastWeightLoss = 922
            )
        )


        for (testData: CalorieCalculatorTestData in testCases) {
            viewModel.calculateCalories(
                age = testData.age,
                height = testData. height,
                weight = testData.weight,
                isMale = testData.isMale,
                activityLevel = testData.activityLevel
            )

            val result: CalorieCalculatorResult = viewModel.calculateResult.value!!

            assertEquals("Expected ${testData.bmr} but got ${result.bmr}", testData.bmr, result.bmr)
            assertEquals("Expected ${testData.maintain} but got ${result.maintain}", testData.maintain, result.maintain)
            assertEquals("Expected ${testData.gainWeight} but got ${result.gainWeight}", testData.gainWeight, result.gainWeight)
            assertEquals("Expected ${testData.slowWeightLoss} but got ${result.slowWeightLoss}", testData.slowWeightLoss, result.slowWeightLoss)
            assertEquals("Expected ${testData.weightLoss} but got ${result.weightLoss}", testData.weightLoss, result.weightLoss)
            assertEquals("Expected ${testData.fastWeightLoss} but got ${result.fastWeightLoss}", testData.fastWeightLoss, result.fastWeightLoss)
        }
    }
}