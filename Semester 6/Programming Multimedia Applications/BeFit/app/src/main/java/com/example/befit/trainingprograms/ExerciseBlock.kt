package com.example.befit.trainingprograms

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.OutlinedTextFieldDefaults
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.compose.ui.window.Dialog
import androidx.navigation.NavHostController
import com.example.befit.common.CustomText
import com.example.befit.constants.TrainingProgramsRoutes
import com.example.befit.constants.adaptiveWidth
import com.example.befit.constants.appBackground
import com.example.befit.constants.bright
import com.example.befit.constants.darkBackground
import com.example.befit.constants.editColor
import com.example.befit.common.floatToString
import com.example.befit.constants.mediumFontSize
import com.example.befit.constants.smallFontSize
import com.example.befit.database.TrainingDayExercise
import com.example.befit.database.TrainingDayExerciseSet

@Composable
fun ExerciseBlock(
    exerciseNumber: Int,
    trainingDayExercise: TrainingDayExercise,
    viewModel: TrainingProgramsViewModel,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    val exercises by viewModel.exercises
    val allSets by viewModel.sets

    val trainingDayExerciseId = trainingDayExercise.id

    var weight by remember { mutableStateOf(trainingDayExercise.weight) }

    val exercise = exercises.find { it.id == trainingDayExercise.exerciseId }

    var chosenSet by remember { mutableStateOf<TrainingDayExerciseSet?>(null)}

    val isEditMode by viewModel.isEditMode

    var weightEdit by remember { mutableStateOf(false) }
    var setEdit by remember { mutableStateOf(false) }

    if (weightEdit) {
        EditWeightPopup(
            exercise = trainingDayExercise,
            closePopup = {
                weight = it
                weightEdit = false
            },
            weight = weight,
            viewModel = viewModel
        )
    }
    if (setEdit) {
        EditSetPopup(
            set = chosenSet,
            closePopup = {
                setEdit = false
            },
            viewModel = viewModel
        )
    }

    Column(
        modifier = modifier
            .fillMaxWidth()
            .clip(RoundedCornerShape(adaptiveWidth(16).dp))
            .background(color = darkBackground)
            .padding(adaptiveWidth(16).dp)
    ) {
        // first line - exercise name
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .clip(RoundedCornerShape(adaptiveWidth(10).dp))
                .background(color = if (isEditMode) editColor else bright)
                .clickable(
                    onClick = {
                        if (isEditMode) {
                            navController.navigate(TrainingProgramsRoutes.EDIT_EXERCISE_FROM_DAY(trainingDayExerciseId))
                        } else {
                            navController.navigate(TrainingProgramsRoutes.VIEW_EXERCISE_FROM_DAY(trainingDayExerciseId))
                        }
                    },
                )
                .padding(adaptiveWidth(10).dp)
        ) {
            CustomText(
                text = "$exerciseNumber.  " + exercise?.name,
                color = if (isEditMode) bright else darkBackground
            )
        }
        Spacer(modifier = Modifier.height(adaptiveWidth(24).dp))

        // second line - weight (optional) and sets

        Row(
            horizontalArrangement = Arrangement.SpaceBetween,
            modifier = Modifier
                .fillMaxWidth()
        ) {
            if (trainingDayExercise.weight != null) {
                Row(
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    EditableItem(
                        text = floatToString(weight),
                        modifier = Modifier
                            .clickable { weightEdit = !weightEdit }
                    )
                    CustomText(
                        text = " kg ",
                        fontSize = smallFontSize,
                        modifier = Modifier.padding(adaptiveWidth(5).dp)
                    )
                }
            }
            Row(
                verticalAlignment = Alignment.CenterVertically,
                modifier = Modifier
                    .fillMaxWidth()
                    .horizontalScroll(rememberScrollState())
            ) {
                val sets = allSets.filter { it.trainingDayExerciseId == trainingDayExerciseId }
                for (i in sets.indices) {
                    EditableItem(
                        text = sets[i].reps?.toString() ?: " ? ",
                        modifier = Modifier
                            .clickable {
                                chosenSet = sets[i]
                                setEdit = true
                            }
                    )
                    if (i < sets.size - 1) {
                        CustomText(
                            text = " / "
                        )
                    }
                }
            }
        }
    }
    Spacer(modifier = Modifier.height(adaptiveWidth(32).dp))
}

