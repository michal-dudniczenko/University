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
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomExercisePicker
import com.example.befit.common.CustomFloatPicker
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomIntPicker
import com.example.befit.common.CustomStringPicker
import com.example.befit.common.Heading
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.TrainingProgramsRoutes
import com.example.befit.constants.adaptiveHeight
import com.example.befit.constants.adaptiveWidth
import com.example.befit.database.Exercise

@Composable
fun AddExerciseToDayScreen(
    trainingDayId: Int,
    navController: NavHostController,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    val exercises by viewModel.exercises
    val trainingDayExercises by viewModel.trainingDaysExercises

    var selectedExercise by remember { mutableStateOf<Exercise?>(null) }
    var selectedSetsNumber by remember { mutableIntStateOf(0) }
    var selectedRestTime by remember { mutableStateOf("") }
    var selectedWeight by remember { mutableStateOf<Float?>(null) }

    Box(
        modifier = modifier.fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.CHECK_ON_ADD_CONFIRM,
            description = "Confirm button",
            color = Themes.ADD_CONFIRM_COLOR,
            onClick = {
                if (selectedExercise != null) {
                    // nie mozna miec dwa razy tego samego cwiczenia w tym samym dniu treningowym
                    if (trainingDayExercises.find { it.trainingDayId == trainingDayId && it.exerciseId == selectedExercise!!.id } == null) {
                        viewModel.addTrainingDayExercise(
                            trainingDayId = trainingDayId,
                            exerciseId = selectedExercise!!.id,
                            restTime = selectedRestTime,
                            setsNumber = selectedSetsNumber,
                            weight = selectedWeight
                        )
                        navController.navigate(TrainingProgramsRoutes.VIEW_TRAINING_DAY(trainingDayId))
                    }
                }
            },
            modifier = Modifier
                .align(Alignment.BottomEnd)
                .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
        )
        CustomFloatingButton(
            icon = Themes.CANCEL_ON_DELETE_CANCEL,
            description = "Cancel button",
            color = Themes.DELETE_CANCEL_COLOR,
            onClick = { navController.navigate(TrainingProgramsRoutes.VIEW_TRAINING_DAY(trainingDayId)) },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = adaptiveWidth(32).dp, y = adaptiveWidth(-32).dp)
        )
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading(Strings.ADD_EXERCISE)
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                verticalArrangement = Arrangement.Center,
                modifier = modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight()
            ) {
                CustomExercisePicker(
                    exercises = exercises,
                    onExerciseSelected = { selectedExercise = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(30).dp))
                CustomStringPicker(
                    label = Strings.REST_TIME,
                    imageId = Themes.CLOCK_ON_PRIMARY,
                    onValueChange = { selectedRestTime = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(30).dp))
                CustomIntPicker(
                    label = Strings.HOW_MANY_SETS,
                    imageId = Themes.NUMBERS_ON_PRIMARY,
                    onValueChange = { selectedSetsNumber = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(30).dp))
                CustomFloatPicker(
                    label = Strings.WEIGHT_GYM,
                    imageId = Themes.NUMBERS_ON_PRIMARY,
                    onValueChange = { selectedWeight = it }
                )
                Spacer(modifier = Modifier.height(adaptiveHeight(100).dp))
            }
        }
    }
}