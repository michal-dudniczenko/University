package com.example.gymtools.trainingprograms

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
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableFloatStateOf
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.gymtools.R
import com.example.gymtools.common.CustomExercisePicker
import com.example.gymtools.common.CustomFloatPicker
import com.example.gymtools.common.CustomFloatingButton
import com.example.gymtools.common.CustomIntPicker
import com.example.gymtools.common.CustomStringPicker
import com.example.gymtools.common.CustomText
import com.example.gymtools.common.Heading
import com.example.gymtools.common.adaptiveHeight
import com.example.gymtools.common.adaptiveWidth
import com.example.gymtools.common.lightGreen
import com.example.gymtools.common.lightRed
import kotlinx.coroutines.launch

@Composable
fun EditExerciseFromDayScreen(
    trainingDayExerciseId: Int,
    navController: NavHostController,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    val exercises by viewModel.exercises.collectAsState()

    val trainingDayExercise = viewModel.getTrainingDayExerciseById(trainingDayExerciseId)

    val trainingDayId = trainingDayExercise?.trainingDayId

    var selectedExercise by remember { mutableStateOf(viewModel.getExerciseById(trainingDayExercise?.exerciseId ?: 0)) }
    var selectedSetsNumber by remember { mutableIntStateOf(viewModel.getNumberOfSets(trainingDayExerciseId)) }
    var selectedRestTime by remember { mutableStateOf(trainingDayExercise?.restTime ?: "") }
    var selectedWeight by remember { mutableFloatStateOf(trainingDayExercise?.weight ?: 0f) }

    val coroutineScope = rememberCoroutineScope()

    val initialExerciseId = remember { selectedExercise?.id ?: 0 }

    Box(
        modifier = modifier.fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = R.drawable.check,
            description = "Confirm button",
            color = lightGreen,
            onClick = {
                coroutineScope.launch {
                    if (selectedExercise != null) {
                        val wasExerciseChanged = selectedExercise?.id != initialExerciseId
                        if (!wasExerciseChanged || !viewModel.checkIfExerciseExists(trainingDayId ?: 0, selectedExercise?.id ?: 0)) {
                            viewModel.updateTrainingDayExercise(
                                trainingDayExerciseId = trainingDayExerciseId,
                                trainingDayId = trainingDayId ?: 0,
                                exerciseId = selectedExercise?.id ?: 0,
                                wasExerciseChanged = wasExerciseChanged,
                                setsNumber = selectedSetsNumber,
                                restTime = selectedRestTime,
                                weight = if (selectedWeight == 0f) null else selectedWeight
                            )
                            navController.navigate("View training day/$trainingDayId")
                        }
                    }
                }
            },
            modifier = Modifier
                .align(Alignment.BottomEnd)
                .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
        )
        CustomFloatingButton(
            icon = R.drawable.cancel,
            description = "Cancel button",
            color = lightRed,
            onClick = { navController.navigate("View training day/$trainingDayId") },
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
            Heading("Edit exercise")
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
                    label = "Rest time",
                    imageId = R.drawable.clock,
                    onValueChange = { selectedRestTime = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(30).dp))
                CustomIntPicker(
                    selectedValue = selectedSetsNumber,
                    label = "How many sets",
                    imageId = R.drawable.numbers,
                    onValueChange = { selectedSetsNumber = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(30).dp))
                CustomFloatPicker(
                    selectedValue = selectedWeight,
                    label = "Weight",
                    imageId = R.drawable.numbers,
                    onValueChange = { selectedWeight = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(40).dp))
                Box(
                    contentAlignment = Alignment.Center,
                    modifier = modifier
                        .fillMaxWidth()
                        .background(
                            color = lightRed,
                            shape = RoundedCornerShape(adaptiveWidth(16).dp)
                        )
                        .clickable(
                            onClick = {
                                coroutineScope.launch {
                                    viewModel.deleteTrainingDayExercise(id = trainingDayExerciseId)
                                    navController.navigate("View training day/$trainingDayId")
                                }
                            }
                        )
                ) {
                    CustomText(
                        text = "Delete exercise",
                        color = Color.White,
                        modifier = Modifier.padding(adaptiveWidth(16).dp)
                    )
                }
                Spacer(modifier = Modifier.height(adaptiveHeight(100).dp))
            }
        }
    }
}