@Composable
fun EditableItem(
    text: String,
    modifier: Modifier = Modifier
) {
    Box(
        contentAlignment = Alignment.Center,
        modifier = modifier
            .clip(RoundedCornerShape(5.dp))
            .background(color = bright)
            .padding(5.dp)
    ) {
        CustomText(
            text = text,
            color = darkBackground
        )
    }
}

@Composable
fun EditWeightPopup(
    exercise: TrainingDayExercise?,
    weight: Float?,
    closePopup: (Float?) -> Unit,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    var text by remember { mutableStateOf(floatToString(weight)) }

    Dialog(
        onDismissRequest = {
            if (exercise != null) {
                val weight = text.toFloatOrNull()
                viewModel.updateTrainingDayExercise(
                    trainingDayExerciseId = exercise.id,
                    trainingDayId = exercise.trainingDayId,
                    exerciseId = exercise.exerciseId,
                    wasExerciseChanged = false,
                    setsNumber = exercise.id,
                    restTime = exercise.restTime,
                    weight = weight
                )
                closePopup(weight)
            }
        }
    ) {
        Box(
            modifier = modifier
                .clip(RoundedCornerShape(16.dp))
                .background(color = appBackground)
                .fillMaxWidth(0.5f)
        ) {
            OutlinedTextField(
                textStyle = TextStyle(
                    color = bright,
                    fontSize = adaptiveWidth(mediumFontSize).sp,
                    fontWeight = FontWeight.Bold,
                    textAlign = TextAlign.Center
                ),
                colors = OutlinedTextFieldDefaults.colors(
                    focusedBorderColor = bright,
                    unfocusedBorderColor = bright,
                    focusedTextColor = bright,
                    unfocusedTextColor = bright,
                    focusedContainerColor = darkBackground,
                    unfocusedContainerColor = darkBackground,
                ),
                singleLine = true,
                value = text,
                onValueChange = { text = it },
                keyboardOptions = KeyboardOptions.Default.copy(
                    keyboardType = KeyboardType.Number
                ),
                modifier = Modifier.fillMaxWidth()
            )
        }
    }
}

@Composable
fun EditSetPopup(
    set: TrainingDayExerciseSet?,
    closePopup: () -> Unit,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    var text by remember { mutableStateOf(if (set?.reps == null) "" else set.reps.toString()) }

    Dialog(
        onDismissRequest = {
            if (set != null) {
                viewModel.updateSet(
                    trainingDayExerciseSetId = set.id,
                    trainingDayExerciseId = set.trainingDayExerciseId,
                    reps = text.toIntOrNull()
                )
                closePopup()
            }
        }
    ) {
        Box(
            modifier = modifier
                .clip(RoundedCornerShape(16.dp))
                .background(color = appBackground)
                .fillMaxWidth(0.5f)
        ) {
            OutlinedTextField(
                textStyle = TextStyle(
                    color = bright,
                    fontSize = adaptiveWidth(mediumFontSize).sp,
                    fontWeight = FontWeight.Bold,
                    textAlign = TextAlign.Center
                ),
                colors = OutlinedTextFieldDefaults.colors(
                    focusedBorderColor = bright,
                    unfocusedBorderColor = bright,
                    focusedTextColor = bright,
                    unfocusedTextColor = bright,
                    focusedContainerColor = darkBackground,
                    unfocusedContainerColor = darkBackground,
                ),
                singleLine = true,
                value = text,
                onValueChange = { text = it },
                keyboardOptions = KeyboardOptions.Default.copy(
                    keyboardType = KeyboardType.Number
                ),
                modifier = Modifier.fillMaxWidth()
            )
        }
    }
}