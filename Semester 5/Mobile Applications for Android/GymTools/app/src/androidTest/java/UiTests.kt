import android.util.Log
import androidx.compose.ui.test.assertTextEquals
import androidx.compose.ui.test.junit4.createComposeRule
import androidx.compose.ui.test.onNodeWithTag
import androidx.compose.ui.test.performClick
import androidx.lifecycle.viewmodel.compose.viewModel
import com.example.gymtools.stopwatches.StopwatchesScreen
import com.example.gymtools.stopwatches.StopwatchesViewModel
import org.junit.Rule
import org.junit.Test

class UiTests {

    @get:Rule
    val composeTestRule = createComposeRule()

    @Test
    fun fullStopwatchesScreenTest() {
        val stopwatchesViewModel = StopwatchesViewModel()

        composeTestRule.setContent {
            StopwatchesScreen(
                viewModel = stopwatchesViewModel
            )
        }

        composeTestRule.waitForIdle()
        composeTestRule.onNodeWithTag("stopwatch1 text").assertTextEquals("00:00")
        composeTestRule.onNodeWithTag("stopwatch2 text").assertTextEquals("00:00")


        stopwatchesViewModel.startPause1()

        var currentTime: Long

        var startTime = System.currentTimeMillis()
        while (true) {
            currentTime = System.currentTimeMillis()
            if (currentTime - startTime >= 3100) {
                break
            }
        }

        composeTestRule.waitForIdle()
        composeTestRule.onNodeWithTag("stopwatch1 text").assertTextEquals("00:03")
        composeTestRule.onNodeWithTag("stopwatch2 text").assertTextEquals("00:00")

        stopwatchesViewModel.startPause2()

        startTime = System.currentTimeMillis()
        while (true) {
            currentTime = System.currentTimeMillis()
            if (currentTime - startTime >= 3100) {
                break
            }
        }

        composeTestRule.waitForIdle()
        composeTestRule.onNodeWithTag("stopwatch1 text").assertTextEquals("00:06")
        composeTestRule.onNodeWithTag("stopwatch2 text").assertTextEquals("00:03")

        stopwatchesViewModel.reset1()

        startTime = System.currentTimeMillis()
        while (true) {
            currentTime = System.currentTimeMillis()
            if (currentTime - startTime >= 3100) {
                break
            }
        }

        composeTestRule.waitForIdle()
        composeTestRule.onNodeWithTag("stopwatch1 text").assertTextEquals("00:00")
        composeTestRule.onNodeWithTag("stopwatch2 text").assertTextEquals("00:06")

        stopwatchesViewModel.startPause2()

        startTime = System.currentTimeMillis()
        while (true) {
            currentTime = System.currentTimeMillis()
            if (currentTime - startTime >= 2100) {
                break
            }
        }

        composeTestRule.waitForIdle()
        composeTestRule.onNodeWithTag("stopwatch1 text").assertTextEquals("00:00")
        composeTestRule.onNodeWithTag("stopwatch2 text").assertTextEquals("00:06")

        stopwatchesViewModel.reset2()

        startTime = System.currentTimeMillis()
        while (true) {
            currentTime = System.currentTimeMillis()
            if (currentTime - startTime >= 2100) {
                break
            }
        }

        composeTestRule.waitForIdle()
        composeTestRule.onNodeWithTag("stopwatch1 text").assertTextEquals("00:00")
        composeTestRule.onNodeWithTag("stopwatch2 text").assertTextEquals("00:00")
    }
}