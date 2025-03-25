package com.example.gymtools.trainingprograms

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
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.gymtools.R
import com.example.gymtools.common.CustomExercisePicker
import com.example.gymtools.common.CustomFloatPicker
import com.example.gymtools.common.CustomFloatingButton
import com.example.gymtools.common.CustomIntPicker
import com.example.gymtools.common.CustomStringPicker
import com.example.gymtools.common.Heading
import com.example.gymtools.common.adaptiveHeight
import com.example.gymtools.common.adaptiveWidth
import com.example.gymtools.common.lightGreen
import com.example.gymtools.common.lightRed
import com.example.gymtools.database.Exercise
import kotlinx.coroutines.launch

@Composable
fun AddExerciseToDayScreen(
    trainingDayId: Int,
    navController: NavHostController,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    val exercises by viewModel.exercises.collectAsState()

    var selectedExercise by remember { mutableStateOf<Exercise?>(null) }
    var selectedSetsNumber by remember { mutableIntStateOf(0) }
    var selectedRestTime by remember { mutableStateOf("") }
    var selectedWeight by remember { mutableStateOf<Float?>(null) }

    val coroutineScope = rememberCoroutineScope()

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
                        if (!viewModel.checkIfExerciseExists(trainingDayId, selectedExercise?.id ?: 0)) {
                            viewModel.addTrainingDayExercise(
                                trainingDayId = trainingDayId,
                                exerciseId = selectedExercise?.id ?: 0,
                                restTime = selectedRestTime,
                                setsNumber = selectedSetsNumber,
                                weight = selectedWeight
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
            Heading("Add exercise")
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
                    label = "Rest time",
                    imageId = R.drawable.clock,
                    onValueChange = { selectedRestTime = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(30).dp))
                CustomIntPicker(
                    label = "How many sets",
                    imageId = R.drawable.numbers,
                    onValueChange = { selectedSetsNumber = it }
                )
                Spacer(modifier = Modifier.height(adaptiveWidth(30).dp))
                CustomFloatPicker(
                    label = "Weight",
                    imageId = R.drawable.numbers,
                    onValueChange = { selectedWeight = it }
                )
                Spacer(modifier = Modifier.height(adaptiveHeight(100).dp))
            }
        }
    }
}