package com.example.befit.stopwatches

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomStringPicker
import com.example.befit.common.Heading
import com.example.befit.constants.StopwatchesRoutes
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes

@Composable
fun EditStopwatchNameScreen(
    stopwatchId: Int,
    navController: NavHostController,
    viewModel: StopwatchesViewModel,
    modifier: Modifier = Modifier
) {
    var selectedName by remember { mutableStateOf(viewModel.getStopwatchName(stopwatchId) ?: "") }

    Box(
        modifier = modifier.fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.BACK_ON_SECONDARY,
            description = "Back button",
            onClick = {
                if (selectedName.isNotEmpty()) {
                    viewModel.updateStopwatch(id = stopwatchId, name = selectedName)
                }
                navController.navigate(StopwatchesRoutes.STOPWATCHES)
            },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = 30.dp, y = (-30).dp)
        )
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading(Strings.EDIT_STOPWATCH)
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                verticalArrangement = Arrangement.Center,
                modifier = modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight()
                    .padding(bottom = 125.dp)
                    .verticalScroll(rememberScrollState())
            ) {
                CustomStringPicker(
                    selectedValue = selectedName,
                    label = Strings.STOPWATCH_NAME,
                    onValueChange = { selectedName = it }
                )
            }
        }
    }
}