package com.example.befit.trainingprograms

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.R
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomStringPicker
import com.example.befit.common.Heading
import com.example.befit.common.TrainingProgramsRoutes
import com.example.befit.common.adaptiveHeight
import com.example.befit.common.adaptiveWidth
import com.example.befit.common.bright

@Composable
fun EditTrainingDayScreen(
    trainingDayId: Int,
    navController: NavHostController,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    val trainingDays by viewModel.trainingDays
    val trainingDay = trainingDays.find { it.id == trainingDayId }

    val programId = trainingDay?.programId ?: 0

    var selectedName by remember { mutableStateOf(trainingDay?.name ?: "") }

    Box(
        modifier = modifier.fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = R.drawable.back,
            description = "Back button",
            onClick = {
                if (selectedName.isNotEmpty() && trainingDay != null) {
                    viewModel.updateTrainingDay(trainingDayId = trainingDayId, programId = programId, name = selectedName)
                }
                navController.navigate(TrainingProgramsRoutes.TRAINING_DAYS_LIST(programId))
            },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = adaptiveWidth(32).dp, y = adaptiveWidth(-32).dp)
        )
        if (trainingDay == null) {
            Box(
                modifier = Modifier.fillMaxSize()
            ) {
                CircularProgressIndicator(
                    color = bright,
                    modifier = Modifier.align(Alignment.Center)
                )
                return
            }
        }
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading("Edit training day")
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                verticalArrangement = Arrangement.Center,
                modifier = modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight()
            ) {
                CustomStringPicker(
                    selectedValue = selectedName,
                    label = "Name",
                    onValueChange = { selectedName = it }
                )
                Spacer(modifier = Modifier.height(adaptiveHeight(200).dp))
            }
        }
    }
}