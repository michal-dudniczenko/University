package com.example.gymtools.trainingprograms

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
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
import androidx.compose.foundation.verticalScroll
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.gymtools.R
import com.example.gymtools.common.CustomFloatingButton
import com.example.gymtools.common.CustomText
import com.example.gymtools.common.Heading
import com.example.gymtools.common.adaptiveHeight
import com.example.gymtools.common.adaptiveWidth
import com.example.gymtools.common.bigFontSize
import com.example.gymtools.common.bright
import com.example.gymtools.common.editColor
import com.example.gymtools.common.mediumGreen

@Composable
fun ViewTrainingDayScreen(
    trainingDayId: Int,
    viewModel: TrainingProgramsViewModel,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    var isEditMode by viewModel.isEditMode

    val trainingDay = viewModel.getTrainingDayById(trainingDayId)

    val programId = trainingDay?.programId ?: 0

    val exercises = viewModel.getExercisesFromTrainingDay(trainingDayId)

    val scrollState by viewModel.scrollState

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = R.drawable.back,
            description = "Back button",
            onClick = {
                navController.navigate("Training days list/$programId")
                isEditMode = false
                viewModel.resetScroll()
            },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = adaptiveWidth(32).dp, y = adaptiveWidth(-32).dp)
        )
        CustomFloatingButton(
            icon = if (isEditMode) R.drawable.edit_white else R.drawable.edit,
            description = "Edit mode",
            color = if (isEditMode) editColor else bright,
            onClick = { isEditMode = !isEditMode },
            modifier = Modifier
                .align(Alignment.BottomEnd)
                .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
        )
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading(
                text = trainingDay?.name ?: "",
            )
            if (exercises.isEmpty() && !isEditMode) {
                Box(
                    modifier = Modifier
                        .fillMaxSize()
                ) {
                    CustomText(
                        text = "Nothing here yet!",
                        fontSize = bigFontSize,
                        modifier = Modifier
                            .align(Alignment.Center)
                            .offset(y = adaptiveWidth(-75).dp)
                    )
                }
            } else {
                if (isEditMode) {
                    Box(
                        contentAlignment = Alignment.Center,
                        modifier = modifier
                            .fillMaxWidth()
                            .background(
                                color = mediumGreen,
                                shape = RoundedCornerShape(adaptiveWidth(16).dp)
                            )
                            .clickable {
                                navController.navigate("Add exercise to day/$trainingDayId")
                            }
                    ) {
                        CustomText(
                            text = "Add exercise",
                            modifier = Modifier.padding(adaptiveWidth(16).dp)
                        )
                    }
                    Spacer(modifier = Modifier.height(adaptiveWidth(32).dp))
                }
                Column(
                    horizontalAlignment = Alignment.CenterHorizontally,
                    modifier = Modifier
                        .fillMaxWidth()
                        .verticalScroll(scrollState)
                ) {
                    var i = 1
                    for (exercise in exercises) {
                        ExerciseBlock(
                            exerciseNumber = i,
                            trainingDayExercise = exercise,
                            viewModel = viewModel,
                            navController = navController
                        )
                        i++
                    }
                    Spacer(modifier = Modifier.height(adaptiveHeight(300).dp))
                }
            }
        }
    }
}