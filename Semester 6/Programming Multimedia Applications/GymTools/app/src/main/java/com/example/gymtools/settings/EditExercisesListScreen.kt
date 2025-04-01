package com.example.gymtools.settings

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.horizontalScroll
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
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
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
import com.example.gymtools.common.darkBackground
import com.example.gymtools.database.Exercise
import com.example.gymtools.trainingprograms.TrainingProgramsViewModel

@Composable
fun EditExercisesListScreen(
    viewModel: TrainingProgramsViewModel,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {

    val exercises by viewModel.exercises.collectAsState()

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = R.drawable.add,
            description = "Add new exercise",
            onClick = {
                navController.navigate("Add exercise")
            },
            modifier = Modifier
                .align(Alignment.BottomEnd)
                .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
        )
        CustomFloatingButton(
            icon = R.drawable.back,
            description = "Back button",
            onClick = {
                navController.navigate("Settings")
            },
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
            Heading("Edit exercises")
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                modifier = Modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight()
                    .verticalScroll(rememberScrollState())
            ) {
                for (exercise in exercises) {
                    SettingsExerciseItem(
                        exercise = exercise,
                        navController = navController
                    )
                }
                Spacer(Modifier.height(adaptiveHeight(200).dp))
            }
        }
    }
}

@Composable
fun SettingsExerciseItem(
    exercise: Exercise,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    Row(
        horizontalArrangement = Arrangement.Center,
        modifier = modifier
            .fillMaxWidth()
            .background(
                color = darkBackground,
                shape = RoundedCornerShape(adaptiveWidth(16).dp)
            )
            .clickable {
                navController.navigate("Edit exercise/${exercise.id}")
            }
            .padding(adaptiveWidth(16).dp)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .horizontalScroll(rememberScrollState())
        ) {
            CustomText(
                text = exercise.name
            )
        }
    }
    Spacer(Modifier.height(adaptiveWidth(32).dp))
}