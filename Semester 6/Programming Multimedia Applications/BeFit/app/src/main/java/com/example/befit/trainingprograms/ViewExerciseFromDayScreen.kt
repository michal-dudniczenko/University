package com.example.befit.trainingprograms

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.OutlinedTextFieldDefaults
import androidx.compose.material3.Text
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
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomText
import com.example.befit.common.Heading
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.TrainingProgramsRoutes
import com.example.befit.constants.adaptiveWidth
import com.example.befit.constants.bigFontSize
import com.example.befit.constants.mediumFontSize

@Composable
fun ViewExerciseFromDayScreen(
    trainingDayExerciseId: Int,
    navController: NavHostController,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    val trainingDayExercises by viewModel.trainingDaysExercises
    val exercises by viewModel.exercises

    val trainingDayExercise = trainingDayExercises.find { it.id == trainingDayExerciseId }
    val exercise = exercises.find { it.id == trainingDayExercise?.exerciseId }

    val trainingDayId = trainingDayExercise?.trainingDayId ?: -1

    var viewModelNotes by viewModel.currentNotes

    var notes by remember { mutableStateOf(viewModelNotes.ifEmpty { exercise?.notes ?: "" }) }

    val restTime = trainingDayExercise?.restTime ?: ""

    Box(
        modifier = modifier.fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.BACK_ON_SECONDARY,
            description = "Back button",
            onClick = {
                if (exercise != null && exercise.notes != notes) {
                    viewModel.updateExercise(
                        id = exercise.id,
                        name = exercise.name,
                        notes = notes,
                        videoId = exercise.videoId
                    )
                }
                viewModelNotes = ""
                if (trainingDayId != -1) {
                    navController.navigate(TrainingProgramsRoutes.VIEW_TRAINING_DAY(trainingDayId))
                } else {
                    navController.navigate(TrainingProgramsRoutes.START)
                }
            },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = 30.dp, y = (-30).dp)
        )

        if (trainingDayExercise == null || exercise == null) {
            Box(
                modifier = Modifier.fillMaxSize()
            ) {
                CircularProgressIndicator(
                    color = Themes.ON_BACKGROUND,
                    modifier = Modifier.align(Alignment.Center)
                )
                return
            }
        }
        if (exercise!!.videoId > 0) {
            CustomFloatingButton(
                icon = Themes.VIDEO_ON_SECONDARY,
                description = "Exercise video",
                onClick = {
                    navController.navigate(TrainingProgramsRoutes.EXERCISE_VIDEO(exercise.id, trainingDayExerciseId))
                },
                modifier = Modifier
                    .align(Alignment.BottomEnd)
                    .offset(x = (-30).dp, y = (-30).dp)
            )
        }
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading(Strings.EXERCISE_INFO)
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                modifier = modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight(0.85f)
                    .clip(RoundedCornerShape(adaptiveWidth(16).dp))
                    .background(color = Themes.PRIMARY)
            ) {
                Spacer(Modifier.height(adaptiveWidth(16).dp))
                Row(
                    horizontalArrangement = Arrangement.Center,
                    modifier = Modifier
                        .fillMaxWidth(0.9f)
                        .clip(RoundedCornerShape(adaptiveWidth(10).dp))
                        .background(color = Themes.ON_PRIMARY)
                        .padding(8.dp)
                ) {
                    CustomText(
                        text = exercise.name,
                        color = Themes.PRIMARY,
                        fontSize = bigFontSize,
                    )
                }
                Spacer(Modifier.height(adaptiveWidth(32).dp))
                Row(
                    modifier = Modifier
                        .fillMaxWidth(0.85f)
                ) {
                    CustomText(
                        text = "${Strings.REST_TIME}:  $restTime"
                    )
                }
                Spacer(Modifier.height(adaptiveWidth(16).dp))
                OutlinedTextField(
                    textStyle = TextStyle(
                        color = Themes.ON_PRIMARY,
                        fontSize = adaptiveWidth(mediumFontSize).sp,
                        fontWeight = FontWeight.Bold
                    ),
                    colors = OutlinedTextFieldDefaults.colors(
                        focusedBorderColor = Themes.ON_PRIMARY,
                        unfocusedBorderColor = Themes.ON_PRIMARY,
                        focusedTextColor = Themes.ON_PRIMARY,
                        unfocusedTextColor = Themes.ON_PRIMARY,
                        focusedLabelColor = Themes.ON_PRIMARY,
                        unfocusedLabelColor = Themes.ON_PRIMARY,
                        focusedContainerColor = Themes.PRIMARY,
                        unfocusedContainerColor = Themes.PRIMARY,
                    ),
                    value = notes,
                    onValueChange = { input: String ->
                        notes = input
                        viewModelNotes = input
                    },
                    label = { Text(Strings.NOTES) },
                    modifier = Modifier
                        .fillMaxWidth(0.9f)
                        .fillMaxHeight(0.95f)
                )
            }
        }
    }
}