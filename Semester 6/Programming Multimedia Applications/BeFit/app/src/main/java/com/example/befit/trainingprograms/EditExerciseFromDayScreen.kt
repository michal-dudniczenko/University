package com.example.befit.trainingprograms

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableFloatStateOf
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomExercisePicker
import com.example.befit.common.CustomFloatPicker
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomIntPicker
import com.example.befit.common.CustomStringPicker
import com.example.befit.common.CustomText
import com.example.befit.common.Heading
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.TrainingProgramsRoutes
import com.example.befit.constants.adaptiveHeight
import com.example.befit.constants.adaptiveWidth

@Composable
fun EditExerciseFromDayScreen(
    trainingDayExerciseId: Int,
    navController: NavHostController,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    val exercises by viewModel.exercises
    val trainingDayExercises by viewModel.trainingDaysExercises
    val sets by viewModel.sets

    val trainingDayExercise = trainingDayExercises.find { it.id == trainingDayExerciseId }

    val trainingDayId = trainingDayExercise?.trainingDayId ?: 0

    var selectedExercise by remember { mutableStateOf(exercises.find { it.id == (trainingDayExercise?.exerciseId ?: 0) }) }
    var selectedSetsNumber by remember { mutableIntStateOf(sets.count { it.trainingDayExerciseId == trainingDayExerciseId }) }
    var selectedRestTime by remember { mutableStateOf(trainingDayExercise?.restTime ?: "") }
    var selectedWeight by remember { mutableFloatStateOf(trainingDayExercise?.weight ?: 0f) }

    val initialExerciseId = remember { selectedExercise?.id }

    Box(
        modifier = modifier.fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.CHECK_ON_ADD_CONFIRM,
            description = "Confirm button",
            color = Themes.ADD_CONFIRM_COLOR,
            onClick = {
                if (selectedExercise != null) {
                    val wasExerciseChanged = selectedExercise?.id != initialExerciseId
                    if (!wasExerciseChanged || trainingDayExercises.find { it.trainingDayId == trainingDayId && it.exerciseId == selectedExercise!!.id } == null) {
                        viewModel.updateTrainingDayExercise(
                            trainingDayExerciseId = trainingDayExerciseId,
                            trainingDayId = trainingDayId,
                            exerciseId = selectedExercise!!.id,
                            wasExerciseChanged = wasExerciseChanged,
                            setsNumber = selectedSetsNumber,
                            restTime = selectedRestTime,
                            weight = if (selectedWeight == 0f) null else selectedWeight
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
            Heading(Strings.EDIT_EXERCISE)
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                verticalArrangement = Arrangement.Center,
                modifier = modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight()
            ) {
                CustomExercisePicker(
                    selectedExercise = selectedExercise,
                    exercises = exercises,
                    onExerciseSelected = { selectedExercise = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(30).dp))
                CustomStringPicker(
                    selectedValue = selectedRestTime,
                    label = Strings.REST_TIME,
                    imageId = Themes.CLOCK_ON_PRIMARY,
                    onValueChange = { selectedRestTime = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(30).dp))
                CustomIntPicker(
                    selectedValue = selectedSetsNumber,
                    label = Strings.HOW_MANY_SETS,
                    imageId = Themes.NUMBERS_ON_PRIMARY,
                    onValueChange = { selectedSetsNumber = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(30).dp))
                CustomFloatPicker(
                    selectedValue = selectedWeight,
                    label = Strings.WEIGHT_GYM,
                    imageId = Themes.NUMBERS_ON_PRIMARY,
                    onValueChange = { selectedWeight = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(40).dp))
                Box(
                    contentAlignment = Alignment.Center,
                    modifier = modifier
                        .fillMaxWidth()
                        .clip(RoundedCornerShape(adaptiveWidth(16).dp))
                        .background(color = Themes.DELETE_CANCEL_COLOR)
                        .clickable(
                            onClick = {
                                viewModel.deleteTrainingDayExercise(id = trainingDayExerciseId)
                                navController.navigate(TrainingProgramsRoutes.VIEW_TRAINING_DAY(trainingDayId))
                            }
                        )
                ) {
                    CustomText(
                        text = Strings.DELETE_EXERCISE,
                        color = Themes.ON_DELETE_CANCEL,
                        modifier = Modifier.padding(adaptiveWidth(16).dp)
                    )
                }
                Spacer(modifier = Modifier.height(adaptiveHeight(100).dp))
            }
        }
    }
}