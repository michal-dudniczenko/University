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
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.TrainingProgramsRoutes

@Composable
fun AddTrainingDayScreen(
    programId: Int,
    navController: NavHostController,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    var selectedName by remember { mutableStateOf("") }

    Box(
        modifier = modifier.fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.CHECK_ON_ADD_CONFIRM,
            description = "Confirm button",
            color = Themes.ADD_CONFIRM_COLOR,
            onClick = {
                if (selectedName.isNotEmpty()) {
                    viewModel.addTrainingDay(programId = programId, name = selectedName)
                    navController.navigate(TrainingProgramsRoutes.TRAINING_DAYS_LIST(programId))
                }
            },
            modifier = Modifier
                .align(Alignment.BottomEnd)
                .offset(x = (-30).dp, y = (-30).dp)
        )
        CustomFloatingButton(
            icon = Themes.CANCEL_ON_DELETE_CANCEL,
            description = "Cancel button",
            color = Themes.DELETE_CANCEL_COLOR,
            onClick = { navController.navigate(TrainingProgramsRoutes.TRAINING_DAYS_LIST(programId)) },
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
            Heading(Strings.ADD_TRAINING_DAY)
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                verticalArrangement = Arrangement.Center,
                modifier = modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight()
            ) {
                CustomStringPicker(
                    label = Strings.NAME,
                    onValueChange = { selectedName = it }
                )
                Spacer(modifier = Modifier.height(200.dp))
            }
        }
    }
}